using System.Text.Json.Serialization;

namespace BeerLike.PackageDeployer.Models.Files;

internal class TeamSecurityRoles
{
    [JsonPropertyName("teamIdentifier")]
    internal string TeamIdentifier { get; set; } = null!;

    [JsonPropertyName("removeUnassigned")]
    internal bool? RemoveUnassigned { get; set; } = false;

    [JsonPropertyName("securityRoles")]
    internal List<Guid>? SecurityRoles { get; set; } = new List<Guid>();
}