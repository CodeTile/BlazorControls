using System.Collections.Generic;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorControls.Components.Shared;

namespace BlazorControls.Tests.Components.Shared.DonutChartTests
{
	/// <summary>
	/// Tests validating rendering, DOM structure, interactivity,
	/// tooltips, legends, and event callbacks using bUnit.
	/// </summary>
	[TestClass]
	public class DonutChartBunitTests : BunitContext
	{
		[TestMethod]
		public void CenterClick_InvokesCallback()
		{
			bool clicked = false;

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.IsDonut, true)
				.Add(x => x.OnCenterClick, EventCallback.Factory.Create(this, () => clicked = true))
			);

			cut.Find("g.donut-center").Click();

			Assert.IsTrue(clicked);
		}

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
			StringAssert.Contains(tooltip.TextContent, "Total");
			StringAssert.Contains(tooltip.TextContent, "30");
		}

		[TestMethod]
		public void InnerCircle_IsNotRendered_WhenPie()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.IsDonut, false)
			);

			Assert.AreEqual(0, cut.FindAll("g.donut-center").Count);
		}

		[TestMethod]
		public void InnerCircle_IsRendered_WhenDonut()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.IsDonut, true)
			);

			Assert.AreEqual(1, cut.FindAll("g.donut-center").Count);
		}

		[TestMethod]
		public void InnerTitle_IsRendered()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.IsDonut, true)
				.Add(x => x.InnerTitle, "Inner")
			);
			Assert.AreEqual(1, cut.FindAll(".donut-inner-title").Count);
		}

		[TestMethod]
		public void Legend_IsNotRendered_WhenDisabled()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.ShowLegend, false)
			);

			Assert.AreEqual(0, cut.FindAll("ul.donut-legend").Count);
		}

		[TestMethod]
		public void Legend_IsRendered_WhenEnabled()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 } })
				.Add(x => x.ShowLegend, true)
			);

			Assert.AreEqual(2, cut.FindAll("ul.donut-legend li").Count);
		}

		[TestMethod]
		public void NoSlices_RendersNoPaths()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 0 } })
			);

			Assert.AreEqual(0, cut.FindAll("path.donut-slice").Count);
		}

		[TestMethod]
		public void SingleSlice_RendersOnePath()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "Only", 100 } })
			);

			Assert.AreEqual(1, cut.FindAll("path.donut-slice").Count);
		}

		[TestMethod]
		public void SliceClick_InvokesCallback()
		{
			string? clicked = null;

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 } })
				.Add(x => x.OnSliceClick, EventCallback.Factory.Create<string>(this, v => clicked = v))
			);

			cut.FindAll("path.donut-slice")[1].Click();

			Assert.AreEqual("B", clicked);
		}

		[TestMethod]
		public void SliceHover_ShowsTooltip()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 123 } })
			);

			cut.Find("path.donut-slice").MouseOver();

			Assert.AreEqual(1, cut.FindAll("div.donut-tooltip").Count);
		}

		[TestMethod]
		public void SvgAndPaths_AreRendered()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 } })
			);

			Assert.AreEqual(1, cut.FindAll("svg.donut-chart").Count);
			Assert.AreEqual(2, cut.FindAll("path.donut-slice").Count);
		}

		[TestMethod]
		public void Title_IsRendered()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
				.Add(x => x.Title, "My Chart")
			);

			Assert.AreEqual(1, cut.FindAll("h3.chart-title").Count);
		}

		[TestMethod]
		public void Tooltip_IsHiddenByDefault()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
			);

			Assert.AreEqual(0, cut.FindAll("div.donut-tooltip").Count);
		}

		[TestMethod]
		public void Tooltip_IsHiddenOnMouseLeave()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 } })
			);

			cut.Find("path.donut-slice").MouseOver();
			Assert.AreEqual(1, cut.FindAll("div.donut-tooltip").Count);

			cut.Find("svg.donut-chart").MouseLeave();
			Assert.AreEqual(0, cut.FindAll("div.donut-tooltip").Count);
		}
	}
}