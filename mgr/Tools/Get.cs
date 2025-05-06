using System;
using System.Collections.ObjectModel;
using System.IO;

namespace mgr.Tools
{
    public class Get
    {
        // -- port for the api / web-frontend
        public static int MgrPort = 0;

        // -- getting the name for the server in between the log
        public static string ServerResponseName()
        {
            string name = "mgr";
            return $"[{name}] ";
        }

        // -- getting player from entity-id
        public static EntityPlayer Player(int id)
        {
            GameManager.Instance.World.Players.dict.TryGetValue(id, out EntityPlayer entityPlayer);
            return entityPlayer;
        }

        // -- getting player-id
        public static string PlayerId(ClientInfo cInfo)
        {
            return cInfo.CrossplatformId.CombinedString;
        }

        // -- getting the root path from the game
        private static string Root()
        {
            return GameIO.GetGamePath();
        }

        // -- getting the path for this mod
        public static string Wd()
        {
            return Path.Combine(Root(), "Mods", "mgr");
        }

        // -- getting the path for the web / api
        public static string WebRoot()
        {
            return Path.Combine(Wd(), "web");
        }

        // -- getting ClientInfo from a string
        public static ClientInfo GetClientInfoFromWhatWeHaveGot(string wwhg)
        {
            foreach (ClientInfo cInfo in ConnectionManager.Instance.Clients.List)
            {
                if (cInfo != null)
                {
                    if (cInfo.CrossplatformId.CombinedString == wwhg ||
                        cInfo.playerName == wwhg ||
                        cInfo.entityId.ToString() == wwhg ||
                        cInfo.CrossplatformId.PlatformIdentifierString == wwhg)
                    {
                        return cInfo;
                    }
                }
            }
            return null;
        }
    }
}