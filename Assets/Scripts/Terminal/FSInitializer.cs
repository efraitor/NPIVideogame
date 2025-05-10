using System.IO;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class FSInitializer : MonoBehaviour
{
    void Awake()
    {
        string basePath = Application.persistentDataPath;
        string root = Path.Combine(basePath, "ubuntu_sim");
        Directory.CreateDirectory(root);

        string[] dirs = new[]
        {
            "home/usuario1/Documentos",
            "var/lib/apt/archives",
            "var/lib/apt/lists",
            "var/lib/dpkg",
            "var/spool/mail"
        };
        foreach (var d in dirs)
            Directory.CreateDirectory(Path.Combine(root, d));

        string docs = Path.Combine(root, "home/usuario1/Documentos");
        string ipsFile = Path.Combine(docs, "IPS_El_Bohío.txt");
        if (!File.Exists(ipsFile))
        {
            File.WriteAllText(ipsFile,
                "# IPs asignadas al Departamento de Ventas - El Bohío\n\n" +
                "192.168.10.10   pc_ventas_1\n" +
                "192.168.10.11   pc_ventas_2\n");
        }

        string debs = Path.Combine(root, "var/lib/apt/archives");
        foreach (var pkg in new[] { "nano_1.0.deb", "htop_1.0.deb" })
        {
            string path = Path.Combine(debs, pkg);
            if (!File.Exists(path))
                File.WriteAllText(path, $"Contenido simulado de {pkg}");
        }
    }
}