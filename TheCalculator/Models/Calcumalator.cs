
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace TheCalculator.Models {
	public static class Calcumalator {
		public static void Main (string [] args) {
			BadAssert ("(1+1*2+sin(0)", CalcumalateError.MissingCloseBracket);
			BadAssert ("1+1*1/(1+1^(sin(0))))", CalcumalateError.MissingOpenBracket);
			BadAssert ("1&1", CalcumalateError.UnknownOperator);
			BadAssert ("1_1", CalcumalateError.UnknownOperator);
			BadAssert ("1++1", CalcumalateError.SyntaxError);

			Assert ("sin(sin(0))", 0);
			Assert ("(((sin((((0)))))))", 0);
			Assert ("(-1)", -1);
			Assert ("-1", -1);
			
			Assert ("1", 1);
			Assert ("1*2+2", 4);
			Assert ("1+1*2-1*2", 1);
			Assert ("1*2^2^2", 16);
			Assert ("1*2+1+4/2", 5);
			Assert ("1", 1);
			Assert ("1+1-1", 1);
			Assert ("1-1+1", 1);
			Assert ("1+1*3", 4);
			Assert ("4/2", 2);
			Assert ("2^2", 4);

			Assert ("(1+1)*2", 4);
			
			

			Assert ("-1+1", 0);
			Assert ("1+-1", 0);
			Assert ("1+-1+1", 1);

			Assert ("sin(0)", 0);
			Assert ("tan(0)", 0);
			Assert ("cos(0)", 1);
			Assert ("tan(45)", 1);
		}

		private static List <string> functions;

		static Calcumalator () {
			functions = new List <string> { "sin", "cos", "tan" };
		}

		public static void BadAssert (string input, CalcumalateError output) {
			CalcumalateResult result = Calcumalate (input);

			if (result.Error != output) {
				Debug.WriteLine (input + " = " + output.ToString () + ", GOT " + result.Error.ToString ());
			}
		}

		public static void Assert (string input, double output) {
			CalcumalateResult result = Calcumalate (input);

			if (result.Result == output) {
				
			} else {
				Debug.WriteLine (input + " = " + output + ", GOT " + result.Result);
			}
		}

		public static CalcumalateResult Calcumalate (string input) {
			CalcumalateError bracketError = MissingBracket (input);

			if (bracketError != CalcumalateError.None) {
				return new CalcumalateResult (bracketError);
			}

			input = input.Replace (" ", "").ToLower ();
			
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
							token = "_";

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
					try {
						token = SolveList (lists).Result.ToString ();
					} catch (Exception) {
						return new CalcumalateResult (CalcumalateError.SyntaxError);
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
				try {
					result = SolveList (lists).Result;
				} catch (Exception) {
					return new CalcumalateResult (CalcumalateError.SyntaxError);
				}

				if (lists.Count > 0) {
					lists [lists.Count - 1].Add (result.ToString ());
				}
			}

			return new CalcumalateResult { Result = result };
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
			
			while (list.Count > 1) {
				double n1;
				double n2 = 0;
				string op;

				//check if first token is a number
				if (double.TryParse (list [0], out n1)) {
					//second token is the operator
					op = list [1];

					//third token is the second operand
					n2 = double.Parse (list [2]);

					//remove all 3 tokens from list
					list.RemoveRange (0, 3);
				} else {  //if first token is not a number
					//first token is an operator
					op = list [0];
					
					if (IsBinaryOperator (op)) {
						//get the first operand from the parent list
						List <string> previousList = lists [lists.Count - 2];
						n1 = double.Parse (previousList [previousList.Count - 1]);

						//remove the operand from the parent list
						previousList.RemoveAt (previousList.Count - 1);

						//second operand is after operator
						n2 = double.Parse (list [1]);
					} else {
						//unary operator
						//operand is after operator
						n1 = double.Parse (list [1]);
					}

					//remove operator and operand from child list
					list.RemoveRange (0, 2);
				}
				
				//solve the expression and put the result at the front of the list
				double value = 0;

				if (IsBinaryOperator (op)) {
					value = Solve (op, n1, n2);
				} else {
					value = Solve (op, n1);
				}

				list.Insert (0, value.ToString ());
			}
			
			double result = double.Parse (list [0]);

			if (list.Count == 1) {
				lists.RemoveAt (lists.Count - 1);
			}

			return new CalcumalateResult { Result = result };
		}

		private static bool OperatorIsHigher (string previousOp, string op) {
			List <string> prescedences = new List <string> { "+-", "*/", "^", "_" };

			//get precedence of operators if they are the basic operators
			int p = prescedences.FindIndex (a => a.Contains (op));
			int pp = previousOp == null ? -1 : prescedences.FindIndex (a => a.Contains (previousOp));

			if (p > pp) {
				return true;
			}

			if ("^_".Contains (op) || functions.Contains (op)) {
				return true;
			}

			return false;
		}

		private static bool IsNumber (string token) {
			return Regex.IsMatch (token, @"^\d*\.?\d+$");
		}

		private static bool IsOperator (string token) {
			return "+-*/^_".Contains (token) || functions.Contains (token);
		}

		private static bool IsBinaryOperator (string op) {
			return "+-*/^".Contains (op);
		}

		private static double Solve (string op, double n) {
			switch (op) {
				case "sin":
					return Math.Sin (n);
				case "cos":
					return Math.Cos (n * Math.PI / 180);
				case "tan":
					return Math.Tan (n * Math.PI / 180);
				case "_":
					return -n;
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
									 @"^" + string.Join ("|", functions),
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
