using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginSystem
{
    public class PluginManager
    {
        public void LoadAndExecutePlugins(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Директория {directoryPath} не найдена");
                return;
            }
            var pluginTypes = new List<Type>();
            var dllFiles = Directory.GetFiles(directoryPath, "*.dll");
            foreach (var file in dllFiles)
            {
                try
                {
                    Assembly asssembly = Assembly.LoadFrom(file);
                    var types = asssembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<PluginLoadAttribute>() != null);
                    pluginTypes.AddRange(types);
                }
                catch (Exception ex) { Console.WriteLine($"Не удалось загрузить библиотеку {file}: {ex.Message}"); }
            }
            List<Type> sortedPlugins;
            try
            {
                sortedPlugins = TopologicalSort(pluginTypes);
            }
            catch (InvalidOperationException ex) {
                Console.WriteLine($"Ошбика разрешения зависимостей: {ex.Message}");
                return;
            }
            foreach (var type in sortedPlugins)
            {
                try
                {
                    object pluginInstance = Activator.CreateInstance(type);
                    MethodInfo executeMethod = type.GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public);
                    if (executeMethod != null)
                    {
                        Console.WriteLine($"Выполнение плагина {type.Name}");
                        executeMethod.Invoke(pluginInstance, null);
                    }
                    else
                    {
                        Console.WriteLine($"В плагине {type.Name} не найден публичный метод Execute().");
                    }
                }
                catch (Exception ex) { Console.WriteLine($"Ошибка при выполнении плагина {type.Name}: {ex.Message}"); }
            }
        }
        private List<Type> TopologicalSort(List<Type> pluginTypes) {
            var graph = new Dictionary<Type, List<Type>>();
            var inDegree = new Dictionary<Type, int>();
            foreach(var type in pluginTypes)
            {
                graph[type] = new List<Type>();
                inDegree[type] = 0;
            }
            foreach (var type in pluginTypes)
            {
                var attribute = type.GetCustomAttribute<PluginLoadAttribute>();
                foreach (var depName in attribute.Dependencies)
                {
                    var dependencyType = pluginTypes.FirstOrDefault(t => t.Name == depName);
                    if (dependencyType != null)
                    {
                        graph[dependencyType].Add(type);
                        inDegree[type]++;
                    } else
                    {
                        Console.WriteLine($"Плагин {type.Name} требует {depName}, но он не найден");
                    }
                }
            }
            var queue = new Queue<Type>();
            foreach(var type in pluginTypes)
            {
                if (inDegree[type] == 0)
                {
                    queue.Enqueue(type);
                }
            }
            var sortedList = new List<Type>();
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                sortedList.Add(current);
                foreach(var neighbor in graph[current])
                {
                    inDegree[neighbor]--;
                    if (inDegree[neighbor] == 0)
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
            if (sortedList.Count != pluginTypes.Count)
            {
                throw new InvalidOperationException("Обнаружена циклическая зависимость");
            }
            return sortedList;
        }
    }
}
