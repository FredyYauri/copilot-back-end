using CRM.Application.Common.Models;
using CRM.Application.Features.Clients.Commands.CreateClient;
using MediatR;

namespace CRM.Application.Features.Clients.Commands.UpdateClientContacts;

public sealed record UpdateClientContactsCommand(
    Guid ClientId,
    IEnumerable<CreateClientContactItem> Contacts
) : IRequest<Result<bool>>;
