using System.Text.Json.Serialization;
namespace BeerLike.PackageDeployer.Models.Files;

internal class TeamCsps
{
    [JsonPropertyName("teamIdentifier")]
    internal string TeamIdentifier { get; set; } = null!;

    [JsonPropertyName("removeUnassigned")]
    internal bool? RemoveUnassigned { get; set; } = false;

    [JsonPropertyName("columnSecurityProfiles")]
    internal List<Guid>? ColumnSecurityProfiles { get; set; } = new List<Guid>();
}