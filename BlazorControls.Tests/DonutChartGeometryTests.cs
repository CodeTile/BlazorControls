using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorControls.Components;

namespace BlazorControls.Components.Tests
{
	/// <summary>
	/// Tests validating the SVG path geometry produced by DonutSlice,
	/// including donut vs pie behavior and full-circle handling.
	/// </summary>
	[TestClass]
	public class DonutChartGeometryTests
	{
		[TestMethod]
		public void DonutPath_ContainsInnerAndOuterArcs()
		{
			var slice = new DonutSlice("Donut", 50, 0, 90, 90, 70, "#0000FF");
			var path = slice.PathData;

			StringAssert.Contains(path, "A 90 90");
			StringAssert.Contains(path, "A 70 70");
		}

		[TestMethod]
		public void DonutPath_DoesNotStartAtCenter()
		{
			var slice = new DonutSlice("Donut", 10, 0, 60, 90, 70, "#123456");
			Assert.IsFalse(slice.PathData.StartsWith("M 100 100"));
		}

		[TestMethod]
		public void FullCircleDonut_UsesTwoInnerAndTwoOuterArcs()
		{
			var slice = new DonutSlice("Full", 100, 0, 360, 90, 70, "#654321");
			var path = slice.PathData;

			Assert.AreEqual(2, path.Split("A 90 90").Length - 1);
			Assert.AreEqual(2, path.Split("A 70 70").Length - 1);
		}

		[TestMethod]
		public void FullCirclePie_UsesTwoOuterArcs()
		{
			var slice = new DonutSlice("FullPie", 100, 0, 360, 90, 0, "#123456");
			Assert.AreEqual(2, slice.PathData.Split("A 90 90").Length - 1);
		}

		[TestMethod]
		public void PiePath_ContainsOuterArcOnly()
		{
			var slice = new DonutSlice("Pie", 50, 0, 90, 90, 0, "#00FF00");
			var path = slice.PathData;

			StringAssert.StartsWith(path, "M 100 100");
			StringAssert.Contains(path, "A 90 90");
		}

		[TestMethod]
		public void PiePath_DoesNotContainInnerArc()
		{
			var slice = new DonutSlice("Pie", 25, 0, 120, 90, 0, "#EEEEEE");
			Assert.IsFalse(slice.PathData.Contains("A 70 70"));
		}

		[TestMethod]
		public void Slice_HasNonEmptyPath()
		{
			var slice = new DonutSlice("Basic", 10, 0, 45, 90, 70, "#FF0000");
			Assert.IsFalse(string.IsNullOrWhiteSpace(slice.PathData));
		}

		[TestMethod]
		public void Slice_SetsBasicProperties()
		{
			var slice = new DonutSlice("Test", 10, 0, 90, 90, 70, "#FF0000");

			Assert.AreEqual("Test", slice.Label);
			Assert.AreEqual(10, slice.Value);
			Assert.AreEqual("#FF0000", slice.Color);
			Assert.AreEqual("donut-slice", slice.CssClass);
		}
	}
}