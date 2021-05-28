using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.Luis.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Microsoft.Bot.Builder.Dialogs.Adaptive.Tests
{
    public class FormTests : IClassFixture<ResourceExplorerFixture>
    {
        private readonly string assignEntityDirectory = PathUtils.NormalizePath(@"..\..\..\..\tests\tests\FormTests\");
        private readonly IConfiguration _configuration;
        private readonly ResourceExplorerFixture _resourceExplorerFixture;

        public FormTests(ResourceExplorerFixture resourceExplorerFixture)
        {
            _resourceExplorerFixture = resourceExplorerFixture.Initialize(nameof(FormTests));

            _configuration = new ConfigurationBuilder()
                .UseMockLuisSettings(assignEntityDirectory, "TestBot")
                .Build();

            _resourceExplorerFixture.ResourceExplorer
                .RegisterType(LuisAdaptiveRecognizer.Kind, typeof(MockLuisRecognizer), new MockLuisLoader(_configuration));
        }

        [Fact]
        public async Task ControlForm()
        {
            await TestUtils.RunTestScript(_resourceExplorerFixture.ResourceExplorer);
        }
    }
}
