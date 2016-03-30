

using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace TinyCalc.Models.Modules {
	public class CoreModule:IModule {
		private const string LeftBracket = "(";
		private const string RightBracket = ")";

		public bool IsToken (string input) {
			if (this.IsNumber (input)) {
				return true;
			}

			List <string> tokens = new List <string> {
				CoreModule.LeftBracket,
				CoreModule.RightBracket,
			};

			return tokens.Contains (input);
		}

		public bool IsNumber (string input) {
			return Regex.IsMatch (input, @"^\d*\.?\d+$");
		}
		
		public bool IsLeftBracket (string input) {
			return input == CoreModule.LeftBracket;
		}

		public bool IsRightBracket (string input) {
			return input == CoreModule.RightBracket;
		}

		public double Solve (string input) {
			return double.Parse (input);
		}
	}
}
