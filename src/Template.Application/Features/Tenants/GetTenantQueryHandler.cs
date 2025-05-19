using MediatR;

using Microsoft.EntityFrameworkCore;

using Template.Application.Common.Contracts;
using Template.Application.Common.Results;
using Template.Application.Features.Tenants.Queries;
using Template.Domain.Entities;
using Template.Domain.ValueObjects;

namespace Template.Application.Features.Tenants
{
    // -------------------------------
    // Tenant Query Handlers
    // -------------------------------

    public class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, Result<TenantDto>>
    {
        private readonly IQueryContext<Tenant, TenantId> _context;

        public GetTenantQueryHandler(IQueryContext<Tenant, TenantId> context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<TenantDto>> Handle(GetTenantQuery query, CancellationToken cancellationToken)
        {
            IQueryable<Tenant> queryable = _context.Query(); // Handles tenant filtering

            var tenant = await queryable
                .Where(t => t.Id == query.TenantId)
                .FirstOrDefaultAsync(cancellationToken);


            // 1. Get Tenant
            if (tenant == null)
            {
                return Result<TenantDto>.Failure("Tenant not found.");
            }

            // 2. Map to DTO
            var dto = new TenantDto(
                tenant.Id,
                tenant.Name,
                tenant.Status, // Assuming you have a Status property in your Tenant entity
                tenant.Metadata,
                tenant.CreatedOn
            );

            // 3. Return
            return Result<TenantDto>.Success(dto);
        }
    }
}
