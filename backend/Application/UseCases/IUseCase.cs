using EncuentraTuHogar.Domain.Common;
using System.Threading.Tasks;

namespace EncuentraTuHogar.Application.UseCases;

public interface IUseCase<in TRequest, TResponse>
{
    Task<Result<TResponse>> ExecuteAsync(TRequest request);
}
