

using System.Text.RegularExpressions;
namespace TinyCalc.Models.Modules {
	public class CoreModule:IModule {

		public bool IsToken (string input) {
			if (this.IsNumber (input)) {
				return true;
			}

			return false;
		}

		private bool IsNumber (string input) {
			return Regex.IsMatch (input, @"^\d*\.?\d+$");
		}
	}
}
