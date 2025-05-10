using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Interfaz para todos los comandos de terminal.
/// </summary>
public interface ITerminalCommand
{
    string Name { get; }
    string Description { get; }
    void Execute(string[] args, TerminalContext ctx);
}

/// <summary>
/// Contexto compartido para comandos: UI, estado de ruta, historial, etc.
/// </summary>
public class TerminalContext
{
    public TextMeshProUGUI HistoryText;
    public TMP_InputField InputField;
    public string BasePath;
    public string CurrentPath;
    public CommandProcessor Processor { get; set; }
    public List<string> History { get; } = new List<string>();

    public TerminalContext(TextMeshProUGUI historyText, TMP_InputField inputField)
    {
        HistoryText = historyText;
        InputField = inputField;
        // Ajuste: apuntar la raíz a persistentDataPath/ubuntu_sim
        BasePath = Path.Combine(Application.persistentDataPath, "ubuntu_sim");
        CurrentPath = "/";
    }

    public string GetAbsolutePath(string path)
    {
        string p = path.StartsWith("/") ? path : Path.Combine(CurrentPath, path);
        string combined = Path.Combine(BasePath, p.TrimStart('/'));
        return Path.GetFullPath(combined);
    }

    public void Print(string message)
    {
        HistoryText.text += "\n" + message;
    }

    public void Clear()
    {
        HistoryText.text = string.Empty;
    }

    public string GetHelp()
    {
        return Processor != null ? Processor.GetHelp() : string.Empty;
    }
}

/// <summary>
/// Procesa líneas y despacha a cada comando registrado.
/// </summary>
public class CommandProcessor
{
    private readonly Dictionary<string, ITerminalCommand> _commands;
    public TerminalContext Context { get; }

    public CommandProcessor(TerminalContext ctx, IEnumerable<ITerminalCommand> commands)
    {
        Context = ctx;
        _commands = commands.ToDictionary(c => c.Name);
    }

    public void Process(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        var parts = input.Split(' ');
        var cmdName = parts[0];
        var args = parts.Skip(1).ToArray();

        Context.Print($"> {input}");

        if (_commands.TryGetValue(cmdName, out var cmd))
        {
            try { cmd.Execute(args, Context); }
            catch (Exception e) { Context.Print($"Error en {cmdName}: {e.Message}"); }
        }
        else
        {
            Context.Print("Comando no reconocido.");
        }
    }

    public string GetHelp()
    {
        return string.Join("\n", _commands.Values.Select(c => $"{c.Name}: {c.Description}"));
    }
}

// ---------- Implementaciones de comandos ----------

public class CdCommand : ITerminalCommand
{
    public string Name => "cd";
    public string Description => "Cambia el directorio. Uso: cd <ruta>";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1) { ctx.Print("Uso: cd <ruta>"); return; }
        var target = args[0];
        string newPath;
        if (target == ".") newPath = ctx.CurrentPath;
        else if (target == "..") newPath = UpOne(ctx.CurrentPath);
        else if (target.StartsWith("/")) newPath = Normalize(target);
        else newPath = Normalize(Path.Combine(ctx.CurrentPath, target));

        var abs = ctx.GetAbsolutePath(newPath);
        if (Directory.Exists(abs))
        {
            ctx.CurrentPath = newPath;
            ctx.Print($"Directorio actual: {ctx.CurrentPath}");
        }
        else
        {
            ctx.Print($"No existe el directorio: {newPath}");
        }
    }

    private string UpOne(string path)
    {
        var parts = path.TrimEnd('/').Split('/');
        if (parts.Length <= 1) return "/";
        return "/" + string.Join("/", parts, 1, parts.Length - 1);
    }

    private string Normalize(string path)
    {
        var p = path.Replace("\\", "/").Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        return "/" + string.Join("/", p);
    }
}

public class LsCommand : ITerminalCommand
{
    public string Name => "ls";
    public string Description => "Lista archivos y carpetas.";

