

using System;
using System.Diagnostics;
using TinyCalc.Models;
namespace TinyCalc {
	public class UnitTest2 {
		public static void Main (string [] args) {
			Assert ("pi", Math.PI);
			Assert ("1.557", 1.557);

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

			Assert ("1+2-3+4", 1);
			Assert ("1*2+3", 5);
			Assert ("1+2^3", 1);
			Assert ("1+2*3", 1);
			Assert ("1+2-3/4*5", 1);
		}

		public static void BadAssert (string input, CalcumalateError output) {
			CalcumalateResult result = Calcumalator.Calcumalate (input);

			if (result.Error != output) {
				Debug.WriteLine (input + " = " + output.ToString () + ", GOT " + result.Error.ToString ());
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
