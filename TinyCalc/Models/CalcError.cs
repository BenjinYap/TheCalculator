

namespace TinyCalc.Models {
	public enum CalcError {
		None,
		Unknown,
		InfiniteLoop,

		MissingLeftBracket,
		MissingRightBracket,
	}
}
