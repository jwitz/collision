using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
    public int MaxSpeed { get; set; } = 70; // How fast the player will move (pixels/sec).
	public int Acceleration { get; set; } = 2; // How fast the player will accelerate
	public int Speed { get; set; } = 0; // How fast the player will move (pixels/sec).
	public float Deceleration = 0.20f;
	public int FrameCount { get; set; } = 0;

    public Vector2 ScreenSize; // Size of the game window.

	[Export]
	public float Sensitivity { get; set; } = 0.40f;

	public override void _Ready()
	{
   		ScreenSize = GetViewportRect().Size;
	}

	[Signal]
	public delegate void HitEventHandler(Vector2 collisionPosition);

	[Signal]
	public delegate void MoveEventHandler(Vector2 playerPosition);

	public override void _PhysicsProcess(double delta)
	{
    	var velocity = Vector2.Zero; // The player's movement vector.
		var rotation = GlobalRotation;

    	if (Input.IsActionPressed("rotate_right"))
    	{
    	    Rotate(Mathf.DegToRad(1) * Sensitivity);
    	}

    	if (Input.IsActionPressed("rotate_left"))
    	{
    	    Rotate(Mathf.DegToRad(-1) * Sensitivity);
    	}

    	if (Input.IsActionPressed("move_forward"))
    	{
			velocity.X += (float)Math.Sin(rotation);
    	    velocity.Y -= (float)Math.Cos(rotation);
    	}
    	if (velocity.Length() > 0)
    	{
			// Accelerate the player up to top speed
			int accelSpeed = Speed + 1;
			Speed = Math.Min(accelSpeed, MaxSpeed);
    	    velocity = velocity.Normalized() * Speed;
			Velocity = velocity;
    	}
		else
		{
			// Decelerate the player
			Speed = 0;
			Velocity = Velocity.Lerp(Vector2.Zero, Deceleration);
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
