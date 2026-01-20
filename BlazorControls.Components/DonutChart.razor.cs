using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace BlazorControls.Components;

/// <summary>
/// A reusable SVG-based donut or pie chart component supporting
/// filtering, custom colours, legends, tooltips, and click events.
/// </summary>
public partial class DonutChart : ComponentBase
{
	/// <summary>
	/// Optional title displayed above the chart.
	/// </summary>
	[Parameter] public string Title { get; set; }

	/// <summary>
	/// Optional text displayed inside the donut hole when <see cref="IsDonut"/> is true.
	/// </summary>
	[Parameter] public string InnerTitle { get; set; }

	/// <summary>
	/// Determines whether the chart renders as a donut (true) or a pie chart (false).
	/// Defaults to true.
	/// </summary>
	[Parameter] public bool IsDonut { get; set; } = true;

	/// <summary>
	/// Thickness of the donut ring.
	/// Affects <see cref="InnerRadius"/> when <see cref="IsDonut"/> is true.
	/// </summary>
	[Parameter] public int Thickness { get; set; } = 20;

	/// <summary>
	/// The primary data source for the chart.
	/// Keys represent labels; values represent numeric quantities.
	/// </summary>
	[Parameter] public Dictionary<string, int> Data { get; set; }

	/// <summary>
	/// Optional list of labels to include.
	/// Any labels not in this list are excluded from rendering.
	/// </summary>
	[Parameter] public IEnumerable<string> IncludeLabels { get; set; }

	/// <summary>
	/// Explicit colour overrides for specific labels.
	/// If a label exists in this dictionary, its colour is used.
	/// </summary>
	[Parameter] public Dictionary<string, string> StatusColors { get; set; }

	/// <summary>
	/// A list of default colours used when no explicit colour is provided.
	/// Colours cycle in order for each slice.
	/// </summary>
	[Parameter] public List<string> DefaultColors { get; set; }

	/// <summary>
	/// Event fired when a slice is clicked.
	/// The slice label is passed as the event argument.
	/// </summary>
	[Parameter] public EventCallback<ChartClickEventArgs> OnSliceClick { get; set; }

	/// <summary>
	/// Event fired when the donut center is clicked.
	/// Only applies when <see cref="IsDonut"/> is true.
	/// </summary>
	[Parameter] public EventCallback<ChartClickEventArgs> OnCenterClick { get; set; }

	/// <summary>
	/// Determines whether a legend is rendered below the chart.
	/// </summary>
	[Parameter] public bool ShowLegend { get; set; } = false;

	/// <summary>
	/// The list of computed slices used for rendering.
	/// </summary>
	public List<DonutSlice> Slices { get; private set; } = [];

	/// <summary>
	/// The total value of all rendered slices.
	/// </summary>
	public int TotalValue => Slices.Sum(s => s.Value);

	/// <summary>
	/// The inner radius of the donut.
	/// Donut mode: 90 - Thickness
	/// Pie mode: 0
	/// </summary>
	public int InnerRadius => IsDonut ? Math.Max(0, 90 - Thickness) : 0;

	/// <summary>
	/// Whether the tooltip is currently visible.
	/// </summary>
	internal bool ShowTooltip { get; set; }

	/// <summary>
	/// The label displayed in the tooltip.
	/// </summary>
	internal string TooltipLabel { get; set; } = string.Empty;

	/// <summary>
	/// The numeric value displayed in the tooltip.
	/// </summary>
	internal string TooltipValue { get; set; } = string.Empty;

	/// <inheritdoc />
	protected override void OnParametersSet()
	{
		BuildSlices();
	}

	/// <summary>
	/// Builds the list of slices based on the provided data,
	/// filtering rules, and colour configuration.
	/// </summary>
	public void BuildSlices()
	{
		Slices.Clear();

		if (Data == null)
			return;

		var source = Data
			.Where(d => IncludeLabels?.Contains(d.Key) ?? true)
			.Where(d => d.Value > 0)
			.ToList();

		int total = source.Sum(s => s.Value);
		if (total < 1)
			return;

		double startAngle = 0;
		int colorIndex = 0;

		foreach (var item in source)
		{
			double sweep = item.Value / (double)total * 360.0;
			string color = ResolveColor(item.Key, ref colorIndex);

			var slice = new DonutSlice(
				item.Key,
				item.Value,
				startAngle,
				sweep,
				90,
				InnerRadius,
				color
			);

			Slices.Add(slice);
			startAngle += sweep;
		}
	}

	/// <summary>
	/// Determines the colour for a given label using:
	/// 1. Explicit status colours
	/// 2. Default palette
	/// 3. Stable fallback colour
	/// </summary>
	private string ResolveColor(string label, ref int index)
	{
		if (StatusColors != null && StatusColors.TryGetValue(label, out var explicitColor))
			return explicitColor;

		if (DefaultColors != null && DefaultColors.Count > 0)
		{
			string color = DefaultColors[index % DefaultColors.Count];
			index++;
			return color;
		}

		return GenerateColor(label);
	}

	/// <summary>
	/// Generates a stable fallback colour based on the label's hash.
	/// </summary>
	private static string GenerateColor(string key)
	{
		int hash = key.GetHashCode();
		var random = new Random(hash);
		return $"hsl({random.Next(0, 360)}, 70%, 55%)";
	}

	private async Task OnSliceClickAsync(DonutSlice slice)
	{
		await OnSliceClick.InvokeAsync(new ChartClickEventArgs() { SliceLabel = slice.Label });
	}

	private void OnSliceHover(DonutSlice slice)
	{
		TooltipLabel = slice.Label;
		TooltipValue = slice.Value.ToString("N0");
		ShowTooltip = true;
	}

	private void ClearHover()
	{
		ShowTooltip = false;
	}

	private void OnCenterHover()
	{
		TooltipLabel = InnerTitle ?? string.Empty;
		TooltipValue = TotalValue.ToString("N0");
		ShowTooltip = true;
	}

	private async Task OnCenterClickAsync()
	{
		await OnCenterClick.InvokeAsync(new ChartClickEventArgs() { SliceLabel = InnerTitle });
	}
}