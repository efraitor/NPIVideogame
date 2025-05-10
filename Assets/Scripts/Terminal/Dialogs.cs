/// <summary>
/// Arrays de diálogos originales extraídos de dialogos.gd
/// </summary>
public static class Dialogs
{
    public static readonly string[] Mision_Inicial_Dialogs1 = new string[]
    {
        "Excelente, ya te encuentras en el directorio donde está el archivo.\n" +
        "Ahora falta listarlo para asegurarnos que se encuentra ahí.\n" +
        "Para ello usaremos el comando `ls`. Solo tienes que escribir `ls` y pulsar la tecla intro."
    };
    public static readonly string[] Mision_Inicial_Dialogs2 = new string[]
    {
        "¡Perfecto! Has encontrado el archivo `IPS_El_Bohío.txt`.\n" +
        "Ahora falta un paso más: ver el contenido de dicho fichero.\n" +
        "Para ello vas a usar el comando `cat` seguido del nombre del fichero,\n" +
        "por ejemplo: `cat IPS_El_Bohío.txt`. ¡Vamos, usa a ese gatito!"
    };
    public static readonly string[] ssh_clean_dialogs9 = new string[]
    {
        "Viejo: ¡Perfecto! Verifica el uso del disco nuevamente con:\ndf -h /contabilidad"
    };
    public static readonly string[] ssh_clean_dialogs10 = new string[]
    {
        "Viejo: ¡Un 70%! Eso ya es otra cosa. Ya podrán volver a descar... jeje, guiño guiño.\nAhora puedes salir del servidor con:\nexit"
    };
    public static readonly string[] ssh_clean_dialogs11 = new string[]
    {
        "Viejo: Ya hemos terminado esta increíble aventura. ¡Ni WALL-E ...aba tan bien!\nMisión terminada: Liberar espacio.\nFin del día."
    };
    public static readonly string[] ssh_clean_dialogs12 = new string[]
    {
        "Viejo: Y ahora contabilidad/.local/share/Trash/info/*"
    };
    // TODO: Añadir el resto de arrays de dialogos (mision_inicial_dialogs3..8, ssh_cp_dialogsX, apache_dialogsX, etc.)
}