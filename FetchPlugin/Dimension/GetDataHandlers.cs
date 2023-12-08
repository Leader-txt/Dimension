using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Net;
using System.Reflection;
using MaxMind;
using TShockAPI;

namespace Dimension;

public class GetDataHandlers
{
	private static Dictionary<PacketTypes, GetDataHandlerDelegate> _getDataHandlerDelegates;

	private readonly Dimensions Dimensions;

	public GetDataHandlers(Dimensions Dimensions)
	{
		this.Dimensions = Dimensions;
		_getDataHandlerDelegates = new Dictionary<PacketTypes, GetDataHandlerDelegate> { 
		{
			PacketTypes.Placeholder,
			HandleDimensionsMessage
		} };
	}

	public bool HandlerGetData(PacketTypes type, TSPlayer player, MemoryStream data)
	{
		if (_getDataHandlerDelegates.TryGetValue(type, out GetDataHandlerDelegate value))
		{
			try
			{
				return value(new GetDataHandlerArgs(player, data));
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
		}
		return false;
	}

	private bool HandleDimensionsMessage(GetDataHandlerArgs args)
	{
		if (args.Player == null)
		{
			return false;
		}
		_ = args.Player.Index;
		short num = args.Data.ReadInt16();
		string remoteAddress = args.Data.ReadString();
		bool result = false;
		if (num == 0)
		{
			result = HandleIpInformation(remoteAddress, args.Player);
		}
		return result;
	}

	private bool HandleIpInformation(string remoteAddress, TSPlayer player)
	{
		typeof(TSPlayer).GetField("CacheIP", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(player, remoteAddress);
		if (Dimensions.Geo != null)
		{
			string text = Dimensions.Geo.TryGetCountryCode(IPAddress.Parse(remoteAddress));
			player.Country = ((text == null) ? "N/A" : GeoIPCountry.GetCountryNameByCode(text));
			if (text == "A1" && TShock.Config.Settings.KickProxyUsers)
			{
				player.Kick("Proxies are not allowed.", force: true, silent: true);
				return false;
			}
		}
		return true;
	}
}
