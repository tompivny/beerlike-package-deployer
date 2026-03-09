using Microsoft.Xrm.Tooling.PackageDeployment.CrmPackageExtentionBase;
using BeerLike.PackageDeployer.Services;
using Newtonsoft.Json;
using BeerLike.PackageDeployer.Models.Files;
using System.Diagnostics;
using BeerLike.PackageDeployer.Models.Dataverse;

namespace BeerLike.PackageDeployer.Tasks;

internal class SyncTeamCspsTask : TaskBase
{
    private const string JSON_SCHEMA_FILE_PATH = "Schemas/TeamCsps.schema.json";

    internal SyncTeamCspsTask(DataverseService dataverseService, TraceLogger packageLogger)
        : base(dataverseService, packageLogger)
    {
    }

    internal void Execute(string pathToJsonFile)
    {
        JsonSchemaValidator.Validate(pathToJsonFile, JSON_SCHEMA_FILE_PATH);
        var teamCspsAssignments = JsonConvert.DeserializeObject<List<TeamCsps>>(File.ReadAllText(pathToJsonFile));
        if (teamCspsAssignments == null)
        {
            PackageLogger.Log("No team CSP assignments found in the JSON file. Skipping team CSP synchronization.", TraceEventType.Warning);
            return;
        }
        foreach (var teamCspAssignment in teamCspsAssignments)
        {
            var team = DataverseService.RetrieveTeam(teamCspAssignment.TeamIdentifier);
            if (team == null)
            {
                PackageLogger.Log($"Team with identifier {teamCspAssignment.TeamIdentifier} not found. Skipping assignment for this team.", TraceEventType.Warning);
                continue;
            }

            var currentTeamCsps = DataverseService.RetrieveTeamProfiles(team);
            var alreadyAssignedCspIds = new HashSet<Guid>(currentTeamCsps.Select(csp => csp.GetAttributeValue<Guid>(TeamProfiles.Fields.FieldSecurityProfileId)));
            var cspsToAssign = teamCspAssignment.ColumnSecurityProfiles?
                .Select(cspId => DataverseService.RetrieveFieldSecurityProfile(cspId))
                .Where(csp => csp != null)
                .Select(c => c!)
                .Where(c => !alreadyAssignedCspIds.Contains(c.Id))
                .ToList();

            if (cspsToAssign != null && cspsToAssign.Count > 0)
            {
                DataverseService.AssignFieldSecurityProfilesToTeam(team, cspsToAssign);
                PackageLogger.Log($"Assigned {cspsToAssign.Count} new column security profiles to team {team.Name} ({team.Id}): {string.Join(", ", cspsToAssign.Select(c => c.Name))}", TraceEventType.Information);
            }

            if (teamCspAssignment.RemoveUnassigned ?? false)
            {
                var columnSecurityProfiles = teamCspAssignment.ColumnSecurityProfiles ?? new List<Guid>();
                var cspsToRemove = currentTeamCsps.Where(csp => !columnSecurityProfiles.Contains(csp.GetAttributeValue<Guid>(TeamProfiles.Fields.FieldSecurityProfileId))).ToList();
                if (cspsToRemove != null && cspsToRemove.Count > 0)
                {
                    DataverseService.RemoveFieldSecurityProfilesFromTeam(team, cspsToRemove);
                    PackageLogger.Log($"Removed {cspsToRemove.Count} column security profiles from team {team.Name} ({team.Id}): {string.Join(", ", cspsToRemove.Select(c => c.Id))}", TraceEventType.Information);
                }
            }
        }
    }
}