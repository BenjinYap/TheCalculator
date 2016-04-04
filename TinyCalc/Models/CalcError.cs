

namespace TinyCalc.Models {
	public enum CalcError {
		None,
		Unknown,
		InfiniteLoop,
		UnknownToken,
		SyntaxError,

		MissingLeftBracket,
		MissingRightBracket,
		MissingFunctionBrackets,
	}
}
