
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace TheCalculator.Models {
	public static class Calcumalator {
		public static void Main (string [] args) {
			Debug.WriteLine (Calcumalate ("1+1"));
		}

		public static double Calcumalate (string input) {
			input = input.Replace (" ", "").ToLower ();

			Stack <List <string>> stacks = new Stack <List <string>> ();
			stacks.Push (new List <string> ());
			List <string> topStack = stacks.Peek ();

			while (input.Length > 0) {
				string token = GetNextToken (input);
			
				input = ReplaceFirst (input, token, "");

				topStack.Add (token);
			}

			while (topStack.Count > 1) {
				double n2 = double.Parse (topStack [topStack.Count - 1]);

				if (topStack.Count > 2) {
					string op = topStack [topStack.Count - 2];

					if ("+-*/^".Contains (op)) {
						double n1 = double.Parse (topStack [topStack.Count - 3]);
						double value = 0;

						switch (op) {
							case "+":
								value = n1 + n2;
								break;
						}

						ReplaceStackItems (topStack, 3, value.ToString ());
					} else {

					}
				}
			}

			return double.Parse (topStack [0]);
		}

		private static void ReplaceStackItems (List <string> stack, int n, string value) {
			stack.RemoveRange (stack.Count - n, n);
			stack.Add (value);
		}

		private static string GetNextToken (string input) {
			string [] patterns = {
									 @"^\d+",
									 @"^[+\-*/()^]",
									 @"^sin|cos|tan",
								 };
			
			//check input against each pattern
			foreach (string pattern in patterns) {
				Match match = Regex.Match (input, pattern);
				
				//return a token if match
				if (match.Success) {
					return match.Value;
				}
			}

			//return NOTHING!
			return "";
		}

		private static string ReplaceFirst (string text, string search, string replace) {
			int pos = text.IndexOf (search);

			if (pos < 0) {
				return text;
			}

			return text.Substring (0, pos) + replace + text.Substring (pos + search.Length);
		}

		private class Token {
			public string Value;
			public TokenType Type;
			public int Precedence;

			public Token (string value) {
				this.Value = value;
				this.Type = TokenType.Operator;

				//set the operator precedence based on the value
				string [] precedences = { "+-", "*/", "^", "sincostan", "()" };

				for (int i = 0; i < precedences.Length; i++) {
					if (precedences [i].Contains (value)) {
						this.Precedence = i;
						break;
					}
				}
			}

			public override string ToString () {
				 return this.Value;
			}
		}

		private enum TokenType {
			Number,
			Operator
		}
	}
}
