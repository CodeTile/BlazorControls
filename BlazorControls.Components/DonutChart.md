# 🍩 DonutChart Component
A reusable, SVG‑based donut / pie chart component for Blazor.
Supports custom colours, legends, tooltips, filtering, and click events.

---

## ✨ Features

- Render as donut or pie
- Fully SVG‑based (no JS dependencies)
- Smooth hover animations
- Optional inner title
- Automatic colour assignment
- Custom colour palettes
- Per‑label colour overrides
- Slice click events
- Donut‑center click events
- Tooltip on hover
- Optional legend
- Label filtering
- Stable fallback colours (HSL)

---

## 📦 Installation

Add the component to your Blazor project:

    @using BlazorControls.Components.Shared

Include the stylesheet:

    <link href="css/donut-chart.css" rel="stylesheet" />

---

## 🚀 Basic Usage

### Donut Chart (default)

    <DonutChart
        Title="Sales Breakdown"
        Data="new() { { "Books", 40 }, { "Games", 25 }, { "Music", 35 } }"
        ShowLegend="true" />

### Pie Chart

    <DonutChart
        IsDonut="false"
        Title="Market Share"
        Data="new() { { "A", 50 }, { "B", 30 }, { "C", 20 } }" />

---

## 🧩 Parameters

Parameter | Type | Description
--------- | ---- | -----------
Title | string? | Optional title displayed above the chart
InnerTitle | string? | Text shown inside donut center (donut mode only)
IsDonut | bool | Donut (true) or pie (false). Default: true
Thickness | int | Donut ring thickness
Data | Dictionary<string,int>? | Required data source
IncludeLabels | IEnumerable<string>? | Optional whitelist of labels
StatusColors | Dictionary<string,string>? | Per‑label colour overrides
DefaultColors | List<string>? | Palette used when no explicit colour exists
ShowLegend | bool | Whether to show the legend
OnSliceClick | EventCallback<string> | Fired when a slice is clicked
OnCenterClick | EventCallback | Fired when donut center is clicked

---

## 🎨 Colour Rules

Colours are resolved in this order:

1. StatusColors[label]
2. DefaultColors[index] (cycled)
3. Generated HSL fallback (stable per label)

Example:

    <DonutChart
        Data="data"
        StatusColors="new() { { "Critical", "#FF0000" } }"
        DefaultColors="new() { "#4CAF50", "#2196F3" }" />

---

## 🧠 Behaviour

### Slice Generation

- Zero or negative values are ignored
- Labels not in IncludeLabels are skipped
- Total value < 1 → no slices
- Sweep angle = (value / total) * 360

### Donut Center

Rendered only when:

    IsDonut == true

Inner radius:

    InnerRadius = max(0, 90 - Thickness)

---

## 🖱️ Events

### Slice Click

    <DonutChart
        Data="data"
        OnSliceClick="HandleSliceClick" />

    @code {
        void HandleSliceClick(string label)
        {
            Console.WriteLine($"Clicked slice: {label}");
        }
    }

### Donut Center Click

    <DonutChart
        IsDonut="true"
        OnCenterClick="HandleCenterClick" />

    @code {
        void HandleCenterClick()
        {
            Console.WriteLine("Center clicked");
        }
    }

---

## 🧰 Tooltip Behaviour

- Appears on slice hover
- Shows label + formatted value
- In donut mode, hovering the center shows:
  - InnerTitle
  - TotalValue

---

## 📊 Legend

Enable via:

    ShowLegend="true"

Sorted alphabetically by label.

---

## 🎨 Styling

All styles are included in donut-chart.css:

- Hover animations
- Tooltip positioning
- Legend layout
- Slice scaling
- Inner title styling

Override any class:

    .donut-slice:hover {
        transform: scale(1.1);
    }

---

## 🧪 Testing

The component is fully testable using bUnit:

    var cut = Render<DonutChart>(p => p
        .Add(x => x.Data, new() { { "A", 10 } })
    );

    Assert.AreEqual(1, cut.Instance.Slices.Count);

---

## 📄 License

MIT — free to use, modify, and integrate.
