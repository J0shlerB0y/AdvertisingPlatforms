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
                ������.������:/ru           
                ���������� �������:/ru/svrd/revda,/ru/svrd/pervik           
                ������ ��������� ���������:/ru/msk,/ru/permobl,/ru/chelobl  
                ������ ��������� ���������: / ru / msk ,/ru/permobl,/ru/chelobl  
                ������ �������  :   /ru/svrd
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
        [InlineData("/ru", new[] { "������.������" })]
        [InlineData("/ru/msk ", new[] { "������.������", "������ ��������� ���������" })]
        [InlineData("/ru/svrd", new[] { "������.������", "������ �������" })]
        [InlineData("/ru/svrd/revda", new[] { "������.������", "������ �������", "���������� �������" })]
        [InlineData("/ ru / svrd / revda ", new[] { "������.������", "������ �������", "���������� �������" })]
        [InlineData("/ru/svrd/pervik", new[] { "������.������", "������ �������", "���������� �������" })]
        [InlineData("/ru/permobl", new[] { "������.������", "������ ��������� ���������" })]
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
            Assert.Contains("������.������", result);
            Assert.Contains("������ �������", result);
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