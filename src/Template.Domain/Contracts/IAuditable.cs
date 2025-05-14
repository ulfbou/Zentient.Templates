namespace Template.Domain.Contracts
{
    /// <summary>
    /// Base interface for all entities.
    /// </summary>
    public interface IAuditable
    {
        DateTime CreatedOn { get; }
        string CreatedBy { get; }
        DateTime? ModifiedOn { get; }
        string? ModifiedBy { get; }
    }
}
