using System.Linq;
using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorControls.Components.Shared;

namespace BlazorControls.Tests.Components.Shared.DonutChartTests
{
	[TestClass]
	public class DonutChartGenerateTests : BunitContext
	{
		[TestMethod]
		public void DataNull_ProducesNoSlices()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, null)
			);

			Assert.AreEqual(0, cut.Instance.Slices.Count);
		}

		[TestMethod]
		public void DefaultColors_AreAppliedInCyclicOrder()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 }, { "C", 30 } })
				.Add(x => x.DefaultColors, new() { "#AA", "#BB" })
			);

			var slices = cut.Instance.Slices;

			Assert.AreEqual("#AA", slices.Single(s => s.Label == "A").Color);
			Assert.AreEqual("#BB", slices.Single(s => s.Label == "B").Color);
			Assert.AreEqual("#AA", slices.Single(s => s.Label == "C").Color);
		}

		[TestMethod]
		public void DefaultColors_AreIgnoredWhenStatusColorExists()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 10 } })
				.Add(x => x.StatusColors, new() { { "A", "#111111" } })
				.Add(x => x.DefaultColors, new() { "#222222" })
			);

			var slices = cut.Instance.Slices;

			Assert.AreEqual("#111111", slices.Single(s => s.Label == "A").Color);
			Assert.AreEqual("#222222", slices.Single(s => s.Label == "B").Color);
		}

		[TestMethod]
		public void GeneratedColor_IsUsedWhenNoDefaultsOrStatusColors()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "Only", 42 } })
				.Add(x => x.DefaultColors, null)
				.Add(x => x.StatusColors, null)
			);

			var color = cut.Instance.Slices.Single().Color;

			StringAssert.StartsWith(color, "hsl(");
		}

		[TestMethod]
		public void IncludeLabels_FiltersDataCorrectly()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 }, { "C", 30 } })
				.Add(x => x.IncludeLabels, new[] { "A", "C" })
			);

			var labels = cut.Instance.Slices.Select(s => s.Label).ToArray();

			CollectionAssert.AreEquivalent(new[] { "A", "C" }, labels);
		}

		[TestMethod]
		public void InnerRadius_IsZeroWhenNotDonut()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.IsDonut, false)
				.Add(x => x.Thickness, 20)
			);

			Assert.AreEqual(0, cut.Instance.InnerRadius);
		}

		[TestMethod]
		public void InnerRadius_IsZeroWhenThicknessTooLarge()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.IsDonut, true)
				.Add(x => x.Thickness, 999)
			);

			Assert.AreEqual(0, cut.Instance.InnerRadius);
		}

		[TestMethod]
		public void InnerRadius_UsesThicknessCorrectly()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.IsDonut, true)
				.Add(x => x.Thickness, 20)
			);

			Assert.AreEqual(70, cut.Instance.InnerRadius);
		}

		[TestMethod]
		public void StatusColors_OverrideDefaults()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "X", 5 } })
				.Add(x => x.StatusColors, new() { { "X", "#ABCDEF" } })
				.Add(x => x.DefaultColors, new() { "#000000" })
			);

			Assert.AreEqual("#ABCDEF", cut.Instance.Slices.Single().Color);
		}

		[TestMethod]
		public void TotalValue_ComputesSumCorrectly()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 5 }, { "B", 15 } })
			);

			Assert.AreEqual(20, cut.Instance.TotalValue);
		}

		[TestMethod]
		public void ZeroOrNegativeValues_AreIgnored()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 0 }, { "B", -5 }, { "C", 10 } })
			);

			var slices = cut.Instance.Slices;

			Assert.AreEqual(1, slices.Count);
			Assert.AreEqual("C", slices[0].Label);
		}

		[TestMethod]
		public void ZeroTotal_ProducesNoSlices()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 0 }, { "B", 0 } })
			);

			Assert.AreEqual(0, cut.Instance.Slices.Count);
		}
	}
}