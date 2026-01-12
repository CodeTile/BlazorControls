using BlazorControls.Components.Shared;

namespace BlazorControls.Tests.Components.Shared
{
	// Subclass to expose protected OnParametersSetAsync for testing
	public class TestableCheckBoxList<TItem> : CheckBoxList<TItem>
	{
		public async Task OnParametersSetPublicAsync() => await base.OnParametersSetAsync();
	}
}