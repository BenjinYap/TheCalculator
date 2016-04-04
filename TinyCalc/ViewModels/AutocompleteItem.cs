

namespace TinyCalc.ViewModels {
	public class AutocompleteItem {
		public AutocompleteItemType Type { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public AutocompleteItem (AutocompleteItemType type, string name, string description) {
			this.Type = type;
			this.Name = name;
			this.Description = description;
		}
	}

	public enum AutocompleteItemType {
		Constant,
		Function,
	}
}
