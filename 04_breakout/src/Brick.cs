using Godot;

namespace Breakout;

public partial class Brick : StaticBody2D
{
    public const string GroupName = "bricks";

    public override void _Ready()
    {
        AddToGroup(GroupName);
    }
}
