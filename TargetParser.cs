using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PortScanner.Core
{
    /// <summary>
    /// Parses target strings into a list of IP addresses.
    /// Supports single IPs and CIDR ranges (IPv4 only).
    /// </summary>
    public static class TargetParser
    {
        /// <summary>
        /// Parses a string representing a target IP or CIDR block.
        /// </summary>
        /// <param name="target">Target IP (e.g., "192.168.1.1") or CIDR (e.g., "192.168.1.0/24").</param>
        /// <returns>A list of IP addresses.</returns>
        /// <exception cref="FormatException">Thrown if the IP format is invalid.</exception>
        /// <exception cref="ArgumentException">Thrown if the CIDR range is out of bounds.</exception>
        public static List<IPAddress> Parse(string target)
        {
            if (string.IsNullOrWhiteSpace(target))
                throw new ArgumentException("Target string cannot be null or empty.", nameof(target));

            return target.Contains('/')
                ? ParseCidr(target)
                : new List<IPAddress> { ParseSingleIp(target) };
        }

        private static IPAddress ParseSingleIp(string ipStr)
        {
            if (!IPAddress.TryParse(ipStr, out var ip))
                throw new FormatException($"Invalid IP address format: {ipStr}");

            if (ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                throw new NotSupportedException("Only IPv4 addresses are supported.");

            return ip;
        }

        private static List<IPAddress> ParseCidr(string cidrNotation)
        {
            var parts = cidrNotation.Split('/');
            if (parts.Length != 2)
                throw new FormatException($"Invalid CIDR notation: {cidrNotation}");

            var baseIp = ParseSingleIp(parts[0]);
            if (!int.TryParse(parts[1], out int cidr) || cidr < 0 || cidr > 32)
                throw new ArgumentException($"Invalid CIDR value: {parts[1]}");

            uint baseAddress = BitConverter.ToUInt32(baseIp.GetAddressBytes().Reverse().ToArray(), 0);
            int numberOfHosts = 1 << (32 - cidr);

            var list = new List<IPAddress>(numberOfHosts);

            for (int i = 0; i < numberOfHosts; i++)
            {
                uint current = baseAddress + (uint)i;
                byte[] bytes = BitConverter.GetBytes(current).Reverse().ToArray();
                list.Add(new IPAddress(bytes));
            }

            return list;
        }
    }
}
