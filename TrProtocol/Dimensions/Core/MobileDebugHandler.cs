using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrProtocol.Packets.Mobile;
using TrProtocol;

namespace Dimensions.Core
{
    public class MobileDebugHandler : ClientHandler
    {
        public override void OnC2SPacket(PacketReceiveArgs args)
        {
            if (args.Packet is PlayerPlatformInfo packet)
            {
                Logger.Log($"[DEBUG]: PE Client Detected(platform={packet.PlatformId}, playerid={packet.PlayerId})");
            }
        }
    }
}
