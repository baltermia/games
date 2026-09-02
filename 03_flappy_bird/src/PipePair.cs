using Godot;

namespace FlappyBird;

public partial class PipePair : Node2D
{
    public const string GroupName = "pipes";

    [Export] public float Speed { get; set; } = 300;
    [Export] public float GapSize { get; set; } = 400;
    [Export] public float VerticalMargin { get; set; } = 80;
    [Export] public Sprite2D TopPipeSprite { get; set; } = null!;
    [Export] public Area2D TopPipe { get; set; } = null!;
    [Export] public Area2D BottomPipe { get; set; } = null!;
    [Export] public Area2D ScoreZone { get; set; } = null!;

    private float _halfWidth;
    private Vector2 _view;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        TopPipe.AddToGroup(GroupName);
        BottomPipe.AddToGroup(GroupName);
        ScoreZone.AreaEntered += OnScoreZoneEntered;

        Vector2 textureSize = TopPipeSprite.Texture.GetSize();
        _halfWidth = textureSize.X / 2;
        _view = GetViewportRect().Size;

        // Each pipe's sprite/collision is centered on its own node, so it needs to
        // be pushed out by half the gap plus half its own height to just touch the gap.
        float offset = GapSize / 2 + textureSize.Y / 2;
        TopPipe.Position = new Vector2(0, offset);
        BottomPipe.Position = new Vector2(0, -offset);

        EventBus.Instance.GameOver += OnGameOver;
        EventBus.Instance.Restart += OnRestart;

        Reset();
    }

    private void OnRestart()
    {
        SetProcess(true);
        Reset();
    }

    private void OnGameOver()
    {
        SetProcess(false);
    }

    private void OnScoreZoneEntered(Area2D area)
    {
        if (area.IsInGroup(Bird.GroupName))
            EventBus.Instance.EmitSignal(EventBus.SignalName.Passed);
    }

    private void Reset()
    {
        // Keep the whole gap (plus a margin) within the viewport, instead of a
        // fixed range that ignores how tall the gap actually is.
        float minY = VerticalMargin + GapSize / 2;
        float maxY = _view.Y - VerticalMargin - GapSize / 2;
        float y = minY + GD.Randf() * (maxY - minY);

        Position = new Vector2(_view.X + _halfWidth, y);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        float x = Position.X - Speed * (float)delta;

        if (x < -_halfWidth)
            Reset();
        else
            Position = new Vector2(x, Position.Y);

    }
}
