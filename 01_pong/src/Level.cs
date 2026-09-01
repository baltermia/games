using Godot;

namespace Pong;

public partial class Level : Node2D
{
	private int _scoreP1 = 0;
	private int _scoreP2 = 0;

	// Get references to our other nodes so we can use them in code.
	private Ball _ball;
	private Label _labelP1;
	private Label _labelP2;

	// This function runs when the level starts.
	public override void _Ready()
	{
		_ball = GetNode<Ball>("Ball");
		_labelP1 = GetNode<Label>("CanvasLayer/Control/Score1");
		_labelP2 = GetNode<Label>("CanvasLayer/Control/Score2");

		// This is the magic! We tell the Level to listen for the ball's alarm.
		_ball.OutOfBounds += OnBallOutOfBounds;

		// Start a fresh game
		NewGame();
	}

	private void NewGame()
	{
		_scoreP1 = 0;
		_scoreP2 = 0;
		UpdateScoreDisplay();
		_ball.Start();
	}

	// A simple function to keep the text on screen updated.
	private void UpdateScoreDisplay()
	{
		_labelP1.Text = "Left: " + _scoreP1;
		_labelP2.Text = "Right: " + _scoreP2;
	}

	// This function runs automatically whenever the ball emits its "OutOfBounds" signal.
	private void OnBallOutOfBounds()
	{
		// Check where the ball was when it went out
		if (_ball.Position.X < GetViewportRect().Size.X / 2)
		{
			// It was on the left side, so player 2 scores
			_scoreP2++;
		}
		else
		{
			// It was on the right side, so player 1 scores
			_scoreP1++;
		}
		
		GD.Print("Current Score: Player 1: " + _scoreP1 + " | Player 2: " + _scoreP2);

		UpdateScoreDisplay();
	}
}
