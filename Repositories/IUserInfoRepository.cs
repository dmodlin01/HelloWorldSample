using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IUserInfoRepository
    {
       IEnumerable<Claim> RetrieveUserInfoClaims(string accessToken);
    }
}