using System;
using System.Threading;

using MCGalaxy;
using MCGalaxy.Tasks;

namespace MCGalaxy
{
    public class Compass : Plugin
    {
        public override string creator { get { return "Nelyon"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.8"; } }
        public override string name { get { return "Compass"; } }

        public static SchedulerTask task;

        public override void Load(bool startup)
        {
            task = Server.MainScheduler.QueueRepeat(CheckDirection, null, TimeSpan.FromMilliseconds(25));
        }

        void CheckDirection(SchedulerTask task)
        {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players)
            {
                if (!p.Supports(CpeExt.MessageTypes)) continue;

                int yaw = Orientation.PackedToDegrees(p.Rot.RotY);

                // If value is the same, don't bother sending status packets to the client
                if (p.Extras.GetInt("COMPASS_VALUE") == yaw) continue;

                // Store yaw in extras values so we can retrieve it above
                p.Extras["COMPASS_VALUE"] = yaw;

                p.SendCpeMessage(CpeMessageType.Status2, "%9You are currently facing:");
                p.SendCpeMessage(CpeMessageType.Status3, yaw + "°");
            }
        }

        public override void Unload(bool shutdown)
        {
            Server.MainScheduler.Cancel(task);
        }
    }
}
