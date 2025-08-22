using Yarp.ReverseProxy.Configuration;

namespace YarpGateway.ReverseProxy.Clusters;

public static class ClustersDefinitions
{
    public static IReadOnlyList<ClusterConfig> GetCluster()
    {
        return
        [
            new ClusterConfig
            {
                ClusterId = "ums_cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    // { "ums", new DestinationConfig { Address = "https://localhost:7080" } }
                    { "ums", new DestinationConfig { Address = "https://ums:7080" } }

                }
            },
            new ClusterConfig
            {
                ClusterId = "idp_cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    // { "idp", new DestinationConfig { Address = "https://localhost:5090" } }
                    { "idp", new DestinationConfig { Address = "https://idp:5090" } }
                }
            }
        ];
    }
}
