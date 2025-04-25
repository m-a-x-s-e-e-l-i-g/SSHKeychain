using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace SSHKeychain;

internal sealed partial class SSHKeychainPage : ListPage
{
    private static readonly string SshConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh", "config");

    public SSHKeychainPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "SSH Keychain";
        Name = "Open";
    }

    public override IListItem[] GetItems()
    {
        var sshHosts = GetSshHosts();

        var items = sshHosts.Select(hostInfo =>
            new ListItem(new LaunchSshCommand(hostInfo.Host, hostInfo.User, hostInfo.Port, hostInfo.IdentityFile))
            {
                Title = hostInfo.Host
            }).ToList();

        return items.ToArray();
    }

    private static (string Host, string HostName, string User, string Port, string IdentityFile)[] GetSshHosts()
    {
        if (!File.Exists(SshConfigPath))
        {
            return Array.Empty<(string, string, string, string, string)>();
        }

        var lines = File.ReadAllLines(SshConfigPath);
        var hosts = new List<(string Host, string HostName, string User, string Port, string IdentityFile)>();

        string? currentHost = null;
        string? hostName = null;
        string? user = null;
        string? port = null;
        string? identityFile = null;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("Host ", StringComparison.OrdinalIgnoreCase))
            {
                if (currentHost != null)
                {
                    hosts.Add((currentHost, hostName ?? string.Empty, user ?? string.Empty, port ?? string.Empty, identityFile ?? string.Empty));
                }

                currentHost = trimmedLine.Substring(5).Trim();
                hostName = null;
                user = null;
                port = null;
                identityFile = null;
            }
            else if (currentHost != null)
            {
                if (trimmedLine.StartsWith("HostName ", StringComparison.OrdinalIgnoreCase))
                {
                    hostName = trimmedLine.Substring(9).Trim();
                }
                else if (trimmedLine.StartsWith("User ", StringComparison.OrdinalIgnoreCase))
                {
                    user = trimmedLine.Substring(5).Trim();
                }
                else if (trimmedLine.StartsWith("Port ", StringComparison.OrdinalIgnoreCase))
                {
                    port = trimmedLine.Substring(5).Trim();
                }
                else if (trimmedLine.StartsWith("IdentityFile ", StringComparison.OrdinalIgnoreCase))
                {
                    identityFile = trimmedLine.Substring(13).Trim();
                }
            }
        }

        if (currentHost != null)
        {
            hosts.Add((currentHost, hostName ?? string.Empty, user ?? string.Empty, port ?? string.Empty, identityFile ?? string.Empty));
        }

        return hosts.ToArray();
    }
}

internal partial class LaunchSshCommand : Microsoft.CommandPalette.Extensions.Toolkit.InvokableCommand
{
    public LaunchSshCommand(string hostName, string user, string port, string identityFile)
    {
        Name = $"SSH to {hostName}"; // A short name for the command
        Icon = new("\uE756"); // CommandPrompt from Segoe
        _hostName = hostName;
        _user = user;
        _port = port;
        _identityFile = identityFile;
    }

    private readonly string _hostName;
    private readonly string _user;
    private readonly string _port;
    private readonly string _identityFile;

    // Open an SSH session in the terminal
    public override CommandResult Invoke()
    {
        var sshCommand = $"ssh {_hostName}";

        if (!string.IsNullOrEmpty(_user))
        {
            sshCommand += $" -l {_user}";
        }

        if (!string.IsNullOrEmpty(_port))
        {
            sshCommand += $" -p {_port}";
        }

        if (!string.IsNullOrEmpty(_identityFile))
        {
            sshCommand += $" -i \"{_identityFile}\"";
        }

        try
        {
            // Launch Windows Terminal with the SSH command
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "wt.exe",
                Arguments = sshCommand,
                UseShellExecute = true,
                CreateNoWindow = false
            };

            Debug.WriteLine($"Starting terminal with command: {sshCommand}");
            Process.Start(processStartInfo);
            
            // Return success and hide the palette
            return CommandResult.Dismiss();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to start terminal: {ex.Message}");
            // Show an error message to the user
            return CommandResult.ShowToast($"Failed to start SSH session: {ex.Message}");
        }
    }
}
