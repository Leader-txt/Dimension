using System.IO;
using Newtonsoft.Json;

namespace Dimension;

public class Config
{
	public bool EnableGeoIP;

	public string HostIP { get; set; } = "127.0.0.1";


	public int HostRestPort { get; set; } = 7880;


	public Rest[] Rests { get; set; } = new Rest[1]
	{
		new Rest()
	};


	public void Write(string path)
	{
		File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
	}

	public static Config Read(string path)
	{
		if (!File.Exists(path))
		{
			WriteTemplates(path);
		}
		return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
	}

	public static void WriteTemplates(string file)
	{
		Config config = new Config();
		config.EnableGeoIP = false;
		config.Write(file);
	}
}
