using FileWebsite.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FileWebsite
{
    public class RegisterHelper
    {
        /// <summary>
        /// it generates user from given model objet and write it to the database
        /// </summary>
        /// <param name="model"></param>
        public void GenerateUser(RegisterViewModel model)
        {
            using (var myDb = new Entity())
            {
                User newUser = new User();
                byte[] saltByte = GenerateRandomSalt(64);

                newUser.Name = model.username;
                newUser.Password = Convert.ToBase64String(HashMethods.GenerateSaltedHash(Encoding.UTF8.GetBytes(model.password), saltByte));
                newUser.Salt = Convert.ToBase64String(saltByte);
                newUser.Status = 1;

                myDb.Users.Add(newUser);

                try
                {
                    myDb.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
            }
        }
        /// <summary>
        /// it generates a random salt number
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] GenerateRandomSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            return buff;
        }
    }
}