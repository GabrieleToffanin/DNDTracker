using Xunit;

namespace DNDTracker.Main.IntegrationTests.Fixtures;

[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollectionDefinition : ICollectionFixture<MainIntegrationTestsFixture>;