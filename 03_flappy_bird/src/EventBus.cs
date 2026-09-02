using Godot;

namespace FlappyBird;

public partial class EventBus : Node
{
    public static EventBus Instance { get; private set; } = null!;

    [Signal] public delegate void RestartEventHandler();
    [Signal] public delegate void GameOverEventHandler();
    [Signal] public delegate void PassedEventHandler();

	public override void _Ready() => Instance = this;
}
