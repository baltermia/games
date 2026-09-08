using Godot;

namespace Breakout;

public partial class Paddle : CharacterBody2D
{
    public const string GroupName = "paddle";

    [Export] public int Speed { get; set; } = 700;
    [Export] public StringName LeftAction { get; set; } = "ui_left";
    [Export] public StringName RightAction { get; set; } = "ui_right";
    [Export] public CollisionShape2D CollisionShape { get; set; } = null!;

    private float _halfWidth;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddToGroup(GroupName);

        RectangleShape2D shape = (RectangleShape2D)CollisionShape.Shape;
        _halfWidth = shape.Size.X / 2;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        int direction = 0;

        if (Input.IsActionPressed(LeftAction))
            direction = -1;
        else if (Input.IsActionPressed(RightAction))
            direction = 1;

        Velocity = new Vector2(direction * Speed, 0);

        MoveAndCollide(new Vector2(Velocity.X * (float)delta, 0));

        float screenWidth = GetViewportRect().Size.X;
        Position = new Vector2(Mathf.Clamp(Position.X, _halfWidth, screenWidth - _halfWidth), Position.Y);
    }
}
