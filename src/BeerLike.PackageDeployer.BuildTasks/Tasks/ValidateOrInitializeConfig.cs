using Microsoft.Build.Framework;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.IO;
using MSBuildTask = Microsoft.Build.Utilities.Task;

namespace BeerLike.PackageDeployer.BuildTasks.Tasks;

/// <summary>
/// MSBuild Task that validates a JSON file against a schema, or initializes it from a template if it doesn't exist.
/// </summary>
public class ValidateOrInitializeConfig : MSBuildTask
{
    /// <summary>
    /// Path to the JSON file to validate or create.
    /// </summary>
    [Required]
    public string ConfigFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Path to the JSON schema file used for validation.
    /// </summary>
    [Required]
    public string ConfigSchemaFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Path to the template file used for initialization when the JSON file doesn't exist.
    /// </summary>
    [Required]
    public string ConfigTemplateFilePath { get; set; } = string.Empty;

    public override bool Execute()
    {
        try
        {
            if (!File.Exists(ConfigFilePath))
            {
                return InitializeFromTemplate();
            }

            return ValidateAgainstSchema();
        }
        catch (System.Exception ex)
        {
            Log.LogError($"Unexpected error processing '{ConfigFilePath}': {ex.Message}");
            return false;
        }
    }

    private bool InitializeFromTemplate()
    {
        if (!File.Exists(ConfigTemplateFilePath))
        {
            Log.LogError($"Template file not found: '{ConfigTemplateFilePath}'");
            return false;
        }

        try
        {
            var directory = Path.GetDirectoryName(ConfigFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Log.LogMessage(MessageImportance.Normal, $"Created directory: '{directory}'");
            }

            File.Copy(ConfigTemplateFilePath, ConfigFilePath);
            Log.LogMessage(MessageImportance.High, $"Initialized '{ConfigFilePath}' from template '{ConfigTemplateFilePath}'");
            return true;
        }
        catch (System.Exception ex)
        {
            Log.LogError($"Failed to initialize '{ConfigFilePath}' from template: {ex.Message}");
            return false;
        }
    }

    private bool ValidateAgainstSchema()
    {
        if (!File.Exists(ConfigSchemaFilePath))
        {
            Log.LogError($"Schema file not found: '{ConfigSchemaFilePath}'");
            return false;
        }

        try
        {
            var jsonContent = File.ReadAllText(ConfigFilePath);
            var schemaContent = File.ReadAllText(ConfigSchemaFilePath);

            JSchema schema = JSchema.Parse(schemaContent);
            JToken jsonToValidate = JToken.Parse(jsonContent);

            bool isValid = jsonToValidate.IsValid(schema, out IList<ValidationError> errors);

            if (isValid)
            {
                Log.LogMessage(MessageImportance.Normal, $"Validated '{ConfigFilePath}' against schema successfully.");
                return true;
            }

            foreach (var error in errors)
            {
                Log.LogError(
                    subcategory: "JSON Schema",
                    errorCode: error.ErrorType.ToString(),
                    helpKeyword: null,
                    file: ConfigFilePath,
                    lineNumber: error.LineNumber,
                    columnNumber: error.LinePosition,
                    endLineNumber: 0,
                    endColumnNumber: 0,
                    message: $"{error.Message} (Path: {error.Path})");
            }

            return false;
        }
        catch (Newtonsoft.Json.JsonReaderException ex)
        {
            Log.LogError(
                subcategory: "JSON Parse",
                errorCode: "InvalidJson",
                helpKeyword: null,
                file: ConfigFilePath,
                lineNumber: ex.LineNumber,
                columnNumber: ex.LinePosition,
                endLineNumber: 0,
                endColumnNumber: 0,
                message: ex.Message);
            return false;
        }
    }
}

