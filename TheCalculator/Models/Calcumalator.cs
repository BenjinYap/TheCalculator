
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace TheCalculator.Models {
	public static class Calcumalator {
		
		private static List <string> operators;
		private static List <string> functions;
		private static List <string> constants;

		static Calcumalator () {
			operators = new List <string> { "+", "-", "*", "/", "^" };
			functions = new List <string> { "asin", "acos", "atan", "sinh", "cosh", "tanh", "sin", "cos", "tan", "abs", "neg" };
			constants = new List <string> { "π", "pi" };
		}

		public static CalcumalateResult Calcumalate (string input) {
			string rawInput = input;
			input = input.Replace (" ", "").ToLower ();

			CalcumalateError precheckError = PrecheckErrors (input);

			if (precheckError != CalcumalateError.None) {
				return new CalcumalateResult (precheckError);
			}

			List <List <string>> lists = new List <List <string>> ();
			lists.Add (new List <string> ());
			List <string> currentList = lists [0];

			string previousOperator = null;
			string previousToken = null;

			while (input.Length > 0) {
				string token = GetNextToken (input);
			
				input = ReplaceFirst (input, token, "");

				if (token == "") {
					return new CalcumalateResult (CalcumalateError.UnknownOperator);
				}

				if (IsOperator (token)) {
					if (OperatorIsHigher (previousOperator, token) && currentList.Count > 0) {
						lists.Add (new List <string> ());
						currentList = lists [lists.Count - 1];
					}

					if (token == "-") {
						if (previousToken == null || IsOperator (previousToken)) {
							token = "neg";

							if (currentList.Count > 0) {
								lists.Add (new List <string> ());
								currentList = lists [lists.Count - 1];
							}
						}
					}

					previousOperator = token;
				} else if (token == "(") {
					if (currentList.Count > 0) {
						lists.Add (new List <string> ());
						currentList = lists [lists.Count - 1];
					}

					continue;
				} else if (token == ")") {
					CalcumalateResult res = SolveList (lists);

					if (res.Error != CalcumalateError.None) {
						return new CalcumalateResult (res.Error);
					} else {
						token = res.Result.ToString ();
					}
					
					if (lists.Count <= 0) {
						lists.Add (new List <string> ());
					}

					currentList = lists [lists.Count - 1];
				}

				currentList.Add (token);
				previousToken = token;
			}

			double result = 0;
			
			while (lists.Count > 0) {
				CalcumalateResult res = SolveList (lists);

				if (res.Error != CalcumalateError.None) {
					return new CalcumalateResult (res.Error);
				} else {
					result = res.Result;
				}
				
				if (lists.Count > 0) {
					lists [lists.Count - 1].Add (result.ToString ());
				}
			}

			return new CalcumalateResult { Result = result };
		}

		private static CalcumalateError PrecheckErrors (string input) {
			CalcumalateError bracketError = MissingBracket (input);

			if (bracketError != CalcumalateError.None) {
				return bracketError;
			}
			
			MatchCollection matches = Regex.Matches (input, @"[+\-*/^]");
			
			if (matches.Count > 1) {
				for (int i = 0; i < matches.Count - 1; i++) {
					Match m1 = matches [i];
					Match m2 = matches [i + 1];
					
					if (m2.Index - m1.Index <= 1) {
						if (m2.Value != "-") {
							return CalcumalateError.SyntaxError;
						}
					}
				}
			}

			return CalcumalateError.None;
		}

		private static CalcumalateError MissingBracket (string input) {
			MatchCollection open = Regex.Matches (input, @"\(");
			MatchCollection close = Regex.Matches (input, @"\)");
			
			if (open.Count < close.Count) {
				return CalcumalateError.MissingOpenBracket;
			} else if (open.Count > close.Count) {
				return CalcumalateError.MissingCloseBracket;
			}

			return CalcumalateError.None;
		}

		private static CalcumalateResult SolveList (List <List <string>> lists) {
			List <string> list = lists [lists.Count - 1];

			//keep processing when there are things in the list
			while (list.Count > 0) {
				string token = list [0];

				//if token is a math constant, replace the token with the actual value
				if (IsConstant (token)) {
					list.RemoveAt (0);
					list.Insert (0, Solve (token).ToString ());
				} else if (IsBinaryOperator (token)) {  //if token is binary operator at beginning of list
					//if there isn't a previous list, something is wrong
					if (lists.Count < 2) {
						return new CalcumalateResult (CalcumalateError.SyntaxError);
					}
					
					List <string> previousList = lists [lists.Count - 2];

					//get the first operand from the previous list
					double n1 = double.Parse (previousList [previousList.Count - 1]);
					string op = token;  //current token is the binary operator
					double n2;  //second operand is after the operator

					//if the second operand isn't there, something is wrong
					if (double.TryParse (list [1], out n2) == false) {
						return new CalcumalateResult (CalcumalateError.SyntaxError);
					}

					//remove the tokens from both lists
					previousList.RemoveAt (previousList.Count - 1);
					list.RemoveRange (0, 2);

					//solve and insert result to front of current list
					list.Insert (0, Solve (op, n1, n2).ToString ());
				} else if (IsFunction (token)) {  //token is a function
					string op = list [0];
					double n;  //operand is after the operator

					//if operand isn't there, something is wrong
					if (list.Count < 2 || double.TryParse (list [1], out n) == false) {
						return new CalcumalateResult (CalcumalateError.SyntaxError);
					}

					list.RemoveRange (0, 2);  //remove tokens from list
					list.Insert (0, Solve (op, n).ToString ());  //solve and insert to front of list
				} else if (list.Count >= 3) {  //token is a number and there are at least 3 tokens
					//treat this as a binary operation
					double n1 = double.Parse (list [0]);  //first operand is current token
					string op = list [1];  //operator is after first operand
					double n2;  //second operand is after operator

					//if the second operand isn't there, something is wrong
					if (double.TryParse (list [2], out n2) == false) {
						return new CalcumalateResult (CalcumalateError.SyntaxError);
					}

					list.RemoveRange (0, 3);  //remove tokens from list
					list.Insert (0, Solve (op, n1, n2).ToString ());  //solve and insert result to front of list
				}

				//if the list only have one token left, stop the loop
				if (list.Count == 1) {
					break;
				}
			}

			
			double result = double.NaN;  //the remaining token is a number

			//if number isn't there, something is wrong
			if (list.Count < 1 || double.TryParse (list [0], out result) == false) {
				return new CalcumalateResult (CalcumalateError.SyntaxError);
			}

			//remove the now empty list from the list of lists
			lists.RemoveAt (lists.Count - 1);
			
			return new CalcumalateResult { Result = result };
		}

		private static bool OperatorIsHigher (string previousOp, string op) {
			List <string> prescedences = new List <string> { "+-", "*/", "^" };

			//get precedence of operators if they are the basic operators
			int p = prescedences.FindIndex (a => a.Contains (op));
			int pp = previousOp == null ? -1 : prescedences.FindIndex (a => a.Contains (previousOp));

			if (p > -1 && pp > -1 && p > pp) {
				return true;
			}

			if (op == "^" || functions.Contains (op)) {
				return true;
			}

			return false;
		}

		private static bool IsNumber (string token) {
			return Regex.IsMatch (token, @"^\d*\.?\d+$");
		}

		private static bool IsConstant (string token) {
			return constants.Contains (token);
		}

		private static bool IsOperator (string token) {
			return "+-*/^_".Contains (token) || functions.Contains (token);
		}

		private static bool IsBinaryOperator (string op) {
			return "+-*/^".Contains (op);
		}

		private static bool IsFunction (string op) {
			return functions.Contains (op);
		}

		private static double Solve (string constant) {
			switch (constant) {
				case "π":
				case "pi":
					return Math.PI;
			}

			return double.NaN;
		}

		private static double Solve (string op, double n) {
			switch (op) {
				case "sin":
					return Math.Sin (n);
				case "cos":
					return Math.Cos (n);
				case "tan":
					return Math.Tan (n);
				case "sinh":
					return Math.Sinh (n);
				case "cosh":
					return Math.Cosh (n);
				case "tanh":
					return Math.Tanh (n);
				case "asin":
					return Math.Asin (n);
				case "acos":
					return Math.Acos (n);
				case "atan":
					return Math.Atan (n);
				case "neg":
					return -n;
				case "abs":
					return Math.Abs (n);
			}

			return double.NaN;
		}

		private static double Solve (string op, double n1, double n2) {
			switch (op) {
				case "+":
					return n1 + n2;
				case "-":
					return n1 - n2;
				case "*":
					return n1 * n2;
				case "/":
					return n1 / n2;
				case "^":
					return Math.Pow (n1, n2);
			}

			return double.NaN;
		}

		private static void ReplaceStackItems (List <string> stack, int count, string value) {
			stack.RemoveRange (0, count);
			stack.Insert (0, value);
		}

		private static string GetNextToken (string input) {
			string [] patterns = {
									 @"^\d+",
									 @"^[+\-*/()^]",
								 };
			
			//check input against each pattern
			foreach (string pattern in patterns) {
				Match match = Regex.Match (input, pattern);
				
				//return a token if match
				if (match.Success) {
					return match.Value;
				}
			}

			//match against the lists of tokens that are words
			List <string> [] tokenWords = { functions, constants };

			foreach (List <string> words in tokenWords) {
				string m = words.Find (a => input.IndexOf (a) == 0);
				
				if (m != null) {
					return m;
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
	}

	public class CalcumalateResult {
		public bool Success;

		private double result;
		public double Result {
			get { return this.result; }
			set {
				this.result = value;
				this.Success = true;
				this.Error = CalcumalateError.None;
			}
		}

		private CalcumalateError error;
		public CalcumalateError Error {
			get { return this.error; }
			set {
				this.error = value;

				if (value != CalcumalateError.None) {
					this.result = double.NaN;
					this.Success = false;
				}
			}
		}

		public CalcumalateResult () {
			
		}

		public CalcumalateResult (CalcumalateError error) {
			if (error != CalcumalateError.None) {
				this.Success = false;
				this.Error = error;
			}
		}
	}

	public enum CalcumalateError {
		None,
		MissingOpenBracket,
		MissingCloseBracket,
		UnknownOperator,
		SyntaxError,
	}
}
