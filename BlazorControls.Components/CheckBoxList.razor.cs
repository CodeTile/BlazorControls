using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace BlazorControls.Components
{
	/// <summary>
	/// A clean, strongly typed checkbox list component.
	/// </summary>
	/// <typeparam name="TItem">The type of items displayed.</typeparam>
	public partial class CheckBoxList<TItem> : ComponentBase
	{
		/// <summary>
		/// The items to display.
		/// </summary>
		[Parameter, EditorRequired]
		public IEnumerable<TItem> Items { get; set; }

		/// <summary>
		/// Extracts a unique key for each item.
		/// </summary>
		[Parameter, EditorRequired]
		public Func<TItem, string> KeySelector { get; set; }

		/// <summary>
		/// Extracts the display label for each item.
		/// </summary>
		[Parameter, EditorRequired]
		public Func<TItem, string> LabelSelector { get; set; }

		/// <summary>
		/// Determines whether an item is checked.
		/// </summary>
		[Parameter]
		public Func<TItem, bool> CheckedSelector { get; set; } = _ => false;

		/// <summary>
		/// Fired when a checkbox is toggled.
		/// </summary>
		[Parameter]
		public EventCallback<(string Key, bool IsChecked)> OnChanged { get; set; }

		[Parameter] public EventCallback SelectAll { get; set; }
		[Parameter] public EventCallback ClearAll { get; set; }

		private Task OnCheckboxChanged(string key, object? value)
		{
			bool isChecked = value is bool b && b;
			return OnChanged.InvokeAsync((key, isChecked));
		}
	}
}