namespace mgr.Tools
{
    public class Logger
    {
        // -- log to server console
        public static void Server(string message) =>
            Log.Out($"{Get.ServerResponseName()}{message}");
        
        // -- log to the client console
        public static void Client(string message) =>
            SdtdConsole.Instance.Output($"{Get.ServerResponseName()}{message}");
    }
}