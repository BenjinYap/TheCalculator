

using System.Collections.Generic;
using System.Diagnostics;
using TinyCalc.Models.Modules;
namespace TinyCalc.Models {
	public sealed class Calc {
		private CoreModule core = new CoreModule ();
		private BinaryModule binary = new BinaryModule ();

		private List <IModule> modules = new List <IModule> ();

		public Calc () {
			//create the modules
			this.modules.Add (core);
			this.modules.Add (binary);
		}

		public CalcResult Solve (string input) {
			string postfix = this.ParseInput (input);

			Debug.WriteLine (postfix);

			return new CalcResult (1);
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
					//if number, push to output
					if (this.core.IsNumber (buffer)) {
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
			return string.Join (" ", output) + " " + string.Join (" ", stack.ToArray ());
		}
	}

	public sealed class CalcResult {
		private double result;
		public double Result { get { return this.result; } }

		private CalcError error;
		public CalcError Error { get { return this.error; } }

		public CalcResult (double result) {
			this.result = result;
			this.error = null;
		}

		public CalcResult (CalcError error) {
			this.result = double.NaN;
			this.error = error;
		}
	}
}
