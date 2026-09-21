using System.Globalization;
using System.Xml.Linq;
using AssetTracker.Models;

namespace AssetTracker;


// Fetches current ECB exchange rates and converts values through EUR.
public sealed class CurrencyConverter
{
    private const string EcbUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
    private readonly HttpClient httpClient = new();
    
    private readonly Dictionary<Currency, decimal> ratesFromEuro = new()
    {
        [Currency.EUR] = 1m
    };

    
    public bool HasRates { get; private set; }


    // Uses HttpClient to download the ECB data asynchronously.
    // Reads XML document and finds exchange-rate elements.
    // Converts currency codes and rates to C# values. Stores the updated rates in ratesFromEuro.
    // Checks at least three exchange rates were successfully loaded. Returns true if the update was successful.
    // Uses CancellationToken so the network operation can be cancelled if necessary.
    public async Task<bool> UpdateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var stream = await httpClient.GetStreamAsync(EcbUrl, cancellationToken);
            var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

            var namespaces = document.Root?.GetDefaultNamespace() ?? XNamespace.None;
            var cubes = document.Descendants(namespaces + "Cube")
                .Where(element => element.Attribute("currency") is not null);

            var updatedRates = new Dictionary<Currency, decimal>
            {
                [Currency.EUR] = 1m
            };

            foreach (var cube in cubes)
            {
                var code = cube.Attribute("currency")?.Value;
                var rateText = cube.Attribute("rate")?.Value;

                if (code is null || rateText is null || !Enum.TryParse<Currency>(code, out var currency))
                {
                    continue;
                }

                if (decimal.TryParse(rateText, NumberStyles.Number, CultureInfo.InvariantCulture, out var rate))
                {
                    updatedRates[currency] = rate;
                }
            }

            ratesFromEuro.Clear();
            foreach (var pair in updatedRates)
            {
                ratesFromEuro[pair.Key] = pair.Value;
            }

            HasRates = ratesFromEuro.Count >= 3;
            return HasRates;
        }
        catch (Exception)
        {
            HasRates = false;
            return false;
        }
    }

    
    // Converts an amount between two currencies using EUR as the base currency.
    public decimal Convert(decimal amount, Currency from, Currency to)
    {
        if (from == to)
        {
            return amount;
        }

        if (!ratesFromEuro.TryGetValue(from, out var fromRate) ||
            !ratesFromEuro.TryGetValue(to, out var toRate))
        {
            throw new InvalidOperationException("Exchange rates are not available for the selected currencies.");
        }

        var euroValue = from == Currency.EUR ? amount : amount / fromRate;
        return to == Currency.EUR ? euroValue : euroValue * toRate;
    }
    
}
