﻿

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TinyCalc.Models;
namespace TinyCalc {
	public class UnitTest2 {
		public static void Main (string [] args) {
			BadAssert ("sinawd(0)", CalcError.UnknownToken, "sinawd(0)");
			BadAssert ("sinhh(0)", CalcError.UnknownToken, "sinhh(0)");
			BadAssert ("asinh(0)", CalcError.UnknownToken, "asinh(0)");

			BadAssert ("abs1", CalcError.MissingFunctionBrackets);
			BadAssert ("sin(1", CalcError.MissingRightBracket);
			BadAssert ("abs1", CalcError.MissingFunctionBrackets);
			BadAssert ("abs1", CalcError.MissingFunctionBrackets);
			BadAssert ("(1))", CalcError.MissingLeftBracket);
			BadAssert ("((((1))", CalcError.MissingRightBracket);
			BadAssert ("(1", CalcError.MissingRightBracket);

			BadAssert ("pia", CalcError.UnknownToken, "pia");

			BadAssert ("*", CalcError.SyntaxError);
			BadAssert ("abs()", CalcError.SyntaxError);

			Assert ("sin(rad(90))", 1);
			Assert ("sin(rad(270))", -1);
			Assert ("deg(asin(1))", 90);
			Assert ("deg(asin(-1))", -90);

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
			Assert ("sin(pi/2)", 1);
			Assert ("sin(pi)", 0);
			Assert ("sin(2*pi)", 0);
			
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

		public static void BadAssert (string input, CalcError output, [CallerLineNumber] int line = 0) {
			CalcResult result = new Calc ().Solve (input);

			if (result.Error != output) {
				Debug.WriteLine ("Line " + line + ": " + input + " = " + output.ToString () + ", GOT " + result.Error.ToString ());
			}
		}

		public static void BadAssert (string input, CalcError output, string errorObject, [CallerLineNumber] int line = 0) {
			CalcResult result = new Calc ().Solve (input);

			if (result.Error != output) {
				Debug.WriteLine ("Line " + line + ": " + input + " = " + output.ToString () + ", GOT " + result.Error.ToString ());
			} else if (result.ErrorObject != errorObject) {
				Debug.WriteLine ("Line " + line + ": " + input + " = " + output.ToString () + " and " + errorObject + ", GOT " + result.Error.ToString () + " and " + result.ErrorObject);
			}
		}

		public static void Assert (string input, double output, [CallerLineNumber] int line = 0) {
			CalcResult result = new Calc ().Solve (input);
			
			if (Math.Abs (result.Result - output) <= Math.Abs (result.Result * 0.00001)) {
				
			} else if (result.Error != CalcError.None) {
				Debug.WriteLine ("Line " + line + ": " + input + " = " + output + ", GOT " + result.Error);
			} else {
				Debug.WriteLine ("Line " + line + ": " + input + " = " + output + ", GOT " + result.Result);
			}
		}

	}
}
