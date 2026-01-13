using BlazorControls.Components;

namespace BlazorControls.Components.Tests
{
	// Subclass to expose protected OnParametersSetAsync for testing
	public class TestableCheckBoxList<TItem> : CheckBoxList<TItem>
	{
		public async Task OnParametersSetPublicAsync() => await base.OnParametersSetAsync();
	}
}