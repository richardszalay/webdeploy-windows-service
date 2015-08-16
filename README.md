Proof of concept extension to Microsoft Web Deploy (MSDeploy) that supports the deployment of Windows Services.

Short term goals:

* Update services on a remote server by name (not file path)
* Automatically stop/start the service
* Integrate with permission delegation in IIS

Longer term goals:

* Support remote installation/uninstallation of services

## Installation

Download a release and copy the DLL to `%programfiles%\IIS\Microsoft Web Deploy V3\Extensibility` (create the `Extensibility` directory if required). Then you can use it like any other provider:

```
msdeploy -verb:sync ^
         -source:servicePath=MyService ^
         -dest:servicePath=MyService,serviceTimeout=60,computerName=...
```

### Server Configuration

TODO: Information on how to continue non-Administrator deployments


## Usage

The `servicePath` provider works very similarly to `contentPath` in that it can either be an absolute file path or the name of a service.

### Settings

`servicePath` supports the following settings:

Setting       |Type  |Description
--------------|------|-----------
serviceTimeout|int   |Number of seconds given for a service to start or top

### Rules

Setting       |Enabled by default?  |Description
--------------|------|-----------
StopService|yes   |Stops the remote service before deploying
StartService|yes   |Starts the remote service after deploying
SkipServiceState|yes|Skips service state files (.InstallLog, .InstallState) to prevent later issues when uninstalling

## Build

Build requires Visual Studio 2015 as it makes use of some C# 6 features

The extension currently needs to be built against Web Deploy 3.5, as 3.6 introduced a strange dual-dependency between .NET 3.5 and 4.0 that causes havok in Visual Studio. If you already have Web Deploy 3.6 installed, you can extract Web Deploy 3.5 using the following command:

```
msiexec /a WebDeploy_amd64_en-US.msi /qb TARGETDIR=c:\WebDeploy35
```

And then add the following into your `RichardSzalay.Web.Deployment.WindowsService.csproj.user` file

```
<PropertyGroup>
  <MSDeployAssemblyPath>c:\WebDeploy35\x64\IIS\Microsoft Web Deploy V3</MSDeployAssemblyPath>
</PropertyGroup>
```

Debugging against the installed version will still work fine.