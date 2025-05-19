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

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Template.Application.Features.TenantUsers
{
    public class GetTenantUserQueryHandler : BaseQueryHandler<GetTenantUserQuery, IResult<TenantUserDto>, TenantUser, UserId>, IRequestHandler<GetTenantUserQuery, IResult<TenantUserDto>>
    {
        public GetTenantUserQueryHandler(
            IQueryContext<TenantUser, UserId> context,
            IUserContext userContext,
            ILogger<BaseQueryHandler<GetTenantUserQuery, IResult<TenantUserDto>, TenantUser, UserId>> logger,
            IMapper mapper,
            ActivitySource? activitySource = null)

            : base(context ?? throw new ArgumentNullException(nameof(context)),
                  userContext ?? throw new ArgumentNullException(nameof(userContext)),
                  logger ?? throw new ArgumentNullException(nameof(logger)),
                  mapper ?? throw new ArgumentNullException(nameof(mapper)),
                    activitySource)
        { }

        protected override async Task<IResult<TenantUserDto>> ExecuteQuery(GetTenantUserQuery query, CancellationToken ct)
        {
            // 1. Get User
            var user = await _context.Query()
                .Where(u => u.Id == query.UserId)
                .Include(u => u.Tenant)
                .Select(u => _mapper.Map<TenantUserDto>(u))
                .FirstOrDefaultAsync(ct);

            if (user == null)
            {
                return Result.Failure<TenantUserDto>(null, $"User with ID {query.UserId} not found.");
            }

            return Result.Success<TenantUserDto>(user);
        }
    }

    public class GetTenantUsersQueryHandler : PagedQueryHandler<GetTenantUsersQuery, TenantUserDto, TenantUser, UserId>, IRequestHandler<GetTenantUsersQuery, IResult<PaginatedList<TenantUserDto>>>
    {
        private readonly IMapper _mapper;

        public GetTenantUsersQueryHandler(
            IQueryContext<TenantUser, UserId> context,
            ILogger<GetTenantUsersQueryHandler> logger,
            IMapper mapper)
            : base(context, logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override async Task<IResult<PaginatedList<TenantUserDto>>> FetchEntities(GetTenantUsersQuery query, CancellationToken cancellationToken)
        {
            var usersQuery = _context.Query()
                .Include(u => u.Tenant);

            // Optional: Apply filtering
            if (!string.IsNullOrWhiteSpace(query.Paging.Filter))
            {
                usersQuery = (IIncludableQueryable<TenantUser, ITenant>)usersQuery
                    .Where(u => u.UserName.Contains(query.Paging.Filter) || u.Email.Contains(query.Paging.Filter));
            }

            // Optional: Apply sorting
            IOrderedQueryable<TenantUser> orderedQuery = null!;
            if (!string.IsNullOrWhiteSpace(query.Paging.SortBy))
            {
                if (query.Paging.SortBy.Equals(nameof(TenantUser.UserName), StringComparison.OrdinalIgnoreCase))
                    orderedQuery = query.Paging.IsAscending ? usersQuery.OrderBy(u => u.UserName) : usersQuery.OrderByDescending(u => u.UserName);
                else if (query.Paging.SortBy.Equals(nameof(TenantUser.Email), StringComparison.OrdinalIgnoreCase))
                    orderedQuery = query.Paging.IsAscending ? usersQuery.OrderBy(u => u.Email) : usersQuery.OrderByDescending(u => u.Email);
            }
            else
            {
                orderedQuery = usersQuery.OrderBy(u => u.UserName);
            }

            var totalCount = await orderedQuery.CountAsync(cancellationToken);
            var items = await usersQuery
                .Skip((GetPageNumber(query) - 1) * GetPageSize(query))
                .Take(GetPageSize(query))
                .ToListAsync(cancellationToken);

            var dtos = items.Select(_mapper.Map<TenantUserDto>).ToList();

            var paginatedList = new PaginatedList<TenantUserDto>(dtos, totalCount, GetPageNumber(query), GetPageSize(query));
            return Result.Success(paginatedList);
        }

        protected override int GetPageNumber(GetTenantUsersQuery query) => query.Paging.PageNumber > 0 ? query.Paging.PageNumber : 1;

        protected override int GetPageSize(GetTenantUsersQuery query) => query.Paging.PageSize > 0 ? query.Paging.PageSize : 20;
    }
}

