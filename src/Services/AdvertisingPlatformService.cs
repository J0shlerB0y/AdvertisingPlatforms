using AdvertisingPlatforms.Enities;

namespace AdvertisingPlatforms.Services
{
    public class AdvertisingPlatformService : IAdvertisingPlatformService
    {
        private TreeNode root = new TreeNode();

        public async Task LoadPlatformsAsync(Stream stream)
        {
            var newRoot = new TreeNode();

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
                        AddPlatformToTree(newRoot, platformName, location);
                    }
                }
            }
            Interlocked.Exchange(ref root, newRoot);
        }

        private void AddPlatformToTree(TreeNode root, string platformName, string locationPath)
        {
            var currentNode = root;
            var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var segment in segments)
            {
                if (!currentNode.Children.ContainsKey(segment))
                {
                    currentNode.Children[segment] = new TreeNode();
                }
                currentNode = currentNode.Children[segment];
            }

            currentNode.Platforms.Add(platformName);
        }

        public IEnumerable<string> FindPlatformsForLocation(string locationPath)
        {
            var resultPlatforms = new HashSet<string>();
            var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            TreeNode currentNode = root;

            foreach (var segment in segments)
            {
                if (!currentNode.Children.TryGetValue(segment.Trim(), out var childNode))
                {
                    break;
                }
                currentNode = childNode;
                foreach (var platform in currentNode.Platforms)
                {
                    resultPlatforms.Add(platform.Trim());
                }
            }

            return resultPlatforms;
        }
    }
}
