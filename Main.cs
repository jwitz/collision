using Godot;
using System;
using System.Linq;
using System.Numerics;

public partial class Main : Node2D
{
    private ImageTexture CircleTexture  { get; set; } = new ImageTexture();
    public Image FogImage { get; set; }
    public Sprite2D Fog { get; set; }
    public ImageTexture FogTexture { get; set; } = new ImageTexture();
    public int BatteryCount { get; set; } = 4;
    public Sprite2D[] BatteryLines { get; set; } = new Sprite2D[5];
    public bool IsLightOn = true;
    public int CleanCount { get; set; } = 0;
    public int TotalTileCount { get; set; }
    public TileMap Area { get; set; }

    [Signal]
    public delegate void ShowGameOverEventHandler();

    public override void _Ready()
    {
        return;
    }

    private void OnPlayerClean(int column, int row)
    {
        if (GetNode<LevelMap>("LevelMap").GetTileStatus(column, row) == false)
        {
            GD.Print("Cleaning tile...");
            GetNode<LevelMap>("LevelMap").CleanTile(column, row);
            CleanCount++;
            GD.Print("Player Cleaned!");
            GD.Print("Tiles cleaned:" + CleanCount + "/" + TotalTileCount);
        }
    }


    private void OnPlayerHit(Godot.Vector2 collisionPosition)
    {
        // Load details of dissolve circle
        var circleImage = CircleTexture.GetImage();
        var circleRect = circleImage.GetUsedRect();
        var dissolvePosition = collisionPosition - (circleRect.Size / 2);

        // Create new texture with white detection circle blended in
        FogImage.BlendRect(circleImage, (Rect2I)circleRect, (Vector2I)dissolvePosition);
        UpdateFogImageTexture();
    }

    private void OnPlayerMove(Godot.Vector2 playerPosition)
    {
        // Load details of dissolve circle
        var circleImage = CircleTexture.GetImage();
        var circleRect = circleImage.GetUsedRect();
        var dissolvePosition = playerPosition - (circleRect.Size / 2);

        // Create new texture with white detection circle blended in
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
        var image = Image.LoadFromFile(ProjectSettings.GlobalizePath(filepath));
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
            GameOver();
            BatteryCount = 4;
            GetNode<Timer>("Player/BatteryTimer").Stop();
        }
    }

    private void OnFlickerTimerTimeout()
    {
        if (GetNode<Player>("Player").IsConsumingBattery == false)
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

    public void GameOver()
    {
        // Refill battery
        for (int i = 0; i < BatteryLines.Length; i++)
        {
            BatteryLines[i].Visible = true;
        }

        GetNode<Player>("Player").CanMove = false;

        GetNode<AudioStreamPlayer>("Music").Stop();

        // Fill map out with darkness. 
        Fog = GetNode<Sprite2D>("FogContainer/Fog");
        FogImage = Image.Create(2700, 2700, false, Image.Format.Rgba8);
        FogImage.Fill(Colors.Black);
        FogTexture.SetImage(FogImage);
        Fog.Texture = FogTexture;
        EmitSignal(SignalName.ShowGameOver, CleanCount, TotalTileCount, true);
    }

    public void StartGame()
    {

        GetNode<Player>("Player").Start(GetNode<Marker2D>("StartPos").Position);

        // Fill map out with darkness. 
        Fog = GetNode<Sprite2D>("FogContainer/Fog");
        FogImage = Image.Create(2700, 2700, false, Image.Format.Rgba8);
        FogImage.Fill(Colors.Black);
        FogTexture.SetImage(FogImage);
        Fog.Texture = FogTexture;

        // Load image we'll use to reveal the map
        Image CircleImage = ImageLoad("res://Pixel-128.png");
        CircleImage.Resize(CircleImage.GetWidth(), CircleImage.GetHeight());
        CircleImage.Convert(Image.Format.Rgba8);
        CircleTexture = ImageTexture.CreateFromImage(CircleImage);  

        //Load batteries
        BatteryLines[0] = GetNode<Sprite2D>("Player/Hud/BatteryLine0"); 
        BatteryLines[1] = GetNode<Sprite2D>("Player/Hud/BatteryLine1"); 
        BatteryLines[2] = GetNode<Sprite2D>("Player/Hud/BatteryLine2"); 
        BatteryLines[3] = GetNode<Sprite2D>("Player/Hud/BatteryLine3"); 
        BatteryLines[4] = GetNode<Sprite2D>("Player/Hud/BatteryLine4"); 

        //Get total tiles to clean
        Godot.Collections.Array<Godot.Vector2I> TotalTiles = GetNode<LevelMap>("LevelMap").GetUsedCells(0);
        TotalTileCount = TotalTiles.Count;

        //Start battery timer and allow player to move
        GetNode<Timer>("Player/FlickerTimer").Start();
        GetNode<Player>("Player").CanMove = true;

        GetNode<AudioStreamPlayer>("Music").Play();

    }

    private void OnMenuResetGame()
    {
        //Reset player position and score
        GetNode<Player>("Player").Start(GetNode<Marker2D>("StartPos").Position);
        CleanCount = 0;
        GetNode<LevelMap>("LevelMap").ResetTileMap();
    }


}
