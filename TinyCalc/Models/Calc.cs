

namespace TinyCalc.Models {
	public sealed class Calc {

		public Calc () {
			
		}

		public CalcResult Solve (string input) {
			return new CalcResult (1);
		}
	}

	public class CalcResult {
		private double result;
		public double Result { get { return this.result; } }

		private CalcError error;
		public CalcError Error { get { return this.error; } }

		public CalcResult (double result) {
			this.result = result;
			this.error = null;
		}

		public CalcResult (CalcError error) {
			this.result = double.NaN;
			this.error = error;
		}
	}
}
