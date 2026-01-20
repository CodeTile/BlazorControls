using Microsoft.AspNetCore.Components;

namespace BlazorControls.Components.Pages;

public partial class Charts : ComponentBase, IDisposable
{
	[Inject] private NavigationManager Nav { get; set; } = default!;

	// -----------------------------
	// Donut chart data
	// -----------------------------
	private Dictionary<string, int> SalesData = new()
	{
		["North"] = 120,
		["South"] = 80,
		["East"] = 140,
		["West"] = 60
	};

	private List<string> AllSalesLabels => [.. SalesData.Keys];
	private IEnumerable<string>? CurrentSalesLabels;

	// -----------------------------
	// Pie chart data (now a dictionary)
	// -----------------------------
	private Dictionary<string, int> TaskData = new()
	{
		["Completed"] = 30,
		["In Progress"] = 15,
		["Blocked"] = 5
	};

	private List<string> AllTaskLabels => [.. TaskData.Keys];
	private IEnumerable<string>? CurrentTaskLabels;

	// -----------------------------
	// Timers
	// -----------------------------
	private System.Timers.Timer? _salesTimer;

	private System.Timers.Timer? _taskTimer;

	private readonly Random _rand = new();

	protected override void OnInitialized()
	{
		CurrentSalesLabels = AllSalesLabels;
		CurrentTaskLabels = AllTaskLabels;

		_salesTimer = new System.Timers.Timer(3000);
		_salesTimer.Elapsed += (_, __) => UpdateSalesLabels();
		_salesTimer.AutoReset = true;
		_salesTimer.Enabled = true;

		_taskTimer = new System.Timers.Timer(3000);
		_taskTimer.Elapsed += (_, __) => UpdateTaskLabels();
		_taskTimer.AutoReset = true;
		_taskTimer.Enabled = true;
	}

	private void UpdateSalesLabels()
	{
		var count = _rand.Next(1, AllSalesLabels.Count + 1);
		CurrentSalesLabels = AllSalesLabels
			.OrderBy(_ => _rand.Next())
			.Take(count)
			.ToList();

		InvokeAsync(StateHasChanged);
	}

	private void UpdateTaskLabels()
	{
		var count = _rand.Next(1, AllTaskLabels.Count + 1);
		CurrentTaskLabels = AllTaskLabels
			.OrderBy(_ => _rand.Next())
			.Take(count)
			.ToList();

		InvokeAsync(StateHasChanged);
	}

	private void HandleSliceClick(ChartClickEventArgs e)
	{
		Console.WriteLine($"[Home] Slice clicked: {e.SliceLabel}");
		Nav.NavigateTo("/counter");
	}

	private void HandleCenterClick()
	{
		Console.WriteLine("[Home] Center clicked");
		Nav.NavigateTo("/weather");
	}

	public void Dispose()
	{
		_salesTimer?.Dispose();
		_taskTimer?.Dispose();
	}

	// Colours for Sales chart
	private Dictionary<string, string> SalesColors = new()
	{
		["North"] = "#4CAF50",      // green
		["South"] = "#2196F3",      // blue
		["East"] = "#FFC107",       // amber
		["West"] = "#F44336"        // red
	};

	// Colours for Task chart
	private Dictionary<string, string> TaskColors = new()
	{
		["Completed"] = "#4CAF50",
		["In Progress"] = "#FFC107",
		["Blocked"] = "#F44336"
	};
}