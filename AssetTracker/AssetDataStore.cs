using System.Globalization;
using System.Text;
using System.Text.Json;
using AssetTracker.Models;

namespace AssetTracker;


// Provides JSON persistence and CSV report export without a database.
public class AssetDataStore
{
    private readonly string filePath;
    private readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public AssetDataStore(string filePath)
    {
        this.filePath = filePath;
    }

    // Saves the complete asset collection to disk.
    public void Save(IEnumerable<Asset> assets)
    {
        var json = JsonSerializer.Serialize(assets, options);
        File.WriteAllText(filePath, json);
    }

    // Loads the asset collection from disk. A missing file returns an empty list.
    public IReadOnlyList<Asset> Load()
    {
        if (!File.Exists(filePath))
        {
            return [];
        }

        try
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Asset>>(json, options) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    // Writes a portable CSV asset report.
    public void ExportCsv(IEnumerable<Asset> assets, string path)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Id,AssetType,Brand,Model,PurchaseDate,PurchasePrice,PurchaseCurrency,LocalPrice,LocalCurrency,Office,EndOfLifeStatus");

        foreach (var asset in assets)
        {
            builder.AppendLine(string.Join(",",
                Escape(asset.Id.ToString()),
                Escape(asset.AssetType),
                Escape(asset.Brand),
                Escape(asset.Model),
                asset.PurchaseDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                asset.PurchasePrice.Amount.ToString(CultureInfo.InvariantCulture),
                asset.PurchasePrice.Currency,
                asset.LocalPrice.Amount.ToString(CultureInfo.InvariantCulture),
                asset.LocalPrice.Currency,
                Escape(asset.Office.Name),
                asset.EndOfLifeStatus));
        }

        File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
    }

    // Escapes CSV values that contain commas, quotes or line breaks.
    private static string Escape(string value)
        => value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
}



// Creates the default sample data supplied with the project brief.
public static class SampleData
{
    // Adds the default assets to the tracker.
    public static void Seed(TheAssetTracker tracker)
    {
        var usa = new Office("USA", Currency.USD);
        var sweden = new Office("Sweden", Currency.SEK);
        var germany = new Office("Germany", Currency.EUR);

        tracker.AddAsset(new Smartphone(new Price(200, Currency.USD), DateTime.Now.AddMonths(-36 + 4), "Motorola", "X3", usa));
        tracker.AddAsset(new Smartphone(new Price(400, Currency.USD), DateTime.Now.AddMonths(-36 + 5), "Motorola", "X3", usa));
        tracker.AddAsset(new Smartphone(new Price(400, Currency.USD), DateTime.Now.AddMonths(-36 + 10), "Motorola", "X2", usa));
        tracker.AddAsset(new Smartphone(new Price(4500, Currency.SEK), DateTime.Now.AddMonths(-36 + 6), "Samsung", "Galaxy 10", sweden));
        tracker.AddAsset(new Smartphone(new Price(4500, Currency.SEK), DateTime.Now.AddMonths(-36 + 7), "Samsung", "Galaxy 10", sweden));
        tracker.AddAsset(new Smartphone(new Price(3000, Currency.SEK), DateTime.Now.AddMonths(-36 + 4), "Sony", "XPeria 7", sweden));
        tracker.AddAsset(new Smartphone(new Price(3000, Currency.SEK), DateTime.Now.AddMonths(-36 + 5), "Sony", "XPeria 7", sweden));
        tracker.AddAsset(new Smartphone(new Price(220, Currency.EUR), DateTime.Now.AddMonths(-36 + 12), "Siemens", "Brick", germany));

        tracker.AddAsset(new Computer(new Price(100, Currency.USD), DateTime.Now.AddMonths(-38), "Dell", "Desktop 900", usa));
        tracker.AddAsset(new Computer(new Price(100, Currency.USD), DateTime.Now.AddMonths(-37), "Dell", "Desktop 900", usa));
        tracker.AddAsset(new Computer(new Price(300, Currency.USD), DateTime.Now.AddMonths(-36 + 1), "Lenovo", "X100", usa));
        tracker.AddAsset(new Computer(new Price(300, Currency.USD), DateTime.Now.AddMonths(-36 + 4), "Lenovo", "X200", usa));
        tracker.AddAsset(new Computer(new Price(500, Currency.USD), DateTime.Now.AddMonths(-36 + 9), "Lenovo", "X300", usa));
        tracker.AddAsset(new Computer(new Price(1500, Currency.SEK), DateTime.Now.AddMonths(-36 + 7), "Dell", "Optiplex 100", sweden));
        tracker.AddAsset(new Computer(new Price(1400, Currency.SEK), DateTime.Now.AddMonths(-36 + 8), "Dell", "Optiplex 200", sweden));
        tracker.AddAsset(new Computer(new Price(1300, Currency.SEK), DateTime.Now.AddMonths(-36 + 9), "Dell", "Optiplex 300", sweden));
        tracker.AddAsset(new Computer(new Price(1600, Currency.EUR), DateTime.Now.AddMonths(-36 + 14), "Asus", "ROG 600", germany));
        tracker.AddAsset(new Computer(new Price(1200, Currency.EUR), DateTime.Now.AddMonths(-36 + 4), "Asus", "ROG 500", germany));
        tracker.AddAsset(new Computer(new Price(1200, Currency.EUR), DateTime.Now.AddMonths(-36 + 3), "Asus", "ROG 500", germany));
        tracker.AddAsset(new Computer(new Price(1300, Currency.EUR), DateTime.Now.AddMonths(-36 + 2), "Asus", "ROG 500", germany));
    }
}
