using Godot;

public partial class Food : Node2D
{
	private Snake _snake;
	
	private uint _tiles;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_snake = GetNode<Snake>("../Snake");
		_snake.AteFood += OnAteFood;

		Vector2 size = GetViewportRect().Size;
		_tiles = (uint)(size.X / Snake.TileSize);
		
		NewPosition();
	}

	private void OnAteFood()
	{
		NewPosition();
	}

	private void NewPosition()
	{
		uint x = GD.Randi() % _tiles;
		uint y = GD.Randi() % _tiles;
		
		Position = new Vector2(x, y) * Snake.TileSize;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
