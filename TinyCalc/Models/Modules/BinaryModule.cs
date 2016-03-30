

using System.Collections.Generic;
namespace TinyCalc.Models.Modules {
	public class BinaryModule:IModule {
		private const string Addition = "+";
		private const string Subtraction = "-";
		private const string Multiplication = "*";
		private const string Division = "/";
		private const string Exponent = "^";

		public bool IsToken (string input) {
			List <string> tokens = new List <string> {
				BinaryModule.Addition,
				BinaryModule.Subtraction,
				BinaryModule.Multiplication,
				BinaryModule.Division,
				BinaryModule.Exponent,
			};

			return tokens.Contains (input);
		}
	}
}
