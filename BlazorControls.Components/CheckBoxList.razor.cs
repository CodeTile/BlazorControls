using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Components;

namespace BlazorControls.Components
{
	/// <summary>
	/// A reusable checkbox list component that supports:
	/// - Object lists and string lists
	/// - Auto-selection on startup
	/// - UncheckedInitially for default unchecked items
	/// - Two-way binding via @bind-SelectedMap
	/// - Positional maps for objects, key-only maps for strings
	/// </summary>
	public partial class CheckBoxList<TItem> : ComponentBase
	{
		/// <summary>
		/// The data source for the checkbox list.
		/// </summary>
		[Parameter] public IEnumerable<TItem> Data { get; set; } = Enumerable.Empty<TItem>();

		/// <summary>
		/// Function to extract display text from an item.
		/// </summary>
		[Parameter] public Func<TItem, string>? TextField { get; set; }

		/// <summary>
		/// Function to extract value from an item.
		/// </summary>
		[Parameter] public Func<TItem, string>? ValueField { get; set; }

		/// <summary>
		/// The list of selected values.
		/// </summary>
		[Parameter] public List<string> SelectedValues { get; set; } = new();

		/// <summary>
		/// Raised when SelectedValues changes.
		/// </summary>
		[Parameter] public EventCallback<List<string>> SelectedValuesChanged { get; set; }

		/// <summary>
		/// The list of selected display texts.
		/// </summary>
		[Parameter] public List<string> SelectedTexts { get; set; } = new();

		/// <summary>
		/// Raised when SelectedTexts changes.
		/// </summary>
		[Parameter] public EventCallback<List<string>> SelectedTextsChanged { get; set; }

		private Dictionary<string, int> _selectedMap = new();

		/// <summary>
		/// Two-way bound map of selected items.
		/// For string lists: key-only (value = 0).
		/// For object lists: key:index.
		/// </summary>
		[Parameter]
		public Dictionary<string, int> SelectedMap
		{
			get => _selectedMap;
			set
			{
				if (!ReferenceEquals(_selectedMap, value))
				{
					_selectedMap = value ?? new();
					SelectedMapChanged.InvokeAsync(_selectedMap);
				}
			}
		}

		/// <summary>
		/// Raised when SelectedMap changes.
		/// </summary>
		[Parameter] public EventCallback<Dictionary<string, int>> SelectedMapChanged { get; set; }

		/// <summary>
		/// Items listed here will start unchecked.
		/// </summary>
		[Parameter] public IEnumerable<string>? UncheckedInitially { get; set; }

		private bool _initialized = false;

		private bool IsStringList => typeof(TItem) == typeof(string);

		protected override void OnAfterRender(bool firstRender)
		{
			if (firstRender && !_initialized)
			{
				_initialized = true;

				InitializeSelections();
				BuildMap();
				EmitAll();
			}
		}

		/// <summary>
		/// Auto-selects all items except those in UncheckedInitially.
		/// </summary>
		private void InitializeSelections()
		{
			foreach (var item in Data)
			{
				string text = GetText(item);
				string value = GetValue(item);

				bool shouldStartUnchecked =
					UncheckedInitially != null &&
					UncheckedInitially.Contains(text);

				if (!shouldStartUnchecked)
				{
					if (!SelectedValues.Contains(value))
						SelectedValues.Add(value);

					if (!SelectedTexts.Contains(text))
						SelectedTexts.Add(text);
				}
			}
		}

		/// <summary>
		/// Handles checkbox toggle events.
		/// </summary>
		public void ToggleValue(string value, string text, object changed)
		{
			bool isChecked = changed is bool b && b;

			if (isChecked)
			{
				if (!SelectedValues.Contains(value))
					SelectedValues.Add(value);

				if (!SelectedTexts.Contains(text))
					SelectedTexts.Add(text);
			}
			else
			{
				SelectedValues.Remove(value);
				SelectedTexts.Remove(text);
			}

			BuildMap();
			EmitAll();
		}

		/// <summary>
		/// Builds the SelectedMap based on current selections.
		/// </summary>
		private void BuildMap()
		{
			// Case 1: String list → keys only
			if (IsStringList)
			{
				SelectedMap = SelectedValues
					.Distinct()
					.ToDictionary(v => v, v => 0);

				return;
			}

			// Case 2: Dictionary<string,int> → use dictionary values
			if (Data is IEnumerable<KeyValuePair<string, int>> kvps)
			{
				var dict = kvps.ToDictionary(k => k.Key, k => k.Value);

				SelectedMap = SelectedValues
					.Where(v => dict.ContainsKey(v))
					.ToDictionary(v => v, v => dict[v]);

				return;
			}

			// Case 3: Object list → positional index
			SelectedMap = SelectedValues
				.Select((v, i) => new { v, i })
				.ToDictionary(x => x.v, x => x.i);
		}

		/// <summary>
		/// Emits all change events.
		/// </summary>
		private void EmitAll()
		{
			SelectedValuesChanged.InvokeAsync(SelectedValues);
			SelectedTextsChanged.InvokeAsync(SelectedTexts);
			StateHasChanged();
		}

		private string GetText(TItem item)
		{
			if (TextField != null)
				return TextField(item);

			return item?.ToString() ?? string.Empty;
		}

		private string GetValue(TItem item)
		{
			if (ValueField != null)
				return ValueField(item);

			return item?.ToString() ?? string.Empty;
		}
	}
}