using Newtonsoft.Json;

namespace BeerLike.PackageDeployer.Models.Files;

internal class TeamSecurityRoles
{
    [JsonProperty("team")]
    internal string TeamIdentifier { get; set; } = null!;

    [JsonProperty("removeUnassigned")]
    internal bool? RemoveUnassigned { get; set; } = false;

    [JsonProperty("securityRoles")]
    internal List<Guid>? SecurityRoles { get; set; } = new List<Guid>();
}