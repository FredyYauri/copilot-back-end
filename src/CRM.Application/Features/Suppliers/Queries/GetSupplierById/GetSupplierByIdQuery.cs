using CRM.Application.Common.Models;
using CRM.Application.DTOs.Suppliers;
using MediatR;

namespace CRM.Application.Features.Suppliers.Queries.GetSupplierById;

public sealed record GetSupplierByIdQuery(Guid Id) : IRequest<Result<SupplierDetailDto>>;
