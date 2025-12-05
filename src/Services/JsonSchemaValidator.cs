using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Text;

namespace BeerLike.PackageDeployer.Services;

internal static class JsonSchemaValidator
{
    internal static void Validate(string pathToJsonFile, string pathToSchemaFile)
    {
        try
        {
            var jsonContent = File.ReadAllText(pathToJsonFile);
            var schemaContent = File.ReadAllText(pathToSchemaFile);
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

}
