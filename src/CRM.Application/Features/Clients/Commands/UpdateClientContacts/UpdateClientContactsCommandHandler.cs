using CRM.Application.Common.Models;
using CRM.Application.Features.Clients.Commands.CreateClient;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Clients.Commands.UpdateClientContacts;

public sealed class UpdateClientContactsCommandHandler(
    IClientRepository clientRepository,
    ILogger<UpdateClientContactsCommandHandler> logger
) : IRequestHandler<UpdateClientContactsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateClientContactsCommand request, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Result<bool>.Failure("El cliente no fue encontrado.");
        }

        await clientRepository.DeleteContactsByClientIdAsync(request.ClientId, cancellationToken);

        foreach (var contactDto in request.Contacts)
        {
            var contact = ClientContact.Create(
                request.ClientId,
                contactDto.Nombre,
                contactDto.Cargo,
                contactDto.Telefono,
                contactDto.Correo,
                contactDto.Comentarios
            );
            await clientRepository.InsertContactAsync(contact, cancellationToken);
        }

        logger.LogInformation("Contacts for client {ClientId} updated successfully", request.ClientId);

        return Result<bool>.Success(true);
    }
}
