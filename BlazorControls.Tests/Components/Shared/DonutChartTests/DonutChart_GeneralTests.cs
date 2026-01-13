using Bunit;
using BunitContext = Bunit.BunitContext;
using BlazorControls.Components.Shared;

namespace BlazorControls.Tests.Components.Shared.DonutChartTests
{
	// ============================================================
	//  GENERAL LOGIC TESTS
	// ============================================================
	[TestClass]
	public class DonutChart_GeneralTests
	{
		private BunitContext _ctx = null!;

		[TestInitialize]
		public void Init() => _ctx = new BunitContext();

		[TestCleanup]
		public void Cleanup() => _ctx.Dispose();

		[TestMethod]
		public void InnerRadius_Computed_Correctly()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.Thickness, 20)
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 10 } })
			);

			Assert.AreEqual(70, cut.Instance.InnerRadius);
		}

		[TestMethod]
		public void InnerRadius_Zero_When_Pie()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.IsDonut, false)
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 10 } })
			);

			Assert.AreEqual(0, cut.Instance.InnerRadius);
		}

		[TestMethod]
		public void Zero_And_Negative_Values_Are_Ignored()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.Data, new Dictionary<string, int>
				{
					{ "A", 0 },
					{ "B", -5 },
					{ "C", 10 }
				})
			);

			Assert.HasCount(1, cut.Instance.Slices);
			Assert.AreEqual("C", cut.Instance.Slices[0].Label);
		}

		[TestMethod]
		public void No_Slices_When_Data_Null()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.Data, null!)
			);

			Assert.IsEmpty(cut.Instance.Slices);
		}
	}
}