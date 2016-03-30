

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

		public bool Op1PrecedenceLessOrEqualOp2 (string op1, string op2) {
			if (this.IsExponent (op1)) {
				return false;
			}

			return (op1 == BinaryModule.Addition || op1 == BinaryModule.Subtraction) &&
				(op2 == BinaryModule.Multiplication || op2 == BinaryModule.Division);
		}

		public bool IsExponent (string input) {
			return input == BinaryModule.Exponent;
		}
	}
}
