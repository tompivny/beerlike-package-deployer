using Microsoft.Xrm.Tooling.PackageDeployment.CrmPackageExtentionBase;
using BeerLike.PackageDeployer.Tasks;
using BeerLike.PackageDeployer.Services;

namespace BeerLike.PackageDeployer;
public abstract class PackageExtension : ImportExtension
{
    /// <summary>
    /// Synchronizes assignments of security roles to teams
    /// </summary>
    /// <param name="pathToJsonFile">Path to the JSON file containing the team roles assignments</param>
    public void SyncTeamRoles(string pathToJsonFile)
    {
        var syncTeamRolesTask = new SyncTeamRolesTask(new DataverseService(CrmSvc), PackageLog);

        try
        {
            RaiseUpdateEvent("Syncing team roles assignments...", ProgressPanelItemStatus.Working);
            syncTeamRolesTask.Execute(pathToJsonFile);
            RaiseUpdateEvent("Team roles assignments synced successfully", ProgressPanelItemStatus.Complete);
        }
        catch (Exception ex)
        {
            RaiseFailEvent($"Failed to sync team roles assignments: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Synchronizes assignments of column security profiles to teams
    /// </summary>
    /// <param name="pathToJsonFile">Path to the JSON file containing the team column security profiles assignments</param>
    public void SyncTeamCsps(string pathToJsonFile)
    {
        var syncTeamCspsTask = new SyncTeamCspsTask(new DataverseService(CrmSvc), PackageLog);

        try
        {
            RaiseUpdateEvent("Syncing team Column Security Profiles assignments...", ProgressPanelItemStatus.Working);
            syncTeamCspsTask.Execute(pathToJsonFile);
            RaiseUpdateEvent("Team Column Security Profiles assignments synced successfully", ProgressPanelItemStatus.Complete);
        }
        catch (Exception ex)
        {
            RaiseFailEvent($"Failed to sync team Column Security Profiles assignments: {ex.Message}", ex);
        }
    }
}