using WebManagementApp.Models;

namespace WebManagementApp.DataAccess.Interfaces
{
    public interface IManageUserService
    {
        Task<IEnumerable<ManageUserVM>> GetManageUserData();
        Task AddUser(string name, string email, string password);
        bool IsExistingEmail(string email, string idUser);
        Task UpdateUser(string name, string email, string password, string idUser);
        Task DeleteUser(string idUser);
    }
}
