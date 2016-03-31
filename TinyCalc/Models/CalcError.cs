

namespace TinyCalc.Models {
	public enum CalcError {
		None,
		Unknown,
		InfiniteLoop,
		UnknownToken,

		MissingLeftBracket,
		MissingRightBracket,
		MissingFunctionBrackets,
	}
}
