namespace AssetTracker.Models;

// Supported currencies used by company offices.
public enum Currency
{
    EUR,
    USD,
    SEK,
    TRY
}



// Represents the end-of-life warning level displayed to the user.
public enum EndOfLifeStatus
{
    Normal,
    Yellow,
    Red
}