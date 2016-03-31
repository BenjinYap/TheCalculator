

using System;
using System.Collections.Generic;
namespace TinyCalc.Models.Modules {
	public class OperatorModule:IModule {
		private const string Negation = "_";

		private const string Addition = "+";
		private const string Subtraction = "-";
		private const string Multiplication = "*";
		private const string Division = "/";
		private const string Exponent = "^";

		private readonly List <string> tokens;

		public OperatorModule () {
			this.tokens = new List <string> {
				OperatorModule.Negation,

				OperatorModule.Addition,
				OperatorModule.Subtraction,
				OperatorModule.Multiplication,
				OperatorModule.Division,
				OperatorModule.Exponent,
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

		public bool IsSubtraction (string input) {
			return input == OperatorModule.Subtraction;
		}

		public bool IsNegation (string input) {
			return input == OperatorModule.Negation;
		}

		//assumes that op1 and op2 are both operatorr tokens
		public bool Op1PrecedenceLessOrEqualOp2 (string op1, string op2) {
			//if op1 is exponent, then op1 always has higher precedence
			if (this.IsExponent (op1)) {
				return false;
			}

			List <List <string>> operators = new List <List <string>> {
				new List <string> {  //first level of precedence
					OperatorModule.Addition,
					OperatorModule.Subtraction,
				},
				new List <string> {  //second level of precedence
					OperatorModule.Multiplication,
					OperatorModule.Division,
				},
				new List <string> {  //thid level of precedence
					OperatorModule.Negation,
				},
			};

			int index1 = -1;  //precedence of op1
			int index2 = -1;  //precedence of op2

			//determine the precedences of both ops
			for (int i = 0; i < operators.Count; i++) {
				if (operators [i].Contains (op1)) {
					index1 = i;
				}

				if (operators [i].Contains (op2)) {
					index2 = i;
				}
			}
			
			//compare the precedences and return
			return index1 <= index2;
		}

		public bool IsExponent (string input) {
			return input == OperatorModule.Exponent;
		}

		public double Solve (string num, string op) {
			double n = double.Parse (num);

			switch (op) {
				case OperatorModule.Negation:
					return -n;
			}

			return double.NaN;
		}

		public double Solve (string num1, string num2, string op) {
			double n1 = double.Parse (num1);
			double n2 = double.Parse (num2);

			switch (op) {
				case OperatorModule.Addition:
					return n1 + n2;
				case OperatorModule.Subtraction:
					return n1 - n2;
				case OperatorModule.Multiplication:
					return n1 * n2;
				case OperatorModule.Division:
					return n1 / n2;
				case OperatorModule.Exponent:
					return Math.Pow (n1, n2);
			}

			return double.NaN;
		}
	}
}
