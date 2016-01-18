

using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace TheCalculator.Models {
	public static class ShuntingYard {

		public static void Main (string [] args) {
			Debug.WriteLine (Shunt ("1+1"));
			Debug.WriteLine (Shunt ("1*1"));
			Debug.WriteLine (Shunt ("5-3"));
			Debug.WriteLine (Shunt ("56/8"));
		}

		public static string Shunt (string input) {
			input = input.Replace (" ", "").ToLower ();

			Stack <string> operatorStack = new Stack <string> ();
			string output = "";
			
			//while input has stuff
			while (input.Length > 0) {
				//get the next token and remove it from the input
				Token token = ShuntingYard.GetToken (input);

				if (token == null) {
					break;
				}

				//remove the token from the input
				input = ReplaceFirst (input, token.Value, "");

				//do things based on the token type
				switch (token.Type) {
					case TokenType.Number:
						//add to output
						output += token.Value + " ";
						break;
					case TokenType.Operator:
						//push operator to stack
						operatorStack.Push (token.Value);
						break;
				}
			}

			//pop all remaining operators onto stack
			while (operatorStack.Count > 0) {
				output += operatorStack.Pop () + " ";
			}
			
			//trim the trailing space and return output
			return output.TrimEnd (' ');
		}

		private static Token GetToken (string input) {
			//create the token type regex patterns
			Dictionary <TokenType, string> tokenPatterns = new Dictionary <TokenType, string> ();
			tokenPatterns [TokenType.Number] = @"\d+";
			tokenPatterns [TokenType.Operator] = @"[+\-*/]";
			
			//check input against each pattern
			foreach (KeyValuePair <TokenType, string> pair in tokenPatterns) {
				Match match = Regex.Match (input, pair.Value);
				
				//return a token if match
				if (match.Success) {
					return new Token (pair.Key, match.Value);
				}
			}

			//return NOTHING!
			return null;
		}

		private static string ReplaceFirst (string text, string search, string replace) {
			int pos = text.IndexOf (search);

			if (pos < 0) {
				return text;
			}

			return text.Substring (0, pos) + replace + text.Substring (pos + search.Length);
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
