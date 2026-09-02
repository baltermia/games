using Godot;

namespace FlappyBird;

public partial class Bird : AnimatedSprite2D
{
    public const string GroupName = "bird";

    [Export] public float Gravity { get; set; } = 1400;
    [Export] public float JumpVelocity { get; set; } = -550;
    [Export] public StringName JumpAction { get; set; } = "jump";
    [Export] public Area2D Hitbox { get; set; } = null!;
    [Export] public CollisionShape2D HitboxShape { get; set; } = null!;

    private float _velocity;
    private Vector2 _view;
    private float _halfHeight;

    public override void _Ready()
    {
        Hitbox.AddToGroup(GroupName);
        Hitbox.AreaEntered += OnAreaEntered;
        EventBus.Instance.Restart += OnRestart;

        _view = GetViewportRect().Size;

        // The capsule is rotated 90°, so its Radius (not Height) is the vertical half-extent.
        // Radius is in the shape's own unscaled units, so scale it up to real on-screen pixels.
        CapsuleShape2D shape = (CapsuleShape2D)HitboxShape.Shape;
        _halfHeight = shape.Radius * HitboxShape.GlobalScale.Y;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.IsInGroup(PipePair.GroupName))
            Die();
    }

    private void Die()
    {
        SetProcess(false);
        EventBus.Instance.EmitSignal(EventBus.SignalName.GameOver);
    }

    private void OnRestart()
    {
        SetProcess(true);
        Position = _view / 2;
        _velocity = 0;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _velocity += Gravity * (float)delta;

        if (Input.IsActionJustPressed(JumpAction))
            _velocity = JumpVelocity;

        Position = new Vector2(Position.X, Position.Y + _velocity * (float)delta);

        if (Position.Y - _halfHeight <= 0 || Position.Y + _halfHeight >= _view.Y)
            Die();
    }
}
