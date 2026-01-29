using System.Collections.Generic;
using Bunit;
using Microsoft.AspNetCore.Components;
using BlazorControls.Components;
using Shouldly;
using Xunit;

namespace BlazorControls.Components.Tests
{
	/// <summary>
	/// Provides bUnit rendering tests for the <see cref="DonutChart"/> component.
	/// These tests validate DOM structure, interactivity, tooltips, legends,
	/// and event callback behaviour.
	/// </summary>
	public class DonutChartBunitTests : BunitContext
	{
		/// <summary>
		/// Verifies that clicking the donut centre triggers the <see cref="DonutChart.OnCenterClick"/>
		/// callback and that the emitted <see cref="ChartClickEventArgs"/> contains the expected label.
		/// </summary>
		[Fact]
		public void CenterClick_InvokesCallback()
		{
			ChartClickEventArgs? received = null;

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.IsDonut, true)
				.Add(x => x.InnerTitle, "Center")
				.Add(x => x.OnCenterClick,
					EventCallback.Factory.Create<ChartClickEventArgs>(this, args => received = args))
			);

			cut.Find("g.donut-center").Click();

			received.ShouldNotBeNull();
			received!.SliceLabel.ShouldBe("Center");
		}

		/// <summary>
		/// Ensures that hovering over the donut centre displays a tooltip
		/// showing the inner title and the aggregated total value.
		/// </summary>
		[Fact]
		public void CenterHover_ShowsTotalTooltip()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 } })
				.Add(x => x.IsDonut, true)
				.Add(x => x.InnerTitle, "Total")
			);

			cut.Find("g.donut-center").MouseOver();

			var tooltip = cut.Find("div.donut-tooltip");
			tooltip.TextContent.ShouldContain("Total");
			tooltip.TextContent.ShouldContain("30");
		}

		/// <summary>
		/// Confirms that the donut centre element is not rendered when the chart
		/// is configured as a pie chart (<c>IsDonut = false</c>).
		/// </summary>
		[Fact]
		public void InnerCircle_IsNotRendered_WhenPie()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.IsDonut, false)
			);

			cut.FindAll("g.donut-center").Count.ShouldBe(0);
		}

		/// <summary>
		/// Confirms that the donut centre element is rendered when <c>IsDonut = true</c>.
		/// </summary>
		[Fact]
		public void InnerCircle_IsRendered_WhenDonut()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.IsDonut, true)
			);

			cut.FindAll("g.donut-center").Count.ShouldBe(1);
		}

		/// <summary>
		/// Verifies that the inner title text is rendered when supplied.
		/// </summary>
		[Fact]
		public void InnerTitle_IsRendered()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.IsDonut, true)
				.Add(x => x.InnerTitle, "Inner")
			);

			cut.FindAll(".donut-inner-title").Count.ShouldBe(1);
		}

		/// <summary>
		/// Ensures that the legend is not rendered when <c>ShowLegend = false</c>.
		/// </summary>
		[Fact]
		public void Legend_IsNotRendered_WhenDisabled()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.ShowLegend, false)
			);

			cut.FindAll("ul.donut-legend").Count.ShouldBe(0);
		}

		/// <summary>
		/// Ensures that the legend is rendered with one entry per slice
		/// when <c>ShowLegend = true</c>.
		/// </summary>
		[Fact]
		public void Legend_IsRendered_WhenEnabled()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 } })
				.Add(x => x.ShowLegend, true)
			);

			cut.FindAll("ul.donut-legend li").Count.ShouldBe(2);
		}

		/// <summary>
		/// Verifies that no SVG slice paths are rendered when all data values are zero.
		/// </summary>
		[Fact]
		public void NoSlices_RendersNoPaths()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 0 } })
			);

			cut.FindAll("path.donut-slice").Count.ShouldBe(0);
		}

		/// <summary>
		/// Ensures that a single slice is rendered when the dataset contains one entry.
		/// </summary>
		[Fact]
		public void SingleSlice_RendersOnePath()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "Only", 100 } })
			);

			cut.FindAll("path.donut-slice").Count.ShouldBe(1);
		}

		/// <summary>
		/// Verifies that clicking a slice triggers the <see cref="DonutChart.OnSliceClick"/>
		/// callback and that the emitted <see cref="ChartClickEventArgs"/> contains the correct label.
		/// </summary>
		[Fact]
		public void SliceClick_InvokesCallback()
		{
			ChartClickEventArgs? received = null;

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 } })
				.Add(x => x.OnSliceClick,
					EventCallback.Factory.Create<ChartClickEventArgs>(this, args => received = args))
			);

			cut.FindAll("path.donut-slice")[1].Click();

			received.ShouldNotBeNull();
			received!.SliceLabel.ShouldBe("B");
		}

		/// <summary>
		/// Ensures that hovering over a slice displays the tooltip.
		/// </summary>
		[Fact]
		public void SliceHover_ShowsTooltip()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 123 } })
			);

			cut.Find("path.donut-slice").MouseOver();

			cut.FindAll("div.donut-tooltip").Count.ShouldBe(1);
		}

		/// <summary>
		/// Confirms that the SVG container and slice paths are rendered
		/// when valid data is supplied.
		/// </summary>
		[Fact]
		public void SvgAndPaths_AreRendered()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 } })
			);

			cut.FindAll("svg.donut-chart").Count.ShouldBe(1);
			cut.FindAll("path.donut-slice").Count.ShouldBe(2);
		}

		/// <summary>
		/// Verifies that the chart title is rendered when provided.
		/// </summary>
		[Fact]
		public void Title_IsRendered()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.Title, "My Chart")
			);

			cut.FindAll("h3.chart-title").Count.ShouldBe(1);
		}

		/// <summary>
		/// Ensures that the tooltip is not visible before any hover interaction occurs.
		/// </summary>
		[Fact]
		public void Tooltip_IsHiddenByDefault()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
			);

			cut.FindAll("div.donut-tooltip").Count.ShouldBe(0);
		}

		/// <summary>
		/// Verifies that the tooltip becomes hidden again when the mouse leaves the chart.
		/// </summary>
		[Fact]
		public void Tooltip_IsHiddenOnMouseLeave()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
			);

			cut.Find("path.donut-slice").MouseOver();
			cut.FindAll("div.donut-tooltip").Count.ShouldBe(1);

			cut.Find("svg.donut-chart").MouseLeave();
			cut.FindAll("div.donut-tooltip").Count.ShouldBe(0);
		}
	}
}