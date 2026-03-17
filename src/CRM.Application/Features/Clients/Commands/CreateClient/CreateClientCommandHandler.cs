using CRM.Application.Common.Models;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Clients.Commands.CreateClient;

public sealed class CreateClientCommandHandler(
    IClientRepository clientRepository,
    ILogger<CreateClientCommandHandler> logger
) : IRequestHandler<CreateClientCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Ruc))
        {
            var rucExists = await clientRepository.ExistsByRucAsync(request.Ruc, cancellationToken);
            if (rucExists)
            {
                logger.LogWarning("Create client failed: RUC {Ruc} already exists", request.Ruc);
                return Result<Guid>.Failure("Ya existe un cliente con ese RUC.");
            }
        }

        if (!string.IsNullOrEmpty(request.Dni))
        {
            var dniExists = await clientRepository.ExistsByDniAsync(request.Dni, cancellationToken);
            if (dniExists)
            {
                logger.LogWarning("Create client failed: DNI {Dni} already exists", request.Dni);
                return Result<Guid>.Failure("Ya existe un cliente con ese DNI.");
            }
        }

        var client = Client.Create(
            request.Nombre,
            request.Ruc,
            request.Dni,
            request.Direccion,
            request.Distrito,
            request.Referencia,
            request.Telefono
        );

        var clientId = await clientRepository.InsertAsync(client, cancellationToken);

        if (request.Contacts is not null)
        {
            foreach (var contactDto in request.Contacts)
            {
                var contact = ClientContact.Create(
                    clientId,
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
            var info = ClientCommercialInfo.Create(
                clientId,
                request.CommercialInfo.AsesorComercial,
                request.CommercialInfo.CodigoAsesor,
                request.CommercialInfo.MedioCaptacion,
                request.CommercialInfo.CentralRiesgo,
                request.CommercialInfo.LineaCredito,
                request.CommercialInfo.Comentarios
            );
            await clientRepository.InsertCommercialInfoAsync(info, cancellationToken);
        }

        logger.LogInformation("Client {ClientId} created successfully", clientId);

        return Result<Guid>.Success(clientId);
    }
}
