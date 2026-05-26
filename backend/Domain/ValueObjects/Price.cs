namespace EncuentraTuHogar.Domain.ValueObjects;

public record Price
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }
    
    public Price(decimal amount, string currency = "COP")
    {
        if (amount <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0");
            
        Amount = amount;
        Currency = currency;
    }
    
    public Price CalculateCommission(decimal percentage) =>
        new Price(Amount * (percentage / 100m), Currency);
    
    public override string ToString() => 
        $"{Currency} ${Amount:N0}";
}
