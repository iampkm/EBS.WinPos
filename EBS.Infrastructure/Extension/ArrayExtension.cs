using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBS.Infrastructure.Extension
{
   public static class ArrayExtension
    {
       public static int[] ToIntArray(this string[] value)
       {
           int[] array = new int[value.Length];
           for (int i = 0; i < value.Length; i++)
           {
               array[i] = Convert.ToInt32(value[i].ToString());
           }
           return array;
       }
    }
}
