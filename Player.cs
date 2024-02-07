using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
    public int Speed { get; set; } = 50; // How fast the player will move (pixels/sec).

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
    	    velocity = velocity.Normalized() * Speed;
    	}

        Velocity = velocity;
		MoveAndSlide();
		if(velocity.X != 0 && velocity.Y != 0)
		{
			// Send position data for player to clean the floor
			Vector2 playerPosition = GetNode<CharacterBody2D>(this.GetPath()).GlobalPosition;
			EmitSignal(SignalName.Move, playerPosition);
		}
		int collideCount = 0;
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			// Send position data for first collision to main scene so that fog is removed
			if (collideCount == 0)
			{
		    	Vector2 collisionPosition = GetSlideCollision(i).GetPosition();
				EmitSignal(SignalName.Hit, collisionPosition);
				collideCount++;
				break;
			}
		}
	}
}
