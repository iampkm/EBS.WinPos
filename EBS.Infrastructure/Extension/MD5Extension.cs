﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace EBS.Infrastructure.Extension
{
   public static class MD5Extension
    {
       public static string GetMd5Hash(this MD5CryptoServiceProvider md5Hasher, string input)
       {
           // Convert the input string to a byte array and compute the hash.
           byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

           // Create a new Stringbuilder to collect the bytes
           // and create a string.
           StringBuilder sBuilder = new StringBuilder();

           // Loop through each byte of the hashed data 
           // and format each one as a hexadecimal string.
           for (int i = 0; i < data.Length; i++)
           {
               sBuilder.Append(data[i].ToString("x2"));
           }

           // Return the hexadecimal string.
           return sBuilder.ToString();
       }

       public static bool VerifyMd5Hash(this MD5CryptoServiceProvider md5Hasher, string input, string hash)
       {
           // Hash the input.
           string hashOfInput = GetMd5Hash(md5Hasher, input);

           // Create a StringComparer an compare the hashes.
           StringComparer comparer = StringComparer.OrdinalIgnoreCase;

           if (0 == comparer.Compare(hashOfInput, hash))
           {
               return true;
           }
           else
           {
               return false;
           }
       }

       public static string GetMd5Hash(this MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
       public static bool VerifyMd5Hash(this MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
   
}
