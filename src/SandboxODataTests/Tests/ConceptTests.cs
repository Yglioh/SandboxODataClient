using FluentAssertions;
using Xunit.Abstractions;

namespace SandboxODataTests.Tests
{
    public class ConceptTests
    {
        private readonly ITestOutputHelper _output;

        public ConceptTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public async Task GetConceptsTest()
        {
            var context = new Context(_output);

            var concepts = (await context.Concepts.ExecuteAsync()).ToArray();

            concepts.Should().NotBeEmpty();
        }
    }
}