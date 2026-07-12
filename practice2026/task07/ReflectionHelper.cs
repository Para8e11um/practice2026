using System;
using System.Reflection;
namespace task07
{
    public class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {
            Console.WriteLine("Анализ типа" + type.Name);
            var customAttrName = type.GetCustomAttribute<DisplayNameAttribute>();
            if (customAttrName != null)
            {
                Console.WriteLine("Имя: " + customAttrName.DisplayName);
            }
            var customAttrVer = type.GetCustomAttribute<VersionAttribute>();
            if (customAttrVer != null)
            {
                Console.WriteLine("Версия: " + customAttrVer.Major + "." + customAttrVer.Minor);
            }
            var properties = type.GetProperties();
            if (properties != null)
            {
                Console.WriteLine("Свойства:");
                foreach (var property in properties)
                {
                    var propertyDisplayName = property.GetCustomAttribute<DisplayNameAttribute>();
                    if (propertyDisplayName != null)
                    {
                        Console.WriteLine("Свойство: " + property.Name + " | Отображаемое имя: " + propertyDisplayName.DisplayName);
                    }

                }
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly );
                if (properties != null)
                {
                    Console.WriteLine("Методы:");
                    foreach (var method in methods)
                    {
                        var methodDisplayName = method.GetCustomAttribute<DisplayNameAttribute>();
                        if (methodDisplayName != null)
                        {
                            Console.WriteLine("Метод: " + method.Name + " | Отображаемое имя: " + methodDisplayName.DisplayName);
                        }
                    }
                }
            }
        }
    }
}
