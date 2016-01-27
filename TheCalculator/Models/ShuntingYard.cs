

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace TheCalculator.Models {
	public static class ShuntingYard {

		public static void Main (string [] args) {
			ShuntResult awd = Shunt ("2 + (-1) - 1");
			
			if (awd.Error == ShuntError.None) {
				Debug.WriteLine (awd.Result);
			} else {
				Debug.WriteLine (awd.Error);
			}
		}

		public static ShuntResult Shunt (string input) {
			input = input.Replace (" ", "").ToLower ();

			Stack <Operator> operatorStack = new Stack <Operator> ();
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
						Operator op = Operator.Parse (token.Value);

						//if left bracket push immediately and stop
						if (op.Value == "(") {
							operatorStack.Push (op);
						} else if (op.Value == ")") {
							//if right bracket, pop stack into output until left bracket is found
							while (operatorStack.Count > 0) {
								Operator topOperator = operatorStack.Pop ();
								
								//found left bracket, throw away the bracket and stop
								if (topOperator.Value == "(") {
									break;
								} else {
									//not left bracket, push to output
									output += topOperator.Value + " ";
								}
							}
						} else {
							//check if this is a minus sign for a negative number
							if (token.Value == "-") {
								if (operatorStack.Count > 0 && operatorStack.Peek ().Value == "(") {
									output += token.Value;
									break;
								}

								////peek the next token
								//Token nextToken = ShuntingYard.GetToken (input);

								////left bracket or number means negative number, push minus sign to output without a space after
								//if (nextToken.Value == "(" || nextToken.Type == TokenType.Number) {
								//	output += token.Value;
								//	break;
								//}
							}

							//pop operators into output until a lower precedence operator is at the top of stack
							while (operatorStack.Count > 0) {
								Operator topOperator = operatorStack.Peek ();
								
								//check if top operator is higher or equal precedence
								if (topOperator.Value != "(" &&
									op.Associativity == OperatorAssociativity.Left && op.Precedence <= topOperator.Precedence ||
									op.Associativity == OperatorAssociativity.Right && op.Precedence < topOperator.Precedence) {
									//pop operator into output
									output += operatorStack.Pop ().Value + " ";
								} else {
									//found lower precedence, stop
									break;
								}
							}

							//push operator to stack
							operatorStack.Push (op);
						}

						break;
					case TokenType.Function:
						//push to stack
						operatorStack.Push (Operator.Parse (token.Value));
						break;
				}
			}

			//pop all remaining operators onto stack
			while (operatorStack.Count > 0) {
				output += operatorStack.Pop ().Value + " ";
			}
			
			//trim the trailing space
			output = output.TrimEnd (' ');

			//return the result
			return new ShuntResult { Error = ShuntError.None, Result = output };
		}

		private static Token GetToken (string input) {
			//create the token type regex patterns
			Dictionary <TokenType, string> tokenPatterns = new Dictionary <TokenType, string> ();
			tokenPatterns [TokenType.Number] = @"^\d+";
			tokenPatterns [TokenType.Operator] = @"^[+\-*/()^]";
			tokenPatterns [TokenType.Function] = @"^sin|cos|tan";

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
			Function,
		}

		private class Operator {
			public string Value;
			public int Precedence;
			public OperatorAssociativity Associativity;

			public static Operator Parse (string value) {
				Operator op = new Operator ();
				op.Value = value;

				//set the operator precedence based on the value
				string [] precedences = { "+-", "*/", "^", "sincostan" };

				for (int i = 0; i < precedences.Length; i++) {
					if (precedences [i].Contains (value)) {
						op.Precedence = i;
						break;
					}
				}

				//set the operator associativity based on the value
				if (value == "^") {
					op.Associativity = OperatorAssociativity.Right;
				} else {
					op.Associativity = OperatorAssociativity.Left;
				}

				return op;
			}
		}

		private enum OperatorAssociativity {
			Left,
			Right
		}

		public struct ShuntResult {
			public ShuntError Error;
			public string Result;
		}

		public enum ShuntError {
			None,

		}
	}
}
