using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortScanner.Core
{
    /// <summary>
    /// Provides methods for detecting service banners from open TCP ports.
    /// </summary>
    public static class ServiceDetector
    {
        private const int DefaultReadBufferSize = 1024;
        private const int DefaultReadTimeoutMilliseconds = 2000;

        /// <summary>
        /// Attempts to read the banner from a connected TCP client.
        /// </summary>
        /// <param name="client">An already connected TcpClient instance.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the read operation.</param>
        /// <returns>The banner as a string if available; otherwise, an empty string.</returns>
        public static async Task<string> GrabBannerAsync(TcpClient client, CancellationToken cancellationToken = default)
        {
            if (client is null || !client.Connected)
                return string.Empty;

            try
            {
                using var stream = client.GetStream();

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(DefaultReadTimeoutMilliseconds);

                byte[] buffer = new byte[DefaultReadBufferSize];
                int bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cts.Token);

                if (bytesRead > 0)
                {
                    return Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                }
            }
            catch (OperationCanceledException)
            {
                // Timeout or external cancellation — silently ignore
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ServiceDetector] Failed to grab banner: {ex.Message}");
            }

            return string.Empty;
        }
    }
}
