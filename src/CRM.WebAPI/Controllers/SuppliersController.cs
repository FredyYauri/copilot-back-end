using CRM.Application.Common.Models;
using CRM.Application.DTOs.Suppliers;
using CRM.Application.Features.Suppliers.Commands.CreateSupplier;
using CRM.Application.Features.Suppliers.Commands.UpdateSupplier;
using CRM.Application.Features.Suppliers.Commands.UpdateSupplierContacts;
using CRM.Application.Features.Suppliers.Queries.GetSupplierById;
using CRM.Application.Features.Suppliers.Queries.GetSuppliers;
using CRM.Application.Features.Suppliers.Queries.SearchSuppliers;
using CRM.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.WebAPI.Controllers;

/// <summary>
/// Controlador de gestión de proveedores. Requiere autenticación y permisos específicos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Suppliers")]
[Produces("application/json")]
[Authorize]
public class SuppliersController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Obtiene la lista paginada de proveedores.
    /// </summary>
    /// <param name="page">Número de página (default: 1).</param>
    /// <param name="pageSize">Tamaño de página (default: 10).</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista paginada de proveedores.</returns>
    /// <response code="200">Lista de proveedores obtenida exitosamente.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">No autorizado.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SupplierDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "suppliers", "read" })]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);
        var result = await sender.Send(new GetSuppliersQuery(page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene un proveedor por su identificador.
    /// </summary>
    /// <param name="id">Identificador del proveedor.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Datos completos del proveedor (incluye contactos).</returns>
    /// <response code="200">Proveedor encontrado.</response>
    /// <response code="404">Proveedor no encontrado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SupplierDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "suppliers", "read" })]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetSupplierByIdQuery(id), ct);

        if (!result.IsSuccess)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Supplier not found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Crea un nuevo proveedor en el sistema.
    /// </summary>
    /// <param name="request">Datos del nuevo proveedor.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Identificador del proveedor creado.</returns>
    /// <response code="201">Proveedor creado exitosamente.</response>
    /// <response code="400">Datos inválidos o RUC duplicado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "suppliers", "create" })]
    public async Task<IActionResult> Create([FromBody] CreateSupplierRequestDto request, CancellationToken ct)
    {
        var command = new CreateSupplierCommand(
            request.Nombre,
            request.Ruc,
            request.Telefono,
            request.Direccion,
            request.Distrito,
            request.Ciudad,
            request.Correo,
            request.PaginaWeb,
            request.NumeroCuenta,
            request.Banco,
            request.Productos,
            request.Observaciones,
            request.Contacts?.Select(c => new CreateSupplierContactItem(
                c.Nombre, c.Cargo, c.Telefono, c.Correo))
        );

        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Create supplier failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Actualiza los datos de un proveedor existente.
    /// </summary>
    /// <param name="id">Identificador del proveedor.</param>
    /// <param name="request">Datos actualizados del proveedor.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de actualización.</returns>
    /// <response code="204">Proveedor actualizado exitosamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="404">Proveedor no encontrado.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "suppliers", "update" })]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierRequestDto request, CancellationToken ct)
    {
        var command = new UpdateSupplierCommand(
            id,
            request.Nombre,
            request.Ruc,
            request.Telefono,
            request.Direccion,
            request.Distrito,
            request.Ciudad,
            request.Correo,
            request.PaginaWeb,
            request.NumeroCuenta,
            request.Banco,
            request.Productos,
            request.Observaciones,
            request.IsActive,
            request.Contacts?.Select(c => new CreateSupplierContactItem(
                c.Nombre, c.Cargo, c.Telefono, c.Correo))
        );

        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Update supplier failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Actualiza los contactos de un proveedor existente.
    /// </summary>
    /// <param name="id">Identificador del proveedor.</param>
    /// <param name="request">Lista de contactos actualizada.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de actualización.</returns>
    /// <response code="204">Contactos actualizados exitosamente.</response>
    /// <response code="400">Datos de contactos inválidos.</response>
    /// <response code="404">Proveedor no encontrado.</response>
    [HttpPut("{id:guid}/contacts")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "suppliers", "update" })]
    public async Task<IActionResult> UpdateContacts(Guid id, [FromBody] UpdateSupplierContactsRequestDto request, CancellationToken ct)
    {
        var command = new UpdateSupplierContactsCommand(
            id,
            request.Contacts.Select(c => new CreateSupplierContactItem(
                c.Nombre, c.Cargo, c.Telefono, c.Correo))
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
    /// Busca proveedores por nombre o RUC (autocompletado).
    /// </summary>
    /// <param name="q">Término de búsqueda.</param>
    /// <param name="maxResults">Máximo de resultados (default: 10).</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista de proveedores que coinciden con la búsqueda.</returns>
    /// <response code="200">Resultados de búsqueda.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<SupplierSearchDto>), StatusCodes.Status200OK)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "suppliers", "read" })]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int maxResults = 10, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Ok(Array.Empty<SupplierSearchDto>());
        }

        maxResults = Math.Clamp(maxResults, 1, 50);
        var result = await sender.Send(new SearchSuppliersQuery(q, maxResults), ct);
        return Ok(result);
    }
}
