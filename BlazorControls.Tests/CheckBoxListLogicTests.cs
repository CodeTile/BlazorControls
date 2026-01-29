using System.Collections.Generic;

using Shouldly;

using Xunit;

namespace BlazorControls.Tests
{
	/// <summary>
	/// Contains logic‑only tests for consumer‑maintained state patterns used with
	/// the <see cref="BlazorControls.Components.CheckBoxList{TItem}"/> component.
	/// <para>
	/// These tests validate dictionary‑based selection tracking and selector behaviour.
	/// The component itself does not maintain internal selection state, so these tests
	/// focus on the patterns expected of consuming pages.
	/// </para>
	/// </summary>
	public class CheckBoxListLogicTests
	{
		/// <summary>
		/// Ensures that a consumer‑maintained dictionary correctly updates when
		/// receiving a change notification from the component.
		/// </summary>
		[Fact]
		public void StateDictionary_Updates_When_OnChanged_Fires()
		{
			var state = new Dictionary<string, bool>
			{
				["A"] = false,
				["B"] = false
			};

			// Simulate the component's callback
			(string Key, bool IsChecked) change = ("A", true);

			state[change.Key] = change.IsChecked;

			state["A"].ShouldBeTrue();
			state["B"].ShouldBeFalse();
		}

		/// <summary>
		/// Ensures that a selector function used to determine initial checked state
		/// behaves as expected for a given set of selected keys.
		/// </summary>
		[Fact]
		public void CheckedSelector_Returns_Correct_Initial_State()
		{
			var selected = new HashSet<string> { "B" };

			bool Selector(string key) => selected.Contains(key);

			Selector("A").ShouldBeFalse();
			Selector("B").ShouldBeTrue();
			Selector("C").ShouldBeFalse();
		}

		/// <summary>
		/// Ensures that a "Select All" operation correctly marks all keys as selected.
		/// </summary>
		[Fact]
		public void SelectAll_Sets_All_Values_True()
		{
			var state = new Dictionary<string, bool>
			{
				["A"] = false,
				["B"] = false,
				["C"] = false
			};

			foreach (var key in state.Keys)
				state[key] = true;

			state.Values.ShouldAllBe(v => v == true);
		}

		/// <summary>
		/// Ensures that a "Clear All" operation correctly marks all keys as unselected.
		/// </summary>
		[Fact]
		public void ClearAll_Sets_All_Values_False()
		{
			var state = new Dictionary<string, bool>
			{
				["A"] = true,
				["B"] = true,
				["C"] = true
			};

			foreach (var key in state.Keys)
				state[key] = false;

			state.Values.ShouldAllBe(v => v == false);
		}
	}
}