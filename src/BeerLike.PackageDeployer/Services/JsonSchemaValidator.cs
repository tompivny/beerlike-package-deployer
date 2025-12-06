using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Reflection;
using System.Text;

namespace BeerLike.PackageDeployer.Services;

internal static class JsonSchemaValidator
{
    internal static void Validate(string pathToJsonFile, string schemaResourceName)
    {
        try
        {
            var jsonContent = File.ReadAllText(pathToJsonFile);
            var schemaContent = GetEmbeddedSchema(schemaResourceName);
            
            JSchema schema = JSchema.Parse(schemaContent);
            JToken jsonToValidate = JToken.Parse(jsonContent);

            bool isValid = jsonToValidate.IsValid(schema, out IList<ValidationError> errors);

            if (!isValid)
            {
                var errorMessage = new StringBuilder();
                foreach (var error in errors)
                {
                    errorMessage.AppendLine($"JSON Schema Validation Error: {error.ErrorType}. Message: {error.Message}, Path: {error.Path}, Line: {error.LineNumber}, Pos: {error.LinePosition}");
                }
                throw new Exception(errorMessage.ToString());
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during JSON schema validation: {ex.Message}", ex);
        }
    }

    private static string GetEmbeddedSchema(string schemaResourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // Convert path format (e.g., "Schemas/TeamCsps.schema.json") to resource name format
        // Resource names use dots as separators and include the assembly name as prefix
        var resourceNameSuffix = schemaResourceName.Replace("/", ".").Replace("\\", ".");
        
        // Find the matching resource
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(resourceNameSuffix, StringComparison.OrdinalIgnoreCase));
        
        if (resourceName == null)
        {
            var availableResources = string.Join(", ", assembly.GetManifestResourceNames());
            throw new FileNotFoundException(
                $"Schema resource '{schemaResourceName}' not found. Available resources: {availableResources}");
        }
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Could not load schema resource stream for '{resourceName}'");
        }
        
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

}
