using Godot;
using System;

public partial class LevelMap : TileMap
{
	public bool[,] tileLocation { get; set; }
    public Godot.Collections.Dictionary CleanValues = new Godot.Collections.Dictionary();
    public override void _Ready()
    {
		Rect2I dimensions = GetUsedRect();
		Vector2I vectorDimensions = dimensions.Size;
		tileLocation = new bool[vectorDimensions.X, vectorDimensions.Y];
        for(int row = 0; row < vectorDimensions.Y; row++) {
			for (int column = 0; column < vectorDimensions.X; column++) {
				tileLocation[column, row] = false;
			}
		}
    }

	public void ResetTileMap()
	{
		_Ready();
	}

	public void CleanTile(int column, int row)
	{
		tileLocation[column, row] = true;
		
	}

	public bool GetTileStatus(int column, int row)
	{
		return tileLocation[column, row];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}