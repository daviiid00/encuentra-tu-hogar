namespace EncuentraTuHogar.Domain.ValueObjects;

public record PropertyId
{
    public Guid Value { get; }
    
    private PropertyId(Guid value) => Value = value;
    
    public static PropertyId New() => new(Guid.NewGuid());
    
    public static PropertyId From(Guid id) => new(id);
    
    public override string ToString() => Value.ToString();
}
