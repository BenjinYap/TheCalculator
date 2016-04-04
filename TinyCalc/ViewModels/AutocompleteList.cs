﻿

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TinyCalc.Models.Modules;
namespace TinyCalc.ViewModels {
	public class AutocompleteList:ObservableCollection <AutocompleteItem> {
		private int selectedIndex;
		public int SelectedIndex { 
			get { return this.selectedIndex; }
			set {
				this.selectedIndex = value;
				this.OnPropertyChanged (new PropertyChangedEventArgs ("SelectedIndex"));
			}
		}

		public string SelectedItemName {
			get { return this [this.SelectedIndex].Name; }
		}

		public bool IsPopulated { get { return this.Count > 0; } }

		private List <AutocompleteItem> allItems = new List <AutocompleteItem> ();

		public AutocompleteList () {
			this.SelectedIndex = -1;

			List <string> tokens = new FunctionModule ().GetTokens ();

			foreach (string token in tokens) {
				this.allItems.Add (new AutocompleteItem (AutoCompleteItemType.Function, token, token));
			}
		}

		private void Reset () {
			this.Clear ();
			this.SelectedIndex = -1;
		}

		public void Populate (string token) {
			this.Reset ();

			if (string.IsNullOrWhiteSpace (token)) {
				return;
			}

			//populate autocomplete list with items that contain the token
			for (int i = 0; i < this.allItems.Count; i++) {
				AutocompleteItem item = this.allItems [i];

				if (item.Name.Contains (token)) {
					this.Add (item);
				}
			}

			//set selected item to the first item that matches the token from the start of the string
			for (int i = 0; i < this.Count; i++) {
				if (this [i].Name.IndexOf (token) == 0 && this.SelectedIndex == -1) {
					this.SelectedIndex = i;
				}
			}
		}
	}
}
