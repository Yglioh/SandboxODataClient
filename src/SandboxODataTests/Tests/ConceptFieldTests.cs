using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.OData.Client;
using Sandbox.OData.Client;
using SandboxODataClient.Helpers;
using System.Reflection.Emit;
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

        [Fact]
        public async Task CreateConceptStandardFieldTest()
        {
            var context = new Context(_output, false);

            ConceptField conceptField = new();
            context.AddAndTrackEntity(conceptField);
            conceptField.ConceptId = 1;
            conceptField.FieldDefinitionId = "SF23";

            await context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
        }

        [Fact]
        public async Task CreateConceptStandardFieldUsingNavigationTest()
        {
            var context = new Context(_output, false);

            ConceptField conceptField = new();
            context.AddAndTrackEntity(conceptField);

            conceptField.Concept = new Concept { Id = 1 };
            conceptField.FieldDefinition = new FieldDefinition { Id = "SF23" };

            context.AddRelatedObject(conceptField, nameof(ConceptField.Concept), conceptField.Concept);
            context.AddRelatedObject(conceptField, nameof(ConceptField.FieldDefinition), conceptField.FieldDefinition);

            await context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
        }

        [Fact]
        public async Task CreateDefinitionAndConceptFieldTest()
        {
            var context = new Context(_output, true);

            var concept = new Concept { Id = 1 };
            context.AttachEntity(concept);

            TextFieldDefinition definition = new();
            context.AddAndTrackEntity(definition);
            definition.Code = "TextCustomField5";
            definition.Label = "Text field (custom 5)";
            definition.IsActive = true;
            
            ConceptField field = new();
            context.AddAndTrackEntity(field);
            field.Concept = concept;
            field.FieldDefinition = definition;

            await context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
        }

        [Fact(Skip = "Resulting change order is wrong")]
        public async Task CreateWithDeepInsert1Test()
        {
            var context = new Context(_output, false);

            var concept = new Concept { Id = 1 };
            context.AttachEntity(concept);

            ConceptField field = new();
            context.AddAndTrackEntity(field);

            field.Concept = concept;
            field.FieldDefinition = new TextFieldDefinition();
            field.FieldDefinition.Code = "TextCustomField2";
            field.FieldDefinition.Label = "Text field (custom 2)";
            field.FieldDefinition.IsActive = true;

            await context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
        }

        [Fact]
        public async Task CreateWithDeepInsert2Test()
        {
            var context = new Context(_output, false);

            var concept = new Concept { Id = 1 };
            context.AttachEntity(concept);

            ConceptField field = new();
            context.AddAndTrackEntity(field);

            field.Concept = concept;
            field.FieldDefinition = new TextFieldDefinition();
            field.FieldDefinition.Code = "TextCustomField3";
            field.FieldDefinition.Label = "Text field (custom 3)";
            field.FieldDefinition.IsActive = true;

            context.Detach(field);
            context.AddEntity(field);
            context.SetLink(field, nameof(ConceptField.Concept), concept);
            context.SetLink(field, nameof(ConceptField.FieldDefinition), field.FieldDefinition);

            await context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
        }

        [Fact]
        public async Task CreateWithDeepInsert3Test()
        {
            var context = new Context(_output, false);
            context.SaveChangesDefaultOptions = SaveChangesOptions.DeepInsertEntityProperties;

            var concept = new Concept { Id = 1 };
            context.AttachEntity(concept);

            ConceptField field = new();
            context.AddAndTrackEntity(field);
            field.Concept = concept;
            field.FieldDefinition = new TextFieldDefinition();
            field.FieldDefinition.Code = "TextCustomField4";
            field.FieldDefinition.Label = "Text field (custom 4)";
            field.FieldDefinition.IsActive = true;

            await context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.DeepInsertEntityProperties);
        }
    }
}