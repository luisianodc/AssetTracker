using AssetTracker.Models;

namespace AssetTracker;

// Handles all console input/output so business logic remains separate from the UI.
public sealed class ApplicationUI
{
    private const int PageSize = 5;
    private readonly TheAssetTracker tracker;
    private readonly CurrencyConverter currencyConverter;

    
    public ApplicationUI(TheAssetTracker tracker, CurrencyConverter currencyConverter)
    {
        this.tracker = tracker;
        this.currencyConverter = currencyConverter;
    }

    
    // Starts the interactive menu loop.
    public void Start()
    {
        while (true)
        {
            UI.DisplayHeader();
            UI.DisplayMenu();
            var choice = ReadText(UI.chooseOption);

            try
            {
                switch (choice)
                {
                    case "1": DisplayAssets(tracker.Assets); break;
                    case "2": DisplayAssets(tracker.SortByTypeAndPurchaseDate()); break;
                    case "3": DisplayAssets(tracker.SortByOfficeAndPurchaseDate()); break;
                    case "4": AddAsset(); break;
                    case "5": SearchAssets(); break;
                    case "6": EditAsset(); break;
                    case "7": RemoveAsset(); break;
                    case "8": SplitAssetsIntoPages(); break;
                    case "9": ExportCsv(); break;
                    case "10": UpdateExchangeRates(); break;
                    case "11": tracker.Save(); Console.WriteLine(UI.assetsSaved); Pause(); break;
                    case "0": tracker.Save(); Console.WriteLine(UI.assetsSavedBye); return;
                    default: Console.WriteLine(UI.invalidOption); Pause(); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(UI.operationFailed + ex.Message);
                Pause();
            }
        }
    }
    

    // Displays assets with colour-coded end-of-life warnings.
    private static void DisplayAssets(IEnumerable<Asset> assets)
    {
        UI.DisplayHeader();
        var list = assets.ToList();
        UI.DisplayAssetHeader(list.Count);  
        foreach (var asset in list)
        {
            SetStatusColor(asset.EndOfLifeStatus);
            Console.WriteLine($"{asset.Id} {asset.AssetType,-11} {asset.Brand,-12} {asset.Model,-18} {asset.Office.Name,-8} {asset.PurchaseDate:yyyy-MM-dd} {asset.LocalPrice,12} {asset.EndOfLifeStatus}");
            Console.ResetColor();
        }
        Pause();
    }

    
    // Sets console colors according to the project warning rules.
    private static void SetStatusColor(EndOfLifeStatus status)
    {
        if (!Console.IsOutputRedirected)
        {
            switch (status)
            {
                case EndOfLifeStatus.Red:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case EndOfLifeStatus.Yellow:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
        }
    }

    
    // Adds a new computer or smartphone using validated user input.
    private void AddAsset()
    {
        UI.DisplayHeader();
        Console.WriteLine(UI.addAsset);
        Console.WriteLine(UI.ratesNotAvailable);
        var updateRates = ReadChoice(UI.wishToUpdate, "y", "n");

        var ratesWereUpdated = false;
        if (updateRates.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(UI.updatingRates);
            ratesWereUpdated = currencyConverter.UpdateAsync().GetAwaiter().GetResult();

            if (ratesWereUpdated)
            {
                Console.WriteLine(UI.liveRatesUpdated);
            }
            else
            {
                Console.WriteLine(UI.liveRatesNotUpdated);
            }
        }

        var type = ReadChoice(UI.assetType, "1", "2");
        var brand = ReadRequired("Brand: ");
        var model = ReadRequired("Model: ");
        var purchaseDate = ReadDate(UI.purchaseDate);
        var amount = ReadDecimal(UI.purchasePrice);
        var currency = ReadCurrency(UI.purchaseCurrency);
        var office = ReadOffice();

        var price = new Price(amount, currency);
        Asset asset = type == "1"
            ? new Computer(price, purchaseDate, brand, model, office)
            : new Smartphone(price, purchaseDate, brand, model, office);

        if (ratesWereUpdated && currencyConverter.HasRates)
        {
            tracker.UpdateLocalPrice(asset);
        }

        tracker.AddAsset(asset);
        tracker.Save();

        Console.WriteLine();
        Console.WriteLine($"Asset added. ID: {asset.Id}");
        DisplayAddedAssetProperties(asset, ratesWereUpdated && currencyConverter.HasRates);
        Pause();
    }

    // Displays the properties of the newly added asset in the format requested by the project.
    private static void DisplayAddedAssetProperties(Asset asset, bool ratesWereUpdated)
    {
        Console.WriteLine();
        Console.WriteLine("Property");
        Console.WriteLine($"Asset Type {asset.AssetType}");
        Console.WriteLine($"Brand {asset.Brand}");
        Console.WriteLine($"Model {asset.Model}");
        Console.WriteLine($"Purchase Date {asset.PurchaseDate:yyyy-MM-dd}");
        Console.WriteLine($"Price ({asset.PurchasePrice.Currency}) {asset.PurchasePrice.Amount:N2}");

        if (ratesWereUpdated)
        {
            Console.WriteLine($"Price (Local) {asset.LocalPrice.Amount:N2} {asset.LocalPrice.Currency}");
        }
        else
        {
            Console.WriteLine(UI.priceLocal);
        }
        Console.WriteLine($"Office {asset.Office.Name}");
    }

    
    // Searches for assets by brand or model.
    private void SearchAssets()
    {
        var text = ReadRequired(UI.searchBrand);
        DisplayAssets(tracker.Search(text));
    }

    
    // Edits the main user-editable asset fields.
    private void EditAsset()
    {
        UI.DisplayHeader();
        var asset = FindAssetById();
        if (asset is null) return;

        asset.Brand = ReadWithDefault("Brand", asset.Brand);
        asset.Model = ReadWithDefault("Model", asset.Model);
        asset.PurchaseDate = ReadDateWithDefault("Purchase date", asset.PurchaseDate);
        asset.PurchasePrice.Amount = ReadDecimalWithDefault("Purchase price", asset.PurchasePrice.Amount);
        asset.PurchasePrice.Currency = ReadCurrencyWithDefault("Purchase currency", asset.PurchasePrice.Currency);
        asset.Office = ReadOfficeWithDefault(asset.Office);

        if (currencyConverter.HasRates)
        {
            tracker.UpdateLocalPrice(asset);
        }

        tracker.Save();
        Console.WriteLine("Asset updated.");
        Pause();
    }

    
    // Removes an asset after confirmation.
    private void RemoveAsset()
    {
        UI.DisplayHeader();
        var asset = FindAssetById();
        if (asset is null) return;

        var confirmation = ReadChoice($"Remove {asset.Brand} {asset.Model}? (y/n): ", "y", "n");
        if (confirmation == "y")
        {
            tracker.RemoveAsset(asset.Id);
            tracker.Save();
            Console.WriteLine("Asset removed.");
        }
        else
        {
            Console.WriteLine("Removal cancelled.");
        }
        Pause();
    }

    
    // Shows assets in pages as the optional pagination challenge.
    private void SplitAssetsIntoPages()
    {
        var page = 1;
        var totalPages = Math.Max(1, (int)Math.Ceiling(tracker.Assets.Count / (double)PageSize));

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Page {page} of {totalPages}");
            DisplayAssetsWithoutPause(tracker.GetPage(page, PageSize));
            var command = ReadText(UI.nextPrevious).ToUpperInvariant();

            if (command == "N" && page < totalPages) page++;
            else if (command == "P" && page > 1) page--;
            else if (command == "Q") return;
        }
    }

    
    // Exports a CSV report to the application folder.
    private void ExportCsv()
    {
        const string path = "asset-report.csv";
        tracker.ExportCsv(path);
        Console.WriteLine($"CSV report exported to {Path.GetFullPath(path)}");
        Pause();
    }


    // Gets the latest exchange rates from the ECB, updates the local prices of all assets,
    // and saves the changes. If the exchange-rate update fails, the existing prices are kept.
    // async = "This method may have to wait for something."
    // await = "Wait for that operation to finish before continuing."
    private async void UpdateExchangeRates()
    {
        Console.WriteLine("Updating exchange rates from the European Central Bank...");
        if (await currencyConverter.UpdateAsync())
        {
            foreach (var asset in tracker.Assets)
            {
                tracker.UpdateLocalPrice(asset);
            }
            tracker.Save();
            Console.WriteLine(UI.ratesUpdated);
        }
        else
        {
            Console.WriteLine(UI.notUpdated);
        }
        Pause();
    }

    
    // Finds an asset using its unique ID.
    private Asset? FindAssetById()
    {
        var idText = ReadRequired("Asset ID: ");
        if (!Guid.TryParse(idText, out var id))
        {
            Console.WriteLine("Invalid asset ID.");
            Pause();
            return null;
        }

        var asset = tracker.Assets.FirstOrDefault(item => item.Id == id);
        if (asset is null)
        {
            Console.WriteLine("Asset not found.");
            Pause();
        }
        return asset;
    }

    
    // Reads a supported office and its associated currency.
    private static Office ReadOffice()
    {
        var office = ReadChoice(UI.officeChoices, "1", "2", "3", "4");
        switch (office)
        {
            case "1":
                return new Office("Sweden", Currency.SEK);
            case "2":
                return new Office("USA", Currency.USD);
            case "3":
                return new Office("Turkey", Currency.TRY);
            default:
                return new Office("Germany", Currency.EUR);
        }
    }

    
    // Reads an office while allowing the existing value to be kept.
    private static Office ReadOfficeWithDefault(Office current)
    {
        Console.WriteLine(UI.officeChoicesExt);
        var input = Console.ReadLine();
        switch (input)
        {
            case "1":
                return new Office("Sweden", Currency.SEK);
            case "2":
                return new Office("USA", Currency.USD);
            case "3":
                return new Office("Turkey", Currency.TRY);
            case "4":
                return new Office("Germany", Currency.EUR);
            default:
                return current;
        }
    }

    
    // Reads and validates a date from the console.
    private static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out var date)) return date;
            Console.WriteLine(UI.enterValidDate);
        }
    }

    
    // Reads a decimal monetary value from the console.
    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out var value) && value >= 0) return value;
            Console.WriteLine(UI.noneNegativeNbr);
        }
    }

    
    // Reads one of the supported currencies.
    private static Currency ReadCurrency(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (Enum.TryParse<Currency>(Console.ReadLine(), true, out var currency)) return currency;
            Console.WriteLine(UI.supportedCurrencies);
        }
    }

    
    // Reads a required non-empty string.
    private static string ReadRequired(string prompt)
    {
        while (true)
        {
            var value = ReadText(prompt);
            if (!string.IsNullOrWhiteSpace(value)) return value;
            Console.WriteLine(UI.requiredField);
        }
    }

    
    // Reads a string from the console.
    private static string ReadText(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    
    // Reads a menu choice from the supplied valid choices.
    private static string ReadChoice(string prompt, params string[] choices)
    {
        while (true)
        {
            var value = ReadText(prompt);
            if (choices.Contains(value, StringComparer.OrdinalIgnoreCase)) return value;
            Console.WriteLine("Invalid choice.");
        }
    }

    
    // Reads a string but keeps the existing value when Enter is pressed.
    private static string ReadWithDefault(string label, string current)
    {
        Console.Write($"{label} [{current}]: ");
        var value = Console.ReadLine();
        return string.IsNullOrWhiteSpace(value) ? current : value.Trim();
    }

    
    // Reads a date but keeps the existing value when Enter is pressed.
    private static DateTime ReadDateWithDefault(string label, DateTime current)
    {
        while (true)
        {
            Console.Write($"{label} [{current:yyyy-MM-dd}]: ");
            var value = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(value)) return current;
            if (DateTime.TryParse(value, out var date)) return date;
            Console.WriteLine(UI.enterValidDate);
        }
    }

    
    // Reads a decimal but keeps the existing value when Enter is pressed.
    private static decimal ReadDecimalWithDefault(string label, decimal current)
    {
        while (true)
        {
            Console.Write($"{label} [{current:N2}]: ");
            var value = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(value)) return current;
            if (decimal.TryParse(value, out var number) && number >= 0) return number;
            Console.WriteLine(UI.noneNegativeNbr);
        }
    }

    
    // Reads a currency but keeps the existing value when Enter is pressed.
    private static Currency ReadCurrencyWithDefault(string label, Currency current)
    {
        while (true)
        {
            Console.Write($"{label} [{current}]: ");
            var value = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(value)) return current;
            if (Enum.TryParse<Currency>(value, true, out var currency)) return currency;
            Console.WriteLine(UI.supportedCurrencies);
        }
    }

    
    // Displays assets without changing pagination state.
    private static void DisplayAssetsWithoutPause(IEnumerable<Asset> assets)
    {
        foreach (var asset in assets)
        {
            SetStatusColor(asset.EndOfLifeStatus);
            Console.WriteLine($"{asset.Id} | {asset.AssetType} | {asset.Brand} {asset.Model} | {asset.Office.Name} | {asset.PurchaseDate:yyyy-MM-dd} | {asset.LocalPrice} | {asset.EndOfLifeStatus}");
            Console.ResetColor();
        }
    }

    
    // Waits for any key before clearing the console and returning to the main menu.
    private static void Pause()
    {
        Console.WriteLine(UI.pressToReturn);
        Console.ReadKey(true);
        Console.Clear();
    }
    
}
