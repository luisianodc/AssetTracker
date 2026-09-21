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
    public IReadOnlyList<Asset> Assets
    {
        get
        {
            return assets;
        }
    }

    
    // Indicates whether at least one asset is currently stored.
    public bool HasAssets
    {
        get
        {
            return assets.Count > 0;   
        }
    }

    
    // Adds an asset after checking that its ID is unique.
    public void AddAsset(Asset asset)
    {
        foreach (Asset existing in assets)
        {
            if (existing.Id == asset.Id)
            {
                throw new InvalidOperationException("An asset with that ID already exists.");
            }
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
        List<Asset> results = new List<Asset>();

        foreach (Asset asset in assets)
        {
            if (asset.Brand.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                asset.Model.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(asset);
            }
        }

        results.Sort(delegate (Asset first, Asset second)
        {
            int brandResult = string.Compare(
                first.Brand,
                second.Brand,
                StringComparison.OrdinalIgnoreCase);

            if (brandResult != 0)
            {
                return brandResult;
            }

            return string.Compare(
                first.Model,
                second.Model,
                StringComparison.OrdinalIgnoreCase);
        });

        return results;
    }

    
    // Sorts assets by asset type and then purchase date as required in Level 2.
    public IReadOnlyList<Asset> SortByTypeAndPurchaseDate()
    {
        List<Asset> sortedAssets = new List<Asset>();

        foreach (Asset asset in assets)
        {
            sortedAssets.Add(asset);
        }

        sortedAssets.Sort(delegate (Asset first, Asset second)
        {
            int typeResult = string.Compare(
                first.AssetType,
                second.AssetType,
                StringComparison.OrdinalIgnoreCase);

            if (typeResult != 0)
            {
                return typeResult;
            }

            return DateTime.Compare(
                first.PurchaseDate,
                second.PurchaseDate);
        });

        return sortedAssets;
    }
    
    
    // Sorts assets by office and then purchase date as required in Level 3.
    public IReadOnlyList<Asset> SortByOfficeAndPurchaseDate()
    {
        List<Asset> sortedAssets = new List<Asset>();

        foreach (Asset asset in assets)
        {
            sortedAssets.Add(asset);
        }

        sortedAssets.Sort(delegate (Asset first, Asset second)
        {
            int officeResult = string.Compare(
                first.Office.Name,
                second.Office.Name,
                StringComparison.OrdinalIgnoreCase);

            if (officeResult != 0)
            {
                return officeResult;
            }

            return DateTime.Compare(
                first.PurchaseDate,
                second.PurchaseDate);
        });

        return sortedAssets;
    }
    
    
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

        foreach (Asset asset in loadedAssets)
        {
            bool alreadyExists = false;

            foreach (Asset existingAsset in assets)
            {
                if (existingAsset.Id == asset.Id)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
            {
                assets.Add(asset);
            }
        }
    }
    

    // Exports all assets as a CSV report.
    public void ExportCsv(string path) => _dataStore.ExportCsv(assets, path);
}
