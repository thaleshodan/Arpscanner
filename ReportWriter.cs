using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReportGeneration
{
    /// <summary>
    /// Responsible for writing scan reports to files.
    /// </summary>
    public static class ReportWriter
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Asynchronously saves scan results to a JSON file.
        /// </summary>
        /// <param name="filePath">The destination file path.</param>
        /// <param name="results">The list of scan results.</param>
        public static async Task SaveToJsonAsync(string filePath, IReadOnlyCollection<ScanResult> results)
        {
            try
            {
                string json = JsonSerializer.Serialize(results, JsonOptions);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[Error] Failed to write JSON report: {ex.Message}");
                // Consider logging to a file or using a logging framework
            }
        }
    }

    /// <summary>
    /// Represents the result of a port scan on a specific IP address.
    /// </summary>
    public sealed record ScanResult(
        string IP,
        int Port,
        string Status,
        string? Banner
    );
}
