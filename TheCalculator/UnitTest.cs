

using System;
using System.Diagnostics;
using TheCalculator.Models;
namespace TheCalculator {
	public class UnitTest {
		public static void Main (string [] args) {
			BadAssert ("(1+1*2+sin(0)", CalcumalateError.MissingCloseBracket);
			BadAssert ("1+1*1/(1+1^(sin(0))))", CalcumalateError.MissingOpenBracket);
			BadAssert ("1&1", CalcumalateError.UnknownOperator);
			BadAssert ("1_1", CalcumalateError.UnknownOperator);
			BadAssert ("1++1", CalcumalateError.SyntaxError);
			BadAssert ("1**1", CalcumalateError.SyntaxError);
			BadAssert ("*/1--1", CalcumalateError.SyntaxError);
			BadAssert ("_!41--1", CalcumalateError.UnknownOperator);
			BadAssert ("++++++++", CalcumalateError.SyntaxError);
			BadAssert (")))(((", CalcumalateError.SyntaxError);
			BadAssert ("8-^738^2^^", CalcumalateError.SyntaxError);
			BadAssert ("9^4/*035*1", CalcumalateError.SyntaxError);
			BadAssert ("3^2/07+", CalcumalateError.SyntaxError);
			BadAssert ("+636/9927-", CalcumalateError.SyntaxError);
			BadAssert ("2/3912-15*", CalcumalateError.SyntaxError);
			BadAssert ("0367315^5/", CalcumalateError.SyntaxError);
			BadAssert ("1847/1-24+", CalcumalateError.SyntaxError);

			Assert ("-cos(0)", -1);
			Assert ("-1+1", 0);
			Assert ("1--1", 2);
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
			
			Assert ("1+-1", 0);
			Assert ("1+-1+1", 1);

			Assert ("sin(0)", 0);
			Assert ("tan(0)", 0);
			Assert ("cos(0)", 1);
			Assert ("tan(0)", 0);
			Assert ("sinh(0)", 0);
			Assert ("cosh(0)", 1);
			Assert ("tanh(0)", 0);
			Assert ("asin(0)", 0);
			Assert ("acos(1)", 0);
			Assert ("atan(0)", 0);
			Assert ("abs(-1)", 1);
			Assert ("abs(1)", 1);
			Assert ("abs(-1+abs(-1))", 0);
			
			Assert ("pi", Math.PI);
		}

		public static void BadAssert (string input, CalcumalateError output) {
			CalcumalateResult result = Calcumalator.Calcumalate (input);

			if (result.Error != output) {
				Debug.WriteLine (input + " = " + output.ToString () + ", GOT " + result.Error.ToString ());
			}
		}

		public static void Assert (string input, double output) {
			CalcumalateResult result = Calcumalator.Calcumalate (input);

			if (Math.Abs (result.Result - output) <= Math.Abs (result.Result * 0.00001)) {
				
			} else {
				Debug.WriteLine (input + " = " + output + ", GOT " + result.Result);
			}
		}

	}
}
