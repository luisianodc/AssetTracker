namespace AssetTracker.Models;


// Represents an office and the currency used by that office.
public sealed class Office
{
    public string Name { get; set; } = string.Empty;
    public Currency Currency { get; set; }

    public Office()
    {
    }

    public Office(string name, Currency currency)
    {
        Name = name;
        Currency = currency;
    }

    public override string ToString() => $"{Name} ({Currency})";
}
