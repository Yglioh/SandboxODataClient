using Microsoft.OData.Client;
using Sandbox.OData.Client;
using SandboxODataClient.Helpers;

var context = new Container(new Uri("http://localhost:5001/"))
{
    MergeOption = MergeOption.NoTracking, // Needed to have nested expand properties filled in
};

Workspace workspace = new();
context.AddAndTrackEntity(workspace);
workspace.Code = "abc";
workspace.Label = "ABC";
workspace.IsActive = true;

await context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
