# ✔️ CheckBoxList<TItem> Component
A flexible, data‑driven checklist component for Blazor.
Supports complex objects, automatic selection rules, two‑way binding, and exclusion logic.

---

## ✨ Features

- Works with any data type (`TItem`)
- Automatic text/value extraction
- Two‑way binding for:
  - `SelectedValues`
  - `SelectedTexts`
- Case‑insensitive exclusion list (`ExcludedTexts`)
- Automatically checks all non‑excluded items on first render
- Supports both simple strings and complex objects
- Lightweight markup and easy styling

---

## 📦 Installation

Add the namespace:

    @using BlazorControls.Components.Shared

Include the stylesheet:

    <link href="css/fui-checklist.css" rel="stylesheet" />

---

## 🚀 Basic Usage

### Simple string list

    <CheckBoxList string>
        Data="new[] { "Apple", "Banana", "Cherry" }"
        SelectedValues="@selected"
        SelectedValuesChanged="@((v) => selected = v)" />

### Complex objects

    <CheckBoxList TItem="Product"
                  Data="@Products"
                  TextField="p => p.Name"
                  ValueField="p => p.Id"
                  SelectedValues="@selectedIds"
                  SelectedValuesChanged="@((v) => selectedIds = v)" />

---

## 🧩 Parameters

Parameter | Type | Description
--------- | ---- | -----------
Data | IEnumerable<TItem> | Items to display
TextField | Func<TItem,string>? | Extracts display text
ValueField | Func<TItem,object>? | Extracts value (converted to string)
SelectedValues | List<string> | Currently selected values
SelectedValuesChanged | EventCallback<List<string>> | Two‑way binding callback
SelectedTexts | List<string> | Selected display texts
SelectedTextsChanged | EventCallback<List<string>> | Two‑way binding callback
ExcludedTexts | IEnumerable<string>? | Items that should start unchecked (case‑insensitive)

---

## 🧠 Initialization Logic

On first render:

- All items **not** in `ExcludedTexts` are automatically checked
- All items **in** `ExcludedTexts` are unchecked
- Matching is **case‑insensitive**
- After initialization:
  - `SelectedValuesChanged` fires if values changed
  - `SelectedTextsChanged` fires if texts changed
- `ExcludedTexts` is cleared to avoid re‑initializing

This ensures user interaction is never overwritten on subsequent renders.

---

## 🖱️ Interaction

When a checkbox changes:

- The component updates:
  - `SelectedValues`
  - `SelectedTexts`
- Then fires:
  - `SelectedValuesChanged`
  - `SelectedTextsChanged`

Example handler:

    void OnValuesChanged(List<string> values)
    {
        Console.WriteLine("Selected: " + string.Join(", ", values));
    }

---

## 🎨 Markup Structure

Rendered HTML:

    <div class="fui-checklist">
        <label>
            <input type="checkbox" />
            Item Text
        </label>
        ...
    </div>

---

## 🎨 Styling

Default CSS:

    .fui-checklist {
        display: flex;
        flex-direction: column;
        gap: 6px;
        padding: 8px 4px;
    }

    .fui-checklist-item {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 4px 6px;
        cursor: pointer;
        font-size: 14px;
        border-radius: 4px;
        transition: background-color 0.15s ease;
    }

    .fui-checklist-item:hover {
        background-color: var(--neutral-layer-2);
    }

    .fui-checklist-item input[type="checkbox"] {
        width: 16px;
        height: 16px;
        accent-color: var(--accent-fill-rest);
    }

---

## 🧪 Example: Two‑Way Binding

    <CheckBoxList TItem="string"
                  Data="@Fruits"
                  SelectedValues="@selected"
                  SelectedValuesChanged="@((v) => selected = v)" />

    @code {
        List<string> Fruits = new() { "Apple", "Banana", "Cherry" };
        List<string> selected = new();
    }

---

## 📄 License

MIT — free to use, modify, and integrate.
