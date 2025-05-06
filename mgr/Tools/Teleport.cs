using System;
using UnityEngine;

namespace mgr.Tools
{
    public class Teleport
    {
        public static bool Player(Vector3 position, ClientInfo cInfo)
        {
            try
            {
                cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>()
                    .Setup(position, null, false));
                return true;
            } catch (Exception)
            {
                return false;
            }
        }
    }
}