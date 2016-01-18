

using System.Collections.Generic;
using System.Diagnostics;
namespace TheCalculator.Models {
	public static class ShuntingYard {

		public static void Main (string [] args) {
			Debug.WriteLine (Shunt ("    1 +       1"));
		}

		public static string Shunt (string input) {
			input = input.Replace (" ", "").ToLower ();

			Stack <string> operatorStack = new Stack <string> ();
			string output = "";

			for (int i = 0; i < input.Length; i++) {
				char c = input [i];


			}
			
			return output;
		}

		private class Token {
			public TokenType Type;
			public string Value;

			public Token (TokenType type, string value) {
				this.Type = type;
				this.Value = value;
			}
		}

		private enum TokenType {
			Number,
			Operator,
		}
	}
}
