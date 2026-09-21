using AssetTracker;

internal class Program

{
    private static void Main(string[] args)
    {
// Application entry point.
        var repository = new AssetDataStore("assets.json");
        var currencyConverter = new CurrencyConverter();
        var tracker = new TheAssetTracker(currencyConverter, repository);

// Load saved assets first. Sample data is only created when no saved data exists.
        tracker.Load();
        if (!tracker.HasAssets)
        {
            SampleData.Seed(tracker);
            tracker.Save();
        }

        var appUI = new ApplicationUI(tracker, currencyConverter);
        appUI.Start();
    }
}