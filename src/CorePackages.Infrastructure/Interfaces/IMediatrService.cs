using MediatR;
using CorePackages.Infrastructure.Dto;

namespace CorePackages.Infrastructure.Interfaces
{
    public interface IMediatrService
    {
        Task<ApiResponse<TResponse>> Send<TResponse>(IRequest<TResponse> request);
    }
}
