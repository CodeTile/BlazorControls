using System.Collections.Generic;
using System.Linq;

using BlazorControls.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlazorControls.Components.Tests
{
	/// <summary>
	/// Tests validating mathematical correctness of sweep angles,
	/// large-arc flags, and proportional slice sizing.
	/// </summary>
	[TestClass]
	public class SliceMathTests
	{
		[TestMethod]
		public void LargeArcFlag_IsOne_WhenSweepIsLarge()
		{
			var slice = new DonutSlice("Large", 75, 0, 270, 90, 70, "#ABCDEF");
			var path = slice.PathData;

			StringAssert.Contains(path, "A 90 90 0 1 1");
			StringAssert.Contains(path, "A 70 70 0 1 0");
		}

		[TestMethod]
		public void LargeArcFlag_IsZero_WhenSweepIsSmall()
		{
			var slice = new DonutSlice("Small", 10, 0, 180, 90, 70, "#AAAAAA");
			var path = slice.PathData;

			StringAssert.Contains(path, "A 90 90 0 0 1");
			StringAssert.Contains(path, "A 70 70 0 0 0");
		}

		[TestMethod]
		public void SweepAngles_AreProportionalToValues()
		{
			// Arrange
			var chart = new DonutChart
			{
				Data = new() { { "A", 10 }, { "B", 30 } }
			};

			// Act
			chart.BuildSlices(); // call your internal slice builder directly if needed

			var sliceA = chart.Slices.Single(s => s.Label == "A");
			var sliceB = chart.Slices.Single(s => s.Label == "B");

			// Assert
			StringAssert.Contains(sliceB.PathData, "A 90 90 0 1 1");
			StringAssert.Contains(sliceA.PathData, "A 90 90 0 0 1");
		}

		[TestMethod]
		public void TotalValue_IsSumOfAllSliceValues()
		{
			var chart = new DonutChart
			{
				Data = new() { { "A", 10 }, { "B", 20 }, { "C", 30 } }
			};

			chart.BuildSlices();

			Assert.AreEqual(60, chart.TotalValue);
		}
	}
}