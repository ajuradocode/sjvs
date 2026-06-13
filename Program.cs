using System.Text.RegularExpressions;
using System.Reflection;

class Program
{
    private static string GetVersion() =>
        typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "unknown";

    static void Main(string[] args)
    {
        string exeDir = AppContext.BaseDirectory;
        string configFile = Path.Combine(exeDir, "sjvs.config");

        if (args.Length == 0)
        {
            PrintWelcome();
            return;
        }

        // Handle global options
        if (args.Length > 0)
        {
            if (args[0].Equals("--version", StringComparison.OrdinalIgnoreCase) ||
                args[0].Equals("-v", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"{GetVersion()}");
                return;
            }

            if (args[0].Equals("--help", StringComparison.OrdinalIgnoreCase) ||
                args[0].Equals("-h", StringComparison.OrdinalIgnoreCase))
            {
                PrintHelp();
                return;
            }
        }

        // Handle commands
        string command = args[0].ToLowerInvariant();

        switch (command)
        {
            case "dir":
                SetDir(args, configFile);
                break;

            case "list":
                WithJdks(configFile, ListJdks);
                break;

            case "use":
                WithJdks(configFile, jdks => UseJdk(jdks, args));
                break;

            case "current":
                ShowCurrent();
                break;

            default:
                PrintHelp();
                break;
        }
    }

    // ================ CONFIG ================

    static void SetDir(string[] args, string configFile)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: sjvs dir <path>");
            return;
        }

        string path = args[1];

        if (!Directory.Exists(path))
        {
            Console.WriteLine("Directory does not exist.");
            return;
        }

        File.WriteAllText(configFile, path);

        Console.WriteLine("JDKs directory configured: " + path);
    }

    static string? GetDir(string configFile)
    {
        if (!File.Exists(configFile))
            return null;

        return File.ReadAllText(configFile).Trim();
    }

    static void WithJdks(string configFile, Action<Jdk[]> action)
    {
        var dir = GetDir(configFile);

        if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
        {
            Console.WriteLine("Invalid configured JDKs directory. Use: sjvs dir <path>");
            return;
        }

        var jdks = LoadJdks(dir);

        action(jdks);
    }

    // ================ COMMANDS ================

    static void ListJdks(Jdk[] jdks)
    {
        if (jdks.Length == 0)
        {
            Console.WriteLine("No JDKs found.");
            return;
        }

        foreach (var j in jdks.OrderByDescending(x => x.Version))
            Console.WriteLine($" - {j.Name}");
    }

    static void UseJdk(Jdk[] jdks, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: sjvs use <version>");
            return;
        }

        string input = args[1];

        if (jdks.Length == 0)
        {
            Console.WriteLine("No JDKs found.");
            return;
        }

        Jdk? selected = null;

        if (input.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            selected = jdks.OrderByDescending(j => j.Version).First();
        }
        else
        {
            selected =
                jdks.FirstOrDefault(j =>
                    j.Name.Equals(input, StringComparison.OrdinalIgnoreCase)) ??
                jdks.FirstOrDefault(j =>
                    j.Name.Contains(input, StringComparison.OrdinalIgnoreCase));
        }

        if (selected == null)
        {
            Console.WriteLine("Version not found.");
            return;
        }

        Environment.SetEnvironmentVariable(
            "JAVA_HOME",
            selected.Path,
            EnvironmentVariableTarget.Process
        );

        Environment.SetEnvironmentVariable(
            "JAVA_HOME",
            selected.Path,
            EnvironmentVariableTarget.User
        );

        Console.WriteLine("JAVA_HOME updated to: " + selected.Path);
    }

    static void ShowCurrent()
    {
        var value = Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.User);

        Console.WriteLine(value);
    }

    static void PrintWelcome()
    {
        Console.WriteLine("NAME:");
        Console.WriteLine($"   sjvs - JDK Version Manager for Windows - {GetVersion()}");
        Console.WriteLine();
        Console.WriteLine("USAGE:");
        Console.WriteLine("   sjvs.exe [global options] command [command options] [arguments...]");
        Console.WriteLine();
        Console.WriteLine("GLOBAL OPTIONS:");
        Console.WriteLine("   --help, -h     Show help");
        Console.WriteLine("   --version, -v  Print the version");
    }


    static void PrintHelp()
    {
        Console.WriteLine("COMMANDS:");
        Console.WriteLine("     dir              Configure the JDK directory");
        Console.WriteLine("     list             List available JDK installations");
        Console.WriteLine("     use              Switch to use the specified version. Can be a full version, partial version, or 'latest', to search for the latest version available.");
        Console.WriteLine("     current          Show the current JAVA_HOME");
        Console.WriteLine();
    }

    // ================ CORE ================

    static Jdk[] LoadJdks(string dir)
    {
        return Directory.GetDirectories(dir)
            .Select(d =>
            {
                string name = Path.GetFileName(d);
                return new Jdk
                {
                    Name = name,
                    Path = d,
                    Version = ParseVersion(name)
                };
            })
            .ToArray();
    }

    static Version ParseVersion(string name)
    {
        var match = Regex.Match(name, @"(\d+)(\.\d+)?(\.\d+)?");

        if (!match.Success)
            return new Version(0, 0);

        return Version.TryParse(match.Value, out var v)
            ? v
            : new Version(int.Parse(match.Groups[1].Value), 0);
    }
}
