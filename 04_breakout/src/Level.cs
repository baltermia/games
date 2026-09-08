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

    public override void _Ready()
    {
        SpawnBricks();
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
    }
}
