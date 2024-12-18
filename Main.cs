using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

public partial class Main : Node2D
{
	private ImageTexture[] CircleTexture { get; set; }
	private ImageTexture GoTexture { get; set; }
	public Image FogImage { get; set; }
	public Sprite2D Fog { get; set; }
	public ImageTexture FogTexture { get; set; } = new ImageTexture();
	public int BatteryCount { get; set; } = 4;
	public Sprite2D[] BatteryLines { get; set; } = new Sprite2D[5];
	public bool IsLightOn = true;
	public int CleanCount { get; set; } = 0;
	public int ImageCount { get; set; } = 0;
	public int TotalTileCount { get; set; } = 582;
	public TileMap Area { get; set; }
	public bool IsLoadBattery { get; set; } = false;
	public int StartFlickerTime {get; set; } = 0;
	public double ShaderLevel {get; set; } = 0;
	public bool StartShaders { get; set; } = false;
	private bool IsRestart { get; set; } = false;
	private bool IsMusicSpeedingUp { get; set; } = false;
	private float StartingPlayerRotation { get; set; }

	[Signal]
	public delegate void ShowGameOverEventHandler();

	public override void _Ready()
	{
		StartingPlayerRotation = GetNode<Player>("Player").Rotation;
		GD.Randomize();
		GetNode<CanvasLayer>("CanvasLayer").Hide();
		return;
	}
	public override void _Process(double delta)
	{
		// Game over shader handler
		if(StartShaders == true && ShaderLevel <= 500) 
		{
			GetNode<AudioStreamPlayer>("Music").PitchScale -= (float)0.0010;
			((ShaderMaterial)GetNode<ColorRect>("Player/Camera/BackBufferPixel/PowerDownCanvasPixel/PowerDownPixel").Material).SetShaderParameter("size_x", (float)Math.Sin(ShaderLevel*.006)*.009);
			((ShaderMaterial)GetNode<ColorRect>("Player/Camera/BackBufferPixel/PowerDownCanvasPixel/PowerDownPixel").Material).SetShaderParameter("size_y", (float)Math.Sin(ShaderLevel*.006)*.009);
			((ShaderMaterial)GetNode<ColorRect>("Player/Camera/BackBufferMirage/PowerDownCanvas/PowerDownMirage").Material).SetShaderParameter("frequency", ShaderLevel*.03);
			((ShaderMaterial)GetNode<ColorRect>("Player/Camera/BackBufferMirage/PowerDownCanvas/PowerDownMirage").Material).SetShaderParameter("depth", ShaderLevel*.00016);
			if (ShaderLevel > 120) {
				GetNode<Player>("Player").IsGameOver = true;
			}
			ShaderLevel += delta * 120;
			if (ShaderLevel > 360) {
				GetNode<CanvasLayer>("Fade").Call("fade_out", 1);
			}
		}

		// Game restart music handler
		// Game over shader handler
		if(IsMusicSpeedingUp == true && ShaderLevel <= 500) 
		{
			GetNode<AudioStreamPlayer>("Music").PitchScale += (float)0.0010;
			ShaderLevel += delta * 120;
		}
	}

	private void OnPlayerClean(int column, int row)
	{
		if (GetNode<LevelMap>("LevelMap").GetTileStatus(column, row) == false)
		{
			// Uncomment for tile debug
			// GD.Print("Cleaning tile...");
			GetNode<LevelMap>("LevelMap").CleanTile(column, row);
			CleanCount++;
			// GD.Print("Player Cleaned!");
			// GD.Print("Tiles cleaned:" + CleanCount + "/" + TotalTileCount);
		}
	}


	private void OnPlayerHit(Godot.Vector2 collisionPosition)
	{
		// Load details of dissolve circle
		var circleImage = CircleTexture[GD.Randi() % ImageCount].GetImage();
		var circleRect = circleImage.GetUsedRect();
		var dissolvePosition = collisionPosition - (circleRect.Size / 2);

		// Create new texture with white detection circle blended in
		FogImage.BlendRect(circleImage, (Rect2I)circleRect, (Vector2I)dissolvePosition);
		UpdateFogImageTexture();
	}

	private void OnPlayerMove(Godot.Vector2 playerPosition)
	{
		// Load details of dissolve circle. Randomize exact image to create jittering effect
		var circleImage = CircleTexture[GD.Randi() % ImageCount].GetImage();
		var circleRect = circleImage.GetUsedRect();
		var dissolvePosition = playerPosition - (circleRect.Size / 2);

		// Create new texture with white dissolve circle blended in
		FogImage.BlendRect(circleImage, (Rect2I)circleRect, (Vector2I)dissolvePosition);
		UpdateFogImageTexture();
	}

