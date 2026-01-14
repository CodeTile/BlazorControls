 # ✔️ CheckBoxList<TItem> Component
 A flexible, data‑driven checklist component for Blazor.
 Supports complex objects, dictionaries, automatic selection rules,
 and full two‑way binding via @bind.

 -----------------------------------------------------------------------------

 ## ✨ Features

 - Works with any data type (`TItem`)
 - Automatic text/value extraction
 - Three two‑way bindings:
   - `@bind-SelectedValues`
   - `@bind-SelectedTexts`
   - `@bind-SelectedMap` (recommended)
 - `UncheckedInitially` to control default checked state
 - Auto‑selects all items except those listed in `UncheckedInitially`
 - Supports:
   - String lists
   - Object lists
   - Dictionary<string,int>
 - Emits initial state on first render
 - Lightweight markup and easy to style

 -----------------------------------------------------------------------------

 ## 📦 Installation

 Add the namespace:
 @using BlazorControls.Components

 Include your stylesheet (optional):
 <link href="css/fui-checklist.css" rel="stylesheet" />

 -----------------------------------------------------------------------------

 ## 🚀 Basic Usage

 ### ✔️ Simple string list

 <CheckBoxList TItem="string"
               Data="@Fruits"
               @bind-SelectedMap="FruitMap" />

 @code {
     List<string> Fruits = new() { "Apple", "Banana", "Cherry" };
     Dictionary<string,int> FruitMap = new();
 }

 Output example:
 Apple:0, Banana:0, Cherry:0
 (String lists always return key‑only maps.)

 -----------------------------------------------------------------------------

 ### ✔️ Complex objects

 <CheckBoxList TItem="Product"
               Data="@Products"
               TextField="p => p.Name"
               ValueField="p => p.Id.ToString()"
               @bind-SelectedMap="SelectedProductMap" />

 @code {
     List<Product> Products = GetProducts();
     Dictionary<string,int> SelectedProductMap = new();
 }

 Output example:
 101:0, 203:1
 (Object lists return key:index based on selection order.)

 -----------------------------------------------------------------------------

 ### ✔️ Dictionary support (key → value)

 <CheckBoxList TItem="KeyValuePair<string,int>"
               Data="@StatusList"
               TextField="s => s.Key"
               ValueField="s => s.Key"
               @bind-SelectedMap="SelectedStatusMap" />

 @code {
     Dictionary<string,int> StatusList = new()
     {
         { "Open", 12 },
         { "Closed", 30 }
     };

     Dictionary<string,int> SelectedStatusMap = new();
 }

 Output example:
 Open:12, Closed:30
 (Dictionaries return key:dictionaryValue.)

 -----------------------------------------------------------------------------

 ## 🧩 Parameters

 | Parameter            | Type                        | Description                              |
 |----------------------|-----------------------------|------------------------------------------|
 | Data                 | IEnumerable<TItem>          | Items to display                         |
 | TextField            | Func<TItem,string>?         | Extracts display text                    |
 | ValueField           | Func<TItem,string>?         | Extracts value key                       |
 | @bind-SelectedValues | List<string>                | Selected value keys                      |
 | @bind-SelectedTexts  | List<string>                | Selected display texts                   |
 | @bind-SelectedMap    | Dictionary<string,int>      | Selected map (recommended)               |
 | UncheckedInitially   | IEnumerable<string>?        | Items that should start unchecked        |

 -----------------------------------------------------------------------------

 ## 🧠 Initialization Logic

 On first render:

 - All items **not** in `UncheckedInitially` start checked
 - Items in `UncheckedInitially` start unchecked
 - The component emits:
   - `SelectedValues`
   - `SelectedTexts`
   - `SelectedMap`
 - No re‑initialization occurs on subsequent renders

 This ensures the parent receives the correct initial state immediately.

 -----------------------------------------------------------------------------

 ## 🖱️ Interaction

 When a checkbox is toggled:

 - `SelectedValues` updates
 - `SelectedTexts` updates
 - `SelectedMap` updates
 - All three bindings emit automatically

 Example:

 <CheckBoxList Data="@Fruits"
               @bind-SelectedMap="FruitMap" />

 -----------------------------------------------------------------------------

 ## 🎨 Markup Structure

 Rendered HTML:

 <div class="checkbox-list">
     <div class="checkbox-row">
         <input type="checkbox" />
         <label>Item Text</label>
     </div>
 </div>

 -----------------------------------------------------------------------------

 ## 🎨 Styling

 .checkbox-list {
     display: flex;
     flex-direction: column;
     gap: 6px;
 }

 .checkbox-row {
     display: flex;
     align-items: center;
     gap: 8px;
 }

 -----------------------------------------------------------------------------

 ## 🧪 Example: All Three Bindings

 <CheckBoxList TItem="string"
               Data="@Fruits"
               @bind-SelectedValues="SelectedValues"
               @bind-SelectedTexts="SelectedTexts"
               @bind-SelectedMap="SelectedMap" />

 @code {
     List<string> Fruits = new() { "Apple", "Banana", "Cherry" };

     List<string> SelectedValues = new();
     List<string> SelectedTexts = new();
     Dictionary<string,int> SelectedMap = new();
 }

 -----------------------------------------------------------------------------

 ## 📄 License

 MIT — free to use, modify, and integrate.

 -----------------------------------------------------------------------------
