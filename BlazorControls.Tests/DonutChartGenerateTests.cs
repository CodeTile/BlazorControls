using System.Linq;
using Bunit;
using BlazorControls.Components;
using Shouldly;
using Xunit;

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
	public class DonutChartGenerateTests : BunitContext
	{
		/// <summary>
		/// Ensures that when <c>Data</c> is null, the chart produces no slices.
		/// </summary>
		[Fact]
		public void DataNull_ProducesNoSlices()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, null)
			);

			cut.Instance.Slices.ShouldBeEmpty();
		}

		/// <summary>
		/// Verifies that default colors are applied in a cyclic (round‑robin)
		/// sequence when more slices exist than colors.
		/// </summary>
		[Fact]
		public void DefaultColors_AreAppliedInCyclicOrder()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 }, { "C", 30 } })
				.Add(x => x.DefaultColors, ["#AA", "#BB"])
			);

			var slices = cut.Instance.Slices;

			slices.Single(s => s.Label == "A").Color.ShouldBe("#AA");
			slices.Single(s => s.Label == "B").Color.ShouldBe("#BB");
			slices.Single(s => s.Label == "C").Color.ShouldBe("#AA");
		}

		/// <summary>
		/// Ensures that explicit status colors override default colors
		/// when both are provided.
		/// </summary>
		[Fact]
		public void DefaultColors_AreIgnoredWhenStatusColorExists()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 10 } })
				.Add(x => x.StatusColors, new() { { "A", "#111111" } })
				.Add(x => x.DefaultColors, new() { "#222222" })
			);

			var slices = cut.Instance.Slices;

			slices.Single(s => s.Label == "A").Color.ShouldBe("#111111");
			slices.Single(s => s.Label == "B").Color.ShouldBe("#222222");
		}

		/// <summary>
		/// Confirms that when no default colors or status colors are supplied,
		/// the component generates a stable fallback HSL color.
		/// </summary>
		[Fact]
		public void GeneratedColor_IsUsedWhenNoDefaultsOrStatusColors()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "Only", 42 } })
				.Add(x => x.DefaultColors, null)
				.Add(x => x.StatusColors, null)
			);

			var color = cut.Instance.Slices.Single().Color;

			color.ShouldStartWith("hsl(");
		}

		/// <summary>
		/// Ensures that <c>IncludeLabels</c> correctly filters the dataset
		/// before slice generation.
		/// </summary>
		[Fact]
		public void IncludeLabels_FiltersDataCorrectly()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 10 }, { "B", 20 }, { "C", 30 } })
				.Add(x => x.IncludeLabels, new[] { "A", "C" })
			);

			var labels = cut.Instance.Slices.Select(s => s.Label).ToArray();

			labels.ShouldBe(new[] { "A", "C" });
		}

		/// <summary>
		/// Verifies that the inner radius is always zero when the chart
		/// is configured as a pie (IsDonut = false).
		/// </summary>
		[Fact]
		public void InnerRadius_IsZeroWhenNotDonut()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.IsDonut, false)
				.Add(x => x.Thickness, 20)
			);

			cut.Instance.InnerRadius.ShouldBe(0);
		}

		/// <summary>
		/// Ensures that excessively large thickness values do not produce
		/// negative inner radii and instead clamp to zero.
		/// </summary>
		[Fact]
		public void InnerRadius_IsZeroWhenThicknessTooLarge()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.IsDonut, true)
				.Add(x => x.Thickness, 999)
			);

			cut.Instance.InnerRadius.ShouldBe(0);
		}

		/// <summary>
		/// Confirms that the inner radius is computed as
		/// <c>90 - Thickness</c> when in donut mode.
		/// </summary>
		[Fact]
		public void InnerRadius_UsesThicknessCorrectly()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.IsDonut, true)
				.Add(x => x.Thickness, 20)
			);

			cut.Instance.InnerRadius.ShouldBe(70);
		}

		/// <summary>
		/// Ensures that explicit status colors override default colors
		/// when both are provided.
		/// </summary>
		[Fact]
		public void StatusColors_OverrideDefaults()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "X", 5 } })
				.Add(x => x.StatusColors, new() { { "X", "#ABCDEF" } })
				.Add(x => x.DefaultColors, new() { "#000000" })
			);

			cut.Instance.Slices.Single().Color.ShouldBe("#ABCDEF");
		}

		/// <summary>
		/// Verifies that <c>TotalValue</c> correctly sums the values
		/// of all generated slices.
		/// </summary>
		[Fact]
		public void TotalValue_ComputesSumCorrectly()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 5 }, { "B", 15 } })
			);

			cut.Instance.TotalValue.ShouldBe(20);
		}

		/// <summary>
		/// Ensures that zero or negative values are excluded from slice generation.
		/// </summary>
		[Fact]
		public void ZeroOrNegativeValues_AreIgnored()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 0 }, { "B", -5 }, { "C", 10 } })
			);

			var slices = cut.Instance.Slices;

			slices.Count.ShouldBe(1);
			slices[0].Label.ShouldBe("C");
		}

		/// <summary>
		/// Confirms that when all values are zero, the chart produces no slices.
		/// </summary>
		[Fact]
		public void ZeroTotal_ProducesNoSlices()
		{
			var cut = Render<DonutChart>(p => p
				.Add(x => x.Data, new() { { "A", 0 }, { "B", 0 } })
			);

			cut.Instance.Slices.ShouldBeEmpty();
		}
	}
}