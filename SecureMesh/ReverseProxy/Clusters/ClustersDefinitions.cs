using Yarp.ReverseProxy.Configuration;

namespace SecureMesh.ReverseProxy.Clusters;

public static class ClustersDefinitions
{
    public static IReadOnlyList<ClusterConfig> GetCluster()
    {
        return new[]
        {
            new ClusterConfig
            {
                ClusterId = "user_cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    // { "user", new DestinationConfig { Address = "https://localhost:4080" } }
                    { "user", new DestinationConfig { Address = "https://user:4080" } }

                }
            },
            new ClusterConfig
            {
                ClusterId = "auth_cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    // { "auth", new DestinationConfig { Address = "https://localhost:5090" } }
                    { "auth", new DestinationConfig { Address = "https://auth:5090" } }
                }
            }
        };
    }
}
