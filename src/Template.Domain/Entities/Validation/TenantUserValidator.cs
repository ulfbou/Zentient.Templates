using System.Text.RegularExpressions;

using Template.Domain.Common.Exceptions;
using Template.Domain.Contracts.Validation;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities.Validation
{
    public class TenantUserValidator : IValidator<TenantUser, UserId>
    {
        public void Validate(TenantUser entity)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(entity.UserName))
            {
                errors.Add("Username cannot be null or empty.");
            }

            if (!Regex.IsMatch(entity.UserName, @"^[a-zA-Z0-9._-]{3,}$"))
            {
                errors.Add("Invalid username format. Must be at least 3 characters long and can contain letters, numbers, dots, underscores, and hyphens.");
            }

            if (string.IsNullOrWhiteSpace(entity.Email))
            {
                errors.Add("Email cannot be null or empty.");
            }

            if (!Regex.IsMatch(entity.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errors.Add("Invalid email format.");
            }

            if (errors.Any())
            {
                throw new AggregateDomainValidationException("TenantUser validation failed.", errors);
            }
        }
    }
}
