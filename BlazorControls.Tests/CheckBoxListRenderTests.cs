using Bunit;

namespace BlazorControls.Components.Tests
{
	[TestClass]
	public class CheckBoxListRenderTests : BunitContext
	{
		// ------------------------------------------------------------
		// Basic Rendering Tests (existing behavior)
		// ------------------------------------------------------------

		[TestMethod]
		public void Component_Renders_With_Data()
		{
			var items = new[] { "A", "B", "C" };

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, items)
			);

			Assert.IsNotNull(cut.Markup);
			Assert.IsTrue(cut.Markup.Contains("A"));
			Assert.IsTrue(cut.Markup.Contains("B"));
			Assert.IsTrue(cut.Markup.Contains("C"));
		}

		[TestMethod]
		public void Component_Renders_With_TextField_And_ValueField()
		{
			var items = new[]
			{
				new { Id = 1, Label = "One" },
				new { Id = 2, Label = "Two" }
			};

			var cut = Render<CheckBoxList<dynamic>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.TextField, i => (string)i.Label)
				.Add(p => p.ValueField, i => (int)i.Id)
			);

			Assert.IsNotNull(cut.Markup);
			Assert.IsTrue(cut.Markup.Contains("One"));
			Assert.IsTrue(cut.Markup.Contains("Two"));
		}

		[TestMethod]
		public void Component_Renders_With_SelectedValues()
		{
			var items = new[] { "A", "B" };
			var selectedValues = new List<string> { "A" };

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.SelectedValues, selectedValues)
			);

			Assert.IsNotNull(cut.Markup);
			Assert.IsTrue(cut.Markup.Contains("A"));
			Assert.IsTrue(cut.Markup.Contains("B"));
		}

		// ------------------------------------------------------------
		// New Rendering Tests for SelectedMap
		// ------------------------------------------------------------

		[TestMethod]
		public void Component_Renders_With_SelectedMap()
		{
			var items = new[] { "A", "B" };
			var selectedMap = new Dictionary<string, int>();

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.SelectedMap, selectedMap)
				.Add(p => p.SelectedMapChanged, map => selectedMap = map)
			);

			Assert.IsNotNull(cut.Markup);
		}

		[TestMethod]
		public void Component_Renders_With_SelectedMap_Prepopulated()
		{
			var items = new[] { "A", "B", "C" };
			var selectedMap = new Dictionary<string, int>
			{
				{ "A", 0 },
				{ "C", 1 }
			};

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.SelectedMap, selectedMap)
				.Add(p => p.SelectedMapChanged, map => selectedMap = map)
			);

			Assert.IsNotNull(cut.Markup);
			Assert.IsTrue(cut.Markup.Contains("A"));
			Assert.IsTrue(cut.Markup.Contains("B"));
			Assert.IsTrue(cut.Markup.Contains("C"));
		}

		// ------------------------------------------------------------
		// Rendering with ExcludedTexts
		// ------------------------------------------------------------

		[TestMethod]
		public void Component_Renders_With_ExcludedTexts()
		{
			var items = new[] { "A", "B", "C" };
			var excluded = new[] { "B" };

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.ExcludedTexts, excluded)
			);

			Assert.IsNotNull(cut.Markup);
			Assert.IsTrue(cut.Markup.Contains("A"));
			Assert.IsTrue(cut.Markup.Contains("B"));
			Assert.IsTrue(cut.Markup.Contains("C"));
		}

		// ------------------------------------------------------------
		// Rendering with Complex Objects
		// ------------------------------------------------------------

		private class Item
		{
			public int Id { get; set; }
			public string Label { get; set; } = "";
		}

		[TestMethod]
		public void Component_Renders_With_ComplexObjects()
		{
			var items = new[]
			{
				new Item { Id = 1, Label = "One" },
				new Item { Id = 2, Label = "Two" }
			};

			var cut = Render<CheckBoxList<Item>>(parameters => parameters
				.Add(p => p.Data, items)
				.Add(p => p.TextField, i => i.Label)
				.Add(p => p.ValueField, i => i.Id)
			);

			Assert.IsNotNull(cut.Markup);
			Assert.IsTrue(cut.Markup.Contains("One"));
			Assert.IsTrue(cut.Markup.Contains("Two"));
		}
	}
}