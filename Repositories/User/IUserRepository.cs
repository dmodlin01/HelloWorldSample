using DTOs;
using System.Collections.Generic;

namespace Repositories.EF
{
    public interface IUserRepository
    {
        UserDTO GetUser(int userId);
        List<UserDTO> GetUsers();
        void AddUser(ref UserDTO user);
    }
}
