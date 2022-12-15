using FluentAssertions;
using Microsoft.OData.Client;
using Sandbox.OData.Client;
using SandboxODataClient.Helpers;
using Xunit.Abstractions;

namespace SandboxODataTests.Tests
{
    public class WorkspaceTests
    {
        private readonly ITestOutputHelper _output;

        public WorkspaceTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public async Task GetWorkspacesTest()
        {
            var context = new Context(_output);

            var workspaces = (await context.Workspaces.ExecuteAsync()).ToArray();

            workspaces.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateWorkspaceTest()
        {
            var context = new Context(_output, false);

            Workspace workspace = new();
            context.AddAndTrackEntity(workspace);
            workspace.Code = "abc";
            workspace.Label = "ABC";
            workspace.IsActive = true;

            await context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
        }
    }
}