using CRM.Application.Common.Models;
using CRM.Application.DTOs.Clients;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Clients.Queries.GetClientById;

public sealed class GetClientByIdQueryHandler(
    IClientRepository clientRepository
) : IRequestHandler<GetClientByIdQuery, Result<ClientDetailDto>>
{
    public async Task<Result<ClientDetailDto>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (client is null)
        {
            return Result<ClientDetailDto>.Failure("El cliente no fue encontrado.");
        }

        var contacts = await clientRepository.GetContactsByClientIdAsync(request.Id, cancellationToken);
        var commercialInfo = await clientRepository.GetCommercialInfoByClientIdAsync(request.Id, cancellationToken);

        var contactDtos = contacts.Select(c => new ClientContactDto(
            c.Id.ToString(),
            c.Nombre,
            c.Cargo,
            c.Telefono,
            c.Correo,
            c.Comentarios
        ));

        ClientCommercialInfoDto? commercialInfoDto = commercialInfo is not null
            ? new ClientCommercialInfoDto(
                commercialInfo.AsesorComercial,
                commercialInfo.CodigoAsesor,
                commercialInfo.MedioCaptacion,
                commercialInfo.CentralRiesgo,
                commercialInfo.LineaCredito,
                commercialInfo.Comentarios)
            : null;

        var dto = new ClientDetailDto(
            client.Id.ToString(),
            client.Nombre,
            client.Ruc,
            client.Dni,
            client.Direccion,
            client.Distrito,
            client.Referencia,
            client.Telefono,
            client.IsActive,
            client.CreatedAt,
            client.LastModifiedAt,
            contactDtos,
            commercialInfoDto
        );

        return Result<ClientDetailDto>.Success(dto);
    }
}
