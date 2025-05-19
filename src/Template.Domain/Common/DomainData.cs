using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Template.Domain.Common;

namespace Template.Domain.Common
{
    public static partial class DomainData
    {
        public static partial class Roles
        {
            public const string SuperAdmin = nameof(TenantRoles.SuperAdmin);
            public const string Admin = nameof(TenantRoles.Admin);
            public const string User = nameof(TenantRoles.User);
            public const string Guest = nameof(TenantRoles.Guest);
        }
    }
}
