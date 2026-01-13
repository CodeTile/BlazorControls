using System;
using System.Collections.Generic;
using Bunit;
using Microsoft.AspNetCore.Components;
using BlazorControls.Components;

namespace BlazorControls.Tests
{
	public class TestableCheckBoxList<TItem> : BunitContext
	{
		public IEnumerable<TItem> Data { get; set; } = Array.Empty<TItem>();
		public Func<TItem, string>? TextField { get; set; }
		public Func<TItem, object>? ValueField { get; set; }

		public List<string> SelectedValues { get; set; } = new();
		public List<string> SelectedTexts { get; set; } = new();
		public Dictionary<string, int> SelectedMap { get; set; } = new();

		public EventCallback<List<string>> SelectedValuesChanged { get; set; }
		public EventCallback<List<string>> SelectedTextsChanged { get; set; }
		public EventCallback<Dictionary<string, int>> SelectedMapChanged { get; set; }

		public IRenderedComponent<CheckBoxList<TItem>> Render()
		{
			return Render<CheckBoxList<TItem>>(parameters => parameters
				.Add(p => p.Data, Data)
				.Add(p => p.TextField, TextField)
				.Add(p => p.ValueField, ValueField)
				.Add(p => p.SelectedValues, SelectedValues)
				.Add(p => p.SelectedValuesChanged, SelectedValuesChanged)
				.Add(p => p.SelectedTexts, SelectedTexts)
				.Add(p => p.SelectedTextsChanged, SelectedTextsChanged)
				.Add(p => p.SelectedMap, SelectedMap)
				.Add(p => p.SelectedMapChanged, SelectedMapChanged)
			);
		}
	}
}