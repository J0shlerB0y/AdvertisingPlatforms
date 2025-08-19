using System.Text;

namespace AdvertisingPlatforms.Services
{
    public interface IAdvertisingPlatformService
    {
        Task LoadPlatformsAsync(Stream stream);
        IEnumerable<string> FindPlatformsForLocation(string locationPath);
    }

    public class AdvertisingPlatformService : IAdvertisingPlatformService
    {
        private Dictionary<string, HashSet<string>> _platformsByLocation = new();

        public async Task LoadPlatformsAsync(Stream stream)
        {
            var newPlatformsByLocation = new Dictionary<string, HashSet<string>>();

            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(':', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (parts.Length != 2) continue;

                    string platformName = parts[0];
                    string[] locations = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    foreach (var location in locations)
                    {
                        AddPlatformToDictionary(newPlatformsByLocation, platformName, location);
                    }
                }
            }
            BuildDictionary(newPlatformsByLocation);
            Interlocked.Exchange(ref _platformsByLocation, newPlatformsByLocation);
        }

        private void AddPlatformToDictionary(Dictionary<string, HashSet<string>> platformsDict, string platformName, string locationPath)
        {
            if (!platformsDict.TryGetValue(locationPath, out var platformsSet))
            {
                platformsSet = new HashSet<string>();
                platformsDict[locationPath] = platformsSet;
            }

            platformsSet.Add(platformName);
        }

        private void BuildDictionary(Dictionary<string, HashSet<string>> platformsDict)
        {
            foreach (KeyValuePair< string, HashSet<string>> adder in platformsDict)
            {
                foreach (KeyValuePair<string, HashSet<string>> added in platformsDict)
                {
                    if (added.Key.StartsWith(adder.Key) && added.Key != adder.Key)
                    {
                        added.Value.UnionWith(adder.Value);
                    }
                }
            }
        }

        public IEnumerable<string> FindPlatformsForLocation(string locationPath)
        {
            var trimmedPath = locationPath.Trim();

            if (string.IsNullOrEmpty(trimmedPath))
            {
                return Enumerable.Empty<string>();
            }

            if (_platformsByLocation.TryGetValue(trimmedPath, out var platforms))
            {
                return platforms;
            }

            return Enumerable.Empty<string>();
        }
    }
}
