namespace Template.Application.Common.Contracts
{
    public interface IPasswordHasher
    {
        string HashPasswordAsync(string adminPassword);
    }
}