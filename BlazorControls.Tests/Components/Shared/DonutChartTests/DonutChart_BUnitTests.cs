using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bunit;
using BunitContext = Bunit.BunitContext;
using BlazorControls.Components.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace BlazorControls.Tests.Components.Shared.DonutChartTests
{
	// ============================================================
	//  BUNIT TESTS
	// ============================================================
	[TestClass]
	public class DonutChart_BUnitTests
	{
		private BunitContext _ctx = null!;

		[TestInitialize]
		public void Init() => _ctx = new BunitContext();

		[TestCleanup]
		public void Cleanup() => _ctx.Dispose();

		[TestMethod]
		public void Renders_Title_When_Provided()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.Title, "My Chart")
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 10 } })
			);

			Assert.Contains("My Chart", cut.Markup);
		}

		[TestMethod]
		public void Does_Not_Render_Title_When_Empty()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.Title, "")
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 10 } })
			);

			Assert.DoesNotContain("chart-title", cut.Markup);
		}

		[TestMethod]
		public void Renders_Legend_When_Enabled()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.ShowLegend, true)
				.Add(x => x.Data, new Dictionary<string, int>
				{
					{ "B", 20 },
					{ "A", 10 }
				})
			);

			var items = cut.FindAll("ul.donut-legend li");
			Assert.HasCount(2, items);
		}

		[TestMethod]
		public void Does_Not_Render_Legend_When_Disabled()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.ShowLegend, false)
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 10 } })
			);

			Assert.DoesNotContain("donut-legend", cut.Markup);
		}

		[TestMethod]
		public void Slice_Click_Invokes_Callback()
		{
			string? clicked = null;

			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 10 } })
				.Add(x => x.OnSliceClick, EventCallback.Factory.Create<string>(this, v => clicked = v))
			);

			cut.Find("path").Click();

			Assert.AreEqual("A", clicked);
		}

		[TestMethod]
		public void Hovering_Slice_Shows_Tooltip()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 1234 } })
			);

			cut.Find("path").MouseOver();

			Assert.IsTrue(cut.Instance.ShowTooltip);
			Assert.AreEqual("A", cut.Instance.TooltipLabel);
			Assert.AreEqual("1,234", cut.Instance.TooltipValue);
		}

		[TestMethod]
		public void MouseLeave_Clears_Tooltip()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 10 } })
			);

			cut.Find("path").MouseOver();
			Assert.IsTrue(cut.Instance.ShowTooltip);

			cut.Find("svg").MouseLeave();
			Assert.IsFalse(cut.Instance.ShowTooltip);
		}

		[TestMethod]
		public void Hovering_Center_Shows_Total()
		{
			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.InnerTitle, "Total")
				.Add(x => x.Data, new Dictionary<string, int>
				{
					{ "A", 10 },
					{ "B", 20 }
				})
			);

			cut.Find("g.donut-center").MouseOver();

			Assert.IsTrue(cut.Instance.ShowTooltip);
			Assert.AreEqual("Total", cut.Instance.TooltipLabel);
			Assert.AreEqual("30", cut.Instance.TooltipValue);
		}

		[TestMethod]
		public void Center_Click_Invokes_Callback()
		{
			bool clicked = false;

			var cut = _ctx.Render<DonutChart>(p => p
				.Add(x => x.OnCenterClick, EventCallback.Factory.Create(this, () => clicked = true))
				.Add(x => x.Data, new Dictionary<string, int> { { "A", 10 } })
			);

			cut.Find("g.donut-center").Click();

			Assert.IsTrue(clicked);
		}
	}
}