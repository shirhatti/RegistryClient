# Registry Client

Registry Client is a client library targetting .NET Standard 2.0 that provides an easy way to interact with the [Docker Registry HTTP API V2](https://docs.docker.com/registry/spec/api/)

## Usage examples

Get list of tags for a repository

```c#
var repo = "microsoft/dotnet";
var dockerHubRegistry = new Registry();
var tags = await dockerHubRegistry.GetTagsAsync(repo);
foreach(var tag in tags)
{
    Console.WriteLine(tag);
}
```