using System.Diagnostics;

namespace YoutubeIPv6BlockForVRChat.Utils;

internal static class FirewallUtils
{
    private static readonly ProcessStartInfo _startInfo = new ProcessStartInfo()
    {

        FileName = @"PowerShell.exe",
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };

    private static readonly string CreateCommand = $"New-NetFirewallRule -DisplayName \"{Properties.Resources.AppName}\" -Direction Outbound -Action Block -RemoteAddress \"2404:6800::/32\" -Profile Any -Protocol Any -Enabled True";
    private static readonly string DeleteCommand = $"Remove-NetFirewallRule -DisplayName \"{Properties.Resources.AppName}\"";

    internal static void CreateFirewallRule()
        => ExecutePowerShellCommand(CreateCommand);

    internal static void DeleteFirewallRule()
        => ExecutePowerShellCommand(DeleteCommand);

    private static void ExecutePowerShellCommand(string psCommandWithArgs)
    {
        _startInfo.Arguments = psCommandWithArgs;
        Process.Start(_startInfo);
    }
}
