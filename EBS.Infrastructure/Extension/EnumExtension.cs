using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Dynamic;
using System.ComponentModel;
namespace EBS.Infrastructure.Extension
{
    public static class EnumExtension
    {
        private static ConcurrentDictionary<Type, List<dynamic>> EnumTypeToInfos = new ConcurrentDictionary<Type, List<dynamic>>();


        public static List<dynamic> GetEnumInfos(this Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException(string.Format("Type {0} is not enum", enumType.FullName));

            List<dynamic> infos = null;
            if (!EnumTypeToInfos.TryGetValue(enumType, out infos))
            {
                infos = new List<dynamic>();
                var fields = enumType.GetFields().Where(t => !t.Name.Equals("value__"));
                foreach (var field in fields)
                {
                    var value = (int)field.GetRawConstantValue();
                    var name = field.Name;
                    var description = name;
                  //  var attr = field.GetCustomAttribute<DescriptionAttribute>();
                    var attr = (DescriptionAttribute)field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
                    if (attr != null)
                        description = attr.Description;

                    dynamic info = new ExpandoObject();
                    info.Value = value;
                    info.Name = name;
                    info.Description = description;

                    infos.Add(info);
                }

                EnumTypeToInfos.TryAdd(enumType, infos);
            }

            return infos;
        }

        public static Dictionary<int, string> GetValueToDescription(this Type enumType)
        {
            return enumType.GetEnumInfos().ToDictionary(t => (int)t.Value, t => (string)t.Description);
        }

        public static object ToEnumByDescription(this string description, Type enumType)
        {
            var info = enumType.GetEnumInfos().Where(t => t.Description == description).FirstOrDefault();
            if (info != null)
                return Enum.Parse(enumType, info.Name);
            else
                return null;
        }

        public static object ToEnumByName(this string name, Type enumType)
        {
            var info = enumType.GetEnumInfos().Where(t => t.Name == name).FirstOrDefault();
            if (info != null)
                return Enum.Parse(enumType, info.Name);
            else
                return null;
        }

        public static object ToEnumByValue(this int value, Type enumType)
        {
            var info = enumType.GetEnumInfos().Where(t => t.Value == value).FirstOrDefault();
            if (info != null)
                return Enum.Parse(enumType, info.Name);
            else
                return null;
        }

        public static T ToEnumByName<T>(this string name)
        {
            return (T)name.ToEnumByName(typeof(T));
        }

        public static T ToEnumByDescription<T>(this string description)
        {
            return (T)description.ToEnumByDescription(typeof(T));
        }

        public static T ToEnumByValue<T>(this int value)
        {
            return (T)value.ToEnumByValue(typeof(T));
        }

        public static string Description(this Enum e)
        {
            var info = e.GetType().GetEnumInfos().Where(t => t.Name == e.ToString()).FirstOrDefault();
            if (info != null)
                return info.Description;
            else
                return null;
        }

        public static string Name(this Enum e)
        {
            var info = e.GetType().GetEnumInfos().Where(t => t.Name == e.ToString()).FirstOrDefault();
            if (info != null)
                return info.Name;
            else
                return null;
        }

        public static int Value(this Enum e)
        {
            var info = e.GetType().GetEnumInfos().Where(t => t.Name == e.ToString()).FirstOrDefault();
            if (info != null)
                return info.Value;
            else
                return -1;
        }
    }

}
