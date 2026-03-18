using CRM.Application.Common.Models;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Clients.Commands.DeleteClient;

public sealed class DeleteClientCommandHandler(
    IClientRepository clientRepository,
    ILogger<DeleteClientCommandHandler> logger
) : IRequestHandler<DeleteClientCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (client is null)
        {
            return Result<bool>.Failure("El cliente no fue encontrado.");
        }

        await clientRepository.DeleteAsync(request.Id, cancellationToken);

        logger.LogInformation("Client {ClientId} soft-deleted successfully", request.Id);

        return Result<bool>.Success(true);
    }
}
