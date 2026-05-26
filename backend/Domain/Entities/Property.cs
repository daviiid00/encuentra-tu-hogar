using EncuentraTuHogar.Domain.Common;
using EncuentraTuHogar.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace EncuentraTuHogar.Domain.Entities;

public class SearchFilter
{
    public string City { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public PropertyType? Type { get; set; }
    public TransactionType? Transaction { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? OwnerId { get; set; }
}

public class Property : Entity
{
    public PropertyId Id { get; private set; }
    public Address Address { get; private set; }
    public PropertyType Type { get; private set; }
    public TransactionType Transaction { get; private set; }
    public Price Price { get; private set; }
    public UserId OwnerId { get; private set; }
    public VerificationStatus Status { get; private set; }
    public string Description { get; private set; }
    public List<string> ImageUrls { get; private set; } = new();
    public DateTime CreatedAt { get; private set; }
    public int Views { get; private set; }
    public bool IsLocalPriority { get; private set; }
    
    private Property() { }
    
    public static Result<Property> Create(
        Address address,
        PropertyType type,
        TransactionType transaction,
        Price price,
        UserId ownerId,
        string description)
    {
        return ValidateInputs(address, type, price, ownerId, description)
            .Map(_ => new Property
            {
                Id = PropertyId.New(),
                Address = address,
                Type = type,
                Transaction = transaction,
                Price = price,
                OwnerId = ownerId,
                Status = VerificationStatus.Pending,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                Views = 0,
                IsLocalPriority = false
            });
    }
    
    private static Result<Unit> ValidateInputs(
        Address address,
        PropertyType type,
        Price price,
        UserId ownerId,
        string description)
    {
        return ValidateAddress(address)
            .FlatMap(_ => ValidatePrice(price))
            .FlatMap(_ => ValidateDescription(description))
            .Map(_ => Unit.Value);
    }
    
    private static Result<Unit> ValidateAddress(Address address) =>
        address.IsValid()
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>("Dirección inválida");
    
    private static Result<Unit> ValidatePrice(Price price) =>
        price.Amount > 0
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>("Precio debe ser positivo");
    
    private static Result<Unit> ValidateDescription(string description) =>
        !string.IsNullOrEmpty(description) && description.Length >= 20
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>("Descripción debe tener mínimo 20 caracteres");
    
    public void AddImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            throw new ArgumentException("URL de imagen inválida");
        if (ImageUrls.Count >= 10)
            throw new InvalidOperationException("Máximo 10 imágenes permitidas");
            
        ImageUrls.Add(imageUrl);
    }
    
    public void IncrementViews() => Views++;
    
    public void MarkAsVerified() => Status = VerificationStatus.Verified;
    
    public void Reject(string reason) => Status = VerificationStatus.Rejected;
    
    public Price GetFinalPrice() => Price;
    
    public bool MatchesFilter(SearchFilter filter) =>
        (string.IsNullOrEmpty(filter.City) || filter.City.ToLower() == Address.City?.ToLower()) &&
        (filter.Type == null || filter.Type == Type) &&
        (filter.Transaction == null || filter.Transaction == Transaction) &&
        (filter.MinPrice == null || Price.Amount >= filter.MinPrice) &&
        (filter.MaxPrice == null || Price.Amount <= filter.MaxPrice) &&
        (filter.OwnerId == null || OwnerId.Value.ToString() == filter.OwnerId);
}
