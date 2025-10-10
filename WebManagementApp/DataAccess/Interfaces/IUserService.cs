using WebManagementApp.Models;

namespace WebManagementApp.DataAccess.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetUser(UserVM userModel);
        string CheckPassword(string email);
        string GeneratePasswordHash(string password);
        void UpdateLastAcivityDate(string email);
    }
}
