using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        string configFile = Path.Combine(exeDir, "sjvs.config");

        if (args.Length == 0)
        {
            PrintHelp();
            return;
        }

        string command = args[0].ToLowerInvariant();

        switch (command)
        {
            case "dir":
                SetDirectory(args, configFile);
                break;

            case "list":
                WithJdkDir(configFile, ListJdks);
                break;

            case "use":
                if (args.Length < 2)
                {
                    Console.WriteLine("Uso: sjvs use <version|latest>");
                    return;
                }

                WithJdkDir(configFile, dir => UseJdk(dir, args[1]));
                break;

            case "current":
                ShowCurrent();
                break;

            default:
                PrintHelp();
                break;
        }
    }

    // ---------------- CONFIG ----------------

    static void SetDirectory(string[] args, string configFile)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Uso: sjvs dir <path>");
            return;
        }

        string path = args[1];

        if (!Directory.Exists(path))
        {
            Console.WriteLine("El directorio no existe.");
            return;
        }

        File.WriteAllText(configFile, path);

        Console.WriteLine("Directorio de JDKs configurado en:");
        Console.WriteLine(path);
    }

    static string? GetConfiguredDir(string configFile)
    {
        if (!File.Exists(configFile))
            return null;

        return File.ReadAllText(configFile).Trim();
    }

    static void WithJdkDir(string configFile, Action<string> action)
    {
        var dir = GetConfiguredDir(configFile);

        if (string.IsNullOrWhiteSpace(dir))
        {
            Console.WriteLine("No hay directorio configurado.");
            Console.WriteLine("Usa: sjvs dir <path>");
            return;
        }

        if (!Directory.Exists(dir))
        {
            Console.WriteLine($"El directorio configurado no existe: {dir}");
            return;
        }

        action(dir);
    }

    // ---------------- COMMANDS ----------------

    static void ListJdks(string sourceDir)
    {
        var jdks = GetJdks(sourceDir);

        if (jdks.Length == 0)
        {
            Console.WriteLine("No hay JDKs disponibles.");
            return;
        }

        Console.WriteLine("JDKs disponibles:");
        foreach (var j in jdks)
            Console.WriteLine(" - " + j.Name);
    }

    static void UseJdk(string sourceDir, string input)
    {
        var jdks = GetJdks(sourceDir);

        if (jdks.Length == 0)
        {
            Console.WriteLine("No hay JDKs disponibles.");
            return;
        }

        string? selected;

        if (input.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            selected = jdks
                .OrderByDescending(j => j.Version)
                .First().Path;
        }
        else
        {
            selected = jdks
                .FirstOrDefault(j =>
                    j.Name.Equals(input, StringComparison.OrdinalIgnoreCase) ||
                    j.Name.Contains(input, StringComparison.OrdinalIgnoreCase)
                )?.Path;
        }

        if (selected == null)
        {
            Console.WriteLine("No se encontró la versión.");
            return;
        }

        Environment.SetEnvironmentVariable(
            "JAVA_HOME",
            selected,
            EnvironmentVariableTarget.User
        );

        Console.WriteLine("JAVA_HOME actualizado a:");
        Console.WriteLine(selected);
    }

    static void ShowCurrent()
    {
        var value = Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.User);

        Console.WriteLine(string.IsNullOrWhiteSpace(value)
            ? "JAVA_HOME no está definido."
            : $"JAVA_HOME actual:\n{value}");
    }

    static void PrintHelp()
    {
        Console.WriteLine("Uso:");
        Console.WriteLine("  sjvs dir <path>");
        Console.WriteLine("  sjvs list");
        Console.WriteLine("  sjvs use <version|latest>");
        Console.WriteLine("  sjvs current");
    }

    // ---------------- CORE ----------------

    static (string Name, string Path, Version Version)[] GetJdks(string dir)
    {
        return Directory.GetDirectories(dir)
            .Select(d =>
            {
                string name = Path.GetFileName(d);
                return (name, d, ParseVersion(name));
            })
            .ToArray();
    }

    static Version ParseVersion(string folderName)
    {
        var match = Regex.Match(folderName, @"(\d+)(\.\d+)?(\.\d+)?");

        if (!match.Success)
            return new Version(0, 0);

        return Version.TryParse(match.Value, out var v)
            ? v
            : new Version(int.Parse(match.Groups[1].Value), 0);
    }
}