    public void Execute(string[] args, TerminalContext ctx)
    {
        var abs = ctx.GetAbsolutePath(ctx.CurrentPath);
        try
        {
            var dirs = Directory.GetDirectories(abs).Select(d => Path.GetFileName(d) + "/");
            var files = Directory.GetFiles(abs).Select(f => Path.GetFileName(f));
            ctx.Print(string.Join("   ", dirs.Concat(files)));
        }
        catch
        {
            ctx.Print("Error al listar.");
        }
    }
}

public class PwdCommand : ITerminalCommand
{
    public string Name => "pwd";
    public string Description => "Muestra el directorio actual.";
    public void Execute(string[] args, TerminalContext ctx) => ctx.Print(ctx.CurrentPath);
}

public class MkdirCommand : ITerminalCommand
{
    public string Name => "mkdir";
    public string Description => "Crea un directorio. Uso: mkdir <nombre>";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1) { ctx.Print("Uso: mkdir <nombre>"); return; }
        var path = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[0]));
        try { Directory.CreateDirectory(path); ctx.Print("Directorio creado."); }
        catch (Exception e) { ctx.Print($"Error: {e.Message}"); }
    }
}

public class RmdirCommand : ITerminalCommand
{
    public string Name => "rmdir";
    public string Description => "Elimina un directorio vacío. Uso: rmdir <nombre>";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1) { ctx.Print("Uso: rmdir <nombre>"); return; }
        var path = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[0]));
        try { Directory.Delete(path); ctx.Print("Directorio eliminado."); }
        catch (Exception e) { ctx.Print($"Error: {e.Message}"); }
    }
}

public class TouchCommand : ITerminalCommand
{
    public string Name => "touch";
    public string Description => "Crea un archivo vacío. Uso: touch <nombre>";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1) { ctx.Print("Uso: touch <nombre>"); return; }
        var path = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[0]));
        try { using (File.Create(path)) { } ctx.Print("Archivo creado."); }
        catch (Exception e) { ctx.Print($"Error: {e.Message}"); }
    }
}

public class RmCommand : ITerminalCommand
{
    public string Name => "rm";
    public string Description => "Elimina un archivo. Uso: rm <nombre>";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1) { ctx.Print("Uso: rm <nombre>"); return; }
        var path = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[0]));
        try { File.Delete(path); ctx.Print("Archivo eliminado."); }
        catch (Exception e) { ctx.Print($"Error: {e.Message}"); }
    }
}

public class CpCommand : ITerminalCommand
{
    public string Name => "cp";
    public string Description => "Copia archivo. Uso: cp <origen> <destino>";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 2) { ctx.Print("Uso: cp <origen> <destino>"); return; }
        var src = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[0]));
        var dst = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[1]));
        try { File.Copy(src, dst, true); ctx.Print("Copia realizada."); }
        catch (Exception e) { ctx.Print($"Error: {e.Message}"); }
    }
}

public class MvCommand : ITerminalCommand
{
    public string Name => "mv";
    public string Description => "Mueve/renombra archivo. Uso: mv <origen> <destino>";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 2) { ctx.Print("Uso: mv <origen> <destino>"); return; }
        var src = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[0]));
        var dst = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[1]));
        try { File.Move(src, dst); ctx.Print("Movimiento realizado."); }
        catch (Exception e) { ctx.Print($"Error: {e.Message}"); }
    }
}

public class CatCommand : ITerminalCommand
{
    public string Name => "cat";
    public string Description => "Muestra contenido de archivo. Uso: cat <archivo>";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1) { ctx.Print("Uso: cat <archivo>"); return; }
        var path = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[0]));
        try { ctx.Print(File.ReadAllText(path)); }
        catch (Exception e) { ctx.Print($"Error: {e.Message}"); }
    }
}

public class EchoCommand : ITerminalCommand
{
    public string Name => "echo";
    public string Description => "Imprime texto o redirige. Uso: echo <texto> [> archivo]";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1) { ctx.Print("Uso: echo <texto> [> archivo]"); return; }
        var parts = string.Join(" ", args);
        if (parts.Contains("> "))
        {
            var split = parts.Split(new[] { ">" }, 2, StringSplitOptions.None);
            var text = split[0].Trim();
            var file = split[1].Trim();
            var path = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, file));
            File.WriteAllText(path, text);
            ctx.Print($"Escrito en {file}");
        }
        else ctx.Print(parts);
    }
}

