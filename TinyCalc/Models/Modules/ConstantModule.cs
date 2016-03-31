

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace TinyCalc.Models.Modules {
	public class ConstantModule:IModule {
		private const string Pi = "pi";
		private const string PiSymbol = "π";
		private const string Answer = "ans";

		public double PreviousAnswer = 0;

		private readonly List <string> tokens;

		public ConstantModule () {
			this.tokens = new List <string> {
				ConstantModule.Pi,
				ConstantModule.PiSymbol,
				ConstantModule.Answer,
			};
		}

		public string GetNextToken (string input) {
			Match match = Regex.Match (input, "^(" + string.Join ("|", this.tokens) + @")(?=[^a-zA-Z]|$)");

			if (match.Success) {
				return match.Value;
			}

			return "";
		}

		public bool IsToken (string input) {
			return this.tokens.Contains (input);
		}

		public void SolveConstants (List <string> tokens) {
			for (int i = 0; i < tokens.Count; i++) {
				if (this.IsToken (tokens [i])) {
					tokens [i] = this.Solve (tokens [i]).ToString ();
				}
			}
		}

		public double Solve (string input) {
			switch (input) {
				case ConstantModule.Pi:
				case ConstantModule.PiSymbol:
					return Math.PI;
				case ConstantModule.Answer:
					return this.PreviousAnswer;
			}

			return double.NaN;
		}
	}
}
