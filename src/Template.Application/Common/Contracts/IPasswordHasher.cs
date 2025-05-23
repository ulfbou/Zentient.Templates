namespace Template.Application.Common.Contracts
{
    public interface IPasswordHasher
    {
        Task<string> HashPasswordAsync(string adminPassword);
    }
}
