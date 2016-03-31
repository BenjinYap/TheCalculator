

using System;
using System.Diagnostics;
using TinyCalc.Models;
namespace TinyCalc {
	public class UnitTest2 {
		public static void Main (string [] args) {
			BadAssert ("sinawd(0)", CalcError.UnknownToken);
			BadAssert ("sinhh(0)", CalcError.UnknownToken);
			BadAssert ("asinh(0)", CalcError.UnknownToken);

			BadAssert ("abs1", CalcError.MissingFunctionBrackets);
			BadAssert ("sin(1", CalcError.MissingRightBracket);
			BadAssert ("abs1", CalcError.MissingFunctionBrackets);
			BadAssert ("abs1", CalcError.MissingFunctionBrackets);
			BadAssert ("(1))", CalcError.MissingLeftBracket);
			BadAssert ("((((1))", CalcError.MissingRightBracket);
			BadAssert ("(1", CalcError.MissingRightBracket);

			BadAssert ("pia", CalcError.UnknownToken, "pia");

			Assert ("abs(5)", 5);
			Assert ("-abs(-5)", -5);

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
			Assert ("π", Math.PI);
			Assert ("π+pi", Math.PI * 2);

			Assert ("pi+1", Math.PI + 1);
			Assert ("-1+1", 0);
			Assert ("1---1", 0);
			Assert ("-1", -1);
			Assert ("-----1", -1);
			Assert ("-(-(-1))", -1);
			Assert ("--1-1", 0);
			Assert ("-1^2", -1);

			Assert ("((1))", 1);
			Assert ("(1+1)", 2);
			Assert ("(1+1)*2", 4);

			Assert ("pi", Math.PI);
			Assert ("1.557", 1.557);
			Assert ("-1.557", -1.557);

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

			Assert ("1+2-3+4", 4);
			Assert ("1*2+3", 5);
			Assert ("1+2^3", 9);
			Assert ("1+2*3", 7);
		}

		public static void BadAssert (string input, CalcError output) {
			CalcResult result = new Calc ().Solve (input);

			if (result.Error != output) {
				Debug.WriteLine (input + " = " + output.ToString () + ", GOT " + result.Error.ToString ());
			}
		}

		public static void BadAssert (string input, CalcError output, string errorObject) {
			CalcResult result = new Calc ().Solve (input);

			if (result.Error != output) {
				Debug.WriteLine (input + " = " + output.ToString () + ", GOT " + result.Error.ToString ());
			} else if (result.ErrorObject != errorObject) {
				Debug.WriteLine (input + " = " + output.ToString () + " and " + errorObject + ", GOT " + result.Error.ToString () + " and " + result.ErrorObject);
			}
		}

		public static void Assert (string input, double output) {
			CalcResult result = new Calc ().Solve (input);
			
			if (Math.Abs (result.Result - output) <= Math.Abs (result.Result * 0.00001)) {
				
			} else if (result.Error != CalcError.None) {
				Debug.WriteLine (input + " = " + output + ", GOT " + result.Error);
			} else {
				Debug.WriteLine (input + " = " + output + ", GOT " + result.Result);
			}
		}

	}
}
