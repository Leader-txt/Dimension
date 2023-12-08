using System;
using System.IO;
using TShockAPI;

namespace Dimension;

internal class GetDataHandlerArgs : EventArgs
{
	public TSPlayer Player { get; private set; }

	public MemoryStream Data { get; private set; }

	public GetDataHandlerArgs(TSPlayer player, MemoryStream data)
	{
		Player = player;
		Data = data;
	}
}
