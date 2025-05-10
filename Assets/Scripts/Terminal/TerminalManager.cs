using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerminalManager : MonoBehaviour
{
    public static TerminalManager Instance;
    public TextMeshProUGUI historyText;
    public TMP_InputField inputField;
    public ScrollRect scrollRect;
    public event Action<string> OnCommandProcessed;
    private CommandProcessor processor;

    void Awake() { Instance = this; }

    void Start()
    {
        Debug.Log("Data Path: " + Application.persistentDataPath);

        var ctx = new TerminalContext(historyText, inputField);
        var commands = new ITerminalCommand[]
        {
            new CdCommand(), new LsCommand(), new PwdCommand(), new MkdirCommand(),
            new RmdirCommand(), new TouchCommand(), new RmCommand(), new CpCommand(),
            new MvCommand(), new CatCommand(), new EchoCommand(), new GrepCommand(),
            new ClearCommand(), new ExitCommand(), new PingCommand(), new SshCommand(),
            new UptimeCommand(), new AptGetCommand(), new AptUpdateCommand(),
            new SystemctlStatusCommand(), new DfCommand(), new DiffCommand(),
            new HeadCommand(), new TailCommand(), new CalCommand(), new DateCommand(),
            new HistoryCommand(), new HelpCommand(), new NanoCommand()
        };
        processor = new CommandProcessor(ctx, commands);
        ctx.Processor = processor;
        inputField.lineType = TMP_InputField.LineType.SingleLine;
        inputField.onEndEdit.AddListener(OnInputSubmitted);
        inputField.ActivateInputField();
    }

    private void OnInputSubmitted(string input)
    {
        if (!Input.GetKeyDown(KeyCode.Return)) return;
        input = input.Trim(); if (string.IsNullOrEmpty(input)) return;
        processor.Context.History.Add(input);
        processor.Process(input);
        OnCommandProcessed?.Invoke(input);
        Canvas.ForceUpdateCanvases();
        if (scrollRect) scrollRect.verticalNormalizedPosition = 0f;
        inputField.text = string.Empty;
        inputField.ActivateInputField();
    }
}