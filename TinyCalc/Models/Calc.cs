

using System;
using System.Collections.Generic;
using System.Diagnostics;
using TinyCalc.Models.Modules;
namespace TinyCalc.Models {
	public sealed class Calc {
		private const int InfiniteLoopBreakingPoint = 1000;

		private readonly CoreModule core = new CoreModule ();
		private readonly BinaryModule binary = new BinaryModule ();
		private readonly ConstantModule constant = new ConstantModule ();

		private readonly List <IModule> modules = new List <IModule> ();

		public Calc () {
			//create the modules
			this.modules.Add (core);
			this.modules.Add (binary);
			this.modules.Add (constant);
		}

		public CalcResult Solve (string input) {
			string postfix = this.ParseInput (input);

			//Debug.WriteLine (postfix);

			//return new CalcResult (1);

			CalcResult result = this.ActualSolve (postfix);

			if (result.Error == CalcError.None) {
				this.constant.PreviousAnswer = result.Result;
			}

			return result;
		}

		private CalcResult ActualSolve (string postfix) {
			List <string> tokens = new List <string> (postfix.Split (' '));
			
			//replace all constants with actual values
			this.SolveConstants (tokens);

			//if there is only one token and it's a number
			if (tokens.Count == 1 && this.core.IsNumber (tokens [0])) {
				//parse the number, it is the final answer
				return new CalcResult (this.core.Solve (tokens [0]));
			}

			int counter = 0;

			while (tokens.Count > 0) {
				int nonNumberIndex = tokens.FindIndex (a => core.IsNumber (a) == false);

				//-1 means the only token is a number
				if (nonNumberIndex == -1) {
					//parse the number, it is the final answer
					return new CalcResult (this.core.Solve (tokens [0]));
				} else if (nonNumberIndex == 0) {  //first token is not a number
					//if token is not ans then error
				} else {
					
				}
				//} else if (nonNumberIndex > 2) {  //if nonnumber token is greater than 2, something went wrong
				//	return new CalcResult (CalcError.Unknown);
				//} else if (nonNumberIndex == 1) {  //nonnumber token is at 1, implies function
					
				//} else if (nonNumberIndex == 2) {  //nonnumber token is at 2, implies binary operator
				//	//solve, remove tokens from list, insert result in place of tokens
				//	double result = this.binary.Solve (tokens [0], tokens [1], tokens [2]);
				//	tokens.RemoveRange (0, 3);
				//	tokens.Insert (0, result.ToString ());
				//}

				counter++;

				if (counter >= Calc.InfiniteLoopBreakingPoint) {
					return new CalcResult (CalcError.InfiniteLoop);
				}
			}

			return new CalcResult (CalcError.Unknown);
		}

		private void SolveConstants (List <string> tokens) {
			for (int i = 0; i < tokens.Count; i++) {
				if (this.constant.IsToken (tokens [i])) {
					tokens [i] = this.constant.Solve (tokens [i]).ToString ();
				}
			}
		}

		private string ParseInput (string input) {
			//remove all whitespace and convert to lower
			input = input.Replace (" ", "").ToLower ();

			//output string list
			List <string> output = new List <string> ();

			//operator stack
			Stack <string> stack = new Stack <string> ();
			
			//parse token until input is empty
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
					throw new Exception ("PARSE ERROR");
				}

				//remove the found token from the input
				input = this.RemoveTokenFromInput (input, token);

				//if number or constant, push to output
				if (this.core.IsNumber (token) || this.constant.IsToken (token)) {
					output.Add (token);
				} else if (this.core.IsLeftBracket (token)) {  //if left bracket, push to stack
					stack.Push (token);
				} else if (this.core.IsRightBracket (token)) {

				} else {
					//if binary token, apply precedence and associativity rules
					if (this.binary.IsToken (token)) {
						//if exponent, push to stack
						if (this.binary.IsExponent (token)) {
							stack.Push (token);
						} else {  //if other binary operators
							//pop stack onto output as long as top of stack is more important than current operator
							while (stack.Count > 0 && this.binary.Op1PrecedenceLessOrEqualOp2 (token, stack.Peek ())) {
								output.Add (stack.Pop ());
							}
								
							stack.Push (token);
						}
					}
				}
			}

			//join the output and the stack and return the string
			return (string.Join (" ", output) + " " + string.Join (" ", stack.ToArray ())).Trim ();
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

		public CalcResult (double result) {
			this.result = result;
			this.error = CalcError.None;
		}

		public CalcResult (CalcError error) {
			this.result = double.NaN;
			this.error = error;
		}
	}
}
