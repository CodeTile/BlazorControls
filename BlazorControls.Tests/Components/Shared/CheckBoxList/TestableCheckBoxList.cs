using BlazorControls.Components.Shared;

namespace BlazorControls.Tests.Components.Shared.CheckBoxList
{
	// Subclass to expose protected OnParametersSetAsync for testing
	public class TestableCheckBoxList<TItem> : CheckBoxList<TItem>
	{
		public async Task OnParametersSetPublicAsync() => await base.OnParametersSetAsync();
	}
}