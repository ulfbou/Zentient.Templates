namespace Template.Application.Common.Contracts
{
    public interface IPagedRequest
    {
        int PageNumber { get; }
        int PageSize { get; }
    }
}
