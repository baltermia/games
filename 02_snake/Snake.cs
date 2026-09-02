using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Snake : Node2D
{
    public const uint TileSize = 32;
	
    [Signal] public delegate void GameOverEventHandler();
    [Signal] public delegate void AteFoodEventHandler();
    [Signal] public delegate void RestartEventHandler();

    [Export] public StringName UpAction { get; set; } = "ui_up";
    [Export] public StringName DownAction { get; set; } = "ui_down";
    [Export] public StringName LeftAction { get; set; } = "ui_left";
    [Export] public StringName RightAction { get; set; } = "ui_right";
    [Export] public StringName AcceptAction { get; set; } = "ui_accept";
	
    [Export] public PackedScene BodySegmentScene { get; set; } = null!;

    private Timer _timer = null!;
    private Food _food = null!;

    private Vector2 _viewSize;
    private Direction _direction = Direction.Right;
    private Direction _lastMovedDirection = Direction.Right;
    private readonly Queue<BodySegment> _segments = new();
    private BodySegment _head = null!;
	
    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += OnTimer;

        GameOver += OnGameOver;
        Restart += OnRestart;
		
        _food = GetNode<Food>("../Food");
		
        _viewSize = GetViewportRect().Size;

        Reset();
    }

    private void OnGameOver()
    {
        _timer.Stop();
    }

    private void OnRestart()
    {
        Reset();
    }

    private void Reset()
    {
        foreach (BodySegment segment in _segments)
            segment.QueueFree();
        _segments.Clear();

        _timer.Start();
        AddSegment(_viewSize / 2);
        _direction = Direction.Right;
        _lastMovedDirection = Direction.Right;
    }

    private static bool IsOpposite(Direction a, Direction b) => (a, b) switch
    {
        (Direction.Up, Direction.Down) or (Direction.Down, Direction.Up) => true,
        (Direction.Left, Direction.Right) or (Direction.Right, Direction.Left) => true,
        _ => false
    };

    private void OnTimer()
    {
        Vector2 movement = _direction switch
        {
            Direction.Up => new Vector2(0, -1),
            Direction.Down => new Vector2(0, 1),
            Direction.Left => new Vector2(-1, 0),
            Direction.Right => new Vector2(1, 0),
            _ => throw new System.Exception("Invalid direction")
        };
        _lastMovedDirection = _direction;

        Vector2 newHeadPosition = _head.Position + movement * TileSize;

        bool outOfBounds = newHeadPosition.X < 0 || newHeadPosition.Y < 0 ||
                            newHeadPosition.X >= _viewSize.X || newHeadPosition.Y >= _viewSize.Y;

        bool hitSelf = _segments.Any(x => x.Position == newHeadPosition);

        if (outOfBounds || hitSelf)
        {
            GD.Print("Game Over");
            EmitSignalGameOver();
            return;
        }

        if (newHeadPosition == _food.Position)
        {
            AddSegment(newHeadPosition);
            EmitSignalAteFood();
            return;
        }

        // Recycle the tail segment into the new head position instead of leaving
        // middle segments stationary.
        BodySegment tail = _segments.Dequeue();
        tail.Position = newHeadPosition;
        _segments.Enqueue(tail);
        _head = tail;
    }

    private void AddSegment(Vector2 position)
    {
        BodySegment segment = BodySegmentScene.Instantiate<BodySegment>(); // builds the full scene tree
        AddChild(segment);                                     // must add to tree to render/process
        segment.Position = position;
        _segments.Enqueue(segment);
        _head = segment;
    }

    private void SetDirection(Direction direction)
    {
        if (!IsOpposite(direction, _lastMovedDirection))
            _direction = direction;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionPressed(UpAction))
            SetDirection(Direction.Up);
        else if (Input.IsActionPressed(DownAction))
            SetDirection(Direction.Down);
        else if (Input.IsActionPressed(LeftAction))
            SetDirection(Direction.Left);
        else if (Input.IsActionPressed(RightAction))
            SetDirection(Direction.Right);
        else if (Input.IsActionPressed(AcceptAction))
            EmitSignalRestart();
    }
}