public class GrepCommand : ITerminalCommand
{
    public string Name => "grep";
    public string Description => "Busca patrón en archivo. Uso: grep <patrón> <archivo>";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 2) { ctx.Print("Uso: grep <patrón> <archivo>"); return; }
        var pattern = args[0];
        var path = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[1]));
        try { foreach (var line in File.ReadAllLines(path)) if (line.Contains(pattern)) ctx.Print(line); }
        catch (Exception e) { ctx.Print($"Error: {e.Message}"); }
    }
}

public class ClearCommand : ITerminalCommand
{
    public string Name => "clear";
    public string Description => "Limpia la pantalla.";
    public void Execute(string[] args, TerminalContext ctx) => ctx.Clear();
}

public class ExitCommand : ITerminalCommand
{
    public string Name => "exit";
    public string Description => "Sale de la aplicación.";
    public void Execute(string[] args, TerminalContext ctx) => Application.Quit();
}

public class PingCommand : ITerminalCommand
{
    public string Name => "ping";
    public string Description => "Hace ping a un host. Uso: ping <host>";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1) { ctx.Print("Uso: ping <host>"); return; }
        var host = args[0];
        if (!Utils.ResolveHostname(host)) { ctx.Print($"Ping fallido a {host}"); return; }
        for (int i = 1; i <= 4; i++)
        {
            float t = UnityEngine.Random.Range(0.1f, 5f);
            ctx.Print($"64 bytes from {host}: icmp_seq={i} time={t:F1} ms");
            System.Threading.Thread.Sleep(200);
        }
        ctx.Print($"--- {host} ping statistics ---");
        ctx.Print("4 packets transmitted, 4 received, 0% packet loss");
    }
}

public class SshCommand : ITerminalCommand
{
    public string Name => "ssh";
    public string Description => "Inicia sesión SSH. Uso: ssh <user>@<host>";

    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1 || !args[0].Contains("@")) { ctx.Print("Uso: ssh <user>@<host>"); return; }
        var parts = args[0].Split('@'); var user = parts[0]; var host = parts[1];
        if (!Utils.ResolveHostname(host)) { ctx.Print($"ssh: could not resolve host {host}"); return; }
        ctx.Print($"Password for {user}@{host}:");
        ctx.Print("Authentication successful.");
    }
}

public class UptimeCommand : ITerminalCommand
{
    public string Name => "uptime";
    public string Description => "Muestra tiempo de actividad del sistema.";
    public void Execute(string[] args, TerminalContext ctx)
    {
        var up = Time.realtimeSinceStartup;
        var t = TimeSpan.FromSeconds(up);
        ctx.Print($"  {t.Days} days, {t.Hours} hours, {t.Minutes} minutes");
    }
}

public class AptGetCommand : ITerminalCommand
{
    public string Name => "apt-get";
    public string Description => "Gestiona paquetes. Uso: apt-get <clean|autoclean|autoremove>";
    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1) { ctx.Print("Uso: apt-get <clean|autoclean|autoremove>"); return; }
        switch (args[0])
        {
            case "clean": ctx.Print("Limpiando caches…"); break;
            case "autoclean": ctx.Print("Autoclean ejecutado."); break;
            case "autoremove": ctx.Print("Paquetes huérfanos eliminados."); break;
            default: ctx.Print("Uso: apt-get <clean|autoclean|autoremove>"); break;
        }
    }
}

public class AptUpdateCommand : ITerminalCommand
{
    public string Name => "apt";
    public string Description => "Uso: apt <update|upgrade>";
    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1) { ctx.Print("Uso: apt <update|upgrade>"); return; }
        if (args[0] == "update") ctx.Print("Listando paquetes…");
        else if (args[0] == "upgrade") ctx.Print("Actualizando paquetes…");
        else ctx.Print("Uso: apt <update|upgrade>");
    }
}

