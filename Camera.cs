using Godot;
using System;
using System.Numerics;

public partial class Camera : Camera2D
{
	private bool IsZooming { get; set; } = false;
	private bool IsZoomedIn { get; set; } = true;
	public double ZoomTimer {get; set; } = 0.0; //Measures progress of zoom interpolation
	public double ZoomOffset {get; set; } = 0.0; //Measures soft zoom effect/ offset of zoom progress
	public bool CanZoom {get; set;} = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("zoom") && IsZooming == false && CanZoom == true)
		{
			IsZooming = true;
		}
		if (IsZooming == true && CanZoom == true)
		{
			ZoomCamera();
		}
	}

	public void ZoomCamera()
	{
		GD.Print("zoom timer:" + ZoomTimer);
		if (ZoomTimer >= 1.0)
		{
			IsZooming = false;
			ZoomTimer = 0.0;
			ZoomOffset = 0.0;
			IsZoomedIn = !IsZoomedIn;
		}
		else if (IsZoomedIn == true)
		{
			ZoomOffset = (float)Mathf.Lerp(0.0f, 0.095f, ZoomTimer);
			float zoomAmount = (float)Mathf.Lerp(1.3f, 0.8f, ZoomTimer);
			Godot.Vector2 zoomVector = new(zoomAmount, zoomAmount);
			Zoom = zoomVector;
			ZoomTimer += (0.1 - ZoomOffset);
		}
		else
		{
			ZoomOffset = (float)Mathf.Lerp(0.0f, 0.095f, ZoomTimer);
			float zoomAmount = (float)Mathf.Lerp(0.8f, 1.3f, ZoomTimer);
			Godot.Vector2 zoomVector = new(zoomAmount, zoomAmount);
			Zoom = zoomVector;
			ZoomTimer += (0.1 - ZoomOffset);
		}
	}
		
}

