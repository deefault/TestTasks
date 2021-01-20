using System.Globalization;
using Xunit;

namespace Localizer.Tests
{
    public class JsonResourceProviderTests
    {
        private const string EnUs = "en-US";
        private const string RuRU = "ru-RU";
        private const string ResourceName = "res";

        [Fact]
        public void Should_Load_When_Resource_Exists()
        {
            CultureInfo.CurrentCulture = new CultureInfo(EnUs);
            var jsonResourceProvider = new JsonResourcesProvider(ResourceName);
            
            var actual = jsonResourceProvider.LoadResourcesForCulture(CultureInfo.CurrentCulture);
            
            Assert.True(actual);
        }
        
        [Fact]
        public void Should_not_Load_When_Resource_Localization_not_Exists()
        {
            CultureInfo.CurrentCulture = new CultureInfo("fr-CA");
            var jsonResourceProvider = new JsonResourcesProvider(ResourceName);
            
            var actual = jsonResourceProvider.LoadResourcesForCulture(CultureInfo.CurrentCulture);
            
            Assert.False(actual);
        }
        
        [Theory]
        [InlineData("Hello", EnUs)]
        [InlineData("Привет", RuRU)]
        public void Should_Get_String_When_Get_Called_And_Key_Exists(string value, string culture)
        {
            var jsonResourceProvider = new JsonResourcesProvider(ResourceName);
            
            var res = jsonResourceProvider.Get("Hello", new CultureInfo(culture));

            Assert.NotNull(res);
            Assert.Equal(value, res);
        }
        
        [Fact]
        public void Should_return_Null_When_Get_Called_And_Key_Not_Exists()
        {
            CultureInfo.CurrentCulture = new CultureInfo(EnUs);
            var jsonResourceProvider = new JsonResourcesProvider(ResourceName);
            
            var res = jsonResourceProvider.Get("NotExistsInEn", CultureInfo.CurrentCulture);

            Assert.Null(res);
        }
    }
}