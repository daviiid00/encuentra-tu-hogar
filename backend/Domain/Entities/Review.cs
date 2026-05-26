using EncuentraTuHogar.Domain.Common;
using System;

namespace EncuentraTuHogar.Domain.Entities;

public class Review : Entity
{
    public Guid Id { get; private set; }
    public Guid VisitId { get; private set; }
    public string ReviewerId { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsApproved { get; private set; }
    
    private Review() { }
    
    public static Result<Review> Create(Guid visitId, string reviewerId, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            return Result.Failure<Review>("La calificación debe estar entre 1 y 5");
            
        if (string.IsNullOrWhiteSpace(comment) || comment.Length < 10)
            return Result.Failure<Review>("El comentario debe tener al menos 10 caracteres");
            
        return Result.Success(new Review
        {
            Id = Guid.NewGuid(),
            VisitId = visitId,
            ReviewerId = reviewerId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow,
            IsApproved = true // Automatically approved for now
        });
    }
}
