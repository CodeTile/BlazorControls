using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BlazorControls.Components;
using BlazorControls.Components.Tests;

using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlazorControls.Components.Tests
{
	[TestClass]
	public class CheckBoxListTests
	{
		private class TestItem
		{
			public int Id { get; set; }
			public string Name { get; set; } = string.Empty;
		}

		[TestMethod]
		public async Task OnParametersSet_InitializesSelectedValues_AndExcludesCorrectly()
		{
			// Arrange
			var data = new List<string> { "One", "Two", "Three", "Four" };
			bool valuesCalled = false;
			bool textsCalled = false;

			var component = new TestableCheckBoxList<string>
			{
				Data = data,
				ExcludedTexts = new[] { "Two", "Four" },
				SelectedValuesChanged = new EventCallback<List<string>>(null,
					new Action<List<string>>(list => valuesCalled = true)),
				SelectedTextsChanged = new EventCallback<List<string>>(null,
					new Action<List<string>>(list => textsCalled = true))
			};

			// Act
			await component.OnParametersSetPublicAsync();

			// Assert: non-excluded items are selected
			CollectionAssert.AreEquivalent(new[] { "One", "Three" }, component.SelectedValues);
			CollectionAssert.AreEquivalent(new[] { "One", "Three" }, component.SelectedTexts);

			// Assert: excluded items are not selected
			Assert.IsFalse(component.SelectedValues.Contains("Two"));
			Assert.IsFalse(component.SelectedValues.Contains("Four"));

			// Assert: callbacks invoked
			Assert.IsTrue(valuesCalled);
			Assert.IsTrue(textsCalled);
		}

		[TestMethod]
		public void ToggleValue_AddsAndRemovesItems_AndInvokesCallbacks()
		{
			// Arrange
			var component = new CheckBoxList<string>();

			bool valuesCalled = false;
			bool textsCalled = false;

			component.SelectedValuesChanged = new EventCallback<List<string>>(null,
				new Action<List<string>>(list => valuesCalled = true));

			component.SelectedTextsChanged = new EventCallback<List<string>>(null,
				new Action<List<string>>(list => textsCalled = true));

			string value = "val1";
			string text = "Text1";

			// Act: check the box
			component.ToggleValue(value, text, true);

			// Assert add
			Assert.IsTrue(component.SelectedValues.Contains(value));
			Assert.IsTrue(component.SelectedTexts.Contains(text));
			Assert.IsTrue(valuesCalled);
			Assert.IsTrue(textsCalled);

			// Reset callback flags
			valuesCalled = false;
			textsCalled = false;

			// Act: uncheck the box
			component.ToggleValue(value, text, false);

			// Assert remove
			Assert.IsFalse(component.SelectedValues.Contains(value));
			Assert.IsFalse(component.SelectedTexts.Contains(text));
			Assert.IsTrue(valuesCalled);
			Assert.IsTrue(textsCalled);
		}

		[TestMethod]
		public void DataAndValueFields_CanBeUsed()
		{
			// Arrange
			var items = new List<TestItem>
			{
				new() { Id = 1, Name = "Item1" },
				new() { Id = 2, Name = "Item2" }
			};

			var component = new CheckBoxList<TestItem>
			{
				Data = items,
				TextField = i => i.Name,
				ValueField = i => i.Id
			};

			// Act & Assert
			Assert.AreEqual(2, component.Data.Count());
			Assert.AreEqual("Item1", component.TextField!(items[0]));
			Assert.AreEqual(2, component.ValueField!(items[1]));
		}
	}
}