using System.Collections.Generic;
using System.Linq;

using BlazorControls.Components;

using Shouldly;

using Xunit;

namespace BlazorControls.Components.Tests
{
	/// <summary>
	/// A suite of mathematical validation tests ensuring that
	/// <see cref="DonutSlice"/> and <see cref="DonutChart"/> compute
	/// sweep angles, large‑arc flags, and proportional slice geometry
	/// correctly according to SVG arc rules.
	/// </summary>

	public class DonutSliceMathTests
	{
		/// <summary>
		/// Verifies that slices with a sweep angle greater than 180°
		/// correctly set the SVG large‑arc flag to <c>1</c> for both
		/// outer and inner arcs.
		/// </summary>
		[Fact]
		public void LargeArcFlag_IsOne_WhenSweepIsLarge()
		{
			var slice = new DonutSlice("Large", 75, 0, 270, 90, 70, "#ABCDEF");
			var path = slice.PathData;

			path.ShouldContain("A 90 90 0 1 1");
			path.ShouldContain("A 70 70 0 1 0");
		}

		/// <summary>
		/// Ensures that slices with a sweep angle of 180° or less
		/// correctly set the SVG large‑arc flag to <c>0</c>.
		/// </summary>
		[Fact]
		public void LargeArcFlag_IsZero_WhenSweepIsSmall()
		{
			var slice = new DonutSlice("Small", 10, 0, 180, 90, 70, "#AAAAAA");
			var path = slice.PathData;

			path.ShouldContain("A 90 90 0 0 1");
			path.ShouldContain("A 70 70 0 0 0");
		}

		/// <summary>
		/// Confirms that slice sweep angles are proportional to their
		/// underlying data values and that the resulting SVG arc flags
		/// reflect the correct sweep size.
		/// </summary>
		[Fact]
		public void SweepAngles_AreProportionalToValues()
		{
			// Arrange
			var chart = new DonutChart
			{
				Data = new() { { "A", 10 }, { "B", 30 } }
			};

			// Act
			chart.BuildSlices();

			var sliceA = chart.Slices.Single(s => s.Label == "A");
			var sliceB = chart.Slices.Single(s => s.Label == "B");

			// Assert
			sliceB.PathData.ShouldContain("A 90 90 0 1 1"); // large sweep
			sliceA.PathData.ShouldContain("A 90 90 0 0 1"); // small sweep
		}

		/// <summary>
		/// Ensures that <see cref="DonutChart.TotalValue"/> correctly
		/// computes the sum of all slice values after slice generation.
		/// </summary>
		[Fact]
		public void TotalValue_IsSumOfAllSliceValues()
		{
			var chart = new DonutChart
			{
				Data = new() { { "A", 10 }, { "B", 20 }, { "C", 30 } }
			};

			chart.BuildSlices();

			chart.TotalValue.ShouldBe(60);
		}
	}
}