public class SystemctlStatusCommand : ITerminalCommand
{
    public string Name => "systemctl";
    public string Description => "Uso: systemctl status <servicio>";
    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 2 || args[0] != "status") { ctx.Print("Uso: systemctl status <servicio>"); return; }
        var svc = args[1];
        ctx.Print($"● {svc}.service - Servicio simulado\n   Loaded: loaded (/etc/systemd/system/{svc}.service)\n   Active: active (running)");
    }
}

public class DfCommand : ITerminalCommand
{
    public string Name => "df";
    public string Description => "Muestra uso de disco. Uso: df -h";
    public void Execute(string[] args, TerminalContext ctx)
    {
        if (!args.Contains("-h")) { ctx.Print("Uso: df -h"); return; }
        ctx.Print("Filesystem      Size  Used Avail Use% Mounted on");
        ctx.Print("rootfs          50G   20G   30G   40% /");
    }
}

public class DiffCommand : ITerminalCommand
{
    public string Name => "diff";
    public string Description => "Compara archivos: diff <f1> <f2>";
    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 2) { ctx.Print("Uso: diff <archivo1> <archivo2>"); return; }
        var f1 = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[0]));
        var f2 = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[1]));
        if (File.Exists(f1) && File.Exists(f2))
        {
            var l1 = File.ReadAllLines(f1);
            var l2 = File.ReadAllLines(f2);
            for (int i = 0; i < Math.Min(l1.Length, l2.Length); i++)
                if (l1[i] != l2[i]) ctx.Print($"{i + 1}c difference");
        }
        else ctx.Print("Uno de los archivos no existe.");
    }
}

public class HeadCommand : ITerminalCommand
{
    public string Name => "head";
    public string Description => "Muestra primeras líneas: head -n <num> <archivo>";
    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 3 || args[0] != "-n") { ctx.Print("Uso: head -n <num> <archivo>"); return; }
        if (!int.TryParse(args[1], out int n)) { ctx.Print("Número inválido"); return; }
        var path = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[2]));
        if (!File.Exists(path)) { ctx.Print("Archivo no encontrado"); return; }
        foreach (var line in File.ReadAllLines(path).Take(n)) ctx.Print(line);
    }
}

public class TailCommand : ITerminalCommand
{
    public string Name => "tail";
    public string Description => "Muestra últimas líneas: tail -n <num> <archivo>";
    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 3 || args[0] != "-n") { ctx.Print("Uso: tail -n <num> <archivo>"); return; }
        if (!int.TryParse(args[1], out int n)) { ctx.Print("Número inválido"); return; }
        var path = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[2]));
        if (!File.Exists(path)) { ctx.Print("Archivo no encontrado"); return; }
        var lines = File.ReadAllLines(path);
        foreach (var line in lines.Skip(Math.Max(0, lines.Length - n))) ctx.Print(line);
    }
}

public class CalCommand : ITerminalCommand
{
    public string Name => "cal";
    public string Description => "Muestra calendario: cal";
    public void Execute(string[] args, TerminalContext ctx) => ctx.Print(DateTime.Now.ToString("MMMM yyyy"));
}

public class DateCommand : ITerminalCommand
{
    public string Name => "date";
    public string Description => "Muestra fecha y hora actual.";
    public void Execute(string[] args, TerminalContext ctx) => ctx.Print(DateTime.Now.ToString());
}

public class HistoryCommand : ITerminalCommand
{
    public string Name => "history";
    public string Description => "Muestra historial de comandos.";
    public void Execute(string[] args, TerminalContext ctx) { foreach (var cmd in ctx.History) ctx.Print(cmd); }
}

public class HelpCommand : ITerminalCommand
{
    public string Name => "help";
    public string Description => "Muestra ayuda con todos los comandos.";
    public void Execute(string[] args, TerminalContext ctx) { foreach (var line in ctx.GetHelp().Split('\n')) ctx.Print(line); }
}

public class NanoCommand : ITerminalCommand
{
    public string Name => "nano";
    public string Description => "Edita un archivo: nano <archivo>";
    public void Execute(string[] args, TerminalContext ctx)
    {
        if (args.Length < 1) { ctx.Print("Uso: nano <archivo>"); return; }
        var path = ctx.GetAbsolutePath(Path.Combine(ctx.CurrentPath, args[0]));
        NanoManager.Instance.Open(path);
    }
}
