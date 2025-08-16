using AdvertisingPlatforms.Services;
using System.Text;
using Xunit;
namespace Tests
{
    public class AdvertisingPlatformServiceTests
    {
        private readonly AdvertisingPlatformService _service;

        public AdvertisingPlatformServiceTests()
        {
            _service = new AdvertisingPlatformService();

            var fileContent = @"
                Яндекс.Директ:/ru           
                Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik           
                Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl  
                Газета уральских москвичей: / ru / msk ,/ru/permobl,/ru/chelobl  
                Крутая реклама  :   /ru/svrd
                MalformedLineWithoutColon   
                :EmptyPlatform
                        EmptyLocation:/
                        /:/
                :/rar
                :rar
                rar:
                ";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            _service.LoadPlatformsAsync(stream).Wait(); 
        }

        [Theory]
        [InlineData("/ru", new[] { "Яндекс.Директ" })]
        [InlineData("/ru/msk ", new[] { "Яндекс.Директ", "Газета уральских москвичей" })]
        [InlineData("/ru/svrd", new[] { "Яндекс.Директ", "Крутая реклама" })]
        [InlineData("/ru/svrd/revda", new[] { "Яндекс.Директ", "Крутая реклама", "Ревдинский рабочий" })]
        [InlineData("/ ru / svrd / revda ", new[] { "Яндекс.Директ", "Крутая реклама", "Ревдинский рабочий" })]
        [InlineData("/ru/svrd/pervik", new[] { "Яндекс.Директ", "Крутая реклама", "Ревдинский рабочий" })]
        [InlineData("/ru/permobl", new[] { "Яндекс.Директ", "Газета уральских москвичей" })]
        public void FindPlatformsForLocation_ShouldReturnCorrectPlatforms(string location, string[] expectedPlatforms)
        {
            // Act
            var result = _service.FindPlatformsForLocation(location).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPlatforms.Length, result.Count);
            foreach (var platform in expectedPlatforms)
            {
                Assert.Contains(platform, result);
            }
        }

        [Fact]
        public void FindPlatformsForLocation_NonExistentLocation_ShouldReturnParentPlatforms()
        {
            // Arrange
            var location = "/ru/svrd/ekb";

            // Act
            var result = _service.FindPlatformsForLocation(location).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains("Яндекс.Директ", result);
            Assert.Contains("Крутая реклама", result);
        }

        [Fact]
        public void FindPlatformsForLocation_EmptyLocation_ShouldReturnNull()
        {
            // Act
            var result = _service.FindPlatformsForLocation("/").ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FindPlatformsForLocation_CompletelyUnknownPath_ShouldReturnEmpty()
        {
            // Act
            var result = _service.FindPlatformsForLocation("/usa/ca/la").ToList();

            // Assert
            Assert.Empty(result);
        }
    }
}