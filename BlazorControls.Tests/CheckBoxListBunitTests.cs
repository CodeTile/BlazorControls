using System;
using System.Collections.Generic;
using System.Linq;

using Bunit;

using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlazorControls.Components.Tests
{
	[TestClass]
	public class CheckBoxListBunitTests
	{
		private BunitContext _ctx = null!;

		[TestInitialize]
		public void Setup()
		{
			_ctx = new BunitContext();
		}

		[TestCleanup]
		public void Teardown()
		{
			_ctx.Dispose();
		}

		// ---------------------------------------------------------
		//  INITIALIZATION CALLBACK TEST
		// ---------------------------------------------------------

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

			Assert.AreEqual(1, comp.Instance.SelectedValues.Count);
			Assert.AreEqual("A", comp.Instance.SelectedValues.Single());

			Assert.AreEqual(1, comp.Instance.SelectedTexts.Count);
			Assert.AreEqual("A", comp.Instance.SelectedTexts.Single());

			Assert.AreEqual(1, comp.Instance.SelectedMap.Count);
			Assert.AreEqual(0, comp.Instance.SelectedMap["A"]);
		}

		[TestMethod]
		public void ToggleValue_Uncheck_RemovesFromSelections()
		{
			var comp = _ctx.Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Data, new[] { "A" })
			);

			comp.Render();

			// Initially selected
			Assert.AreEqual(1, comp.Instance.SelectedValues.Count);

			// MUST run on dispatcher AND be awaited
			comp.InvokeAsync(() => comp.Instance.ToggleValue("A", "A", false)).Wait();

			Assert.AreEqual(0, comp.Instance.SelectedValues.Count);
			Assert.AreEqual(0, comp.Instance.SelectedTexts.Count);
			Assert.AreEqual(0, comp.Instance.SelectedMap.Count);
		}

		// ---------------------------------------------------------
		//  CALLBACK TESTS FOR TOGGLE
		// ---------------------------------------------------------

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

			Assert.AreEqual(0, receivedValues!.Count);
			Assert.AreEqual(0, receivedTexts!.Count);
		}
	}
}