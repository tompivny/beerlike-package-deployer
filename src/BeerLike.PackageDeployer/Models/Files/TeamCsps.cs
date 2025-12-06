using Newtonsoft.Json;

namespace BeerLike.PackageDeployer.Models.Files;

internal class TeamCsps
{
    [JsonProperty("team")]
    internal string TeamIdentifier { get; set; } = null!;

    [JsonProperty("removeUnassigned")]
    internal bool? RemoveUnassigned { get; set; } = false;

    [JsonProperty("columnSecurityProfiles")]
    internal List<Guid>? ColumnSecurityProfiles { get; set; } = new List<Guid>();
}