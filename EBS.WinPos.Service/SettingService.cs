using EBS.WinPos.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using EBS.WinPos.Service.Dto;
using EBS.WinPos.Domain;
using EBS.Infrastructure;
using System.Reflection;
namespace EBS.WinPos.Service
{
   public class SettingService
    {
       Repository _db;
       public SettingService()
       {
           _db = new Repository();
       }

       public PosSettings GetSettings()
       {
           PosSettings settingModel = new PosSettings();
           List<Setting> list = _db.Settings.ToList(); 
           settingModel = GetSettingModel<PosSettings>(list);
           return settingModel;
       }

       private T GetSettingModel<T>(List<Setting> list) where T : new()
       {
           T settingModel = new T();
           Dictionary<string, string> settings = new Dictionary<string, string>();
           list.ForEach((n) => settings.Add(n.Key, n.Value));
           if (settings.Count == 0)
           {
               return settingModel;
           }
           var settingModelType = typeof(T);
           settingModelType.GetProperties().ToList().ForEach((pinfo) =>
           {
               var dicKeys = pinfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
               if (dicKeys != null)
               {
                   string key = ((DisplayNameAttribute)dicKeys[0]).DisplayName;
                   if (settings.ContainsKey(key))
                   {
                       string value = settings[key];
                       //转换类型
                       object objValue = Convert.ChangeType(value, pinfo.PropertyType);
                       pinfo.SetValue(settingModel, objValue, null);
                   }

               };
           });

           return settingModel;
       }

       public void Update(int id, string value)
       {
           var model = _db.Settings.FirstOrDefault(n => n.Id == id);
           if (model == null) { throw new AppException("配置项不存在"); }
           model.Value = value;
           _db.SaveChanges();
       }

       public void SaveSetting(PosSettings setting)
       {
            var modelType= setting.GetType();
            var Keys = GetPropertyInfos(modelType);
            foreach (var key in Keys)
            {
                Setting model = new Setting();
                 key.GetCustomAttributes(false).FirstOrDefault(attr => attr.GetType().Name == typeof(DisplayNameAttribute).Name);
               
            }

       }

       private List<PropertyInfo> GetPropertyInfos(Type type)
       {
           List<PropertyInfo> properties = new List<PropertyInfo>();
           properties = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).ToList();
           return properties;
       }
    }
}
