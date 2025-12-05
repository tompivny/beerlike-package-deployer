using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.PackageDeployment.CrmPackageExtentionBase;
using BeerLike.PackageDeployer.Services;

namespace BeerLike.PackageDeployer.Tasks;

/// <summary>
/// Abstract base class providing common dependencies for all tasks.
/// </summary>
internal abstract class TaskBase
{
    protected DataverseService DataverseService { get; }
    protected TraceLogger PackageLogger { get; }

    protected TaskBase(DataverseService dataverseService, TraceLogger packageLogger)
    {
        DataverseService = dataverseService;
        PackageLogger = packageLogger;
    }
}

