using System.Collections.Generic;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorControls.Components;

namespace BlazorControls.Components.Tests
{
	/// <summary>
	/// Tests validating rendering, DOM structure, interactivity,
	/// tooltips, legends, and event callbacks using bUnit.
	/// </summary>
	[TestClass]
	public class DonutChartBunitTests : BunitContext
	{
		/// <summary>
		/// Verifies that clicking the donut center triggers the OnCenterClick callback
		/// and that the emitted ChartClickEventArgs contains the expected SliceLabel.
		/// </summary>
		[TestMethod]
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

			Assert.IsNotNull(received);
			Assert.AreEqual("Center", received!.SliceLabel);
		}

		/// <summary>
		/// Ensures that hovering over the donut center displays a tooltip
		/// showing the inner title and the total aggregated value.
		/// </summary>
		[TestMethod]
		public void CenterHover_ShowsTotalTooltip()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 } })
				.Add(x => x.IsDonut, true)
				.Add(x => x.InnerTitle, "Total")
			);

			cut.Find("g.donut-center").MouseOver();

			var tooltip = cut.Find("div.donut-tooltip");
			Assert.Contains("Total", tooltip.TextContent);
			Assert.Contains("30", tooltip.TextContent);
		}

		/// <summary>
		/// Confirms that the donut center element is not rendered when the chart
		/// is configured as a pie chart (IsDonut = false).
		/// </summary>
		[TestMethod]
		public void InnerCircle_IsNotRendered_WhenPie()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.IsDonut, false)
			);

			Assert.IsEmpty(cut.FindAll("g.donut-center"));
		}

		/// <summary>
		/// Confirms that the donut center element is rendered when IsDonut = true.
		/// </summary>
		[TestMethod]
		public void InnerCircle_IsRendered_WhenDonut()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.IsDonut, true)
			);

			Assert.HasCount(1, cut.FindAll("g.donut-center"));
		}

		/// <summary>
		/// Verifies that the inner title text is rendered when provided.
		/// </summary>
		[TestMethod]
		public void InnerTitle_IsRendered()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.IsDonut, true)
				.Add(x => x.InnerTitle, "Inner")
			);

			Assert.HasCount(1, cut.FindAll(".donut-inner-title"));
		}

		/// <summary>
		/// Ensures that the legend is not rendered when ShowLegend = false.
		/// </summary>
		[TestMethod]
		public void Legend_IsNotRendered_WhenDisabled()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.ShowLegend, false)
			);

			Assert.IsEmpty(cut.FindAll("ul.donut-legend"));
		}

		/// <summary>
		/// Ensures that the legend is rendered with one entry per slice
		/// when ShowLegend = true.
		/// </summary>
		[TestMethod]
		public void Legend_IsRendered_WhenEnabled()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 } })
				.Add(x => x.ShowLegend, true)
			);

			Assert.HasCount(2, cut.FindAll("ul.donut-legend li"));
		}

		/// <summary>
		/// Verifies that no SVG slice paths are rendered when all data values are zero.
		/// </summary>
		[TestMethod]
		public void NoSlices_RendersNoPaths()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 0 } })
			);

			Assert.IsEmpty(cut.FindAll("path.donut-slice"));
		}

		/// <summary>
		/// Ensures that a single slice is rendered when the dataset contains one entry.
		/// </summary>
		[TestMethod]
		public void SingleSlice_RendersOnePath()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "Only", 100 } })
			);

			Assert.HasCount(1, cut.FindAll("path.donut-slice"));
		}

		/// <summary>
		/// Verifies that clicking a slice triggers the OnSliceClick callback
		/// and that the emitted ChartClickEventArgs contains the correct label.
		/// </summary>
		[TestMethod]
		public void SliceClick_InvokesCallback()
		{
			ChartClickEventArgs? received = null;

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 } })
				.Add(x => x.OnSliceClick,
					EventCallback.Factory.Create<ChartClickEventArgs>(this, args => received = args))
			);

			cut.FindAll("path.donut-slice")[1].Click();

			Assert.IsNotNull(received);
			Assert.AreEqual("B", received!.SliceLabel);
		}

		/// <summary>
		/// Ensures that hovering over a slice displays the tooltip.
		/// </summary>
		[TestMethod]
		public void SliceHover_ShowsTooltip()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 123 } })
			);

			cut.Find("path.donut-slice").MouseOver();

			Assert.HasCount(1, cut.FindAll("div.donut-tooltip"));
		}

		/// <summary>
		/// Confirms that the SVG container and slice paths are rendered
		/// when valid data is supplied.
		/// </summary>
		[TestMethod]
		public void SvgAndPaths_AreRendered()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 } })
			);

			Assert.HasCount(1, cut.FindAll("svg.donut-chart"));
			Assert.HasCount(2, cut.FindAll("path.donut-slice"));
		}

		/// <summary>
		/// Verifies that the chart title is rendered when provided.
		/// </summary>
		[TestMethod]
		public void Title_IsRendered()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.Title, "My Chart")
			);

			Assert.HasCount(1, cut.FindAll("h3.chart-title"));
		}

		/// <summary>
		/// Ensures that the tooltip is not visible before any hover interaction occurs.
		/// </summary>
		[TestMethod]
		public void Tooltip_IsHiddenByDefault()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
			);

			Assert.IsEmpty(cut.FindAll("div.donut-tooltip"));
		}

		/// <summary>
		/// Verifies that the tooltip becomes hidden again when the mouse leaves the chart.
		/// </summary>
		[TestMethod]
		public void Tooltip_IsHiddenOnMouseLeave()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
			);

			cut.Find("path.donut-slice").MouseOver();
			Assert.HasCount(1, cut.FindAll("div.donut-tooltip"));

			cut.Find("svg.donut-chart").MouseLeave();
			Assert.IsEmpty(cut.FindAll("div.donut-tooltip"));
		}
	}
}