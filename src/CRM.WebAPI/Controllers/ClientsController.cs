using CRM.Application.Common.Models;
using CRM.Application.DTOs.Clients;
using CRM.Application.Features.Clients.Commands.CreateClient;
using CRM.Application.Features.Clients.Commands.DeleteClient;
using CRM.Application.Features.Clients.Commands.UpdateClient;
using CRM.Application.Features.Clients.Commands.UpdateClientContacts;
using CRM.Application.Features.Clients.Commands.UpdateClientCommercialInfo;
using CRM.Application.Features.Clients.Queries.GetClientById;
using CRM.Application.Features.Clients.Queries.GetClients;
using CRM.Application.Features.Clients.Queries.SearchClients;
using CRM.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.WebAPI.Controllers;

/// <summary>
/// Controlador de gestión de clientes. Requiere autenticación y permisos específicos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Clients")]
[Produces("application/json")]
[Authorize]
public class ClientsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Obtiene la lista paginada de clientes.
    /// </summary>
    /// <param name="page">Número de página (default: 1).</param>
    /// <param name="pageSize">Tamaño de página (default: 10).</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista paginada de clientes.</returns>
    /// <response code="200">Lista de clientes obtenida exitosamente.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">No autorizado.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ClientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "clients", "read" })]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);
        var result = await sender.Send(new GetClientsQuery(page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene un cliente por su identificador.
    /// </summary>
    /// <param name="id">Identificador del cliente.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Datos completos del cliente (incluye contactos e información comercial).</returns>
    /// <response code="200">Cliente encontrado.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClientDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "clients", "read" })]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetClientByIdQuery(id), ct);

        if (!result.IsSuccess)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Client not found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Crea un nuevo cliente en el sistema.
    /// </summary>
    /// <param name="request">Datos del nuevo cliente.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Identificador del cliente creado.</returns>
    /// <response code="201">Cliente creado exitosamente.</response>
    /// <response code="400">Datos inválidos o RUC/DNI duplicado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "clients", "create" })]
    public async Task<IActionResult> Create([FromBody] CreateClientRequestDto request, CancellationToken ct)
    {
        var command = new CreateClientCommand(
            request.Nombre,
            request.Ruc,
            request.Dni,
            request.Direccion,
            request.Distrito,
            request.Referencia,
            request.Telefono,
            request.Contacts?.Select(c => new CreateClientContactItem(
                c.Nombre, c.Cargo, c.Telefono, c.Correo, c.Comentarios)),
            request.CommercialInfo is not null
                ? new CreateClientCommercialInfoItem(
                    request.CommercialInfo.AsesorComercial,
                    request.CommercialInfo.CodigoAsesor,
                    request.CommercialInfo.MedioCaptacion,
                    request.CommercialInfo.CentralRiesgo,
                    request.CommercialInfo.LineaCredito,
                    request.CommercialInfo.Comentarios)
                : null
        );

        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Create client failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Actualiza los datos de un cliente existente.
    /// </summary>
    /// <param name="id">Identificador del cliente.</param>
    /// <param name="request">Datos actualizados del cliente.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de actualización.</returns>
    /// <response code="204">Cliente actualizado exitosamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "clients", "update" })]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientRequestDto request, CancellationToken ct)
    {
        var command = new UpdateClientCommand(
            id,
            request.Nombre,
            request.Ruc,
            request.Dni,
            request.Direccion,
            request.Distrito,
            request.Referencia,
            request.Telefono,
            request.IsActive,
            request.Contacts?.Select(c => new CreateClientContactItem(
                c.Nombre, c.Cargo, c.Telefono, c.Correo, c.Comentarios)),
            request.CommercialInfo is not null
                ? new CreateClientCommercialInfoItem(
                    request.CommercialInfo.AsesorComercial,
                    request.CommercialInfo.CodigoAsesor,
                    request.CommercialInfo.MedioCaptacion,
                    request.CommercialInfo.CentralRiesgo,
                    request.CommercialInfo.LineaCredito,
                    request.CommercialInfo.Comentarios)
                : null
        );

        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Update client failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Actualiza los contactos de un cliente existente.
    /// </summary>
    /// <param name="id">Identificador del cliente.</param>
    /// <param name="request">Lista de contactos actualizada.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de actualización.</returns>
    /// <response code="204">Contactos actualizados exitosamente.</response>
    /// <response code="400">Datos de contactos inválidos.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpPut("{id:guid}/contacts")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "clients", "update" })]
    public async Task<IActionResult> UpdateContacts(Guid id, [FromBody] UpdateClientContactsRequestDto request, CancellationToken ct)
    {
        var command = new UpdateClientContactsCommand(
            id,
            request.Contacts.Select(c => new CreateClientContactItem(
                c.Nombre, c.Cargo, c.Telefono, c.Correo, c.Comentarios))
        );

        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Update contacts failed",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Actualiza la información comercial de un cliente existente.
    /// </summary>
    /// <param name="id">Identificador del cliente.</param>
    /// <param name="request">Datos de información comercial actualizados.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de actualización.</returns>
    /// <response code="204">Información comercial actualizada exitosamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpPut("{id:guid}/commercial-info")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "clients", "update" })]
    public async Task<IActionResult> UpdateCommercialInfo(Guid id, [FromBody] UpdateClientCommercialInfoRequestDto request, CancellationToken ct)
    {
        var command = new UpdateClientCommercialInfoCommand(
            id,
            request.AsesorComercial,
            request.CodigoAsesor,
            request.MedioCaptacion,
            request.CentralRiesgo,
            request.LineaCredito,
            request.Comentarios
        );

        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Update commercial info failed",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Busca clientes por nombre, RUC o DNI (autocompletado).
    /// </summary>
    /// <param name="q">Término de búsqueda.</param>
    /// <param name="maxResults">Máximo de resultados (default: 10).</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista de clientes que coinciden con la búsqueda.</returns>
    /// <response code="200">Resultados de búsqueda.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ClientSearchDto>), StatusCodes.Status200OK)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "clients", "read" })]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int maxResults = 10, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Ok(Array.Empty<ClientSearchDto>());
        }

        maxResults = Math.Clamp(maxResults, 1, 50);
        var result = await sender.Send(new SearchClientsQuery(q, maxResults), ct);
        return Ok(result);
    }

    /// <summary>
    /// Elimina un cliente (soft delete).
    /// </summary>
    /// <param name="id">Identificador del cliente.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de eliminación.</returns>
    /// <response code="204">Cliente eliminado exitosamente.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "clients", "delete" })]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteClientCommand(id), ct);

        if (!result.IsSuccess)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Delete client failed",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }
}
