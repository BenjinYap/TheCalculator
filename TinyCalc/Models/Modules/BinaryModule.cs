

using System;
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

		//assumes that op1 and op2 are both binary tokens
		public bool Op1PrecedenceLessOrEqualOp2 (string op1, string op2) {
			//if op1 is exponent, then op1 always has higher precedence
			if (this.IsExponent (op1)) {
				return false;
			}

			List <List <string>> operators = new List <List <string>> {
				new List <string> {  //first level of precedence
					BinaryModule.Addition,
					BinaryModule.Subtraction,
				},
				new List <string> {  //second level of precedence
					BinaryModule.Multiplication,
					BinaryModule.Division,
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
			return input == BinaryModule.Exponent;
		}

		public double Solve (string num1, string num2, string op) {
			double n1 = double.Parse (num1);
			double n2 = double.Parse (num2);

			switch (op) {
				case BinaryModule.Addition:
					return n1 + n2;
				case BinaryModule.Subtraction:
					return n1 - n2;
				case BinaryModule.Multiplication:
					return n1 * n2;
				case BinaryModule.Division:
					return n1 / n2;
				case BinaryModule.Exponent:
					return Math.Pow (n1, n2);
			}

			return double.NaN;
		}
	}
}
