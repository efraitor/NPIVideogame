using UnityEngine;

public class MissionController : MonoBehaviour
{
    public TerminalManager terminal;
    public DialogueManager dialogue;
    public float dialogDelay = 2f;
    private int currentMission = 1;

    void Start() { terminal.OnCommandProcessed += OnCommandExecuted; }

    private void OnCommandExecuted(string command)
    {
        if (currentMission == 1 && command.StartsWith("cd /home/usuario1/Documentos"))
        {
            currentMission = 2;
            dialogue.Show(Dialogs.Mision_Inicial_Dialogs2, dialogDelay);
        }
        else if (currentMission == 2 && command.StartsWith("ls"))
        {
            currentMission = 3;
            dialogue.Show(Dialogs.Mision_Inicial_Dialogs1, dialogDelay);
        }
        // TODO: Añadir transiciones de misiones 3..N usando Dialogs.Mision_Inicial_DialogsX y ssh_clean_dialogsX, etc.
    }
}