using Godot;
using System;
using System.Numerics;

public partial class Player : CharacterBody2D
{
	[Export]
    public int MaxSpeed { get; set; } = 70; // How fast the player will move (pixels/sec).
	public int Acceleration { get; set; } = 2; // How fast the player will accelerate
	public int Speed { get; set; } = 0; // How fast the player will move (pixels/sec).
	public float Deceleration = 0.20f;
	public Boolean IsRotating { get; set; }= false;
	public Boolean IsLeft { get; set; }= false;
	public float StartingRotation {get; set; }
	public double RotationTimer {get; set; } = 0.0;
	public int FrameCount { get; set; } = 0;
	public Rid LastTile { get; set; }
	public static float DoubleDelay = 0.12f;
	public float DoubleTapTime { get; set; } = DoubleDelay;
	public bool IsConsumingBattery { get; set; } = false;
	public bool CanMove { get; set; } = false;


	public InputEventAction lastInput = new InputEventAction();

    public Godot.Vector2 ScreenSize; // Size of the game window.

	public Timer BatteryTimer;
	public Timer FlickerTimer;

	[Export]
	public float Sensitivity { get; set; } = 0.40f;

	[Signal]
	public delegate void HitEventHandler(Godot.Vector2 collisionPosition);

	[Signal]
	public delegate void MoveEventHandler(Godot.Vector2 playerPosition);

	[Signal]
	public delegate void CleanEventHandler();

	public override void _Ready()
	{
   		ScreenSize = GetViewportRect().Size;
		BatteryTimer = GetNode<Timer>("BatteryTimer");
		FlickerTimer = GetNode<Timer>("FlickerTimer");
	}

	public void Start(Godot.Vector2 startPos)
	{
		Position = startPos;
	}


    public override void _PhysicsProcess(double delta)
	{
		if (CanMove == false)
		{
			return;
		}
		
		DoubleTapTime -= (float)delta;
    	var velocity = Godot.Vector2.Zero; // The player's movement vector.
		var rotation = GlobalRotation;

		// Flip/ double tap logic. Uses linear interpretation over time to keep moving the player after a double tap.
		if(IsRotating == true)
		{
			// Check if linear interpretation is complete, ending the rotation.
			if (RotationTimer >= 1.0)
			{
				IsRotating = false;
				RotationTimer = 0.0;
				IsLeft = false;
			}
			else if (IsLeft == false ) 
			{
				Rotation = (float)Mathf.LerpAngle(StartingRotation, StartingRotation + 3.14159, RotationTimer);
				RotationTimer += 0.1;
			}
			else
			{
				Rotation = (float)Mathf.LerpAngle(StartingRotation, StartingRotation - 3.14159, RotationTimer);
				RotationTimer += 0.1;
			}
		}

    	if (IsRotating == false && Input.IsActionPressed("rotate_right"))
    	{
			if (lastInput.Action == "rotate_right" && Input.IsActionJustPressed("rotate_right", true) && DoubleTapTime >= 0)
			{
				IsRotating = true;
				StartingRotation = GlobalRotation;
				lastInput.Action = "0";
			}
			else
			{
    	    	Rotate(Mathf.DegToRad(1) * Sensitivity);
				lastInput.Action = "rotate_right";
			}
			DoubleTapTime = DoubleDelay;
    	}

    	if (IsRotating == false && Input.IsActionPressed("rotate_left"))
    	{
    	    if (lastInput.Action == "rotate_left" && Input.IsActionJustPressed("rotate_left", true) && DoubleTapTime >= 0)
			{
				IsRotating = true;
				IsLeft = true;
				StartingRotation = GlobalRotation;
				lastInput.Action = "0";
			}
			else
			{
    	    	Rotate(Mathf.DegToRad(-1) * Sensitivity);
				lastInput.Action = "rotate_left";
			}
			DoubleTapTime = DoubleDelay;
    	}

    	if (IsRotating == false && Input.IsActionPressed("move_forward"))
    	{
			velocity.X += (float)Math.Sin(rotation);
    	    velocity.Y -= (float)Math.Cos(rotation);
			lastInput.Action = "move_forward";
			DoubleTapTime = DoubleDelay;
    	}
    	if (velocity.Length() > 0)
    	{
			// Accelerate the player up to top speed and start battery timer
			int accelSpeed = Speed + 1;
			Speed = Math.Min(accelSpeed, MaxSpeed);
    	    velocity = velocity.Normalized() * Speed;
			Velocity = velocity;
			if (BatteryTimer.IsStopped() == true)
			{
				BatteryTimer.Start();
			}
			else
			{
				BatteryTimer.Paused = false;
			}
			IsConsumingBattery = true;
    	}
		else
		{
			// Decelerate the player
			Speed = 0;
			Velocity = Velocity.Lerp(Godot.Vector2.Zero, Deceleration);
			BatteryTimer.Paused = true;
			IsConsumingBattery = false;
		}
		if(Velocity.X != 0 && Velocity.Y != 0)
		{
			// Send position data for player to clean the floor
			Godot.Vector2 playerPosition = GetNode<CharacterBody2D>(this.GetPath()).GlobalPosition;
			if (FrameCount % 4 == 0)
			{
				EmitSignal(SignalName.Move, playerPosition);
			}
			
		}
		
		MoveAndSlide();

		if (Speed == MaxSpeed && GetSlideCollisionCount() > 0 && FrameCount % 5 == 0)
		{
		    Godot.Vector2 collisionPosition = GetSlideCollision(0).GetPosition();
			EmitSignal(SignalName.Hit, collisionPosition);
			FrameCount = 0;
		}

		FrameCount++;
	}

	private void OnHooverBodyShapeEntered(Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex)
	{
		if (LastTile != bodyRid)
		{
			TileMap currentTileMap = (TileMap) body;
			Vector2I tileCoords = currentTileMap.GetCoordsForBodyRid(bodyRid);
			GD.Print("Cleaning tile...");
			EmitSignal(SignalName.Clean, tileCoords.X, tileCoords.Y);
			LastTile = bodyRid;
		}
	}

}
