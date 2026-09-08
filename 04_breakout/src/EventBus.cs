using Godot;

namespace Breakout;

public partial class EventBus : Node
{
    public static EventBus Instance { get; private set; } = null!;

    [Signal] public delegate void ScoredEventHandler();
    [Signal] public delegate void GameOverEventHandler();
    [Signal] public delegate void WonEventHandler();
    [Signal] public delegate void RestartEventHandler();

    public override void _Ready() => Instance = this;
}
