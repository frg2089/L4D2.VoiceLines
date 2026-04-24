using System.Collections.Frozen;
using System.Collections.Immutable;

using Betalgo.Ranul.OpenAI.Extensions;

using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 26100))
    throw new PlatformNotSupportedException("Only Supported Windows 11 24H2 or later.");

string? installedPath = null;
if (args.Length is 0)
{
    installedPath = Registry.LocalMachine
        .OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 550")
        ?.GetValue("InstallLocation")
        as string;
}
else
{
    installedPath = args[0];
}

if (!Directory.Exists(installedPath))
    throw new DirectoryNotFoundException("Cannot found the Game folder.");

var audioClient = await Whisper.InitializeAsync();

var dist = Path.Combine(AppContext.BaseDirectory, "dist");
if (Directory.Exists(dist))
    Directory.Delete(dist, true);
Directory.CreateDirectory(dist);

ImmutableArray<string> survivors = ["coach", "gambler", "mechanic", "producer", "biker", "manager", "namvet", "teengirl"];

var voiceFolders = Directory
    .EnumerateDirectories(installedPath, "left4dead2*")
    .SelectMany(baseDir => survivors
        .Select(survivor => Path.Combine(baseDir, "sound", "player", "survivor", "voice", survivor)))
    .Where(Directory.Exists)
    .GroupBy(i => Path.GetFileName(i))
    .ToImmutableDictionary(i => i.Key, i => i.ToImmutableArray());

foreach (var item in voiceFolders)
{
    var path = Path.Combine(dist, $"{item.Key}.index");
    await using var writer = File.CreateText(path);

    foreach (var audio in item.Value.SelectMany(Directory.EnumerateFiles))
    {
        var response = await audioClient.TranscribeAudioAsync(audio, CancellationToken.None);

        var name = Path.GetFileName(audio);
        var text = response.Text;

        Console.Write($"{name}|");
        Console.WriteLine($"{text}");
        await writer.WriteLineAsync($"{name}|{text}");
    }
}
