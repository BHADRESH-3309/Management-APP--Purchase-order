using WebManagementApp.Models;
using WebManagementApp.DataAccess.DbAccess;
using WebManagementApp.DataAccess.Interfaces;
using System.Text;
using System.Security.Cryptography;

namespace WebManagementApp.DataAccess.Services
{
    public class UserService : IUserService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public UserService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        // Check if the entered login credentials are valid.
        public async Task<UserDTO> GetUser(UserVM userModel)
        {
            string getUserQuery = @"SELECT idUser,Name, Username,Email, Password,UserRole FROM tblUser where Email=@Email ";
            var user = await _sqlDataAccess.GetData<UserDTO, dynamic>(getUserQuery, new { Email = userModel.Email });
            //string getUserQuery = @"SELECT idUser, Username,Email, Password,UserRole FROM tblUser where Email=@Email And Password = @Password ";
            //var user = await _sqlDataAccess.GetData<UserDTO, dynamic>(getUserQuery, new { Email = userModel.Email, Password = userModel.Password });
            return user.FirstOrDefault();
        }
        public string GeneratePasswordHash(string password)
        {
            string key = "1234567890123456"; // 16 characters for 128-bit AES key
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(password);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
            //// Create a SHA256
            //using (SHA256 sha256Hash = SHA256.Create())
            //{
            //    // ComputeHash - returns byte array
            //    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            //    // Convert byte array to a hexadecimal  string
            //    StringBuilder builder = new StringBuilder();
            //    for (int i = 0; i < bytes.Length; i++)
            //    {
            //        builder.Append(bytes[i].ToString("x2"));
            //    }
            //    return builder.ToString();
            //}
        }
        public string CheckPassword(string email)
        {
            string userPassword = string.Empty;
            string getPasswordQuery = $@"SELECT Password FROM tblUser where Email='{email}' ";
            userPassword = _sqlDataAccess.GetSingleValue(getPasswordQuery);
            return userPassword;
        }


        public void UpdateLastAcivityDate(string email)
        {
            string updateQuery = $@"Update tblUser SET LastLoginTime = GETDATE() where Email='{email}' ";
            _sqlDataAccess.ExecuteDML(updateQuery,new { Email  = email});
        }
    }
}
