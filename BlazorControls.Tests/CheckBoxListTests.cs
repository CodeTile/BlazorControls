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

	/// <summary>
	/// A specialized test wrapper for <see cref="BlazorControls.Components.CheckBoxList{T}"/>
	/// that enables logic-only testing without invoking Blazor's rendering pipeline.
	/// <para>
	/// This wrapper bypasses UI updates, suppresses <c>StateHasChanged()</c>,
	/// and exposes internal initialization logic for direct invocation.
	/// </para>
	/// </summary>
	/// <typeparam name="T">The data type used by the checkbox list.</typeparam>
	public class TestableCheckBoxList<T> : BlazorControls.Components.CheckBoxList<T>
	{
		/// <summary>
		/// Executes the internal initialization logic normally triggered during
		/// <see cref="ComponentBase.OnAfterRender(bool)"/> without performing any rendering.
		/// <para>
		/// Specifically invokes the private <c>InitializeSelections()</c> and <c>BuildMap()</c>
		/// methods using reflection.
		/// </para>
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
		/// Overrides Blazor's rendering trigger to prevent UI updates during logic-only tests.
		/// </summary>
		protected new void StateHasChanged()
		{
			// Intentionally suppressed for test isolation.
		}

		/// <summary>
		/// Overrides <see cref="ComponentBase.OnAfterRender(bool)"/> to prevent
		/// <c>EmitAll()</c> from firing during tests.
		/// </summary>
		/// <param name="firstRender">Indicates whether this is the first render.</param>
		protected override void OnAfterRender(bool firstRender)
		{
			// Skip EmitAll() to avoid triggering callbacks during test setup.
		}
	}

	// ============================================================
	//  Test Model
	// ============================================================

	/// <summary>
	/// A simple model used for object-based checkbox list tests.
	/// </summary>
	internal class Fruit
	{
		/// <summary>
		/// The display name of the fruit.
		/// </summary>
		public string Name { get; set; } = "";

		/// <summary>
		/// A short code representing the fruit.
		/// </summary>
		public string Code { get; set; } = "";
	}

	// ============================================================
	//  MSTest Suite (Logic Only)
	// ============================================================

	/// <summary>
	/// A suite of logic-only tests validating initialization behavior,
	/// mapping rules, and callback invocation for <see cref="CheckBoxList{T}"/>.
	/// <para>
	/// These tests bypass Blazor rendering entirely using <see cref="TestableCheckBoxList{T}"/>.
	/// </para>
	/// </summary>
	[TestClass]
	public class CheckBoxListTests
	{
		// ---------------------------------------------------------
		//  INITIALIZATION TESTS
		// ---------------------------------------------------------

		/// <summary>
		/// Ensures that when <c>UncheckedInitially</c> is null,
		/// all items in <c>Data</c> are selected by default.
		/// </summary>
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

			Assert.HasCount(3, component.SelectedMap);
			Assert.IsTrue(component.SelectedMap.Values.All(v => v == 0));
		}

		/// <summary>
		/// Ensures that <c>UncheckedInitially</c> correctly excludes items
		/// from the initial selection set when matching by text.
		/// </summary>
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

		/// <summary>
		/// Verifies that when using object-based data, the <c>SelectedMap</c>
		/// stores the positional index of each item as its mapped value.
		/// </summary>
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

		/// <summary>
		/// Ensures that when binding to a dictionary, the dictionary's values
		/// are used as the mapped values in <c>SelectedMap</c>.
		/// </summary>
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

		/// <summary>
		/// Verifies that assigning a new dictionary instance to <c>SelectedMap</c>
		/// triggers the <c>SelectedMapChanged</c> callback.
		/// </summary>
		[TestMethod]
		public void SelectedMap_Setter_InvokesCallback_WhenReferenceChanges()
		{
			Dictionary<string, int>? received = null;

			var component = new TestableCheckBoxList<string>
			{
				Data = new[] { "A" },
				SelectedMap = new Dictionary<string, int>()
			};

			component.SelectedMapChanged = EventCallback.Factory.Create<Dictionary<string, int>>(
				new object(),
				(Action<Dictionary<string, int>>)(map => received = map)
			);

			component.SelectedMap = new Dictionary<string, int> { { "A", 0 } };

			Assert.IsNotNull(received);
			Assert.AreEqual("A", received.Keys.Single());
		}

		/// <summary>
		/// Ensures that assigning the same dictionary reference to <c>SelectedMap</c>
		/// does not trigger the <c>SelectedMapChanged</c> callback.
		/// </summary>
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

			component.SelectedMapChanged = EventCallback.Factory.Create<Dictionary<string, int>>(
				new object(),
				(Action<Dictionary<string, int>>)(_ => fired = true)
			);

			component.SelectedMap = map;

			Assert.IsFalse(fired);
		}
	}
}