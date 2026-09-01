using Godot;

namespace Pong;

public partial class Paddle : CharacterBody2D
{
	[Export] public int Speed { get; set; } = 700;
	[Export] public StringName UpAction { get; set; } = "ui_up";
	[Export] public StringName DownAction { get; set; } = "ui_down";
	
	private float _halfSize;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("paddles");
		
		RectangleShape2D shape = (RectangleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		_halfSize = shape.Size.Y / 2;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		int direction = 0;

		if (Input.IsActionPressed(UpAction))
			direction = -1;
		else if (Input.IsActionPressed(DownAction))
			direction = 1;
		
		Velocity = new Vector2(0, direction * Speed);

		MoveAndCollide(new Vector2(0, Velocity.Y * (float)delta));

		Vector2 screenSize = GetViewportRect().Size;
		Position = new Vector2(Position.X, Mathf.Clamp(Position.Y, _halfSize, screenSize.Y - _halfSize));
	}
}
