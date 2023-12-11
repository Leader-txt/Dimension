using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chireiden.TShock.Omni;
using Google.Protobuf.WellKnownTypes;
using LazyUtils;
using MaxMind;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OTAPI;
using PChrome.Core;
using StatusTxtMgr;
using Terraria;
using Terraria.Net;
using Terraria.Net.Sockets;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Configuration;
using TShockAPI.DB;

namespace Dimension;

[ApiVersion(2, 1)]
public class Dimensions : TerrariaPlugin
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<TSPlayer, Task> _003C_003E9__18_3;

		public static ThreadStart _003C_003E9__18_0;

		public static Predicate<TSPlayer> _003C_003E9__18_4;

		public static SocketSendCallback _003C_003E9__25_0;

		internal void _003CInitialize_003Eb__18_0()
		{
			while (true)
			{
				Task.WaitAll(((IEnumerable<TSPlayer>)TShock.Players).Select((Func<TSPlayer, Task>)async delegate(TSPlayer i)
				{
					_ = 1;
					try
					{
						if (i != null && i.Active)
						{
							string[] pings = Dimensions.pings;
							int index = i.Index;
							pings[index] = await PingClass.PingPlayer(i);
						}
						else
						{
							await Task.Delay(3000);
						}
					}
					catch (Exception value)
					{
						TShock.Log.ConsoleError($"PingException {value}");
					}
				}).ToArray());
			}
		}

		internal async Task _003CInitialize_003Eb__18_3(TSPlayer i)
		{
			_ = 1;
			try
			{
				if (i != null && i.Active)
				{
					string[] pings = Dimensions.pings;
					int index = i.Index;
					pings[index] = await PingClass.PingPlayer(i);
				}
				else
				{
					await Task.Delay(3000);
				}
			}
			catch (Exception value)
			{
				TShock.Log.ConsoleError($"PingException {value}");
			}
		}

		internal bool _003CInitialize_003Eb__18_4(TSPlayer pl)
		{
			if (pl == null)
			{
				return false;
			}
			return pl.Active;
		}

		internal void _003Cserver_003Eb__25_0(object _003Cp0_003E)
		{
		}
	}

	public GeoIPCountry Geo;

	public static Config Config;

	private readonly string path = Path.Combine(TShock.SavePath, "Dimensions.json");

	public static Dictionary<string, StringBuilder> status;

	private string online;

    public static string[] OnlinePlayers;

    public override string Author => "popstarfreas";

	public override string Description => "Adds more Dimensions to Terraria Travel";

	public override string Name => "Dimensions";

	public override Version Version => new Version(1, 5, 0);

	public static string[] pings { get; set; }

	public Dimensions(Main game)
		: base(game)
	{
		((TerrariaPlugin)this).Order = 1;
	}

	public override void Initialize()
	{
		ServerApi.Hooks.NetGetData.Register((TerrariaPlugin)(object)this, (HookHandler<GetDataEventArgs>)GetData);
		string text = "Dimensions-GeoIP.dat";
		if (!File.Exists(path))
		{
			Dimension.Config.WriteTemplates(path);
		}
		Config = Dimension.Config.Read(path);
		if (Config.EnableGeoIP && File.Exists(text))
		{
			Geo = new GeoIPCountry(text);
		}
		TShockAPI.Commands.ChatCommands.Add(new Command("", new CommandDelegate(server), "server","turn"));
		TShockAPI.Commands.ChatCommands.Add(new Command("", new CommandDelegate(listPlayers), new string[] { "list","player" }));
		TShockAPI.Commands.ChatCommands.Add(new Command("", new CommandDelegate(advtp), new string[1] { "advtp" }));
		TShockAPI.Commands.ChatCommands.Add(new Command("", new CommandDelegate(advwarp), new string[1] { "advwarp" }));
		TShockAPI.Commands.ChatCommands.Add(new Command("", ol, "ol"));
		Hooks.MessageBuffer.GetData += PingClass.Hook_Ping_GetData;
		ServerApi.Hooks.GameInitialize.Register((TerrariaPlugin)(object)this, (HookHandler<EventArgs>)OnGameInvitatize);
		ServerApi.Hooks.ServerLeave.Register((TerrariaPlugin)(object)this, (HookHandler<LeaveEventArgs>)OnServerLeave);
		ServerApi.Hooks.ServerJoin.Register(this, OnServerJoin);
		new Thread((ThreadStart)delegate
		{
			while (true)
			{
				Task.WaitAll(((IEnumerable<TSPlayer>)TShock.Players).Select((Func<TSPlayer, Task>)async delegate(TSPlayer i)
				{
					_ = 1;
					try
					{
						if (i != null && i.Active)
						{
							string[] array = pings;
							int index = i.Index;
							array[index] = await PingClass.PingPlayer(i);
						}
						else
						{
							await Task.Delay(3000);
						}
					}
					catch (Exception value)
					{
						TShock.Log.ConsoleError($"PingException {value}");
					}
				}).ToArray());
			}
		}).Start();
		new Thread((ThreadStart)delegate
		{
			while (true)
			{
				StringBuilder stringBuilder5 = new StringBuilder();
				stringBuilder5.AppendLine($"[c/DAFF66:当][c/EDFF66:前][c/FFFE66:全][c/FFEC66:服][c/FFD966:在][c/FFC666:线:][c/C8FF66:{OnlinePlayers?.Length.ToString() ?? ""}]/[c/FF7866:9999]");
				StringBuilder stringBuilder6 = stringBuilder5;
				StringBuilder stringBuilder7 = stringBuilder6;
				StringBuilder.AppendInterpolatedStringHandler handler2 = new StringBuilder.AppendInterpolatedStringHandler(60, 1, stringBuilder6);
				handler2.AppendLiteral("[c/66FF94:当][c/71FF66:前][c/ABFF66:世][c/E4FF66:界]:[c/FFAE66:");
				handler2.AppendFormatted(Main.worldName);
				handler2.AppendLiteral("]");
				stringBuilder7.AppendLine(ref handler2);
				stringBuilder6 = stringBuilder5;
				StringBuilder stringBuilder8 = stringBuilder6;
				handler2 = new StringBuilder.AppendInterpolatedStringHandler(97, 2, stringBuilder6);
				handler2.AppendLiteral("[c/FFD766:当][c/FFBE66:前][c/FFA466:世][c/FF8B66:界][c/FF7166:在][c/FF6674:线]: [c/C8FF66:");
				handler2.AppendFormatted(TShock.Players.ToList().FindAll((TSPlayer pl) => pl != null && pl.Active).Count);
				handler2.AppendLiteral("]/[c/FF7866:");
				handler2.AppendFormatted(((ConfigFile<TShockSettings>)(object)TShock.Config).Settings.MaxSlots);
				handler2.AppendLiteral("]");
				stringBuilder8.AppendLine(ref handler2);
				online = stringBuilder5.ToString();
				Thread.Sleep(1000);
			}
		}).Start();
		global::StatusTxtMgr.StatusTxtMgr.Hooks.StatusTextUpdate.Register((StatusTextUpdateDelegate)delegate(StatusTextUpdateEventArgs args)
		{
			TSPlayer tsplayer = args.tsplayer;
			StringBuilder statusTextBuilder = args.statusTextBuilder;
			statusTextBuilder.Append(online);
			StringBuilder stringBuilder = statusTextBuilder;
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(49, 3, stringBuilder);
			handler.AppendLiteral("[c/66FFF6:主][c/66FFDC:城][c/66FFC3:等][c/66FFA9:级]:");
			handler.AppendFormatted(tsplayer.Group.Prefix);
			handler.AppendFormatted(tsplayer.Group.Name);
			handler.AppendFormatted(tsplayer.Group.Suffix);
			stringBuilder2.AppendLine(ref handler);
			if (!string.IsNullOrWhiteSpace(pings[tsplayer.Index]))
			{
				stringBuilder = statusTextBuilder;
				StringBuilder stringBuilder3 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(97, 1, stringBuilder);
				handler.AppendLiteral("[c/FFC566:P][c/FF8B66:i][c/FF667A:n][c/FF66B3:g][c/FF66ED:(][c/D866FF:延][c/9F66FF:迟][c/6667FF:)]:");
				handler.AppendFormatted(pings[tsplayer.Index]);
				stringBuilder3.AppendLine(ref handler);
			}
			if (tsplayer.Account != (UserAccount)null)
			{
				DisposableQuery<Money> val = Db.Get<Money>(tsplayer, (string)null);
				try
				{
					stringBuilder = statusTextBuilder;
					StringBuilder stringBuilder4 = stringBuilder;
					handler = new StringBuilder.AppendInterpolatedStringHandler(36, 1, stringBuilder);
					handler.AppendLiteral("[c/FFE766:经][c/FFC266:济]:[c/66FFFC:");
					handler.AppendFormatted(((IQueryable<Money>)val).Single().money);
					handler.AppendLiteral("]");
					stringBuilder4.Append(ref handler);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			foreach (StringBuilder value2 in status.Values)
			{
				statusTextBuilder.AppendLine(value2.ToString());
			}
		}, 60uL);
	}

    private void ol(CommandArgs args)
    {
		Utils.GetOnline();
		args.Player.SendInfoMessage($"当前总在线:{OnlinePlayers.Length}人");
    }

    private void OnServerJoin(JoinEventArgs args)
    {
		Utils.GetOnline(args.Who);
    }

    private void OnServerLeave(LeaveEventArgs args)
	{
		var players = TShock.Players.Where(x => x?.Active ?? false);
        if (players.Any())
		{
			Utils.GetOnline(players.First().Index);
		}
	}

	private void OnGameInvitatize(EventArgs args)
	{
	}

	private void advwarp(CommandArgs args)
	{
		TShockAPI.Commands.HandleCommand(TSPlayer.FindByNameOrID(args.Parameters[0])[0], "/warp " + args.Parameters[1]);
	}

	private void advtp(CommandArgs args)
	{
		TSPlayer obj = TSPlayer.FindByNameOrID(args.Parameters[0])[0];
		int num = int.Parse(args.Parameters[1]);
		TShockAPI.Commands.HandleCommand(obj, string.Format("/tppos {0} {1}", arg1: int.Parse(args.Parameters[2]), arg0: num));
	}

	private void listPlayers(CommandArgs args)
	{
		Utils.GetOnline();
		if (args.Parameters.Count == 0)
		{
			args.Player.SendInfoMessage("指令用法:/player 页码");
			return;
		}
		int page = int.Parse(args.Parameters[0]);
		int maxPage = OnlinePlayers.Length / 20 + (OnlinePlayers.Length % 20 == 0 ? 0 : 1);
		page = Math.Min(page, maxPage);
		args.Player.SendInfoMessage($"页码{page}/{maxPage}");
		args.Player.SendInfoMessage("当前在线" + string.Join("\n", OnlinePlayers.Skip((page - 1) * 20).Take(20)));
	}

	private void server(CommandArgs args)
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		CommandArgs args2 = args;
		if (args2.Parameters.Count == 1)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((short)0);
				binaryWriter.Write((byte)67);
				binaryWriter.Write((short)2);
				binaryWriter.Write(args2.Parameters[0]);
				binaryWriter.BaseStream.Position = 0L;
				binaryWriter.Write((short)memoryStream.ToArray().Length);
			}
			ISocket socket = Netplay.Clients[args2.Player.Index].Socket;
			byte[] array = memoryStream.ToArray();
			int num = memoryStream.ToArray().Length;
			object obj = _003C_003Ec._003C_003E9__25_0;
			if (obj == null)
			{
				SocketSendCallback val = delegate
				{
				};
				_003C_003Ec._003C_003E9__25_0 = val;
				obj = (object)val;
			}
			socket.AsyncSend(array, 0, num, (SocketSendCallback)obj, (object)null);
			new Task(delegate
			{
				Rest[] rests = Dimension.Config.Read(path).Rests;
				foreach (Rest rest in rests)
				{
					if (rest.Name == args2.Parameters[0])
					{
						new HttpClient().GetAsync($"http://{rest.IP}:{rest.Port}/RestLogin/Add?Player={args2.Player.Name}");
						break;
					}
				}
			}).Start();
		}
		else
		{
			TShockAPI.Commands.HandleCommand(TSPlayer.FindByNameOrID(args2.Parameters[1])[0], "/server " + args2.Parameters[0]);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			Hooks.MessageBuffer.GetData -= PingClass.Hook_Ping_GetData;
		}
		((TerrariaPlugin)this).Dispose();
	}

	private void Reload(CommandArgs e)
	{
		string file = Path.Combine(TShock.SavePath, "Dimensions.json");
		if (!File.Exists(file))
		{
			Dimension.Config.WriteTemplates(file);
		}
		Config = Dimension.Config.Read(file);
		e.Player.SendSuccessMessage("Reloaded Dimensions config.");
	}

	private void GetData(GetDataEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		if ((int)args.MsgID != 67)
		{
			return;
		}
		using MemoryStream input = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
		using BinaryReader binaryReader = new BinaryReader(input);
		if (binaryReader.ReadInt16() == 1)
		{
			string[] array = binaryReader.ReadString().Split(':');
			TcpAddress val = (TcpAddress)Netplay.Clients[args.Msg.whoAmI].Socket.GetRemoteAddress();
			val.Address = IPAddress.Parse(array[0]);
			TSPlayer val2 = TShock.Players[args.Msg.whoAmI];
			((object)val2).GetType().GetField("CacheIP", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(val2, val.Address.ToString());
			TShock.Log.ConsoleInfo($"remote address of client #{args.Msg.whoAmI} set to {((RemoteAddress)val).GetFriendlyName()}");
		}
	}

	static Dimensions()
	{
		pings = new string[255];
		Config = new Config();
		status = new Dictionary<string, StringBuilder>();
	}
}
