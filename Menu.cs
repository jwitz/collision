using Godot;
using System;

public partial class Menu : CanvasLayer
{
	public bool IsTitleScreen {get; set;} = true;
	public bool IsGameOver {get; set;} = false;
	[Signal]
    public delegate void StartGameEventHandler();
	[Signal]
    public delegate void StartTitleSequenceEventHandler();
	[Signal]
	public delegate void ResetGameEventHandler();
	[Signal]
	public delegate void StageGameEventHandler();


	public void ShowMessage(string text)
	{
	    var message = GetNode<Label>("Message");
	    message.Text = text;
	    message.Show();
	}

	async public void OnShowGameOver(int tilesCleaned, int totalTiles, bool isGameOver)
	{
		IsGameOver = isGameOver;
		float cleanFloat = tilesCleaned;
		float totalFloat = totalTiles;
		float percentage = (cleanFloat / totalFloat) * 100;

		GetNode<Sprite2D>("Backdrop").Show();

		await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);

		ShowMessage("As the last of your power drains,\n you remember the contours of a home. \n It was not your home, but it was your duty to clean it. \n ");
		
	    await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);

		ShowMessage("You cleaned " + percentage + "% of the home. \n Your owner is sure to be happy. \n Be proud of your work.");
		
	    await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);

		GetNode<Label>("Message").Hide();

		await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);

	    GetNode<Label>("Title").Show();
		GetNode<Label>("Instructions").Show();
		IsGameOver = false;
		IsTitleScreen = true;
		EmitSignal(SignalName.ResetGame);
	}

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("start_game") == true && IsTitleScreen == true)
		{
			GetNode<Sprite2D>("Backdrop").Hide();
			GetNode<Label>("Title").Hide();
			GetNode<Label>("Instructions").Hide();
			EmitSignal(SignalName.StageGame);
			EmitSignal(SignalName.StartTitleSequence);
			IsTitleScreen = false;
		}
	}

	public void OnStartingSequenceComplete() {
		EmitSignal(SignalName.StartGame);
	}
}
