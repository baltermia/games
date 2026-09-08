using Godot;

namespace Breakout;

public partial class Ball : CharacterBody2D
{
    [Export] public float Speed { get; set; } = 400;
    [Export] public CollisionShape2D CollisionShape { get; set; } = null!;

    private Vector2 _viewSize;
    private float _radius;

    public override void _Ready()
    {
        _viewSize = GetViewportRect().Size;

        CircleShape2D shape = (CircleShape2D)CollisionShape.Shape;
        _radius = shape.Radius;

        EventBus.Instance.Restart += OnRestart;

        Launch();
    }

    private void Launch()
    {
        Position = _viewSize / 2;

        float directionX = GD.Randf() > 0.5f ? 1.0f : -1.0f;
        Velocity = new Vector2(directionX, -1).Normalized() * Speed;
    }

    private void OnRestart()
    {
        SetProcess(true);
        Launch();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Bounce off the left/right screen edges.
        if (Position.X - _radius <= 0 || Position.X + _radius >= _viewSize.X)
            Velocity = new Vector2(-Velocity.X, Velocity.Y);

        // Bounce off the top screen edge.
        if (Position.Y - _radius <= 0)
            Velocity = new Vector2(Velocity.X, -Velocity.Y);

        // Fell past the paddle at the bottom - game over instead of bouncing.
        if (Position.Y - _radius >= _viewSize.Y)
        {
            SetProcess(false);
            EventBus.Instance.EmitSignal(EventBus.SignalName.GameOver);
            return;
        }

        KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);
        if (collision is null)
            return;

        // Bounce() reflects the velocity perfectly off whatever was hit.
        Velocity = Velocity.Bounce(collision.GetNormal());

        // TODO: switch to Brick.GroupName once Brick.cs exists (next step).
        if (collision.GetCollider() is Node node && node.IsInGroup("bricks"))
        {
            node.QueueFree();
            EventBus.Instance.EmitSignal(EventBus.SignalName.Scored);
        }
    }
}
