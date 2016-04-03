

namespace TinyCalc.ViewModels {
	public class AutocompleteItem {
		public AutoCompleteItemType Type { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public AutocompleteItem (AutoCompleteItemType type, string name, string description) {
			this.Type = type;
			this.Name = name;
			this.Description = description;
		}
	}

	public enum AutoCompleteItemType {
		Constant,
		Function,
	}
}
