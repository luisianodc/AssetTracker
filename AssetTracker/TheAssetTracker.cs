using AssetTracker.Models;

namespace AssetTracker;



// Main application service that manages assets, sorting, searching and persistence.
public sealed class TheAssetTracker
{
    private readonly CurrencyConverter currencyConverter;
    private readonly AssetDataStore _dataStore;
    private readonly List<Asset> assets = [];

    public TheAssetTracker(CurrencyConverter currencyConverter, AssetDataStore dataStore)
    {
        this.currencyConverter = currencyConverter;
        this._dataStore = dataStore;
    }

    // Provides read-only access to the current asset collection.
    public IReadOnlyList<Asset> Assets => assets;

    // Indicates whether at least one asset is currently stored.
    public bool HasAssets => assets.Count > 0;

    // Adds an asset after checking that its ID is unique.
    public void AddAsset(Asset asset)
    {
        if (assets.Any(existing => existing.Id == asset.Id))
        {
            throw new InvalidOperationException("An asset with that ID already exists.");
        }

        assets.Add(asset);
    }

    // Removes an asset by its unique ID.
    public bool RemoveAsset(Guid id)
    {
        var asset = assets.FirstOrDefault(item => item.Id == id);
        return asset is not null && assets.Remove(asset);
    }

    // Finds assets whose brand or model contains the search text.
    public IReadOnlyList<Asset> Search(string searchText)
    {
        return assets
            .Where(asset => asset.Brand.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            asset.Model.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .OrderBy(asset => asset.Brand)
            .ThenBy(asset => asset.Model)
            .ToList();
    }

    // Sorts assets by asset type and then purchase date as required in Level 2.
    public IReadOnlyList<Asset> SortByTypeAndPurchaseDate()
        => assets.OrderBy(asset => asset.AssetType).ThenBy(asset => asset.PurchaseDate).ToList();

    // Sorts assets by office and then purchase date as required in Level 3.
    public IReadOnlyList<Asset> SortByOfficeAndPurchaseDate()
        => assets.OrderBy(asset => asset.Office.Name).ThenBy(asset => asset.PurchaseDate).ToList();

    // Returns one page of assets for the optional pagination challenge.
    public IReadOnlyList<Asset> GetPage(int pageNumber, int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return [];
        }

        return assets.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    }

    // Calculates a local price from the stored purchase price when possible.
    public void UpdateLocalPrice(Asset asset)
    {
        var amount = currencyConverter.Convert(asset.PurchasePrice.Amount,
                                                asset.PurchasePrice.Currency,
                                                asset.Office.Currency);
        asset.LocalPrice = new Price(amount, asset.Office.Currency);
    }

    // Saves all current assets to the JSON dataStore.
    public void Save() => _dataStore.Save(assets);

    // Loads assets from the JSON dataStore and rejects duplicate IDs.
    public void Load()
    {
        assets.Clear();
        var loadedAssets = _dataStore.Load();
        foreach (var asset in loadedAssets.GroupBy(item => item.Id).Select(group => group.First()))
        {
            assets.Add(asset);
        }
    }

    // Exports all assets as a CSV report.
    public void ExportCsv(string path) => _dataStore.ExportCsv(assets, path);
}
