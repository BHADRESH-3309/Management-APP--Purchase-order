using System.Text;
using WebManagementApp.Models;
using System.Security.Cryptography;
using WebManagementApp.DataAccess.DbAccess;
using WebManagementApp.DataAccess.Interfaces;

namespace WebManagementApp.DataAccess.Services
{
    public class ManageUserService:IManageUserService
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        public ManageUserService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        
        // Get Manage user data
        public async Task<IEnumerable<ManageUserVM>> GetManageUserData()
        {
            IEnumerable<ManageUserVM> userList = new List<ManageUserVM>();

            string query = @"Select idUser,Name, Username,Email, Password,UserRole,DateAdd,LastLoginTime
                             from tblUser where UserRole != 'admin' Order By LastLoginTime DESC ";
            
            userList = await _sqlDataAccess.GetData<ManageUserVM, dynamic>(query, new { });

            foreach (var user in userList)
            {
                user.Password = Decrypt(user.Password);
            }

            return userList;
        }
        
        //check email exist or not
        public bool IsExistingEmail(string email,string idUser)
        {
            string getEmailCountQuery = string.Empty;
            int emailCount;

            //Check if WarehouseSKU is already in DB tblProduct
            if (string.IsNullOrEmpty(idUser))
            {

                var getEmailCount = _sqlDataAccess.GetSingleValue($"SELECT COUNT(*) FROM tblUser WHERE Email='{email}'");
                emailCount = int.Parse(getEmailCount);
            }
            else
            {
                var getEmailCount = _sqlDataAccess.GetSingleValue($"SELECT COUNT(*) FROM tblUser WHERE Email='{email}' and idUser != '{idUser}'");
                emailCount = int.Parse(getEmailCount);
            }

            if (emailCount > 0) return true;

            return false;
        }

        //add user
        public async Task AddUser(string name,string email, string password)
        {
            string hashPassword = password.ToString();

            string userInputPasswordHash = GeneratePasswordHash(password);

            string userName = email.Split('@')[0];

            await _sqlDataAccess.ExecuteDML("Insert Into tblUser (Username,Name,Email,Password,UserRole) Values(@Username,@Name,@Email,@Password,@UserRole)", 
                new { Username = userName, Email = email , Password  = userInputPasswordHash, UserRole="user",Name= name});
        }

        //password into hash password
        public string GeneratePasswordHash(string plainText)
        {
            string key = "1234567890123456";
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
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        // Update User
        public async Task UpdateUser(string name,string email, string password, string idUser)
        {
            string hashPassword = password.ToString();

            string userInputPasswordHash = GeneratePasswordHash(password);

            string updateQuery = $@"Update tblUser Set Name=@Name, Email = @Email, Password =@Password  where idUser=@idUser";
            await _sqlDataAccess.ExecuteDML(updateQuery, new { Email = email, Password = userInputPasswordHash,idUser = idUser,Name =name});
         
        }

        // Decrypt Password
        public string Decrypt(string cipherText)
        {
            string key = "1234567890123456";
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        // User Delete 
        public async Task DeleteUser(string idUser)
        {
            string query = string.Empty;
            query = $@"DELETE FROM tblUser WHERE idUser = @idUser";

            await _sqlDataAccess.ExecuteDML(query, new { idUser });
        }
    }
}
