using Godot;

public partial class Level : Node2D
{
    private Control _gameOverScreen;
    private Label _scoreLabel;

    private int _score = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _gameOverScreen = GetNode<Control>("CanvasLayer/GameOverScreen");
        _scoreLabel = GetNode<Label>("CanvasLayer/Score");
        Snake snake = GetNode<Snake>("Snake");
        snake.GameOver += OnGameOver;
        snake.Restart += OnRestart;
        snake.AteFood += OnAteFood;
    }

    private void OnRestart()
    {
        _gameOverScreen.Visible = false;
        _score = 1;
        SetScore();
    }
    
    private void OnAteFood()
    {
        _score++;
        SetScore();
    }

    private void SetScore()
    {
        _scoreLabel.Text = $"Score: {_score}";
    }
    
    private void OnGameOver()
    {
        _gameOverScreen.Visible = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}