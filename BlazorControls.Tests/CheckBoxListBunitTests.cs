using Bunit;
using Microsoft.AspNetCore.Components;
using BlazorControls.Components;
using Shouldly;
using Xunit;

namespace BlazorControls.Tests
{
	/// <summary>
	/// Provides bUnit rendering tests for the <see cref="CheckBoxList{TItem}"/> component.
	/// These tests validate rendering behaviour, interaction handling, and optional action buttons.
	/// </summary>
	public class CheckBoxListBunitTests : BunitContext
	{
		/// <summary>
		/// Ensures that the component renders one checkbox for each supplied item.
		/// </summary>
		[Fact]
		public void Renders_All_Items()
		{
			var items = new[] { "A", "B", "C" };

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Items, items)
				.Add(p => p.KeySelector, x => x)
				.Add(p => p.LabelSelector, x => x)
			);

			var checkboxes = cut.FindAll("input[type=checkbox]");
			checkboxes.Count.ShouldBe(3);
		}

		/// <summary>
		/// Ensures that the component renders the expected text labels for each item.
		/// </summary>
		[Fact]
		public void Renders_Labels()
		{
			var items = new[] { "Red", "Green", "Blue" };

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Items, items)
				.Add(p => p.KeySelector, x => x)
				.Add(p => p.LabelSelector, x => x)
			);

			cut.Markup.ShouldContain("Red");
			cut.Markup.ShouldContain("Green");
			cut.Markup.ShouldContain("Blue");
		}

		/// <summary>
		/// Verifies that the <see cref="CheckBoxList{TItem}.CheckedSelector"/> delegate
		/// correctly determines the initial checked state of each checkbox.
		/// </summary>
		[Fact]
		public void CheckedSelector_Sets_Initial_State()
		{
			var items = new[] { "A", "B", "C" };

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Items, items)
				.Add(p => p.KeySelector, x => x)
				.Add(p => p.LabelSelector, x => x)
				.Add(p => p.CheckedSelector, x => x == "B")
			);

			var checkboxes = cut.FindAll("input[type=checkbox]");

			checkboxes[0].HasAttribute("checked").ShouldBeFalse();
			checkboxes[1].HasAttribute("checked").ShouldBeTrue();
			checkboxes[2].HasAttribute("checked").ShouldBeFalse();
		}

		/// <summary>
		/// Ensures that toggling a checkbox triggers the <see cref="CheckBoxList{TItem}.OnChanged"/>
		/// callback with the correct key and checked state.
		/// </summary>
		[Fact]
		public void Clicking_Checkbox_Fires_OnChanged()
		{
			var items = new[] { "A" };
			(string Key, bool IsChecked) received = default;

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Items, items)
				.Add(p => p.KeySelector, x => x)
				.Add(p => p.LabelSelector, x => x)
				.Add(p => p.OnChanged, tuple => received = tuple)
			);

			var checkbox = cut.Find("input[type=checkbox]");
			checkbox.Change(true);

			received.Key.ShouldBe("A");
			received.IsChecked.ShouldBeTrue();
		}

		/// <summary>
		/// Ensures that the optional "Select All" button triggers its associated callback.
		/// </summary>
		[Fact]
		public void SelectAll_Button_Fires_Event()
		{
			bool called = false;

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Items, new[] { "A", "B" })
				.Add(p => p.KeySelector, x => x)
				.Add(p => p.LabelSelector, x => x)
				.Add(p => p.SelectAll, EventCallback.Factory.Create(this, () => called = true))
			);

			cut.Find("button").Click();

			called.ShouldBeTrue();
		}

		/// <summary>
		/// Ensures that the optional "Clear All" button triggers its associated callback.
		/// </summary>
		[Fact]
		public void ClearAll_Button_Fires_Event()
		{
			bool called = false;

			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Items, new[] { "A", "B" })
				.Add(p => p.KeySelector, x => x)
				.Add(p => p.LabelSelector, x => x)
				.Add(p => p.ClearAll, EventCallback.Factory.Create(this, () => called = true))
			);

			cut.Find("button").Click();

			called.ShouldBeTrue();
		}

		/// <summary>
		/// Ensures that both action buttons are rendered when both delegates are supplied.
		/// </summary>
		[Fact]
		public void Renders_Both_Action_Buttons()
		{
			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Items, new[] { "A" })
				.Add(p => p.KeySelector, x => x)
				.Add(p => p.LabelSelector, x => x)
				.Add(p => p.SelectAll, EventCallback.Factory.Create(this, () => { }))
				.Add(p => p.ClearAll, EventCallback.Factory.Create(this, () => { }))
			);

			var buttons = cut.FindAll("button");
			buttons.Count.ShouldBe(2);
		}

		/// <summary>
		/// Ensures that no action buttons are rendered when no delegates are supplied.
		/// </summary>
		[Fact]
		public void No_Action_Buttons_When_No_Delegates()
		{
			var cut = Render<CheckBoxList<string>>(parameters => parameters
				.Add(p => p.Items, new[] { "A" })
				.Add(p => p.KeySelector, x => x)
				.Add(p => p.LabelSelector, x => x)
			);

			cut.FindAll("button").Count.ShouldBe(0);
		}
	}
}