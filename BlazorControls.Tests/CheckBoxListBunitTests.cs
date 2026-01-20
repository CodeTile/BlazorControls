using Bunit;

using Microsoft.AspNetCore.Components;

namespace BlazorControls.Components.Tests
{
	/// <summary>
	/// bUnit test suite validating initialization behavior, toggle logic,
	/// and callback invocation for the <see cref="CheckBoxList{T}"/> component.
	/// </summary>
	[TestClass]
	public class CheckBoxListBunitTests
	{
		private BunitContext _ctx = null!;

		/// <summary>
		/// Creates a fresh <see cref="BunitContext"/> before each test,
		/// ensuring a clean rendering environment.
		/// </summary>
		[TestInitialize]
		public void Setup()
		{
			_ctx = new BunitContext();
		}

		/// <summary>
		/// Disposes the <see cref="BunitContext"/> after each test
		/// to release component instances and DOM resources.
		/// </summary>
		[TestCleanup]
		public void Teardown()
		{
			_ctx.Dispose();
		}

		// ---------------------------------------------------------
		//  INITIALIZATION CALLBACK TEST
		// ---------------------------------------------------------

		/// <summary>
		/// Verifies that the component fires both SelectedValuesChanged
		/// and SelectedTextsChanged during the initial render cycle.
		/// </summary>
		[TestMethod]
		public void Initialization_Fires_SelectedValuesChanged_And_SelectedTextsChanged()
		{
			bool valuesFired = false;
			bool textsFired = false;

			var comp = _ctx.Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, new[] { "A" })
				.Add(p => p.SelectedValuesChanged, EventCallback.Factory.Create<List<string>>(
					new object(), (Action<List<string>>)(_ => valuesFired = true)))
				.Add(p => p.SelectedTextsChanged, EventCallback.Factory.Create<List<string>>(
					new object(), (Action<List<string>>)(_ => textsFired = true)))
			);

			// Trigger first render
			comp.Render();

			Assert.IsTrue(valuesFired);
			Assert.IsTrue(textsFired);
		}

		// ---------------------------------------------------------
		//  TOGGLE TESTS (FULL PIPELINE)
		// ---------------------------------------------------------

		/// <summary>
		/// Ensures that checking a value adds it to SelectedValues,
		/// updates SelectedTexts, and inserts the correct index into SelectedMap.
		/// </summary>
		[TestMethod]
		public void ToggleValue_Check_AddsToSelections_AndUpdatesMap()
		{
			var comp = _ctx.Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, new[] { "A", "B" })
				.Add(p => p.UncheckedInitially, new[] { "A", "B" })
			);

			comp.Render();

			// MUST run on dispatcher
			comp.InvokeAsync(() => comp.Instance.ToggleValue("A", "A", true));

			Assert.HasCount(1, comp.Instance.SelectedValues);
			Assert.AreEqual("A", comp.Instance.SelectedValues.Single());

			Assert.HasCount(1, comp.Instance.SelectedTexts);
			Assert.AreEqual("A", comp.Instance.SelectedTexts.Single());

			Assert.HasCount(1, comp.Instance.SelectedMap);
			Assert.AreEqual(0, comp.Instance.SelectedMap["A"]);
		}

		/// <summary>
		/// Ensures that unchecking a value removes it from all selection
		/// collections and clears its entry from SelectedMap.
		/// </summary>
		[TestMethod]
		public void ToggleValue_Uncheck_RemovesFromSelections()
		{
			var comp = _ctx.Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, new[] { "A" })
			);

			comp.Render();

			// Initially selected
			Assert.HasCount(1, comp.Instance.SelectedValues);

			// MUST run on dispatcher AND be awaited
			comp.InvokeAsync(() => comp.Instance.ToggleValue("A", "A", false)).Wait();

			Assert.IsEmpty(comp.Instance.SelectedValues);
			Assert.IsEmpty(comp.Instance.SelectedTexts);
			Assert.IsEmpty(comp.Instance.SelectedMap);
		}

		// ---------------------------------------------------------
		//  CALLBACK TESTS FOR TOGGLE
		// ---------------------------------------------------------

		/// <summary>
		/// Verifies that toggling a value fires both SelectedValuesChanged
		/// and SelectedTextsChanged with the updated selection lists.
		/// </summary>
		[TestMethod]
		public void ToggleValue_FiresCallbacks()
		{
			List<string>? receivedValues = null;
			List<string>? receivedTexts = null;

			var comp = _ctx.Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, new[] { "A" })
				.Add(p => p.SelectedValuesChanged, EventCallback.Factory.Create<List<string>>(
					new object(), (Action<List<string>>)(v => receivedValues = v)))
				.Add(p => p.SelectedTextsChanged, EventCallback.Factory.Create<List<string>>(
					new object(), (Action<List<string>>)(t => receivedTexts = t)))
			);

			comp.Render();

			// MUST run on dispatcher AND be awaited
			comp.InvokeAsync(() => comp.Instance.ToggleValue("A", "A", false)).Wait();

			Assert.IsNotNull(receivedValues);
			Assert.IsNotNull(receivedTexts);

			Assert.IsEmpty(receivedValues);
			Assert.IsEmpty(receivedTexts);
		}
	}
}