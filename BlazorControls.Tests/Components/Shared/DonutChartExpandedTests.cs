using BlazorControls.Components.Shared;

using Bunit;

using Microsoft.AspNetCore.Components;

namespace DonutChartSolution.Tests.Components.Shared
{
	[TestClass]
	public class DonutChartExpandedTests : BunitContext
	{
		// ---------------------------------------------------------
		// GEOMETRY TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void DonutMode_UsesInnerRadius()
		{
			var data = new Dictionary<string, int> { ["A"] = 100 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
				.Add(x => x.IsDonut, true)
				.Add(x => x.Thickness, 30)
			);

			Assert.AreEqual(60, cut.Instance.InnerRadius);
		}

		[TestMethod]
		public void PieMode_HasZeroInnerRadius()
		{
			var data = new Dictionary<string, int> { ["A"] = 100 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
				.Add(x => x.IsDonut, false)
			);

			Assert.AreEqual(0, cut.Instance.InnerRadius);
		}

		[TestMethod]
		public void SvgContainsCorrectViewBox()
		{
			var data = new Dictionary<string, int> { ["A"] = 100 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
			);

			StringAssert.Contains(cut.Markup, "viewBox=\"0 0 200 220\"");
		}

		// ---------------------------------------------------------
		// SLICE MATH TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void ZeroValueSlices_AreIgnored()
		{
			var data = new Dictionary<string, int>
			{
				["A"] = 0,
				["B"] = 50
			};

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
			);

			Assert.AreEqual(1, cut.FindAll("path.donut-slice").Count);
		}

		[TestMethod]
		public void AllZeroValues_ProduceNoSlices()
		{
			var data = new Dictionary<string, int>
			{
				["A"] = 0,
				["B"] = 0
			};

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
			);

