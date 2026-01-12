namespace BlazorControls.Components.Shared;

/// <summary>
/// Represents a single slice of a donut or pie chart,
/// including geometry, colour, and label metadata.
/// </summary>
public class DonutSlice
{
	/// <summary>
	/// The label associated with this slice.
	/// </summary>
	public string Label { get; }

	/// <summary>
	/// The numeric value represented by this slice.
	/// </summary>
	public int Value { get; }

	/// <summary>
	/// The SVG path data used to render the slice.
	/// </summary>
	public string PathData { get; }

	/// <summary>
	/// The fill colour used when rendering the slice.
	/// </summary>
	public string Color { get; }

	/// <summary>
	/// The CSS class applied to the slice path element.
	/// </summary>
	public string CssClass => "donut-slice";

	/// <summary>
	/// Creates a new <see cref="DonutSlice"/> with the specified geometry and colour.
	/// </summary>
	public DonutSlice(
		string label,
		int value,
		double startAngle,
		double sweepAngle,
		double outerR,
		double innerR,
		string color)
	{
		Label = label;
		Value = value;
		Color = color;
		PathData = BuildPath(startAngle, sweepAngle, outerR, innerR);
	}

	/// <summary>
	/// Builds the SVG path string for either a pie slice or donut slice.
	/// </summary>
	private static string BuildPath(double startAngle, double sweepAngle, double outerR, double innerR)
	{
		double start = startAngle * Math.PI / 180;
		double end = (startAngle + sweepAngle) * Math.PI / 180;

		double cx = 100;
		double cy = 100;

		double x1 = cx + outerR * Math.Cos(start);
		double y1 = cy + outerR * Math.Sin(start);
		double x2 = cx + outerR * Math.Cos(end);
		double y2 = cy + outerR * Math.Sin(end);

		int largeArc = sweepAngle > 180 ? 1 : 0;

		if (innerR <= 0)
		{
			return
				$"M {cx} {cy} " +
				$"L {x1} {y1} " +
				$"A {outerR} {outerR} 0 {largeArc} 1 {x2} {y2} Z";
		}

		double x3 = cx + innerR * Math.Cos(end);
		double y3 = cy + innerR * Math.Sin(end);
		double x4 = cx + innerR * Math.Cos(start);
		double y4 = cy + innerR * Math.Sin(start);

		return
			$"M {x1} {y1} " +
			$"A {outerR} {outerR} 0 {largeArc} 1 {x2} {y2} " +
			$"L {x3} {y3} " +
			$"A {innerR} {innerR} 0 {largeArc} 0 {x4} {y4} Z";
	}
}