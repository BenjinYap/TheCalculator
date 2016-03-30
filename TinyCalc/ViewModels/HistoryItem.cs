

namespace TinyCalc.ViewModels {
	public class HistoryItem {
		public string Input { get; set; }
		public double Output { get; set; }

		public HistoryItem (string input, double output) {
			this.Input = input;
			this.Output = output;
		}
	}
}
