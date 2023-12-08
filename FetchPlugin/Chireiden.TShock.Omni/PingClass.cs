using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using OTAPI;
using Terraria;
using TShockAPI;

namespace Chireiden.TShock.Omni;

public static class PingClass
{
	public static async Task<TimeSpan> Ping(TSPlayer player, CancellationToken token = default(CancellationToken))
	{
		TimeSpan result = TimeSpan.MaxValue;
		int inv = -1;
		for (int i = 0; i < Main.item.Length; i++)
		{
			if (!Main.item[i].active || Main.item[i].playerIndexTheItemIsReservedFor == 255)
			{
				inv = i;
				break;
			}
		}
		if (inv == -1)
		{
			return result;
		}
		DateTime start = DateTime.Now;
		Channel<int> channel = Channel.CreateBounded<int>(new BoundedChannelOptions(30)
		{
			SingleReader = true,
			SingleWriter = true
		});
		player.SetData("chireiden.data.pingchannel1", channel);
		NetMessage.TrySendData(39, -1, -1, null, inv);
		while (!token.IsCancellationRequested)
		{
			try
			{
				if (await channel.Reader.ReadAsync(token) == inv)
				{
					result = DateTime.Now - start;
					break;
				}
			}
			catch (OperationCanceledException)
			{
			}
		}
		player.SetData<Channel<int>>("chireiden.data.pingchannel1", null);
		return result;
	}

	public static void Hook_Ping_GetData(object? sender, Hooks.MessageBuffer.GetDataEventArgs args)
	{
		if (args.PacketId == 22 && args.Instance.readBuffer[args.ReadOffset + 2] == byte.MaxValue)
		{
			int whoAmI = args.Instance.whoAmI;
			Channel<int> channel = TShockAPI.TShock.Players[whoAmI]?.GetData<Channel<int>>("chireiden.data.pingchannel1");
			if (channel != null)
			{
				short item = BitConverter.ToInt16(args.Instance.readBuffer.AsSpan(args.ReadOffset, 2));
				channel.Writer.TryWrite(item);
			}
		}
	}

	public static async Task<string> PingPlayer(TSPlayer plr)
	{
		try
		{
			double totalMilliseconds = (await Ping(plr, new CancellationTokenSource(3000).Token)).TotalMilliseconds;
			string result;
			if (totalMilliseconds >= 200.0)
			{
				result = $"[c/FF0000:{totalMilliseconds:F1}ms]";
			}
			else
			{
				double num = totalMilliseconds;
				if (num > 80.0 && num < 200.0)
				{
					result = $"[c/FFA500:{num:F1}ms]";
				}
				else
				{
					double num2 = totalMilliseconds;
					if (!(num2 <= 80.0))
					{
						throw new SwitchExpressionException();
					}
					result = $"[c/00FF00:{num2:F1}ms]";
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			TShockAPI.TShock.Log.Error(ex.ToString());
			return "[c/FF0000:不可用]";
		}
	}
}
