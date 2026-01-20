using System;

namespace BlazorControls.Components;

public class ChartClickEventArgs : EventArgs
{
	public string SliceLabel { get; set; }
}