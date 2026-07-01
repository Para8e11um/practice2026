using System.Linq;
using System.Reflection;
using System.Collections.Generic;
namespace task05
{
    public class ClassAnalyzer
    {
        private readonly Type _type;
        public ClassAnalyzer(Type type)
        {
            _type = type;
        }
        public IEnumerable<string> GetPublicMethods()
        {
            return from method in _type.GetMethods()
                   select method.Name;
        }
        public IEnumerable<string> GetMethodParams(string methodname)
        {
            var method = _type.GetMethod(methodname);
            if (method == null)
            {
                return Enumerable.Empty<string>().Append($"Returns: {method.ReturnType.Name}");
            } else
            {
                return (from param in method.GetParameters()
                        select param.Name).Append($"Returns: {method.ReturnType.Name}");
            }
        }
        public IEnumerable<string> GetAllFields()
        {
            return from field in _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                   select field.Name;
        }
        public IEnumerable<string> GetProperties()
        {
            return from property in _type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                   select property.Name;
        }
        public bool HasAttribute<T>() where T : Attribute
        {
            return _type.GetCustomAttributes(typeof(T), false).Any();
        }
    }
}
