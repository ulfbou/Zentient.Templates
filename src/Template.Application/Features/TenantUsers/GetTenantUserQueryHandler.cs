using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

using Template.Application.Common;
using Template.Application.Common.Contracts;
using Template.Application.Common.Handlers;
using Template.Application.Common.Results;
using Template.Application.Features.Tenants.Commands;
using Template.Application.Features.Tenants.Queries;
using Template.Application.Features.TenantUsers.Commands;
using Template.Application.Features.TenantUsers.Queries;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.Entities;
using Template.Domain.Entities.Validation;
using Template.Domain.ValueObjects;

using Zentient.Results;

namespace Template.Application.Features.TenantUsers
{
    public class GetTenantUserQueryHandler : BaseQueryHandler<GetTenantUserQuery, IResult<TenantUserDto>, TenantUser, UserId>
    {
        public GetTenantUserQueryHandler(
            IQueryContext<TenantUser, UserId> queryContext,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource,
            IMapper mapper)
            : base(queryContext, userContext, requestContext, activitySource, mapper)
        { }

        protected override async Task<IResult<TenantUserDto>> ExecuteQueryAsync(GetTenantUserQuery query, CancellationToken ct)
        {
            var userQuery = _queryContext.Query()
                                          .Where(u => u.Id == query.UserId);

            if (query.IncludeTenant)
            {
                userQuery = userQuery.Include(u => u.Tenant);
            }

            var user = await userQuery.FirstOrDefaultAsync(ct);

            if (user is null)
            {
                ErrorInfo notFoundError = AppData.Entities.NotFound(query.UserId);
                return Result.Failure<TenantUserDto>(notFoundError);
            }

            var dto = _mapper.Map<TenantUserDto>(user);

            using var activity = _activitySource.StartActivity($"{nameof(GetTenantUserQueryHandler)}.{nameof(ExecuteQueryAsync)}", ActivityKind.Internal);
            activity?.AddEvent(new ActivityEvent(AppData.TenantUsers.EventMappingToDto));

            return Result.Success(dto);
        }

        protected override Task<IResult<TenantUserDto>> ExecuteQuery(GetTenantUserQuery query, CancellationToken ct)
        {
            // Delegate to ExecuteQueryAsync for compatibility
            return ExecuteQueryAsync(query, ct);
        }
    }

    public class GetTenantUsersQueryHandler : PagedQueryHandler<GetTenantUsersQuery, TenantUserDto, TenantUser, UserId>
    {
        public GetTenantUsersQueryHandler(
            IQueryContext<TenantUser, UserId> queryContext,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource,
            IMapper mapper)
            : base(queryContext, userContext, requestContext, activitySource, mapper)
        { }

        protected override Task<IResult<PaginatedList<TenantUserDto>>> ExecuteQuery(GetTenantUsersQuery query, CancellationToken ct)
        {
            // Delegate to ExecuteQueryAsync for compatibility with the base class signature
            return ExecuteQueryAsync(query, ct);
        }

        protected override async Task<IResult<PaginatedList<TenantUserDto>>> FetchEntities(GetTenantUsersQuery query, CancellationToken ct)
        {
            var usersQuery = _queryContext.Query().AsNoTracking();

            // Apply filtering
            if (!string.IsNullOrWhiteSpace(query.Paging.Filter))
            {
                usersQuery = usersQuery
                    .Where(u => u.UserName.Contains(query.Paging.Filter) || u.Email.Contains(query.Paging.Filter));
            }

            // Apply sorting
            IOrderedQueryable<TenantUser> orderedQuery;

            if (!string.IsNullOrWhiteSpace(query.Paging.SortBy))
            {
                orderedQuery = query.Paging.SortBy.Equals(nameof(TenantUser.UserName), StringComparison.OrdinalIgnoreCase)
                    ? (query.Paging.IsAscending ? usersQuery.OrderBy(u => u.UserName) : usersQuery.OrderByDescending(u => u.UserName))
                    : (query.Paging.SortBy.Equals(nameof(TenantUser.Email), StringComparison.OrdinalIgnoreCase)
                        ? (query.Paging.IsAscending ? usersQuery.OrderBy(u => u.Email) : usersQuery.OrderByDescending(u => u.Email))
                        : usersQuery.OrderBy(u => u.UserName));
            }
            else
            {
                orderedQuery = usersQuery.OrderBy(u => u.UserName);
            }

            var totalCount = await orderedQuery.CountAsync(ct);

            var items = await orderedQuery
                .Skip(query.Paging.Skip)
                .Take(query.Paging.Take)
                .ToListAsync(ct);

            var dtos = items.Select(_mapper.Map<TenantUserDto>).ToList();
            using var activity = _activitySource.StartActivity($"{nameof(GetTenantUsersQueryHandler)}.{nameof(FetchEntities)}", ActivityKind.Internal);
            activity?.AddEvent(new ActivityEvent(AppData.TenantUsers.EventMappingToDto));

            var paginatedList = new PaginatedList<TenantUserDto>(
                dtos,
                totalCount,
                query.Paging.PageNumber,
                query.Paging.PageSize
            );

            return Result<PaginatedList<TenantUserDto>>.Success(paginatedList);
        }

        protected override int GetPageNumber(GetTenantUsersQuery query) =>
            query.Paging.PageNumber > 0 ? query.Paging.PageNumber : Default.PageNumber;
        protected override int GetPageSize(GetTenantUsersQuery query) =>
            query.Paging.PageSize > 0 ? query.Paging.PageSize : Default.PageSize;
    }
}
