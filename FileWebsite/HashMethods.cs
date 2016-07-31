using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace FileWebsite
{
    public class HashMethods
    {
        /// <summary>
        /// it generates a hash value from salt and text
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="salt"></param>
        /// <returns></returns>

        public static byte[] GenerateSaltedHash(byte[] Text, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] TextWithSalt =
              new byte[Text.Length + salt.Length];

            for (int i = 0; i < Text.Length; i++)
            {
                TextWithSalt[i] = Text[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                TextWithSalt[Text.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(TextWithSalt);
        }
        /// <summary>
        /// it compares two byte arrays
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}