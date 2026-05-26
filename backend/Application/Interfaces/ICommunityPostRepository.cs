using EncuentraTuHogar.Domain.Entities;

namespace EncuentraTuHogar.Application.Interfaces;

// Spec: community.spec.md
public interface ICommunityPostRepository
{
    Task<IEnumerable<CommunityPost>> GetAllAsync(bool onlyApproved = true);
    Task<CommunityPost?> GetByIdAsync(Guid id);
    Task AddAsync(CommunityPost post);
    Task UpdateAsync(CommunityPost post);
}
