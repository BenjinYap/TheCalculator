

namespace TinyCalc.Models.Modules {
	public class FunctionModule:IModule {

		public bool IsToken (string input) {
			return true;
		}

		public string GetNextToken (string input) {
			return "";
		}
	}
}
