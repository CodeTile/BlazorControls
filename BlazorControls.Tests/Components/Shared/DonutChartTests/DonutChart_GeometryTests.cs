using Bunit;
using BunitContext = Bunit.BunitContext;
using BlazorControls.Components.Shared;

namespace BlazorControls.Tests.Components.Shared.DonutChartTests
{
	// ============================================================
	//  GEOMETRY TESTS
	// ============================================================
	[TestClass]
	public class DonutChart_GeometryTests
	{
		private BunitContext _ctx = null!;

		[TestInitialize]
		public void Init() => _ctx = new BunitContext();

		[TestCleanup]
		public void Cleanup() => _ctx.Dispose();

		[TestMethod]
		public void Each_Slice_Has_PathData()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.Data, new Dictionary<string, int>
				{
					{ "A", 10 },
					{ "B", 20 }
				})
			);

			foreach (var slice in cut.Instance.Slices)
				Assert.IsFalse(string.IsNullOrWhiteSpace(slice.PathData));
		}

		[TestMethod]
		public void Renders_One_Path_Per_Slice()
		{
			var data = new Dictionary<string, int>
			{
				{ "A", 10 },
				{ "B", 20 },
				{ "C", 30 }
			};

			var cut = _ctx.Render<DonutChart>(p => p.Add(x => x.Data, data));

			var paths = cut.FindAll("path");
			Assert.HasCount(data.Count, paths);
		}

		[TestMethod]
		public void Legend_Is_Sorted_Alphabetically()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.ShowLegend, true)
				.Add(x => x.Data, new Dictionary<string, int>
				{
					{ "C", 10 },
					{ "A", 10 },
					{ "B", 10 }
				})
			);

			var items = cut.FindAll("ul.donut-legend li")
						   .Select(i => i.TextContent.Trim())
						   .ToList();

			Assert.StartsWith("A", items[0]);
			Assert.StartsWith("B", items[1]);
			Assert.StartsWith("C", items[2]);
		}

		[TestMethod]
		public void InnerTitle_Renders_When_Valid()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.InnerTitle, "Inner")
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 10 } })
			);

			Assert.Contains("donut-inner-title", cut.Markup);
			Assert.Contains("Inner", cut.Markup);
		}

		[TestMethod]
		public void InnerTitle_Not_Rendered_When_Empty()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.InnerTitle, "")
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 10 } })
			);

			Assert.DoesNotContain("donut-inner-title", cut.Markup);
		}
	}
}