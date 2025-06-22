# Arpscanner
ArpScanner is an advanced network scanning tool written in C# with a focus on experimental security and enterprise network analysis. It combines TCP Port Scanning techniques with ARP Scanning device detection, providing a detailed view of the network infrastructure in IPv4 environments.




---

##Features

- ARP scanning to detect active hosts on local networks.
- TCP port scanning with banner detection.
- Support for IP ranges in CIDR notation (e.g. `192.168.0.0/24`).
- Concurrency control via `SemaphoreSlim`.
- Export of results in formatted JSON.
---

## Compilation

requirements:
- [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

to compile:

```
dotnet build

To execute:

dotnet run --project ./src/portScanner-arpScanner.csproj -- \
    --target 192.168.1.0/24\
    --start-port 20\
    --end-port 1024\
    --threads 100\
    --output results.json

```



Parameters
Parameter Description
--target Single IP or CIDR block (e.g. 192.168.0.0/24)
--start-port Starting port to scan
--end-port Ending port to scan
--threads Maximum number of concurrent operations
--output Path to save the report in JSON format

  # Project Structure
      portScanner-arpScanner/
       ├── src/
       │ ├── Program.cs
       │ ├── Scanner.cs
       │ ├── ServiceDetector.cs
       │ ├── TargetParser.cs
       │ ├── ReportWriter.cs
       │ └── Models/
       │ └── ScanResult.cs
       ├── README.md
       ├── portScanner-arpScanner.csproj

Output

The scan result will be saved in JSON format to the specified path with the following template:

[
{
"IP": "192.168.1.10",
"Port": 22,
"Status": "Open",
"Banner": "OpenSSH 8.2"
},
{
"IP": "192.168.1.15",
"Port": 80,
"Status": "Open",
"Banner": "Apache/2.4.29"
}
]

License

This project is licensed under the MIT License. See the LICENSE file for more details.

---

If you want, I can adapt this `README` to Markdown with GitHub Pages support, generate a `Makef
