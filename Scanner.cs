using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PortScanner.Core
{
    /// <summary>
    /// Performs asynchronous TCP port scanning on a list of IP addresses.
    /// </summary>
    public sealed class Scanner
    {
        private readonly IReadOnlyCollection<IPAddress> ipList;
        private readonly int startPort;
        private readonly int endPort;
        private readonly int maxConcurrency;
        private readonly TimeSpan timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scanner"/> class.
        /// </summary>
        /// <param name="target">Target IP, range, or CIDR notation.</param>
        /// <param name="startPort">Starting port number.</param>
        /// <param name="endPort">Ending port number.</param>
        /// <param name="maxConcurrency">Maximum concurrent connections.</param>
        /// <param name="timeoutSeconds">Connection timeout in seconds (default 1s).</param>
        /// <exception cref="ArgumentException">Thrown when ports are invalid.</exception>
        public Scanner(string target, int startPort, int endPort, int maxConcurrency, int timeoutSeconds = 1)
        {
            if (startPort < 1 || endPort > 65535 || startPort > endPort)
                throw new ArgumentException("Invalid port range.");

            ipList = TargetParser.Parse(target);
            this.startPort = startPort;
            this.endPort = endPort;
            this.maxConcurrency = maxConcurrency;
            timeout = TimeSpan.FromSeconds(timeoutSeconds);
        }

        /// <summary>
        /// Starts the asynchronous scan operation.
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A list of scan results.</returns>
        public async Task<IReadOnlyCollection<ScanResult>> StartScanAsync(CancellationToken cancellationToken = default)
        {
            var results = new ConcurrentBag<ScanResult>();
            var semaphore = new SemaphoreSlim(maxConcurrency);
            var tasks = new List<Task>();

            foreach (var ip in ipList)
            {
                for (int port = startPort; port <= endPort; port++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await semaphore.WaitAsync(cancellationToken);

                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            using var client = new TcpClient();
                            var connectTask = client.ConnectAsync(ip, port);
                            var completedTask = await Task.WhenAny(connectTask, Task.Delay(timeout, cancellationToken));

                            if (completedTask == connectTask && client.Connected)
                            {
                                var banner = await ServiceDetector.GrabBannerAsync(client, cancellationToken);

                                results.Add(new ScanResult(
                                    IP: ip.ToString(),
                                    Port: port,
                                    Status: "Open",
                                    Banner: banner
                                ));
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Graceful cancellation
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"[Scanner] Error scanning {ip}:{port} - {ex.Message}");
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }, cancellationToken));
                }
            }

            await Task.WhenAll(tasks);
            return results.ToArray();
        }
    }
}
