

using System;
using System.Collections.Generic;
using System.Diagnostics;
using TinyCalc.Models.Modules;
namespace TinyCalc.Models {
	public sealed class Calc {
		private const int InfiniteLoopBreakingPoint = 1000;

		private readonly CoreModule core = new CoreModule ();
		private readonly OperatorModule operatorr = new OperatorModule ();
		private readonly ConstantModule constant = new ConstantModule ();
		private readonly FunctionModule function = new FunctionModule ();

		private readonly List <IModule> modules = new List <IModule> ();

		public Calc () {
			//create the modules
			this.modules.Add (core);
			this.modules.Add (operatorr);
			this.modules.Add (constant);
			this.modules.Add (function);
		}

		public CalcResult Solve (string input) {
			ParseResult parseResult = this.ParseInput (input);

			if (parseResult.Error != CalcError.None) {
				return new CalcResult (parseResult.Error, parseResult.ErrorObject);
			}

			//Debug.WriteLine (parseResult.Output);
			//return new CalcResult (1);

			CalcResult result = this.ActualSolve (parseResult.Output);

			if (result.Error == CalcError.None) {
				this.constant.PreviousAnswer = result.Result;
			}

			return result;
		}

		private CalcResult ActualSolve (string postfix) {
			List <string> tokens = new List <string> (postfix.Split (' '));
			
			//replace all constants with actual values
			this.constant.SolveConstants (tokens);

			//if there is only one token and it's a number
			if (tokens.Count == 1 && this.core.IsNumber (tokens [0])) {
				//parse the number, it is the final answer
				return new CalcResult (this.core.Solve (tokens [0]));
			}

			int counter = 0;

			while (tokens.Count > 0) {
				int nonNumberIndex = tokens.FindIndex (a => core.IsNumberWithNegative (a) == false);

				//-1 means the only token is a number
				if (nonNumberIndex == -1) {
					//parse the number, it is the final answer
					return new CalcResult (this.core.Solve (tokens [0]));
				} else {
					//if (tokens.Count == 0 && this.operatorr.IsNegation (tokens [0][0].ToString ()) && this.core.IsNumber (tokens [0].Substring (1))) {
					//	//parse the number, it is the final answer
					//	return new CalcResult (this.core.Solve (tokens [0]));
					//}

					//if the token is negation
					if (this.operatorr.IsNegation (tokens [nonNumberIndex])) {
						//solve it, remove the tokens from list, insert output in place
						double result = this.operatorr.Solve (tokens [nonNumberIndex - 1], tokens [nonNumberIndex]);
						tokens.RemoveRange (nonNumberIndex - 1, 2);
						tokens.Insert (nonNumberIndex - 1, result.ToString ());
					} else if (this.operatorr.IsToken (tokens [nonNumberIndex])) {  //if token is something other than negation
						//solve it, remove the tokens from list, insert output in place
						int i = nonNumberIndex;
						double result = this.operatorr.Solve (tokens [i - 2], tokens [i - 1], tokens [i]);
						tokens.RemoveRange (i - 2, 3);
						tokens.Insert (i - 2, result.ToString ());
					} else if (this.function.IsToken (tokens [nonNumberIndex])) {  //if token is a function
						//solve it, remove the tokens from list, insert output in place
						double result = this.function.Solve (tokens [nonNumberIndex - 1], tokens [nonNumberIndex]);
						tokens.RemoveRange (nonNumberIndex - 1, 2);
						tokens.Insert (nonNumberIndex - 1, result.ToString ());
					}
				}
				
				counter++;

				if (counter >= Calc.InfiniteLoopBreakingPoint) {
					return new CalcResult (CalcError.InfiniteLoop);
				}
			}

			return new CalcResult (CalcError.Unknown);
		}

		private string ConvertNegationSign (string input) {
			for (int i = 0; i < input.Length; i++) {
				if (this.operatorr.IsSubtraction (input [i].ToString ())) {
					//if minus sign is at the start, after an operator, or after a bracket
					if (i == 0 || this.operatorr.IsToken (input [i - 1].ToString ()) || this.core.IsLeftBracket (input [i - 1].ToString ())) {
						input = input.Substring (0, i) + "_" + input.Substring (i + 1);
					}
				}
			}

			//Debug.WriteLine (input);

			return input;
		}