	public void UpdateFogImageTexture()
	{
		FogTexture = ImageTexture.CreateFromImage(FogImage);
		Fog.Texture = FogTexture;
	}

	// Helper function to avoid image load warning
	public static Image ImageLoad(String filepath)
	{
		// var image = Image.LoadFromFile(ProjectSettings.GlobalizePath(filepath));
		CompressedTexture2D texture = (CompressedTexture2D)ResourceLoader.Load(filepath);
		Image image = texture.GetImage();
		return image;
	}

	private void OnBatteryTimerTimeout()
	{
		if (BatteryCount != 0) 
		{
			BatteryLines[BatteryCount].Visible = false;
			BatteryCount --;
		}
		else 
		{
			GetNode<Sprite2D>("CanvasLayer/Hud/NoBatteryLine").Show(); 
			GetNode<Sprite2D>("CanvasLayer/Hud/BatteryLine0").Hide(); 
			BatteryLines[0] = GetNode<Sprite2D>("CanvasLayer/Hud/NoBatteryLine"); 
			GameOver();
			GetNode<Timer>("Player/BatteryTimer").Stop();
		}
	}

	private void OnFlickerTimerTimeout()
	{
		// Logic for showing battery flickering at beginning of the game
		if (IsLoadBattery == true && StartFlickerTime < 5)
		{
			if (IsLightOn == true)
				{
					BatteryLines[0].Visible = true;
					BatteryLines[1].Visible = true;
					BatteryLines[2].Visible = true;
					BatteryLines[3].Visible = true;
					BatteryLines[4].Visible = true;
					IsLightOn = false;
					StartFlickerTime ++;
				}
				else 
				{
					BatteryLines[0].Visible = false;
					BatteryLines[1].Visible = false;
					BatteryLines[2].Visible = false;
					BatteryLines[3].Visible = false;
					BatteryLines[4].Visible = false;
					IsLightOn = true;
				}
		}
		// Core battery logic. Only flicker when player is moving
		else if (GetNode<Player>("Player").IsConsumingBattery == false)
		{
			BatteryLines[BatteryCount].Visible = true;
		}
		else 
		{
			if (IsLightOn == true)
			{
				BatteryLines[BatteryCount].Visible = true;
				IsLightOn = false;
			}
			else 
			{
				BatteryLines[BatteryCount].Visible = false;
				IsLightOn = true;
			}
		}

	}

	public async void GameOver()
	{

		GetNode<Player>("Player").CanMove = false;

		GetNode<CanvasLayer>("Player/Camera/BackBufferPixel/PowerDownCanvasPixel").Visible = true;
		GetNode<CanvasLayer>("Player/Camera/BackBufferMirage/PowerDownCanvas").Visible = true;
		StartShaders = true;
		await Task.Delay(TimeSpan.FromMilliseconds(4500)); 
		// Fill map out withprint darkness. 
		GetNode<CanvasLayer>("Player/Camera/BackBufferPixel/PowerDownCanvasPixel").Visible = false;
		GetNode<CanvasLayer>("Player/Camera/BackBufferMirage/PowerDownCanvas").Visible = false;
		StartShaders = false;
		GetNode<Player>("Player").IsGameOver = false;
		ShaderLevel = 0;
		// Refill battery
		for (int i = 0; i < BatteryLines.Length; i++)
		{
			BatteryLines[i].Visible = true;
		}
		BatteryCount = 4;
		GetNode<CanvasLayer>("CanvasLayer").Hide(); 
		GetNode<Player>("Player").GameOverSensitivity = 0.2f;
		GetNode<CanvasLayer>("Fade").Call("fade_in", 1);
		GetNode<CanvasLayer>("Fade").Hide();
		await Task.Delay(TimeSpan.FromMilliseconds(500)); 
		GetNode<Sprite2D>("CanvasLayer/Hud/NoBatteryLine").Hide(); 
		GetNode<Sprite2D>("CanvasLayer/Hud/BatteryLine0").Show(); 
		StartFlickerTime = 0;
		GetNode<Timer>("Player/FlickerTimer").Stop();
		GetNode<Player>("Player").IsConsumingBattery = false;
		if (GetNode<Camera>("Player/Camera").IsZoomedIn == false) {
			GetNode<Camera>("Player/Camera").ZoomCamera();
		}
		GetNode<Camera>("Player/Camera").CanZoom = false;
		EmitSignal(SignalName.ShowGameOver, CleanCount, TotalTileCount, true);
		IsRestart = true;

	}

