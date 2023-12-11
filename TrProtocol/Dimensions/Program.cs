
using System.Net;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Dimensions.Models;
using Newtonsoft.Json;

namespace Dimensions;

public static class Program
{
    public static Config config;

    public static readonly string MotdPath = "motd.txt";

    static Program()
    {
        config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"))!;
    }

    public static void Main(string[] args)
    {
        if(!File.Exists(MotdPath))
        {
            File.WriteAllText(MotdPath,"Welcome to our server!");
        }
        new Task(() =>
        {
            var listener = new Listener(new(IPAddress.Any, config.listenPort));
            listener.ListenThread();
        }).Start();
        for(; ; )
        {
            var cmd = Console.ReadLine();
            if(cmd == "reload")
            {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"))!;
                Console.WriteLine($"Successfully reload configuration , {config.servers.Length} servers loaded.");
            }
        }
    }
}