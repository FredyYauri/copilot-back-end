using CRM.Application.Common.Models;
using CRM.Application.Features.Clients.Commands.CreateClient;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Clients.Commands.UpdateClient;

public sealed class UpdateClientCommandHandler(
    IClientRepository clientRepository,
    ILogger<UpdateClientCommandHandler> logger
) : IRequestHandler<UpdateClientCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (client is null)
        {
            return Result<bool>.Failure("El cliente no fue encontrado.");
        }

        client.Update(
            request.Nombre,
            request.Ruc,
            request.Dni,
            request.Direccion,
            request.Distrito,
            request.Referencia,
            request.Telefono
        );

        if (!request.IsActive && client.IsActive)
            client.Deactivate();
        else if (request.IsActive && !client.IsActive)
            client.Activate();

        await clientRepository.UpdateAsync(client, cancellationToken);

        if (request.Contacts is not null)
        {
            await clientRepository.DeleteContactsByClientIdAsync(request.Id, cancellationToken);
            foreach (var contactDto in request.Contacts)
            {
                var contact = ClientContact.Create(
                    request.Id,
                    contactDto.Nombre,
                    contactDto.Cargo,
                    contactDto.Telefono,
                    contactDto.Correo,
                    contactDto.Comentarios
                );
                await clientRepository.InsertContactAsync(contact, cancellationToken);
            }
        }

        if (request.CommercialInfo is not null)
        {
            var existingInfo = await clientRepository.GetCommercialInfoByClientIdAsync(request.Id, cancellationToken);
            if (existingInfo is null)
            {
                var info = ClientCommercialInfo.Create(
                    request.Id,
                    request.CommercialInfo.AsesorComercial,
                    request.CommercialInfo.CodigoAsesor,
                    request.CommercialInfo.MedioCaptacion,
                    request.CommercialInfo.CentralRiesgo,
                    request.CommercialInfo.LineaCredito,
                    request.CommercialInfo.Comentarios
                );
                await clientRepository.InsertCommercialInfoAsync(info, cancellationToken);
            }
            else
            {
                existingInfo.Update(
                    request.CommercialInfo.AsesorComercial,
                    request.CommercialInfo.CodigoAsesor,
                    request.CommercialInfo.MedioCaptacion,
                    request.CommercialInfo.CentralRiesgo,
                    request.CommercialInfo.LineaCredito,
                    request.CommercialInfo.Comentarios
                );
                await clientRepository.UpdateCommercialInfoAsync(existingInfo, cancellationToken);
            }
        }

        logger.LogInformation("Client {ClientId} updated successfully", request.Id);

        return Result<bool>.Success(true);
    }
}
