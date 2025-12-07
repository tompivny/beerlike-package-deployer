# BeerLike Package Deployer

> [!WARNING]
> This project is still in development. Use at your own risk in production environments.

A .NET library that provides custom extensions for Power Platform [Package Deployer](https://learn.microsoft.com/en-us/power-platform/alm/package-deployer-tool) projects.

[![NuGet Version](https://img.shields.io/nuget/v/BeerLike.PackageDeployer)](https://www.nuget.org/packages/BeerLike.PackageDeployer)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Overview

Currently, the library provides the following features:

- **Declarative Team Security Role Assignment** — Assign (and optionally remove) security roles to teams via JSON configuration
- **Declarative Column Security Profile Assignment** — Assign (and optionally remove) column security profiles (field-level security) to teams via JSON configuration  

## Usage

### 1. Install the NuGet package

Install the NuGet package in your Package Deployer project:

```bash
dotnet add package BeerLike.PackageDeployer
```

Or just add the package reference to your Package Deployer project>
```xml
<PackageReference Include="BeerLike.PackageDeployer" Version="0.0.*" />
```

### 2. Update Your PackageImportExtension Class

Change your package class to inherit from `BeerLike.PackageDeployer.PackageExtension` instead of the default `ImportExtension`. The custom `PackageExtension` class inherits from `ImportExtension` so we don't lose any of the default functionality.

```csharp
using BeerLike.PackageDeployer;

namespace YourNamespace;

public class PackageImportExtension : PackageExtension
{
...
}
```


### 2. (Optional but recommended) Configure build-time validation and auto-creation of configuration files

#### 2.1 Set props and targets
Add the following properties and target to your package deployer `.csproj` file to enable build-time JSON validation and auto-initialization:

```xml
<!-- Configure paths for BeerLike.PackageDeployer JSON validation/initialization -->
<PropertyGroup>
  <TeamRolesConfig>$(MSBuildProjectDirectory)\PkgAssets\TeamRoles.json</TeamRolesConfig>
  <TeamCspsConfig>$(MSBuildProjectDirectory)\PkgAssets\TeamCsps.json</TeamCspsConfig>
</PropertyGroup>

<!-- Runs JSON validation/initialization before build -->
<Target Name="InitializePackageConfigs" BeforeTargets="Build" DependsOnTargets="ValidatePackageConfigs" />
```

The properties define the paths to the JSON configuration files.

The target runs a task that either creates the config files if they don't exist in given paths (specified by the props) or validates them againts the JSON [schemas](./src/Shared/Schemas).

Feel free to change the paths to the JSON configuration files to your liking but make sure to they are contained in the your package assets folder (default is `PkgAssets`).

#### 2.2. Build the package deployer project 
If you set the props and targets as described above, build the package deployer project locally once and the config files will be created in the given paths.

### 3. Set your configuration

If you skipped the step 2, create manually json files in the package assets folder (default is `PkgAssets`).

If you completed the step 2, the config files already exist.

Set your declarative configuration as in the examples in [Configuration](#configuration).

### 4. Invoke tasks from your PackageImportExtension class
To execute the custom tasks, invoke them in the appropriate stage of the package import process.
```csharp
public override void AfterPrimaryImport()
{
    //GetImportPackageDataFolderName is a property of the default ImportExtension class
    SyncTeamRoles(GetImportPackageDataFolderName + "/TeamRoles.json");
    SyncTeamCsps(GetImportPackageDataFolderName + "/TeamCsps.json");
    return true;
}
```

## Configuration

### Team Security Roles (default file name: `TeamRoles.json`)

Assign security roles to teams declaratively:

```json
[
  {
    "team": "Sales Team",
    "removeUnassigned": false,
    "securityRoles": [
      "00000000-0000-0000-0000-000000000001",
      "00000000-0000-0000-0000-000000000002"
    ]
  },
  {
    "team": "00000000-0000-0000-0000-000000000003",
    "removeUnassigned": true,
    "securityRoles": [
      "00000000-0000-0000-0000-000000000004"
    ]
  }
]
```

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `team` | string | Yes | Team identifier — can be the team **name** or **team ID** (GUID) |
| `securityRoles` | string[] | Yes | Array of security role IDs (GUIDs) to assign |
| `removeUnassigned` | boolean | No | If `true`, removes any security roles from the team that are not in the `securityRoles` array. Default: `false` |

### Team Column Security Profiles (default file name: `TeamCsps.json`)

Assign column security profiles (field-level security) to teams:

```json
[
  {
    "team": "Sales Team",
    "removeUnassigned": false,
    "columnSecurityProfiles": [
      "00000000-0000-0000-0000-000000000001",
      "00000000-0000-0000-0000-000000000002"
    ]
  }
]
```

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `team` | string | Yes | Team identifier — can be the team **name** or **team ID** (GUID) |
| `columnSecurityProfiles` | string[] | Yes | Array of column security profile IDs (GUIDs) to assign |
| `removeUnassigned` | boolean | No | If `true`, removes any column security profiles from the team that are not in the `columnSecurityProfiles` array. Default: `false` |

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built on top of Microsoft's [Package Deployer](https://learn.microsoft.com/en-us/power-platform/alm/package-deployer-tool) framework
- Uses [Newtonsoft.Json.Schema](https://www.newtonsoft.com/jsonschema) for JSON validation
