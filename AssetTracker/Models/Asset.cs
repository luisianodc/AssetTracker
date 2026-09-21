using System.Text.Json.Serialization;


namespace AssetTracker.Models;


// Base class for all company assets.
// The abstract class provides shared data and polymorphic behaviour.
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Computer), "computer")]
[JsonDerivedType(typeof(Smartphone), "smartphone")]
public abstract class Asset
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public Price PurchasePrice { get; set; } = new();
    public Price LocalPrice { get; set; } = new();
    public Office Office { get; set; } = new();

    
    protected Asset()
    {
        Id = Guid.NewGuid();
    }

    
    protected Asset(Price price, DateTime purchaseDate, string brand, string model, Office office)
    {
        Id = Guid.NewGuid();
        PurchasePrice = price;
        PurchaseDate = purchaseDate;
        Brand = brand;
        Model = model;
        Office = office;
        LocalPrice = new Price(price.Amount, price.Currency);
    }

    
    // Returns the asset category used for display and sorting.
    public abstract string AssetType { get; }

    
    // Calculates the current age of the asset.
   // public TimeSpan Age => DateTime.Now - PurchaseDate;
   public TimeSpan Age
   {
       get
       {
           return DateTime.Now - PurchaseDate;
       }
   }

   
    // Calculates the date when the three-year lifespan ends.
    //public DateTime EndOfLifeDate => PurchaseDate.AddYears(3);
    public DateTime EndOfLifeDate
    {
        get
        {
            return PurchaseDate.AddMonths(3);
        }
    }

    
    // Calculates the time remaining until the end-of-life date.
    //public TimeSpan TimeRemaining => EndOfLifeDate - DateTime.Now;
    public TimeSpan TimeRemaining
    {
        get
        {
            return EndOfLifeDate - DateTime.Now;
        }
    }
    

    // Returns the warning status required by the project business rules.
    public EndOfLifeStatus EndOfLifeStatus
    {
        get
        {
            var monthsRemaining = GetApproximateMonthsRemaining(TimeRemaining);

            if (monthsRemaining < 3)
            {
                return EndOfLifeStatus.Red;
            }

            if (monthsRemaining < 6)
            {
                return EndOfLifeStatus.Yellow;
            }

            return EndOfLifeStatus.Normal;
        }
    }

    
    // Converts the remaining lifespan into an approximate month value for the business rule.
    // skriv om denna kod utan att använda =>


    private static double GetApproximateMonthsRemaining(TimeSpan remaining)
    {
        return remaining.TotalDays / 30.4375;
    }

    // Returns a compact description used by the user interface and CSV export.
    public override string ToString()
    {
        return $"{AssetType,-11} {Brand,-12} {Model,-18} " +
               $"{Office.Name,-8} {PurchaseDate:yyyy-MM-dd} {LocalPrice}";
    }
}



// Represents a mobile phone asset.
public sealed class Smartphone : Asset
{
    public Smartphone()
    {
    }

    public Smartphone(Price price, DateTime purchaseDate, string brand, string model, Office office)
        : base(price, purchaseDate, brand, model, office)
    {
    }

    //public override string AssetType => "Smartphone";
    public override string AssetType
    {
        get
        {
            return "Smartphone";
        }
    }
}



// Represents a computer asset.
public sealed class Computer : Asset
{
    public Computer()
    {
    }

    public Computer(Price price, DateTime purchaseDate, string brand, string model, Office office)
        : base(price, purchaseDate, brand, model, office)
    {
    }

    //public override string AssetType => "Computer";
    public override string AssetType
    {
        get
        {
            return "Computer";
        }
    }
}

