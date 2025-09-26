using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        Guid? GetUserIdFromToken(string token);
        string? GetUserRoleFromToken(string token);
    }
}
