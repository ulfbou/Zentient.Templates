using Template.Application.Common.Contracts;

namespace Template.Application.Common
{
    public record PagedQuery(
        string? Filter,
        string? SortBy,
        bool IsAscending,
        int PageNumber = 1,
        int PageSize = 20
    ) : IPagedRequest
    {
        public int Skip => (PageNumber - 1) * PageSize;
        public int Take => PageSize;
    }
}
