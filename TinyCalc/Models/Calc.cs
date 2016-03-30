

using System.Collections.Generic;
using System.Diagnostics;
using TinyCalc.Models.Modules;
namespace TinyCalc.Models {
	public sealed class Calc {
		private List <IModule> modules = new List <IModule> ();

		public Calc () {
			//create the modules
			this.modules.Add (new CoreModule ());
			this.modules.Add (new BinaryModule ());
		}

		public CalcResult Solve (string input) {
			//list of all parsed tokens
			List <string> tokens = this.ParseInput (input);

			

			return new CalcResult (1);
		}

		private List <string> ParseInput (string input) {
			//list of all parsed tokens
			List <string> tokens = new List <string> ();
			
			//remove all whitespace and convert to lower
			input = input.Replace (" ", "").ToLower ();

			//buffer for current token
			string buffer = "";

			while (input.Length > 0) {
				buffer += input [0];
				input = input.Remove (0, 1);

				foreach (IModule module in this.modules) {
					if (module.IsToken (buffer)) {
						tokens.Add (buffer);
						buffer = "";
						break;
					}
				}
			}

			return tokens;
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
