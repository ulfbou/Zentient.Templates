using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

using Template.Application.Common;
using Template.Application.Common.Contracts;
using Template.Application.Common.Handlers;
using Template.Application.Common.Results;
using Template.Application.Features.Tenants.Queries;
using Template.Domain.Common.Result;
using Template.Domain.Entities;
using Template.Domain.ValueObjects;

using Zentient.Results;

namespace Template.Application.Features.Tenants
{
    public class GetTenantQueryHandler : BaseQueryHandler<GetTenantQuery, IResult<TenantDto>, Tenant, TenantId>
    {
        public GetTenantQueryHandler(
            IQueryContext<Tenant, TenantId> queryContext,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource,
            IMapper mapper)
            : base(queryContext, userContext, requestContext, activitySource, mapper)
        { }

        protected override Task<IResult<TenantDto>> ExecuteQuery(GetTenantQuery query, CancellationToken ct) => ExecuteQueryAsync(query, ct);

        protected override async Task<IResult<TenantDto>> ExecuteQueryAsync(GetTenantQuery query, CancellationToken ct)
        {
            var tenantEntity = await _queryContext.Query()
                                                  .Where(t => t.Id == query.TenantId)
                                                  .FirstOrDefaultAsync(ct);

            if (tenantEntity == null)
            {
                return Result.Failure<TenantDto>(
                    AppData.Tenants.TenantNotFoundErrorInfo(),
                    Zentient.Results.ResultStatuses.NotFound
                );
            }

            var dto = _mapper.Map<TenantDto>(tenantEntity);
            return Result.Success(dto);
        }
    }
}
