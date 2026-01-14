using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Components;

namespace BlazorControls.Components.Tests
{
	// ============================================================
	//  Testable Wrapper for Logic-Only Testing
	// ============================================================
	public class TestableCheckBoxList<T> : BlazorControls.Components.CheckBoxList<T>
	{
		/// <summary>
		/// Runs only the logic portions of OnAfterRender(true),
		/// skipping EmitAll() to avoid Blazor rendering.
		/// </summary>
		public void RunInitialization()
		{
			typeof(BlazorControls.Components.CheckBoxList<T>)
				.GetMethod("InitializeSelections", BindingFlags.Instance | BindingFlags.NonPublic)!
				.Invoke(this, null);

			typeof(BlazorControls.Components.CheckBoxList<T>)
				.GetMethod("BuildMap", BindingFlags.Instance | BindingFlags.NonPublic)!
				.Invoke(this, null);
		}

		/// <summary>
		/// Suppresses Blazor rendering during tests.
		/// </summary>
		protected new void StateHasChanged()
		{
			// No-op
		}

		/// <summary>
		/// Suppress EmitAll() during OnAfterRender.
		/// </summary>
		protected override void OnAfterRender(bool firstRender)
		{
			// Skip EmitAll()
		}
	}

	// ============================================================
	//  Test Model
	// ============================================================
	internal class Fruit
	{
		public string Name { get; set; } = "";
		public string Code { get; set; } = "";
	}

	// ============================================================
	//  MSTest Suite (Logic Only)
	// ============================================================
	[TestClass]
	public class CheckBoxListTests
	{
		// ---------------------------------------------------------
		//  INITIALIZATION TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void Initializes_AllItemsSelected_WhenUncheckedInitiallyIsNull()
		{
			var component = new TestableCheckBoxList<string>
			{
				Data = new[] { "A", "B", "C" }
			};

			component.RunInitialization();

			CollectionAssert.AreEqual(new[] { "A", "B", "C" }, component.SelectedValues);
			CollectionAssert.AreEqual(new[] { "A", "B", "C" }, component.SelectedTexts);

			Assert.AreEqual(3, component.SelectedMap.Count);
			Assert.IsTrue(component.SelectedMap.Values.All(v => v == 0));
		}

		[TestMethod]
		public void Initializes_RespectsUncheckedInitially_ByText()
		{
			var component = new TestableCheckBoxList<string>
			{
				Data = new[] { "A", "B", "C" },
				UncheckedInitially = new[] { "B" }
			};

			component.RunInitialization();

			CollectionAssert.AreEqual(new[] { "A", "C" }, component.SelectedValues);
			CollectionAssert.AreEqual(new[] { "A", "C" }, component.SelectedTexts);

			var expected = new Dictionary<string, int>
			{
				{ "A", 0 },
				{ "C", 0 }
			};

			CollectionAssert.AreEquivalent(expected, component.SelectedMap);
		}

		// ---------------------------------------------------------
		//  OBJECT LIST TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void ObjectList_UsesPositionalIndexInMap()
		{
			var data = new[]
			{
				new Fruit { Name = "Apple", Code = "A" },
				new Fruit { Name = "Banana", Code = "B" },
				new Fruit { Name = "Cherry", Code = "C" }
			};

			var component = new TestableCheckBoxList<Fruit>
			{
				Data = data,
				TextField = f => f.Name,
				ValueField = f => f.Code
			};

			component.RunInitialization();

			var expected = new Dictionary<string, int>
			{
				{ "A", 0 },
				{ "B", 1 },
				{ "C", 2 }
			};

			CollectionAssert.AreEquivalent(expected, component.SelectedMap);
		}

		// ---------------------------------------------------------
		//  DICTIONARY LIST TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void DictionaryList_UsesDictionaryValuesInMap()
		{
			var data = new Dictionary<string, int>
			{
				{ "A", 10 },
				{ "B", 20 }
			};

			var component = new TestableCheckBoxList<KeyValuePair<string, int>>
			{
				Data = data,
				TextField = kv => kv.Key,
				ValueField = kv => kv.Key
			};

			component.RunInitialization();

			var expected = new Dictionary<string, int>
			{
				{ "A", 10 },
				{ "B", 20 }
			};

			CollectionAssert.AreEquivalent(expected, component.SelectedMap);
		}

		// ---------------------------------------------------------
		//  SELECTEDMAP SETTER TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void SelectedMap_Setter_InvokesCallback_WhenReferenceChanges()
		{
			Dictionary<string, int>? received = null;

			var component = new TestableCheckBoxList<string>
			{
				Data = new[] { "A" },
				SelectedMap = new Dictionary<string, int>()
			};

			// Attach callback AFTER initial state is set
			component.SelectedMapChanged = EventCallback.Factory.Create<Dictionary<string, int>>(
				new object(),
				(Action<Dictionary<string, int>>)(map => received = map)
			);

			component.SelectedMap = new Dictionary<string, int> { { "A", 0 } };

			Assert.IsNotNull(received);
			Assert.AreEqual("A", received.Keys.Single());
		}

		[TestMethod]
		public void SelectedMap_Setter_DoesNotInvokeCallback_WhenReferenceSame()
		{
			var map = new Dictionary<string, int>();

			bool fired = false;

			var component = new TestableCheckBoxList<string>
			{
				Data = new[] { "A" },
				SelectedMap = map
			};

			// Attach callback AFTER initial assignment
			component.SelectedMapChanged = EventCallback.Factory.Create<Dictionary<string, int>>(
				new object(),
				(Action<Dictionary<string, int>>)(_ => fired = true)
			);

			// Assign same reference again
			component.SelectedMap = map;

			Assert.IsFalse(fired);
		}
	}
}