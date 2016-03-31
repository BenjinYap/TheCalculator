
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace TinyCalc.Models.Modules {
	public class CoreModule:IModule {
		private const string NumberPattern = @"^\d*\.?\d+";
		public const string LeftBracket = "(";
		public const string RightBracket = ")";

		public CoreModule () {
			
		}

		public CalcResult VerifyBrackets (string input) {
			int leftCount = input.Count (a => a.ToString () == CoreModule.LeftBracket);
			int rightCount = input.Count (a => a.ToString () == CoreModule.RightBracket);
			
			if (leftCount < rightCount) {
				return new CalcResult (CalcError.MissingLeftBracket);
			} else if (leftCount > rightCount) {
				return new CalcResult (CalcError.MissingRightBracket);
			}

			return new CalcResult (CalcError.None);
		}

		public string GetNextToken (string input) {
			Match match = Regex.Match (input, CoreModule.NumberPattern);

			if (match.Success) {
				return match.Value;
			}

			if (input.IndexOf (CoreModule.LeftBracket) == 0) {
				return CoreModule.LeftBracket;
			}

			if (input.IndexOf (CoreModule.RightBracket) == 0) {
				return CoreModule.RightBracket;
			}

			return "";
		}

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
			return Regex.IsMatch (input, CoreModule.NumberPattern + "$");
		}

		public bool IsNumberWithNegative (string input) {
			return Regex.IsMatch (input, "^-?" + CoreModule.NumberPattern.Substring (1) + "$");
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
