using BeerLike.PackageDeployer.Services;
using Microsoft.Xrm.Tooling.PackageDeployment.CrmPackageExtentionBase;
using BeerLike.PackageDeployer.Models.Files;
using Newtonsoft.Json;
using System.Diagnostics;
using BeerLike.PackageDeployer.Models.Dataverse;

namespace BeerLike.PackageDeployer.Tasks;

internal class SyncTeamRolesTask : TaskBase
{
    private const string JSON_SCHEMA_FILE_PATH = "Schemas/TeamRoles.schema.json";

    internal SyncTeamRolesTask(DataverseService dataverseService, TraceLogger packageLogger)
        : base(dataverseService, packageLogger)
    {
    }
    internal void Execute(string pathToJsonFile)
    {
        JsonSchemaValidator.Validate(pathToJsonFile, JSON_SCHEMA_FILE_PATH);
        var teamRolesAssignments = JsonConvert.DeserializeObject<List<TeamSecurityRoles>>(File.ReadAllText(pathToJsonFile));
        if (teamRolesAssignments == null)
        {
            PackageLogger.Log("No team roles assignments found in the JSON file. Skipping team roles synchronization.", TraceEventType.Warning);
            return;
        }
        foreach (var teamRoleAssignment in teamRolesAssignments)
        {
            var team = DataverseService.RetrieveTeam(teamRoleAssignment.TeamIdentifier);
            if (team == null)
            {
                PackageLogger.Log($"Team with identifier {teamRoleAssignment.TeamIdentifier} not found. Skipping assignment for this team.", TraceEventType.Warning);
                continue;
            }

            var currentTeamRoles = DataverseService.RetrieveTeamRoles(team);
            var rolesToAssign = teamRoleAssignment.SecurityRoles?.Select(roleId => DataverseService.RetrieveRole(roleId)).Where(role => role != null).Select(r => r!).ToList();

            if (rolesToAssign != null && rolesToAssign.Count > 0)
            {
                DataverseService.AssignRolesToTeam(team, rolesToAssign);
                PackageLogger.Log($"Assigned {rolesToAssign.Count} roles to team {team.Name} ({team.Id}): {string.Join(", ", rolesToAssign.Select(r => r.Name))}", TraceEventType.Information);
            }

            if (teamRoleAssignment.RemoveUnassigned ?? false)
            {
                var securityRoles = teamRoleAssignment.SecurityRoles ?? new List<Guid>();
                var rolesToRemove = currentTeamRoles.Where(tr => !securityRoles.Contains(tr.GetAttributeValue<Guid>(TeamRoles.Fields.RoleId))).ToList();
                if (rolesToRemove != null && rolesToRemove.Count > 0)
                {
                    DataverseService.RemoveRolesFromTeam(team, rolesToRemove);
                    PackageLogger.Log($"Removed {rolesToRemove.Count} roles from team {team.Name} ({team.Id}): {string.Join(", ", rolesToRemove.Select(r => r.Id))}", TraceEventType.Information);
                }
            }
        }
    }
}