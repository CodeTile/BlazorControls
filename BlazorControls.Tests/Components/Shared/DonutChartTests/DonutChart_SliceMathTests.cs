using Bunit;
using BunitContext = Bunit.BunitContext;
using BlazorControls.Components.Shared;

namespace BlazorControls.Tests.Components.Shared.DonutChartTests
{
	// ============================================================
	//  SLICE & MATH TESTS
	// ============================================================
	[TestClass]
	public class DonutChart_SliceMathTests
	{
		private BunitContext _ctx = null!;

		[TestInitialize]
		public void Init() => _ctx = new BunitContext();

		[TestCleanup]
		public void Cleanup() => _ctx.Dispose();

		[TestMethod]
		public void IncludeLabels_Filters_Slices()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.IncludeLabels, new[] { "A", "C" })
				.Add(x => x.Data, new Dictionary<string, int>
				{
					{ "A", 10 },
					{ "B", 20 },
					{ "C", 30 }
				})
			);

			var labels = cut.Instance.Slices.Select(s => s.Label).ToList();

			CollectionAssert.AreEquivalent(new[] { "A", "C" }, labels);
		}

		[TestMethod]
		public void StatusColors_Override_Defaults()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.StatusColors, new Dictionary<string, string>
				{
					{ "A", "#FF0000" }
				})
				.Add(x => x.Data, new Dictionary<string, int>
				{
					{ "A", 10 },
					{ "B", 20 }
				})
			);

			Assert.AreEqual("#FF0000", cut.Instance.Slices[0].Color);
		}

		[TestMethod]
		public void DefaultColors_Are_Used_When_No_StatusColor()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.DefaultColors, new List<string> { "#111", "#222" })
				.Add(x => x.Data, new Dictionary<string, int>
				{
					{ "A", 10 },
					{ "B", 10 }
				})
			);

			var slices = cut.Instance.Slices;

			Assert.AreEqual("#111", slices[0].Color);
			Assert.AreEqual("#222", slices[1].Color);
		}

		[TestMethod]
		public void GenerateColor_Is_Stable()
		{
			var cut1 = _ctx.Render<DonutChart>(p => p
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 10 } })
			);

			var cut2 = _ctx.Render<DonutChart>(p => p
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 20 } })
			);

			Assert.AreEqual(cut1.Instance.Slices[0].Color, cut2.Instance.Slices[0].Color);
		}
	}
}