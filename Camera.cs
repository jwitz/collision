using Godot;
using System;

public partial class Camera : Camera2D
{
	private bool IsZoomedIn { get; set; } = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("zoom"))
		{
			ZoomCamera();
		}
	}

	public void ZoomCamera()
	{
		if(IsZoomedIn)
		{
			Vector2 vector = new(2.0f, 2.0f);
			Zoom = vector;
			IsZoomedIn = false;
		}
		else
		{
			Vector2 vector = new(1.3f, 1.3f);
			Zoom = vector;
			IsZoomedIn = true;
		}
		
	}
}
