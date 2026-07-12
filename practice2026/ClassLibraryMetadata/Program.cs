using System;
using System.Reflection;
using task07;
namespace ClassLibraryMetadata
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Ошибка: не указан путь к динамической библиотеке");
                return;
            }
            string dllPath = args[0];
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Console.WriteLine("Метаданные библиотеки " + assembly.FullName);
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.IsClass)
                    {
                        var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
                        if (displayNameAttr != null)
                        {
                            Console.WriteLine($"Класс {displayNameAttr.DisplayName}({type.Name})");
                        } else
                        {
                            Console.WriteLine($"Класс {type.Name}");
                        }
                            PrintAttributes(type);
                        Console.WriteLine(" Конструкторы: ");
                        ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (constructors.Length == 0)
                        {
                            Console.WriteLine("  Нет конструкторов");
                        }
                        foreach (var constructor in constructors)
                        {
                            PrintMethod(constructor);
                        }
                        Console.WriteLine(" Методы: ");
                        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                        if (methods.Length == 0)
                        {
                            Console.WriteLine("  Методов нет");
                        }
                        foreach (var method in methods)
                        {
                            PrintMethod(method);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
        static void PrintAttributes(MemberInfo member) {
            var attributes = member.GetCustomAttributes();
            bool hasAttributes = false;
            foreach (var attr in attributes)
            {
                if (!hasAttributes)
                {
                    Console.WriteLine("  Атрибуты: ");
                    hasAttributes = true;
                }
                Console.WriteLine($"    -{attr.GetType().Name}");
            }

            if (!hasAttributes && member is Type)
            {
                Console.WriteLine("  Аттрибуты отсутствуют");
            }
        }
        static void PrintMethod(MethodBase method) {
            string typeName = method is MethodInfo methodInfo ? methodInfo.ReturnType.Name : "";
            string signature = string.IsNullOrWhiteSpace(typeName) ? method.Name : $"{typeName} {method.Name}";
            var displayNameAttr = method.GetCustomAttribute<task07.DisplayNameAttribute>();
            if (displayNameAttr != null)
            {
                Console.Write(displayNameAttr.DisplayName);
            }
            Console.Write($" {signature}(");
            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                if (i < parameters.Length - 1)
                {
                    Console.Write(", ");
                }
            }
            Console.WriteLine(")");
            PrintAttributes(method);
        }
    }
}