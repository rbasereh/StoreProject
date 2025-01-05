using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TP.Application.Common.Swagger;

[AttributeUsage(AttributeTargets.Property)]
public class SwaggerExcludeAttribute : Attribute
{
}
public class SwaggerExcludeFilter : ISchemaFilter
{
    #region ISchemaFilter Members


    #endregion
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {

        if (schema?.Properties == null || context.Type == null)
            return;

        var excludedProperties = context.Type.GetProperties()
                                     .Where(t =>
                                            t.GetCustomAttribute<SwaggerExcludeAttribute>()
                                            != null);


        foreach (var excludedProperty in excludedProperties)
        {
            if (schema.Properties.ContainsKey(excludedProperty.Name.FirstCharacterToLower()))
                schema.Properties.Remove(excludedProperty.Name.FirstCharacterToLower());
        }
    }
   
}
public static class StringsExt
{
    public static string FirstCharacterToLower(this string str)
    {
        if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
            return str;

        return Char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}
