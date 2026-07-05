namespace Squrl.App.UI.Shared;

public class MenuItems
{
    public required string Text { get; init; }
    public required string Url { get; init; }
    public string? Icon { get; init; }

    public List<MenuItems> Children { get; init; } = [];

    public static readonly MenuItems Root = new()
    {
        Text = "Root",
        Url = "",
        Children =
        [
            new()
            {
                Text = "Inventory",
                Url = "/",
                Icon = "bi-house-door-fill",
                Children =
                [
                    new() { Text = "Stocks", Url = "/" },
                    new() { Text = "Adjustments", Url = "/inventory/adjustments" },
                    new() { Text = "Return to Supplier", Url = "/inventory/returns" },
                    new() { Text = "Convert", Url = "/inventory/conversions" }
                ]
            },
            new()
            {
                Text = "Items",
                Url = "/items",
                Children =
                [
                    new() { Text = "Master List", Url = "/items" },
                    new() { Text = "Add Item", Url = "/items/new" },
                    new() { Text = "UOM", Url = "/uoms" }
                ]
            },
            new()
            {
                Text = "Alerts",
                Url = "/alerts"
            },
            new()
            {
                Text = "Suppliers",
                Url = "/suppliers",
                Children =
                [
                    new() { Text = "Master List", Url = "/suppliers" },
                    new() { Text = "Add Supplier", Url = "/suppliers/new" }
                ]
            },
            new()
            {
                Text = "Settings",
                Url = "/settings"
            }
        ]
    };
}