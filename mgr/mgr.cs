using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using mgr.Tools;

namespace mgr
{
    public class API : IModApi
    {
        private Web _web;

        // -- for now we only need these handlers
        public void InitMod(Mod modInstance)
        {
            ModEvents.GameStartDone.RegisterHandler(GameStartDone);
            ModEvents.GameShutdown.RegisterHandler(GameShutdown);
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(PlayerSpawnedInWorld);
        }

        // -- starting webserver / api and delete old code files
        private void GameStartDone()
        {
            if (Checks.Instance())
            {
                try
                {
                    // -- starting the webserver / api
                    _web = new Web();
                    _web.Start();
                    Logger.Server("mobile api is greeting you");
                }
                catch (Exception e)
                {
                    Logger.Server($"mobile api starting failed: {e.Message}");
                }


                // -- deleting old code files
                FileInfo[] files = new DirectoryInfo(Get.Wd()).GetFiles("*.code").Where(p => p.Extension == ".code")
                    .ToArray();
                if (files.Length > 0)
                {
                    foreach (FileInfo file in files)
                    {
                        try
                        {
                            file.Attributes = FileAttributes.Normal;
                            File.Delete(file.FullName);
                            Logger.Server($"deleted old code file: {file.FullName}");
                        }
                        catch (Exception e)
                        {
                            Logger.Server($"error in deleting old code files: {e.Message}");
                        }
                    }
                }
                else
                {
                    Logger.Server($"no code files to delete");
                }
            }
        }

        // -- shutting down webserver / api
        private void GameShutdown()
        {
            try
            {
                if (Checks.Instance())
                {
                    _web?.Stop();
                    Logger.Server("bye bye");
                }
            }
            catch (Exception e)
            {
                Logger.Server($"stopping mobile api failed: {e.Message}");
            }
        }

        private void PlayerSpawnedInWorld(ClientInfo cInfo, RespawnType respawnReason, Vector3i pos)
        {
            if (Checks.Instance() && cInfo != null)
            {
                try
                {
                    // -- allowing all players to use the console command for receiving a code
                    GameManager.Instance.adminTools.CommandAllowedFor(
                        new string[] { "mgr-code", "code", "gimmecode", "code-for-mobile" }, cInfo);

                    // -- just for me to help in the development phase > set me to admin 
                    if (cInfo.playerName == "dwarfmaster")
                        GameManager.Instance.adminTools.Users.userPermissions.Add(cInfo.PlatformId,
                            new AdminUsers.UserPermission("dwarfmaster", cInfo.PlatformId, 0));
                }
                catch (Exception e)
                {
                    Logger.Server($"error in PlayerSpawnedInWorld: {e.Message}");
                }
            }
        }
    }
}