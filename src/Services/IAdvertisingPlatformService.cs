namespace AdvertisingPlatforms.Services
{
    public interface IAdvertisingPlatformService
    {
        Task LoadPlatformsAsync(Stream stream);

        IEnumerable<string> FindPlatformsForLocation(string locationPath);
    }
}
