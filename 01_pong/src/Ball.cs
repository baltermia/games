using Godot;

namespace Pong;

public partial class Ball : CharacterBody2D
{
    // This is like an alarm the ball can send out.
    [Signal] public delegate void OutOfBoundsEventHandler();

    // The speed of the ball.
    [Export] public float Speed { get; set; } = 500;

    // Half the ball's collision size, so bounds checks react when the edge touches, not the center.
    private Vector2 _halfSize;

    // This function will reset the ball's position and velocity.
    public void Start()
    {
        // Put the ball in the middle of the screen
        Position = GetViewportRect().Size / 2;

        // Give it a random starting direction
        float directionX = GD.Randf() > 0.5f ? 1.0f : -1.0f; // Go left or right
        float directionY = (float)GD.RandRange(-0.5, 0.5); // Go slightly up or down

        // Apply the direction and speed. Normalized() keeps the speed consistent.
        Velocity = new Vector2(directionX, directionY).Normalized() * Speed;
    }

    // This function is called automatically when the game starts.
    public override void _Ready()
    {
        RectangleShape2D shape = (RectangleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape;
        _halfSize = shape.Size / 2;

        Start();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 screenSize = GetViewportRect().Size;

        // Check if the ball's edge went off the left or right edge
        if (Position.X - _halfSize.X < 0 || Position.X + _halfSize.X > screenSize.X)
        {
            EmitSignal(SignalName.OutOfBounds); // Sound the alarm!
            Start(); // Reset the ball
        }

        // Check if the ball's edge went off the top or bottom edge
        if (Position.Y - _halfSize.Y < 0 || Position.Y + _halfSize.Y > screenSize.Y)
        {
            Velocity = new Vector2(Velocity.X, -1 * Velocity.Y);
        }

        // Move the ball and check if we hit anything
        if (MoveAndCollide(Velocity * (float)delta) is not KinematicCollision2D collision)
            return;
		
        // Bounce() reflects the velocity perfectly
        Velocity = Velocity.Bounce(collision.GetNormal());

        // Let's make the game harder as it goes on
        if (collision.GetCollider() is Node node && node.IsInGroup("paddles"))
            Velocity *= 1.05f; // Increase speed by 5%
    }
}