namespace Template.Application.Common.Results
{
    public record PaginatedList<T>(
    List<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
    );
}
