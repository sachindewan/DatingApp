using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public static class SD
    {
        public const string Admin = "Admin";
        public const string Moderator = "Moderator";
        public const string Member = "Member";
        public const string AdminOnly = "RequiredAdminRole";
        public const string PhotoModerate = "ModeratePhoto";
        public const string VipOnly = "VipOnly";
    }
}
