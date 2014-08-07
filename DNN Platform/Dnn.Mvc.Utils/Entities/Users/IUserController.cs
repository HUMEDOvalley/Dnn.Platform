using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;

namespace Dnn.Mvc.Utils.Entities.Users
{
    public interface IUserController
    {
        UserInfo GetCachedUser(int portalId, string userName);

        UserLoginStatus UserLogin(PortalInfo portal, string username, string password, string ip, bool createPersistentCookie);
    }
}
