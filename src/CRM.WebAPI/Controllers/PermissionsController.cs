using CRM.Application.DTOs.Roles;
using CRM.Application.Features.Roles.Queries.GetPermissions;
using CRM.Application.Features.Roles.Queries.GetUserPermissions;
using CRM.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.WebAPI.Controllers;

/// <summary>
/// Controlador de permisos. Permite consultar permisos del sistema y de usuarios.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Permissions")]
[Produces("application/json")]
[Authorize]
public class PermissionsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Obtiene todos los permisos del sistema agrupados por recurso.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Permisos agrupados por recurso.</returns>
    /// <response code="200">Lista de permisos obtenida exitosamente.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PermissionGroupDto>), StatusCodes.Status200OK)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "roles", "read" })]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await sender.Send(new GetPermissionsQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene los permisos de un usuario basado en su rol.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Permisos del usuario.</returns>
    /// <response code="200">Permisos del usuario obtenidos exitosamente.</response>
    /// <response code="404">Usuario no encontrado.</response>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(UserPermissionsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "users", "read" })]
    public async Task<IActionResult> GetUserPermissions(Guid userId, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserPermissionsQuery(userId), ct);

        if (!result.IsSuccess)
        {
            return NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(result.Value);
    }
}
