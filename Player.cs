using Godot;
using System;

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

	public static float DoubleDelay = 0.12f;
	public float DoubleTapTime { get; set; } = DoubleDelay;
	public bool IsConsumingBattery { get; set; } = false;


	public InputEventAction lastInput = new InputEventAction();

    public Vector2 ScreenSize; // Size of the game window.

	public Timer BatteryTimer;
	public Timer FlickerTimer;

	[Export]
	public float Sensitivity { get; set; } = 0.40f;

	public override void _Ready()
	{
   		ScreenSize = GetViewportRect().Size;
		BatteryTimer = GetNode<Timer>("BatteryTimer");
		FlickerTimer = GetNode<Timer>("FlickerTimer");
	}

	[Signal]
	public delegate void HitEventHandler(Vector2 collisionPosition);

	[Signal]
	public delegate void MoveEventHandler(Vector2 playerPosition);

    public override void _PhysicsProcess(double delta)
	{
		DoubleTapTime -= (float)delta;
    	var velocity = Vector2.Zero; // The player's movement vector.
		var rotation = GlobalRotation;

		// Flip logic. Could be refactored into its own function
		if(IsRotating == true)
		{
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
			Velocity = Velocity.Lerp(Vector2.Zero, Deceleration);
			BatteryTimer.Paused = true;
			IsConsumingBattery = false;
		}
		if(Velocity.X != 0 && Velocity.Y != 0)
		{
			// Send position data for player to clean the floor
			Vector2 playerPosition = GetNode<CharacterBody2D>(this.GetPath()).GlobalPosition;
			if (FrameCount % 4 == 0)
			{
				EmitSignal(SignalName.Move, playerPosition);
			}
			
		}
		
		MoveAndSlide();

		if (Speed == MaxSpeed && GetSlideCollisionCount() > 0 && FrameCount % 5 == 0)
		{
		    Vector2 collisionPosition = GetSlideCollision(0).GetPosition();
			EmitSignal(SignalName.Hit, collisionPosition);
			FrameCount = 0;
		}

		FrameCount++;
	}

}
