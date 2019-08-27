using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.Services
{
    public interface IBinaryTree
    {
        BinaryTreeModel GetUserTree(string userId);
        string GetUserTreeAsJson(string userId, bool isLoggedInUser);
    }
}
