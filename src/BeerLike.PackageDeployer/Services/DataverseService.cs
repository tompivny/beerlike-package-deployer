using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using BeerLike.PackageDeployer.Models.Dataverse;
using Microsoft.Xrm.Sdk.Messages;
namespace BeerLike.PackageDeployer.Services;

internal class DataverseService
{
    private readonly IOrganizationService _organizationService;

    internal DataverseService(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    internal Team? RetrieveTeam(string teamIdentifier)
    {
        var isTeamIdentifierGuid = Guid.TryParse(teamIdentifier, out _);
        var fieldToIdentifyTeam = isTeamIdentifierGuid ? Team.Fields.TeamId : Team.Fields.Name;
        var query = new QueryExpression(Team.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Team.Fields.TeamId, Team.Fields.Name),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(fieldToIdentifyTeam, ConditionOperator.Equal, teamIdentifier)
                }
            }
        };
        return _organizationService.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<Team>();
    }

    internal List<TeamRoles> RetrieveTeamRoles(Team team)
    {
        var query = new QueryExpression(TeamRoles.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(TeamRoles.Fields.TeamId, ConditionOperator.Equal, team.Id)
                }
            }
        };
        return _organizationService.RetrieveMultiple(query).Entities.Select(e => e.ToEntity<TeamRoles>()).ToList();
    }

    internal Role? RetrieveRole(Guid roleId)
    {
        var query = new QueryExpression(Role.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(false),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(Role.Fields.RoleId, ConditionOperator.Equal, roleId)
                }
            }
        };
        return _organizationService.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<Role>();
    }

    internal void AssignRolesToTeam(Team team, List<Role> rolesToAssign)
    {
        var associateRequest = new AssociateRequest()
        {
            Target = team.ToEntityReference(),
            Relationship = new Relationship(TeamRoles.Fields.teamroles_association),
            RelatedEntities = new EntityReferenceCollection(rolesToAssign.Select(r => r.ToEntityReference()).ToList())
        };
        _organizationService.Execute(associateRequest);
    }

    internal void RemoveRolesFromTeam(Team team, List<TeamRoles> rolesToRemove)
    {
        var rolesToRemoveReferences = new EntityReferenceCollection(rolesToRemove.Select(r => new EntityReference(Role.EntityLogicalName, r.GetAttributeValue<Guid>(TeamRoles.Fields.RoleId))).ToList());
        var disassociateRequest = new DisassociateRequest()
        {
            Target = team.ToEntityReference(),
            Relationship = new Relationship(TeamRoles.Fields.teamroles_association),
            RelatedEntities = rolesToRemoveReferences
        };
        _organizationService.Execute(disassociateRequest);
    }

    internal FieldSecurityProfile? RetrieveFieldSecurityProfile(Guid fieldSecurityProfileId)
    {
        var query = new QueryExpression(FieldSecurityProfile.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(false),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(FieldSecurityProfile.Fields.FieldSecurityProfileId, ConditionOperator.Equal, fieldSecurityProfileId)
                }
            }
        };
        return _organizationService.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<FieldSecurityProfile>();
    }

    internal List<TeamProfiles> RetrieveTeamProfiles(Team team)
    {
        var query = new QueryExpression(TeamProfiles.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(TeamProfiles.Fields.TeamId, ConditionOperator.Equal, team.Id)
                }
            }
        };
        return _organizationService.RetrieveMultiple(query).Entities.Select(e => e.ToEntity<TeamProfiles>()).ToList();
    }

    internal void AssignFieldSecurityProfilesToTeam(Team team, List<FieldSecurityProfile> fieldSecurityProfiles)
    {
        var associateRequest = new AssociateRequest()
        {
            Target = team.ToEntityReference(),
            Relationship = new Relationship(TeamProfiles.Fields.teamprofiles_association),
            RelatedEntities = new EntityReferenceCollection(fieldSecurityProfiles.Select(f => f.ToEntityReference()).ToList())
        };
        _organizationService.Execute(associateRequest);
    }

    internal void RemoveFieldSecurityProfilesFromTeam(Team team, List<TeamProfiles> fieldSecurityProfiles)
    {
        var disassociateRequest = new DisassociateRequest()
        {
            Target = team.ToEntityReference(),
            Relationship = new Relationship(TeamProfiles.Fields.teamprofiles_association),
            RelatedEntities = new EntityReferenceCollection(fieldSecurityProfiles.Select(f => f.ToEntityReference()).ToList())
        };
        _organizationService.Execute(disassociateRequest);
    }
}