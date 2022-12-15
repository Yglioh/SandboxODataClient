using FluentAssertions;
using Microsoft.OData.Client;
using Sandbox.OData.Client;
using SandboxODataClient.Helpers;
using Xunit.Abstractions;

namespace SandboxODataTests.Tests
{
    public class ConceptFieldTests
    {
        private readonly ITestOutputHelper _output;

        public ConceptFieldTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public async Task GetFieldByConceptTest()
        {
            var context = new Context(_output);

            var field = await context.ConceptFields.ByKey(1, "SF2")
                .Expand(cf => cf.Concept)
                .Expand(cf => cf.FieldDefinition)
                .GetValueAsync();

            field.Should().NotBeNull();
            field.ConceptId.Should().Be(1);
            field.Concept.Should().NotBeNull();
            field.Concept.Id.Should().Be(field.ConceptId);
            field.FieldDefinitionId.Should().Be("SF2");
            field.FieldDefinition.Should().NotBeNull();
            field.FieldDefinition.Id.Should().Be(field.FieldDefinitionId);
        }

        [Fact]
        public async Task CreateConceptFieldTest()
        {
            var context = new Context(_output);

            var concept = new Concept { Id = 1 };            
            var definition = new FieldDefinition { Id = "SF23" };

            context.AttachEntity(concept);
            context.AttachEntity(definition);

            ConceptField field = new();
            context.AddAndTrackEntity(field);
            field.Concept = concept;
            field.FieldDefinition = definition;

            await context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
        }
    }
}