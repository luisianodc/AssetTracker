namespace AssetTracker.Models;


// Represents a monetary amount together with its currency.
public sealed class Price
{
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }

    public Price()
    {
    }

    public Price(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    // Creates a readable price for console output.
    public override string ToString() => $"{Amount:N2} {Currency}";
}
