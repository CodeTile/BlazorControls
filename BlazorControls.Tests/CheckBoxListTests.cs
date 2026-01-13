using Bunit;
using BlazorControls.Components;

namespace BlazorControls.Tests
{
	[TestClass]
	public class CheckBoxListTests : BunitContext
	{
		// ------------------------------------------------------------
		// Initialization Tests
		// ------------------------------------------------------------

		[TestMethod]
		public void Initialization_SelectsAll_WhenNoExcludedTexts()
		{
			var items = new[] { "A", "B", "C" };
			var selectedValues = new List<string>();
			var selectedTexts = new List<string>();

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.SelectedValues, selectedValues)
				.Add(p => p.SelectedValuesChanged, v => selectedValues = v)
				.Add(p => p.SelectedTexts, selectedTexts)
				.Add(p => p.SelectedTextsChanged, t => selectedTexts = t)
			);

			CollectionAssert.AreEquivalent(items, selectedValues);
			CollectionAssert.AreEquivalent(items, selectedTexts);
		}

		[TestMethod]
		public void Initialization_ExcludesSpecifiedTexts()
		{
			var items = new[] { "A", "B", "C" };
			var excluded = new[] { "B" };

			var selectedValues = new List<string>();
			var selectedTexts = new List<string>();

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.ExcludedTexts, excluded)
				.Add(p => p.SelectedValues, selectedValues)
				.Add(p => p.SelectedValuesChanged, v => selectedValues = v)
				.Add(p => p.SelectedTexts, selectedTexts)
				.Add(p => p.SelectedTextsChanged, t => selectedTexts = t)
			);

			CollectionAssert.AreEquivalent(new[] { "A", "C" }, selectedValues);
			CollectionAssert.AreEquivalent(new[] { "A", "C" }, selectedTexts);
		}

		// ------------------------------------------------------------
		// Toggle Behavior
		// ------------------------------------------------------------

		[TestMethod]
		public void ToggleValue_AddsToSelectedValuesTextsAndMap()
		{
			var items = new[] { "X" };
			var selectedValues = new List<string>();
			var selectedTexts = new List<string>();
			var selectedMap = new Dictionary<string, int>();

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.SelectedValues, selectedValues)
				.Add(p => p.SelectedValuesChanged, v => selectedValues = v)
				.Add(p => p.SelectedTexts, selectedTexts)
				.Add(p => p.SelectedTextsChanged, t => selectedTexts = t)
				.Add(p => p.SelectedMap, selectedMap)
				.Add(p => p.SelectedMapChanged, m => selectedMap = m)
			);

			cut.Instance.ToggleValue("X", "X", true);

			Assert.IsTrue(selectedValues.Contains("X"));
			Assert.IsTrue(selectedTexts.Contains("X"));
			Assert.IsTrue(selectedMap.ContainsKey("X"));
			Assert.AreEqual(0, selectedMap["X"]);
		}

		[TestMethod]
		public void ToggleValue_RemovesFromSelectedValuesTextsAndMap()
		{
			var items = new[] { "X" };
			var selectedValues = new List<string> { "X" };
			var selectedTexts = new List<string> { "X" };
			var selectedMap = new Dictionary<string, int> { { "X", 0 } };

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.SelectedValues, selectedValues)
				.Add(p => p.SelectedValuesChanged, v => selectedValues = v)
				.Add(p => p.SelectedTexts, selectedTexts)
				.Add(p => p.SelectedTextsChanged, t => selectedTexts = t)
				.Add(p => p.SelectedMap, selectedMap)
				.Add(p => p.SelectedMapChanged, m => selectedMap = m)
			);

			cut.Instance.ToggleValue("X", "X", false);

			Assert.IsFalse(selectedValues.Contains("X"));
			Assert.IsFalse(selectedTexts.Contains("X"));
			Assert.IsFalse(selectedMap.ContainsKey("X"));
		}

		// ------------------------------------------------------------
		// Ordering Logic in SelectedMap
		// ------------------------------------------------------------

		[TestMethod]
		public void SelectedMap_AssignsSequentialIndexes()
		{
			var items = new[] { "A", "B", "C" };
			var selectedMap = new Dictionary<string, int>();

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.SelectedMap, selectedMap)
				.Add(p => p.SelectedMapChanged, m => selectedMap = m)
			);

			cut.Instance.ToggleValue("A", "A", true);
			cut.Instance.ToggleValue("B", "B", true);
			cut.Instance.ToggleValue("C", "C", true);

			Assert.AreEqual(0, selectedMap["A"]);
			Assert.AreEqual(1, selectedMap["B"]);
			Assert.AreEqual(2, selectedMap["C"]);
		}

		// ------------------------------------------------------------
		// Complex Object Support
		// ------------------------------------------------------------

		private class Item
		{
			public int Id { get; set; }
			public string Label { get; set; } = "";
		}

		[TestMethod]
		public void SupportsComplexObjects()
		{
			var items = new[]
			{
				new Item { Id = 1, Label = "One" },
				new Item { Id = 2, Label = "Two" }
			};

			var selectedValues = new List<string>();
			var selectedTexts = new List<string>();
			var selectedMap = new Dictionary<string, int>();

			var cut = Render<CheckBoxList<Item>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.TextField, i => i.Label)
				.Add(p => p.ValueField, i => i.Id)
				.Add(p => p.SelectedValues, selectedValues)
				.Add(p => p.SelectedValuesChanged, v => selectedValues = v)
				.Add(p => p.SelectedTexts, selectedTexts)
				.Add(p => p.SelectedTextsChanged, t => selectedTexts = t)
				.Add(p => p.SelectedMap, selectedMap)
				.Add(p => p.SelectedMapChanged, m => selectedMap = m)
			);

			cut.Instance.ToggleValue("1", "One", true);

			Assert.IsTrue(selectedValues.Contains("1"));
			Assert.IsTrue(selectedTexts.Contains("One"));
			Assert.IsTrue(selectedMap.ContainsKey("1"));
		}
	}
}