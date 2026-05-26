namespace EncuentraTuHogar.Domain.ValueObjects;

public record UserId
{
    public Guid Value { get; }
    
    private UserId(Guid value) => Value = value;
    
    public static UserId New() => new(Guid.NewGuid());
    
    public static UserId From(Guid id) => new(id);
    
    public override string ToString() => Value.ToString();
}