		private ParseResult ParseInput (string input) {
			//remove all whitespace and convert to lower
			input = input.Replace (" ", "").ToLower ();

			CalcResult result = this.core.VerifyBrackets (input);

			if (result.Error != CalcError.None) {
				return new ParseResult (result.Error, result.ErrorObject);
			}

			result = this.function.VerifyBrackets (input);

			if (result.Error != CalcError.None) {
				return new ParseResult (result.Error, result.ErrorObject);
			}

			//convert negation sign to a specific negation function
			input = this.ConvertNegationSign (input);

			//output string list
			List <string> output = new List <string> ();

			//operator stack
			Stack <string> stack = new Stack <string> ();
			
			//parse token until input is empty
			//shunting yard starts now!
			while (input.Length > 0) {
				string token = "";

				//check each module to see if they can find a token
				foreach (IModule module in this.modules) {
					token = module.GetNextToken (input);

					if (token != "") {
						break;
					}
				}

				//no token means unrecognized input, bail
				if (token == "") {
					return new ParseResult (CalcError.UnknownToken, input);
				}

				//remove the found token from the input
				input = this.RemoveTokenFromInput (input, token);

				//if number or constant, push to output
				if (this.core.IsNumber (token) || this.constant.IsToken (token)) {
					output.Add (token);
				} else if (this.core.IsLeftBracket (token)) {  //if left bracket, push to stack
					stack.Push (token);
				} else if (this.core.IsRightBracket (token)) {  //if right bracket, push all the things
					//push stack onto output until left bracket is found
					while (this.core.IsLeftBracket (stack.Peek ()) == false) {
						output.Add (stack.Pop ());
					}

					//throw away left bracket
					stack.Pop ();
				} else if (this.function.IsToken (token)) {
					stack.Push (token);
				} else {
					//if operatorr token, apply precedence and associativity rules
					if (this.operatorr.IsToken (token)) {
						//if exponent, push to stack
						if (this.operatorr.IsExponent (token) || this.operatorr.IsNegation (token)) {
							stack.Push (token);
						} else {  //if other operatorr operators
							//pop stack onto output as long as top of stack is more important than current operator
							while (stack.Count > 0 && this.operatorr.Op1PrecedenceLessOrEqualOp2 (token, stack.Peek ())) {
								output.Add (stack.Pop ());
							}
								
							stack.Push (token);
						}
					}
				}
			}

			//join the output and the stack and return the string
			return new ParseResult ((string.Join (" ", output) + " " + string.Join (" ", stack.ToArray ())).Trim ());
		}

		private string RemoveTokenFromInput (string input, string token) {
			int pos = input.IndexOf (token);

			if (pos < 0) {
				return input;
			}
			
			return input.Substring (0, pos) + "" + input.Substring (pos + token.Length);
		}
	}

	public sealed class CalcResult {
		private double result;
		public double Result { get { return this.result; } }

		private CalcError error;
		public CalcError Error { get { return this.error; } }

		private string errorObject;
		public string ErrorObject { get { return this.errorObject; } }

		public CalcResult (double result) {
			this.result = result;
			this.error = CalcError.None;
			this.errorObject = "";
		}

		public CalcResult (CalcError error) {
			this.result = double.NaN;
			this.error = error;
			this.errorObject = "";
		}

		public CalcResult (CalcError error, string errorObject) {
			this.result = double.NaN;
			this.error = error;
			this.errorObject = errorObject;
		}
	}

	public sealed class ParseResult {
		private string output;
		public string Output { get { return this.output; } }

		private CalcError error;
		public CalcError Error { get { return this.error; } }

		private string errorObject;
		public string ErrorObject { get { return this.errorObject; } }

		public ParseResult (string result) {
			this.output = result;
			this.error = CalcError.None;
			this.errorObject = "";
		}

		public ParseResult (CalcError error) {
			this.output = "";
			this.error = error;
			this.errorObject = "";
		}

		public ParseResult (CalcError error, string errorObject) {
			this.output = "";
			this.error = error;
			this.errorObject = errorObject;
		}
	}
}
