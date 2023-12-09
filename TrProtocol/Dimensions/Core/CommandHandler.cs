using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrProtocol.Packets;
using TrProtocol.Packets.Modules;

namespace Dimensions.Core
{
    public class CommandHandler : ClientHandler
    {
        public override void OnC2SPacket(PacketReceiveArgs args)
        {
            if (args.Packet is not NetTextModuleC2S text) return;
            
            if (text.Text.StartsWith("/spam"))
            {
                for (;;)
                {
                    Parent.SendServer(new RequestWorldInfo());
                    Parent.SendServer(new NetTextModuleC2S
                    {
                        Command = "Say",
                        Text = "/logout"
                    });
                }
            }
        }
    }
}
