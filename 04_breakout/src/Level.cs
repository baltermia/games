using Godot;

namespace Breakout;

public partial class Level : Node2D
{
    [Export] public PackedScene BrickScene { get; set; } = null!;
    [Export] public int Rows { get; set; } = 6;
    [Export] public int Columns { get; set; } = 8;
    [Export] public Vector2 BrickSize { get; set; } = new Vector2(64, 24);
    [Export] public Vector2 BrickSpacing { get; set; } = new Vector2(8, 8);
    [Export] public float TopMargin { get; set; } = 60;
    [Export] public StringName RestartAction { get; set; } = "ui_accept";
    [Export] public Label ScoreLabel { get; set; } = null!;
    [Export] public Control GameOverOverlay { get; set; } = null!;
    [Export] public Control WonOverlay { get; set; } = null!;

    private int _bricksRemaining;
    private int _score;

    public override void _Ready()
    {
        SpawnBricks();

        EventBus.Instance.Scored += OnScored;
        EventBus.Instance.GameOver += OnGameOver;
        EventBus.Instance.Won += OnWon;
        EventBus.Instance.Restart += OnRestart;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(RestartAction))
            EventBus.Instance.EmitSignal(EventBus.SignalName.Restart);
    }

    private void OnScored()
    {
        _score++;
        UpdateScore();

        _bricksRemaining--;
        if (_bricksRemaining <= 0)
            EventBus.Instance.EmitSignal(EventBus.SignalName.Won);
    }

    private void OnGameOver()
    {
        GameOverOverlay.Visible = true;
    }

    private void OnWon()
    {
        WonOverlay.Visible = true;
    }

    private void OnRestart()
    {
        GameOverOverlay.Visible = false;
        WonOverlay.Visible = false;

        _score = 0;
        UpdateScore();

        foreach (Node brick in GetTree().GetNodesInGroup(Brick.GroupName))
            brick.QueueFree();

        SpawnBricks();
    }

    private void UpdateScore()
    {
        ScoreLabel.Text = $"Score: {_score}";
    }

    private void SpawnBricks()
    {
        // Center the grid horizontally from the actual viewport/brick sizes,
        // instead of a hardcoded margin that would drift if those change.
        float gridWidth = Columns * BrickSize.X + (Columns - 1) * BrickSpacing.X;
        float leftMargin = (GetViewportRect().Size.X - gridWidth) / 2;

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                Brick brick = BrickScene.Instantiate<Brick>();
                AddChild(brick);

                float x = leftMargin + col * (BrickSize.X + BrickSpacing.X) + BrickSize.X / 2;
                float y = TopMargin + row * (BrickSize.Y + BrickSpacing.Y) + BrickSize.Y / 2;
                brick.Position = new Vector2(x, y);
            }
        }

        _bricksRemaining = Rows * Columns;
    }
}
