using Microsoft.Xrm.Tooling.PackageDeployment.CrmPackageExtentionBase;
using BeerLike.PackageDeployer.Tasks;
using BeerLike.PackageDeployer.Services;
using System.Diagnostics;

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
            PackageLog.Log("Syncing team roles assignments...", TraceEventType.Information);
            syncTeamRolesTask.Execute(pathToJsonFile);
            PackageLog.Log("Team roles assignments synced successfully", TraceEventType.Information);
        }
        catch (Exception ex)
        {
            PackageLog.Log($"Failed to sync team roles assignments: {ex.Message}", TraceEventType.Error, ex);
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
            PackageLog.Log("Syncing team Column Security Profiles assignments...", TraceEventType.Information);
            syncTeamCspsTask.Execute(pathToJsonFile);
            PackageLog.Log("Team Column Security Profiles assignments synced successfully", TraceEventType.Information);
        }
        catch (Exception ex)
        {
            PackageLog.Log($"Failed to sync team Column Security Profiles assignments: {ex.Message}", TraceEventType.Error, ex);
        }
    }
}