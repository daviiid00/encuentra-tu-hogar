namespace EncuentraTuHogar.Domain.ValueObjects;

public record Address
{
    public string City { get; init; }
    public string Zone { get; init; }
    public string Street { get; init; }
    public string PostalCode { get; init; }
    
    public Address(string city, string zone, string street, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(city)) 
            throw new ArgumentException("Ciudad requerida");
        if (string.IsNullOrWhiteSpace(zone)) 
            throw new ArgumentException("Zona requerida");
            
        City = city;
        Zone = zone;
        Street = street;
        PostalCode = postalCode;
    }
    
    public bool IsValid() => 
        !string.IsNullOrEmpty(City) && 
        !string.IsNullOrEmpty(Zone) && 
        !string.IsNullOrEmpty(Street);
}
