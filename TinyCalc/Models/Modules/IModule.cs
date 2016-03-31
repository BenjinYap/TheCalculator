

namespace TinyCalc.Models.Modules {
	public interface IModule {
		string GetNextToken (string input);

		bool IsToken (string input);
	}
}
