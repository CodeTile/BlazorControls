using System.Linq;
using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorControls.Components;

namespace BlazorControls.Components.Tests
{
	/// <summary>
	/// A suite of logic-focused bUnit tests validating slice generation,
	/// color resolution, filtering, and computed properties for the
	/// <see cref="DonutChart"/> component.
	/// <para>
	/// These tests operate on the component instance directly rather than
	/// interacting with rendered DOM elements.
	/// </para>
	/// </summary>
	[TestClass]
	public class DonutChartGenerateTests : BunitContext
	{
		/// <summary>
		/// Ensures that when <c>Data</c> is null, the chart produces no slices.
		/// </summary>
		[TestMethod]
		public void DataNull_ProducesNoSlices()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, null)
			);

			Assert.IsEmpty(cut.Instance.Slices);
		}

		/// <summary>
		/// Verifies that default colors are applied in a cyclic (round‑robin)
		/// sequence when more slices exist than colors.
		/// </summary>
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

		/// <summary>
		/// Ensures that explicit status colors override default colors
		/// when both are provided.
		/// </summary>
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

		/// <summary>
		/// Confirms that when no default colors or status colors are supplied,
		/// the component generates a stable fallback HSL color.
		/// </summary>
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

		/// <summary>
		/// Ensures that <c>IncludeLabels</c> correctly filters the dataset
		/// before slice generation.
		/// </summary>
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

		/// <summary>
		/// Verifies that the inner radius is always zero when the chart
		/// is configured as a pie (IsDonut = false).
		/// </summary>
		[TestMethod]
		public void InnerRadius_IsZeroWhenNotDonut()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.IsDonut, false)
				.Add(x => x.Thickness, 20)
			);

			Assert.AreEqual(0, cut.Instance.InnerRadius);
		}

		/// <summary>
		/// Ensures that excessively large thickness values do not produce
		/// negative inner radii and instead clamp to zero.
		/// </summary>
		[TestMethod]
		public void InnerRadius_IsZeroWhenThicknessTooLarge()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.IsDonut, true)
				.Add(x => x.Thickness, 999)
			);

			Assert.AreEqual(0, cut.Instance.InnerRadius);
		}

		/// <summary>
		/// Confirms that the inner radius is computed as
		/// <c>90 - Thickness</c> when in donut mode.
		/// </summary>
		[TestMethod]
		public void InnerRadius_UsesThicknessCorrectly()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.IsDonut, true)
				.Add(x => x.Thickness, 20)
			);

			Assert.AreEqual(70, cut.Instance.InnerRadius);
		}

		/// <summary>
		/// Ensures that explicit status colors override default colors
		/// when both are provided.
		/// </summary>
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

		/// <summary>
		/// Verifies that <c>TotalValue</c> correctly sums the values
		/// of all generated slices.
		/// </summary>
		[TestMethod]
		public void TotalValue_ComputesSumCorrectly()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 5 }, { "B", 15 } })
			);

			Assert.AreEqual(20, cut.Instance.TotalValue);
		}

		/// <summary>
		/// Ensures that zero or negative values are excluded from slice generation.
		/// </summary>
		[TestMethod]
		public void ZeroOrNegativeValues_AreIgnored()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 0 }, { "B", -5 }, { "C", 10 } })
			);

			var slices = cut.Instance.Slices;

			Assert.HasCount(1, slices);
			Assert.AreEqual("C", slices[0].Label);
		}

		/// <summary>
		/// Confirms that when all values are zero, the chart produces no slices.
		/// </summary>
		[TestMethod]
		public void ZeroTotal_ProducesNoSlices()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 0 }, { "B", 0 } })
			);

			Assert.IsEmpty(cut.Instance.Slices);
		}
	}
}