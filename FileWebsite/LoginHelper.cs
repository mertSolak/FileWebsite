using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace FileWebsite
{
    public class LoginHelper
    {
        /// <summary>
        /// it checks the user exist or not
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        public bool LoginCheck(string username , string password)
        {
            using (var myDb = new Entity())
            {
                var user = myDb.Users.FirstOrDefault(m => m.Name == username);
                byte[] passwordByte = Convert.FromBase64String(user.Password);
                byte[] saltByte = Convert.FromBase64String(user.Salt);
                byte[] hashedByte = HashMethods.GenerateSaltedHash(Encoding.UTF8.GetBytes(password), saltByte);

                return HashMethods.CompareByteArrays(passwordByte, hashedByte);
            }
        }

        public int FindUserID(string username)
        {
            using (var myDb = new Entity())
            {
                return myDb.Users.FirstOrDefault(m => m.Name == username).Id;
            }
        }
    }   
}