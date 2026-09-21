namespace AssetTracker;


// The text used in the menu and in different menu elements
public static class UI
{
    public static String chooseOption = "Choose an option: ";
    public static String invalidOption = "Invalid option. Please choose a number from the menu.";
    public static String assetsSavedBye = "Assets saved. Goodbye!";
    public static String assetsSaved = "Assets saved.";
    public static String ratesUpdated = "Currency rates and local prices updated.";
    public static String notUpdated = "Could not retrieve live exchange rates. Existing prices were kept.";

    public static String officeChoices = "Office (1=Sweden, 2=USA, 3=Turkey, 4=Germany): ";
    public static String officeChoicesExt = "Office: 1=Sweden, 2=USA, 3=Turkey, 4=Germany, Enter=keep current";
    public static String enterValidDate = "Please enter a valid date.";
    public static String noneNegativeNbr= "Please enter a non-negative number.";
    public static String supportedCurrencies = "Supported currencies: EUR, USD, SEK, TRY.";
    public static String requiredField = "This field is required.";

    public static String addAsset = "\nAdd asset";
    public static String ratesNotAvailable = "Live exchange rates are not available. Local price can not be converted.";
    public static String wishToUpdate = "Do you wish to update live exchange rates? (y/n): ";
    public static String updatingRates = "Updating live exchange rates...";
    public static String liveRatesUpdated = "Live exchange rates updated successfully.";

    public static String liveRatesNotUpdated =
        "Live exchange rates could not be updated. The asset will be added without a converted local price.";

    public static String assetType = "Asset type (1=Computer, 2=Smartphone): ";
    public static String purchaseDate = "Purchase date (yyyy-MM-dd): ";

    public static String pressToReturn = "\nPress any key to return to the main menu...";
    public static String operationFailed = "Operation failed: ";
    public static String purchasePrice = "Purchase price: ";
    public static String purchaseCurrency = "Purchase currency (EUR/USD/SEK/TRY): ";
    public static String priceLocal = "Price (Local) Rates are not updated into the system";
    public static String searchBrand = "Search brand/model: ";
    public static String nextPrevious = "N=next, P=previous, Q=quit: ";
    
    
    // Displays the application title.
    public static void DisplayHeader()
    {
        Console.Clear();
        Console.WriteLine("***************************************************");
        Console.WriteLine("***************    ASSETTRACKER    ****************");
        Console.WriteLine("***************************************************");
    }
    
    
    // Displays all available application actions.
    public static void DisplayMenu()
    {
        Console.WriteLine("1. Display all assets");
        Console.WriteLine("2. Sort by asset type + purchase date");
        Console.WriteLine("3. Sort by office + purchase date");
        Console.WriteLine("4. Add asset");
        Console.WriteLine("5. Search by brand/model");
        Console.WriteLine("6. Edit asset");
        Console.WriteLine("7. Remove asset");
        Console.WriteLine("8. Paginate assets");
        Console.WriteLine("9. Export CSV report");
        Console.WriteLine("10. Update live currency rates");
        Console.WriteLine("11. Save now");
        Console.WriteLine("0. Save and exit");
        Console.WriteLine();
    }
    
    
    // Displays a header before showing a list of assets
    public static void DisplayAssetHeader(int count)
    {
        Console.WriteLine();
        Console.WriteLine($"Assets: {count}");
        Console.WriteLine("ID                                   Type        Brand        Model              Office   Purchase    Local price   Status");
        Console.WriteLine(new string('-', 125));
    }
}