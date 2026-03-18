using CRM.Application.Common.Models;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Clients.Commands.UpdateClientCommercialInfo;

public sealed class UpdateClientCommercialInfoCommandHandler(
    IClientRepository clientRepository,
    ILogger<UpdateClientCommercialInfoCommandHandler> logger
) : IRequestHandler<UpdateClientCommercialInfoCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateClientCommercialInfoCommand request, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Result<bool>.Failure("El cliente no fue encontrado.");
        }

        var existingInfo = await clientRepository.GetCommercialInfoByClientIdAsync(request.ClientId, cancellationToken);

        if (existingInfo is null)
        {
            var info = ClientCommercialInfo.Create(
                request.ClientId,
                request.AsesorComercial,
                request.CodigoAsesor,
                request.MedioCaptacion,
                request.CentralRiesgo,
                request.LineaCredito,
                request.Comentarios
            );
            await clientRepository.InsertCommercialInfoAsync(info, cancellationToken);
        }
        else
        {
            existingInfo.Update(
                request.AsesorComercial,
                request.CodigoAsesor,
                request.MedioCaptacion,
                request.CentralRiesgo,
                request.LineaCredito,
                request.Comentarios
            );
            await clientRepository.UpdateCommercialInfoAsync(existingInfo, cancellationToken);
        }

        logger.LogInformation("Commercial info for client {ClientId} updated successfully", request.ClientId);

        return Result<bool>.Success(true);
    }
}
