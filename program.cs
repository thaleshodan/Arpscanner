using System;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
    static async Task Main(string[] args)
    {
        var config = ParseArguments(args);

        if (config.ShowHelp)
        {
            ShowHelp();
            return;
        }

        try
        {
            Console.WriteLine($"Iniciando varredura em {config.Target} ({config.StartPort}-{config.EndPort}) com {config.MaxThreads} threads...");

            var scanner = new Scanner(config.Target, config.StartPort, config.EndPort, config.MaxThreads);
            var results = await scanner.StartScanAsync();

            ReportWriter.SaveToJson(config.OutputFile, results);

            Console.WriteLine($"\n[+] Varredura finalizada. Resultados salvos em: {config.OutputFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] {ex.Message}");
        }
    }

    static ScanConfig ParseArguments(string[] args)
    {
        var config = new ScanConfig();

        if (args.Length == 0 || Array.Exists(args, a => a == "--help" || a == "-h"))
        {
            config.ShowHelp = true;
            return config;
        }

        var argsDict = new Dictionary<string, string>();
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i].StartsWith("-"))
                argsDict[args[i]] = args[i + 1];
        }

        config.Target = argsDict.TryGetValue("-target", out var t) ? t : "127.0.0.1";
        if (argsDict.TryGetValue("-ports", out var p))
        {
            var range = p.Split('-');
            config.StartPort = int.Parse(range[0]);
            config.EndPort = int.Parse(range[1]);
        }

        if (argsDict.TryGetValue("-threads", out var th))
            config.MaxThreads = int.Parse(th);

        if (argsDict.TryGetValue("-output", out var o))
            config.OutputFile = o;

        config.EnableSSL = Array.Exists(args, a => a == "--ssl");
        config.EnableOSDetect = Array.Exists(args, a => a == "--os");
        config.EnableUDP = Array.Exists(args, a => a == "--udp");

        return config;
    }

    static void ShowHelp()
    {
        Console.WriteLine(@"
ArpScanner -target <ip/cidr> -ports <start-end> [options]

Obligated Options:
  -target      IP our Network (ex: 192.168.0.1 ou 10.0.0.0/24)
  -ports       Ports ranges (ex: 1-1000)

Optional Options:
  -threads     Number of concurrent threads (default: 100)
  -output      Output file path JSON (default: report.json)
  --udp        Active scan of UDP ports
  --ssl        Collects information about SSL/TLS (443, 8443, etc)
  --os         Active OS detection
  --help       Show this help
");
    }
}

class ScanConfig
{
    public string Target { get; set; } = "127.0.0.1";
    public int StartPort { get; set; } = 1;
    public int EndPort { get; set; } = 1024;
    public int MaxThreads { get; set; } = 100;
    public string OutputFile { get; set; } = "report.json";

    public bool EnableSSL { get; set; } = false;
    public bool EnableOSDetect { get; set; } = false;
    public bool EnableUDP { get; set; } = false;
    public bool ShowHelp { get; set; } = false;
}