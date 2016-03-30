

using System;
using System.Diagnostics;
using TinyCalc.Models;
namespace TinyCalc {
	public class UnitTest2 {
		public static void Main (string [] args) {
			Assert ("1+1", 2);
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
				
			} else {
				Debug.WriteLine (input + " = " + output + ", GOT " + result.Result);
			}
		}

	}
}