	public void OnMenuStageGame() {

		if (IsRestart == false) {
			GetNode<AudioStreamPlayer>("Music").Play();
		}
		else {
			IsMusicSpeedingUp = true;
		}

		//Create starting sequence scene, but don't let player move until instructions are over.
		GetNode<Player>("Player").Start(GetNode<Marker2D>("StartPos").Position, StartingPlayerRotation);

		//Hide shader canvases
		GetNode<CanvasLayer>("Player/Camera/BackBufferPixel/PowerDownCanvasPixel").Visible = false;
		GetNode<CanvasLayer>("Player/Camera/BackBufferMirage/PowerDownCanvas").Visible = false;

		// Fill map out with darkness. 
		Fog = GetNode<Sprite2D>("FogContainer/Fog");
		FogImage = Godot.Image.CreateEmpty(2700, 2700, false, Image.Format.Rgba8);
		FogImage.Fill(Colors.Black);
		FogTexture.SetImage(FogImage);
		Fog.Texture = FogTexture;
		GetNode<CanvasLayer>("Fade").Visible = true;

		// Load images we'll use to reveal the map
		if (!IsRestart) {
			LoadImages();
		}

		//Get total tiles to clean
		Godot.Collections.Array<Godot.Vector2I> TotalTiles = GetNode<LevelMap>("LevelMap").GetUsedCells(0);
 
		TotalTileCount = 582;
	}

	public void StartGame()
	{
		IsLoadBattery = false;
		IsMusicSpeedingUp = false;
		ShaderLevel = 0;
		BlendGoImage();
		GetNode<Player>("Player").CanMove = true;
		GetNode<Camera>("Player/Camera").CanZoom = true;
	}

	public void BlendGoImage()
	{
		var goImage = GoTexture.GetImage();
		goImage.Rotate90(ClockDirection.Clockwise);
		var goRect = goImage.GetUsedRect();
		var dissolvePosition = GetNode<Marker2D>("GoPos").Position;

		// Create new texture with white detection circle blended in
		FogImage.BlendRect(goImage, (Rect2I)goRect, (Vector2I)dissolvePosition);
		UpdateFogImageTexture();

	}

	private void OnStartingLayerBatteryStatementCompleted()
	{
		//Load batteries. Ensure they start off hidden so that they always appear at same time in resets.
		BatteryLines[0] = GetNode<Sprite2D>("CanvasLayer/Hud/BatteryLine0"); 
		BatteryLines[1] = GetNode<Sprite2D>("CanvasLayer/Hud/BatteryLine1"); 
		BatteryLines[2] = GetNode<Sprite2D>("CanvasLayer/Hud/BatteryLine2"); 
		BatteryLines[3] = GetNode<Sprite2D>("CanvasLayer/Hud/BatteryLine3"); 
		BatteryLines[4] = GetNode<Sprite2D>("CanvasLayer/Hud/BatteryLine4");
		BatteryLines[0].Visible = false;
		BatteryLines[1].Visible = false;
		BatteryLines[2].Visible = false;
		BatteryLines[3].Visible = false;
		BatteryLines[4].Visible = false;
		GetNode<CanvasLayer>("CanvasLayer").Show(); 
		IsLoadBattery = true;
		GetNode<Timer>("Player/FlickerTimer").Start();

	}

	private void OnMenuResetGame()
	{
		//Reset player position and score
		GetNode<Player>("Player").Start(GetNode<Marker2D>("StartPos").Position, StartingPlayerRotation);
		CleanCount = 0;
		GetNode<LevelMap>("LevelMap").ResetTileMap();
	}

	private void LoadImages()
	{
		List<Image> circleImage = new List<Image>();
		var dir = DirAccess.Open("res://Circles");
		GD.Print("dir:" + dir);
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				GD.Print("Filename:" + fileName);
				if(fileName.EndsWith("import"))
				{
					String pngFileName = fileName.Replace(".import", "");
					String fullPath = "res://Circles/" + pngFileName;
					Image loadImage = ImageLoad(fullPath);
					loadImage.Resize(loadImage.GetWidth(), loadImage.GetHeight());
					loadImage.Convert(Image.Format.Rgba8);
					circleImage.Add(loadImage);
					ImageCount++;
				}
				fileName = dir.GetNext();
			}
		}
		else
		{
			GD.Print("An error occurred when trying to access the path.");
		}
		GD.Print("ImageCount:" + ImageCount);
		CircleTexture = new ImageTexture[ImageCount];
		
		for (int i = 0; i < ImageCount; i++)
		{
			CircleTexture[i] = ImageTexture.CreateFromImage(circleImage[i]);
		} 

		// Load Go sprite
		Image goImage = new Image();
		Image goLoadImage = ImageLoad("res://gosprite.png");
		goLoadImage.Resize(goLoadImage.GetWidth(), goLoadImage.GetHeight());
		goLoadImage.Convert(Image.Format.Rgba8);
		goImage = goLoadImage;
		GoTexture = ImageTexture.CreateFromImage(goImage);
		
	}

}