			Assert.AreEqual(0, cut.FindAll("path.donut-slice").Count);
		}

		[TestMethod]
		public void SingleSlice_FillsFullCircle()
		{
			var data = new Dictionary<string, int> { ["A"] = 100 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
			);

			var path = cut.Find("path.donut-slice").GetAttribute("d");

			StringAssert.Contains(path, "A 90 90 0 1 1");
		}

		[TestMethod]
		public void LargeValues_StillProduceValidAngles()
		{
			var data = new Dictionary<string, int>
			{
				["A"] = 1_000_000,
				["B"] = 500_000
			};

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
			);

			Assert.AreEqual(2, cut.FindAll("path.donut-slice").Count);
		}

		// ---------------------------------------------------------
		// COLOUR SYSTEM TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void StatusColors_OverrideDefaultColors()
		{
			var data = new Dictionary<string, int> { ["A"] = 10, ["B"] = 20 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
				.Add(x => x.StatusColors, new Dictionary<string, string>
				{
					["A"] = "#FF0000"
				})
				.Add(x => x.DefaultColors, new List<string> { "#00FF00", "#0000FF" })
			);

			var slices = cut.Instance.Slices;

			Assert.AreEqual("#FF0000", slices[0].Color);
			Assert.AreEqual("#00FF00", slices[1].Color);
		}

		[TestMethod]
		public void DefaultColors_CycleCorrectly()
		{
			var data = new Dictionary<string, int>
			{
				["A"] = 10,
				["B"] = 20,
				["C"] = 30
			};

			var palette = new List<string> { "#111", "#222" };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
				.Add(x => x.DefaultColors, palette)
			);

			var slices = cut.Instance.Slices;

			Assert.AreEqual("#111", slices[0].Color);
			Assert.AreEqual("#222", slices[1].Color);
			Assert.AreEqual("#111", slices[2].Color);
		}

		[TestMethod]
		public void FallbackColor_IsStableForSameLabel()
		{
			var data = new Dictionary<string, int> { ["A"] = 10 };

			var cut1 = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
			);

			var cut2 = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
			);

			Assert.AreEqual(cut1.Instance.Slices[0].Color, cut2.Instance.Slices[0].Color);
		}

		// ---------------------------------------------------------
		// FILTERING TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void IncludeLabels_FiltersSlicesCorrectly()
		{
			var data = new Dictionary<string, int>
			{
				["A"] = 10,
				["B"] = 20,
				["C"] = 30
			};

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, new[] { "A", "C" })
			);

			var slices = cut.Instance.Slices;

			Assert.AreEqual(2, slices.Count);
			Assert.IsTrue(slices.Any(s => s.Label == "A"));
			Assert.IsTrue(slices.Any(s => s.Label == "C"));
		}

		// ---------------------------------------------------------
		// TOOLTIP TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void HoveringSlice_ShowsTooltip()
		{
			var data = new Dictionary<string, int> { ["A"] = 50 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
			);

			cut.Find("path.donut-slice").MouseOver();

			Assert.IsTrue(cut.Instance.ShowTooltip);
			Assert.AreEqual("A", cut.Instance.TooltipLabel);
			Assert.AreEqual("50", cut.Instance.TooltipValue);
		}

		[TestMethod]
		public void HoveringCenter_ShowsTotalTooltip()
		{
			var data = new Dictionary<string, int> { ["A"] = 50 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
				.Add(x => x.IsDonut, true)
				.Add(x => x.InnerTitle, "Total")
			);

			cut.Find("g.donut-center").MouseOver();

			Assert.IsTrue(cut.Instance.ShowTooltip);
			Assert.AreEqual("Total", cut.Instance.TooltipLabel);
			Assert.AreEqual("50", cut.Instance.TooltipValue);
		}

		[TestMethod]
		public void MouseLeave_HidesTooltip()
		{
			var data = new Dictionary<string, int> { ["A"] = 50 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
			);

			cut.Find("svg").MouseLeave();

			Assert.IsFalse(cut.Instance.ShowTooltip);
		}

		// ---------------------------------------------------------
		// CLICK TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void ClickingCenter_InvokesOnCenterClick()
		{
			bool clicked = false;

			var data = new Dictionary<string, int> { ["A"] = 100 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
				.Add(x => x.OnCenterClick, EventCallback.Factory.Create(this, () => clicked = true))
			);

			cut.Find("g.donut-center").Click();

			Assert.IsTrue(clicked);
		}

		[TestMethod]
		public void CenterClick_IgnoredInPieMode()
		{
			bool clicked = false;

			var data = new Dictionary<string, int> { ["A"] = 100 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
				.Add(x => x.IsDonut, false)
				.Add(x => x.OnCenterClick, EventCallback.Factory.Create(this, () => clicked = true))
			);

			Assert.AreEqual(0, cut.FindAll("g.donut-center").Count);
			Assert.IsFalse(clicked);
		}

		// ---------------------------------------------------------
		// LEGEND TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void LegendRenders_WhenEnabled()
		{
			var data = new Dictionary<string, int> { ["A"] = 10 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
				.Add(x => x.ShowLegend, true)
			);

			Assert.AreEqual(1, cut.FindAll("ul.donut-legend li").Count);
		}

		[TestMethod]
		public void LegendDoesNotRender_WhenDisabled()
		{
			var data = new Dictionary<string, int> { ["A"] = 10 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
				.Add(x => x.ShowLegend, false)
			);

			Assert.AreEqual(0, cut.FindAll("ul.donut-legend").Count);
		}

		// ---------------------------------------------------------
		// ARC FLAG TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void SmallSlice_UsesSmallArcFlag()
		{
			var data = new Dictionary<string, int> { ["A"] = 1, ["B"] = 999 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
			);

			var smallSlice = cut.Instance.Slices.First(s => s.Label == "A");

			StringAssert.Contains(smallSlice.PathData, "0 0");
		}

		[TestMethod]
		public void LargeSlice_UsesLargeArcFlag()
		{
			var data = new Dictionary<string, int> { ["A"] = 999, ["B"] = 1 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
			);

			var largeSlice = cut.Instance.Slices.First(s => s.Label == "A");

			StringAssert.Contains(largeSlice.PathData, "0 1");
		}

		// ---------------------------------------------------------
		// PIE MODE TESTS
		// ---------------------------------------------------------

		[TestMethod]
		public void PieMode_RendersPieSlices()
		{
			var data = new Dictionary<string, int> { ["A"] = 100 };

			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, data)
				.Add(x => x.IncludeLabels, data.Keys)
				.Add(x => x.IsDonut, false)
			);

			var slice = cut.Instance.Slices[0];

			StringAssert.Contains(slice.PathData, "M 100 100");
		}
	}
}
