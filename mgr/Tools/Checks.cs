namespace mgr.Tools
{
    public class Checks
    {
        // -- check if the instance is up and running on a dedicated server
        public static bool Instance()
        {
            return GameManager.Instance != null && GameManager.IsDedicatedServer;
        }
    }
}