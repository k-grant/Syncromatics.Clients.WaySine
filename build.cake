#addin nuget:?package=Cake.Git
using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/Syncromatics.Clients.WaySine/bin") + Directory(configuration);
var testDir = Directory("./tests/Syncromatics.Clients.WaySine.IntegrationTests/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
var version = "";
var semVersion = "";
Task("GetVersion")
    .Does(() =>
    {
        var repositoryPath = Directory(".");
        var travisTag = EnvironmentVariable("TRAVIS_TAG");
        if (!string.IsNullOrEmpty(travisTag))
        {
            Information($"GetVersion: Current tag is {travisTag}");
            version = semVersion = travisTag;
        }
        else
        {
            var travisPullRequest = EnvironmentVariable("TRAVIS_PULL_REQUEST");
            var travisBranch = EnvironmentVariable("TRAVIS_BRANCH");
            var branch = !string.IsNullOrEmpty(travisPullRequest) && travisPullRequest != "false"
                ? $"pr-{travisPullRequest}-{EnvironmentVariable("TRAVIS_PULL_REQUEST_BRANCH")}"
                : !string.IsNullOrEmpty(travisBranch)
                    ? travisBranch
                    : GitBranchCurrent(repositoryPath).FriendlyName;

            Information($"GetVersion: Current branch is {branch}");
            var prereleaseTag = Regex.Replace(branch, @"\W+", "-");
            var describe = GitDescribe(repositoryPath, GitDescribeStrategy.Tags);
            var isMaster = prereleaseTag == "master" || prereleaseTag == "-no-branch-";
            version = string.Join(".", describe.Split(new[] { '-' }, 3).Take(2));
            semVersion = version + (isMaster ? "" : $"-{prereleaseTag}");
        }
        Information($"Version: {semVersion}");
    });

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(buildDir);
        CleanDirectory(testDir);
    });

Task("Build")
    .IsDependentOn("InnerTest");

Task("Package")
    .IsDependentOn("InnerPackage");

Task("Publish")
    .IsDependentOn("GetVersion")
    .IsDependentOn("Package")
    .Does(() =>
    {
        var package = $"./Syncromatics.Clients.WaySine.{semVersion}.nupkg";

        NuGetPush(package, new NuGetPushSettings
        {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = EnvironmentVariable("NUGET_API_KEY")
        });
    });

Task("InnerRestore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

Task("InnerBuild")
    .IsDependentOn("InnerRestore")
    .Does(() =>
    {
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
        };
        DotNetCoreBuild("./", settings);
    });

Task("InnerTest")
    .IsDependentOn("InnerBuild")
    .Does(() =>
    {
        DotNetCoreTool("./tests/Syncromatics.Clients.WaySine.IntegrationTests/Syncromatics.Clients.WaySine.IntegrationTests.csproj", "xunit");
    });

Task("InnerPackage")
    .IsDependentOn("GetVersion")
    .IsDependentOn("InnerTest")
    .Does(() =>
    {
        var packageSettings = new DotNetCorePackSettings
        {
            Configuration =  configuration,
            OutputDirectory = "./",
            ArgumentCustomization = args => args.Append($"/p:Version={semVersion}")
        };

        DotNetCorePack(File("./src/Syncromatics.Clients.WaySine/Syncromatics.Clients.WaySine.csproj"), packageSettings);
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
