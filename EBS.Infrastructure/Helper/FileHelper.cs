using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EBS.Infrastructure.Helper
{
   public class FileHelper
    {
       public static string ReadText(string filePath)
       {
           try
           {
               string result = "";
               using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
               {
                   using (StreamReader reader = new StreamReader(fs, Encoding.Default))
                   {
                        result= reader.ReadToEnd();
                   }
               }
               return result;
           }
           catch (Exception ex)
           {
              throw ex;
           }
       }
    }
}
