

using System;
using System.Collections.Generic;
namespace TinyCalc.Models.Modules {
	public class ConstantModule:IModule {
		private const string Pi = "pi";
		private const string PiSymbol = "π";
		private const string Answer = "ans";

		public double PreviousAnswer = 0;

		public bool IsToken (string input) {
			List <string> tokens = new List <string> {
				ConstantModule.Pi,
				ConstantModule.PiSymbol,
				ConstantModule.Answer,
			};

			return tokens.Contains (input);
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
