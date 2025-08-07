using FluentValidation;
using MediatR;
using CorePackages.Infrastructure.Dto;
using CorePackages.Infrastructure.Dto.Exceptions;
using CorePackages.Infrastructure.Interfaces;

namespace CorePackages.Infrastructure.Services
{
    public class MediatrService : IMediatrService
    {
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;

        public MediatrService(IMediator mediator, IServiceProvider serviceProvider)
        {
            _mediator = mediator;
            _serviceProvider = serviceProvider;
        }

        public async Task<ApiResponse<TResponse>> Send<TResponse>(IRequest<TResponse> request)
        {
            var response = new ApiResponse<TResponse>(true, "");
            try
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(request.GetType());
                var validator = _serviceProvider.GetService(validatorType);

                if (validator is not null)
                {
                    var context = new ValidationContext<object>(request);
                    var validationResult = await ((IValidator)validator).ValidateAsync(context);

                    if (!validationResult.IsValid)
                    {
                        var allErrors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                        throw new BusinessException(Errors.Validation.ValidationErrors(allErrors));
                    }
                }

                response.Data = await _mediator.Send(request);
                response.Success = true;
            }
            catch (BusinessException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Code = ex.ErrorCode;
            }

            return response;
        }
    }
}
