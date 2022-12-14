using System.Collections;
using System.Reflection;

namespace MyProject.WebApi
{
    public static class ConfigurationExtensions
    {
        private static readonly Hashtable _environmentVariables;
        static ConfigurationExtensions()
        {
            _environmentVariables = (Hashtable)Environment.GetEnvironmentVariables();
        }

        public static T Load<T>(this IConfiguration configuration) where T : class, new()
        {
            var @object = new T();
            if (_environmentVariables?["ASPNETCORE_ENVIRONMENT"]?.ToString()?.ToLower() == "development")
            {
                configuration.Bind(@object);
            }
            else
            {
                LoadObjectFromEnvironmentVariables(@object);
            }
            return @object;
        }
        private static void LoadObjectFromEnvironmentVariables(object @object, string parentPropertyName = "")
        {
            PropertyInfo[] info = @object.GetType().GetProperties();
            foreach (PropertyInfo property in info)
            {
                if (property.PropertyType.IsPrimitive || property.PropertyType.Equals(typeof(string)))
                {
                    var compoundProperty = (string.IsNullOrEmpty(parentPropertyName) ? null : parentPropertyName + "__") + property.Name;
                    if (_environmentVariables.ContainsKey(compoundProperty))
                    {
                        var value = _environmentVariables[compoundProperty];
                        var parsedValue = Convert.ChangeType(value, property.PropertyType);
                        property.SetValue(@object, parsedValue, null);
                    }
                }
                else if (property.PropertyType.IsArray)
                {

                }
                else
                {
                    var instance = Activator.CreateInstance(property.PropertyType);
                    property.SetValue(@object, instance);
                    LoadObjectFromEnvironmentVariables(instance, property.Name);
                }
            }

        }
    }
}
