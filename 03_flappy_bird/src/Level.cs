using Godot;

namespace FlappyBird;

public partial class Level : Node2D
{
    [Export] public StringName RestartAction { get; set; } = "restart";
    [Export] public Control GameOverLayer { get; set; } = null!;
    [Export] public Label Score { get; set; } = null!;

    private int _score = 0;

    public override void _Ready()
    {
        EventBus.Instance.GameOver += () => GameOverLayer.Visible = true;
        EventBus.Instance.Passed += () =>
        {
            _score++;
            SetScore();
        };
        EventBus.Instance.Restart += () =>
        {
            GameOverLayer.Visible = false;
            _score = 0;
            SetScore();
        };
    }

    private void SetScore()
    {
        Score.Text = $"Score: {_score}";
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(RestartAction))
            EventBus.Instance.EmitSignal(EventBus.SignalName.Restart);
    }
}
