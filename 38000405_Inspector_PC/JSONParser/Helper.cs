using Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders.InspectionProcedure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JsonParser
{
    public static class Helper
    {
        public static T GetEnumValueFromDescription<T>(string description) where T : Enum
        {
            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(XmlEnumAttribute)) is XmlEnumAttribute attribute)
                {
                    if (attribute.Name == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException($"Not found: {description}", nameof(description));
        }

        public static List<InspectionProcedureEntity> ProcessData(string json)
        {
            var jsonData =  JsonSerializer.Deserialize<List<InspectionProcedureJson>>(json);
            return new JsonToXMLClass(jsonData.ToArray()).GetJsonToXml();
        }
    }
}
