

using System;
using System.Collections.Generic;
namespace TinyCalc.Models.Modules {
	public class FunctionModule:IModule {
		private const string AbsoluteValue = "abs";
		private const string Sin = "sin";
		private const string Cos = "cos";
		private const string Tan = "tan";
		private const string Sinh = "sinh";
		private const string Cosh = "cosh";
		private const string Tanh = "tanh";
		private const string Asin = "asin";
		private const string Acos = "acos";
		private const string Atan = "atan";

		private readonly List <string> tokens;

		public FunctionModule () {
			this.tokens = new List <string> {
				FunctionModule.AbsoluteValue,
				FunctionModule.Sinh,
				FunctionModule.Cosh,
				FunctionModule.Tanh,
				FunctionModule.Asin,
				FunctionModule.Acos,
				FunctionModule.Atan,
				FunctionModule.Sin,  //sin cos and tan are checked last because the other functions have them inside them
				FunctionModule.Cos,
				FunctionModule.Tan,
			};
		}

		public string GetNextToken (string input) {
			foreach (string token in this.tokens) {
				if (input.IndexOf (token) == 0) {
					return token;
				}
			}

			return "";
		}

		public bool IsToken (string input) {
			return tokens.Contains (input);
		}

		public double Solve (string num, string func) {
			double n = double.Parse (num);

			switch (func) {
				case FunctionModule.AbsoluteValue:
					return Math.Abs (n);
				case FunctionModule.Sin:
					return Math.Sin (n);
				case FunctionModule.Cos:
					return Math.Cos (n);
				case FunctionModule.Tan:
					return Math.Tan (n);
				case FunctionModule.Sinh:
					return Math.Sinh (n);
				case FunctionModule.Cosh:
					return Math.Cosh (n);
				case FunctionModule.Tanh:
					return Math.Tanh (n);
				case FunctionModule.Asin:
					return Math.Asin (n);
				case FunctionModule.Acos:
					return Math.Acos (n);
				case FunctionModule.Atan:
					return Math.Atan (n);
			}

			return double.NaN;
		}
	}
}
