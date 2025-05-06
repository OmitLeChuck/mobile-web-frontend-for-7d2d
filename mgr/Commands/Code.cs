using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using mgr.Tools;

namespace mgr.Commands
{
    public class Code : ConsoleCmdAbstract
    {
        public override string[] getCommands()
        {
            return new string[] { "mgr-code", "code", "gimmecode", "code-for-mobile" };
        }

        public override string getDescription()
        {
            return $"{Get.ServerResponseName()} - Get Code for Mobile-Web-Frontend-Helper";
        }

        public override void Execute(List<string> parameters, CommandSenderInfo senderInfo)
        {
            if (parameters.Count > 0)
            {
                Logger.Client(
                    $"you gimme some parameters ... i dont know what to do with them. But hey no worries you get your code anyway ...");
            }

            string code = Guid.NewGuid().ToString().Split('-').Last().Substring(0, 5).ToUpper();
            File.WriteAllText(Path.Combine(Get.Wd(), $"{code}.code"), Get.PlayerId(senderInfo.RemoteClientInfo));
            Logger.Client($"here is your code ... >>{code}<<");
            Logger.Client(
                $"goto your favourite browser on your device (tablet, phone, pc, ...) and open this url: http://{GamePrefs.GetString(EnumGamePrefs.ServerIP)}:{Get.MgrPort}");
            Logger.Client($"login there with your code and have a little fun.");
            Logger.Client(
                $"PS: you can get your code everytime again, and the old codes are deleted and cant be used by anyone else.");
        }
    }
}