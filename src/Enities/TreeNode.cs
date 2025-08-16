namespace AdvertisingPlatforms.Enities
{
    public class TreeNode
    {
        public Dictionary<string, TreeNode> Children { get; } = new();

        public HashSet<string> Platforms { get; } = new();
    }
}
