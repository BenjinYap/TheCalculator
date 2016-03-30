

using System.Collections.Generic;
using System.Diagnostics;
using TinyCalc.Models.Modules;
namespace TinyCalc.Models {
	public sealed class Calc {
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
			
			//buffer for current token
			string buffer = "";

			//parse token until input is empty
			while (input.Length > 0) {
				//add next character to buffer
				buffer += input [0];

				//remove that character from input
				input = input.Remove (0, 1);

				bool foundToken = false;

				//ask each module if buffer is a token
				foreach (IModule module in this.modules) {
					if (module.IsToken (buffer)) {
						foundToken = true;
						break;
					}
				}

				//buffer is a token, decide what to do with it
				if (foundToken) {
					//if number or constant, push to output
					if (this.core.IsNumber (buffer) || this.constant.IsToken (buffer)) {
						output.Add (buffer);
					} else if (this.core.IsLeftBracket (buffer)) {  //if left bracket, push to stack
						stack.Push (buffer);
					} else if (this.core.IsRightBracket (buffer)) {

					} else {
						//if binary token, apply precedence and associativity rules
						if (this.binary.IsToken (buffer)) {
							//if exponent, push to stack
							if (this.binary.IsExponent (buffer)) {
								stack.Push (buffer);
							} else {  //if other binary operators
								//pop stack onto output as long as top of stack is more important than current operator
								while (stack.Count > 0 && this.binary.Op1PrecedenceLessOrEqualOp2 (buffer, stack.Peek ())) {
									output.Add (stack.Pop ());
								}
								
								stack.Push (buffer);
							}
						}
					}

					buffer = "";
				}
			}

			if (buffer.Length > 0) {
				Debug.WriteLine ("ERROR");
			}
			
			//join the output and the stack and return the string
			return (string.Join (" ", output) + " " + string.Join (" ", stack.ToArray ())).Trim ();
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
