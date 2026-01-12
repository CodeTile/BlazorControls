using Microsoft.AspNetCore.Components;

namespace BlazorControls.Components.Pages;

public partial class Home : ComponentBase, IDisposable
{
	[Inject] private NavigationManager Nav { get; set; } = default!;

	// -----------------------------
	// Pie chart data (now a dictionary)
	// -----------------------------
	private Dictionary<string, int> pieData = new()
	{
		["Completed"] = 30,
		["In Progress"] = 25,      // 15 + 10
		["Blocked"] = 5,
		["Not Started"] = 12,
		["On Hold"] = 4,
		["Under Review"] = 17,     // 7 + 10
		["Pending Approval"] = 3,
		["Cancelled"] = 2,
		["Deferred"] = 6
	};

	private List<string> AllTaskLabels => pieData.Keys.Order().ToList();

	private List<string> CurrentTaskLabels
	{
		get { return [.. currentTaskLabels!.Order()]; }
		set => currentTaskLabels = value;
	}

	private List<string> SelectedTaskValues
	{
		get { return [.. currentTaskValues!.Order()]; }
		set => currentTaskValues = value;
	}

	private void OnColorNamesChanged(List<string> names) =>
			CurrentTaskLabels = names;

	private void OnColorNameValuesChanged(List<string> names) =>
			currentTaskValues = names;

	// -----------------------------
	// Timers
	// -----------------------------

	private System.Timers.Timer? _pieTimer;

	private readonly Random _rand = new();

	protected override void OnInitialized()
	{
		CurrentTaskLabels = AllTaskLabels;
		UpdateTaskLabels();
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

	private void HandleSliceClick(string label)
	{
		Console.WriteLine($"[Home] Slice clicked: {label}");
		Nav.NavigateTo("/counter");
	}

	private void HandleCenterClick()
	{
		Console.WriteLine("[Home] Center clicked");
		Nav.NavigateTo("/weather");
	}

	public void Dispose()
	{
		_pieTimer?.Dispose();
	}

	// Colours for Task chart
	private Dictionary<string, string> TaskColors = new()
	{
		["Completed"] = "#4CAF50",        // Green
		["In Progress"] = "#2196F3",      // Blue
		["Blocked"] = "#F44336",          // Red
		["Not Started"] = "#9E9E9E",      // Grey
		["On Hold"] = "#FF9800",          // Orange
		["Under Review"] = "#9C27B0",     // Purple
		["Pending Approval"] = "#FFC107", // Amber
		["Cancelled"] = "#000000",        // Black
		["Deferred"] = "#795548"          // Brown
	};

	private List<string>? currentTaskLabels = [];
	private List<string>? currentTaskValues = [];
}