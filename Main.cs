using Godot;
using System;

public partial class Main : Node2D
{
    private ImageTexture CircleTexture  { get; set; } = new ImageTexture();
    public Image FogImage { get; set; }
    public Sprite2D Fog { get; set; }
    public ImageTexture FogTexture { get; set; } = new ImageTexture();
    public int BatteryCount { get; set; } = 4;
    public Sprite2D[] BatteryLines { get; set; } = new Sprite2D[5];
    public bool IsLightOn = true;
    
    public override void _Ready()
    {
        // Fill map out with darkness. 
        Fog = GetNode<Sprite2D>("FogContainer/Fog");
        FogImage = Image.Create(2700, 2700, false, Image.Format.Rgba8);
        FogImage.Fill(Colors.Black);
        FogTexture.SetImage(FogImage);
        Fog.Texture = FogTexture;

        // Load image we'll use to reveal the map
        Image CircleImage = ImageLoad("res://circle-128.png");
        CircleImage.Resize(CircleImage.GetWidth(), CircleImage.GetHeight());
        CircleImage.Convert(Image.Format.Rgba8);
        CircleTexture = ImageTexture.CreateFromImage(CircleImage);  

        //Load batteries
        BatteryLines[0] = GetNode<Sprite2D>("Player/Hud/BatteryLine0"); 
        BatteryLines[1] = GetNode<Sprite2D>("Player/Hud/BatteryLine1"); 
        BatteryLines[2] = GetNode<Sprite2D>("Player/Hud/BatteryLine2"); 
        BatteryLines[3] = GetNode<Sprite2D>("Player/Hud/BatteryLine3"); 
        BatteryLines[4] = GetNode<Sprite2D>("Player/Hud/BatteryLine4"); 
    }

    public override void _Process(double delta)
    {
        
    }


    private void OnPlayerHit(Vector2 collisionPosition)
    {
        // Load details of dissolve circle
        var circleImage = CircleTexture.GetImage();
        var circleRect = circleImage.GetUsedRect();
        var dissolvePosition = collisionPosition - (circleRect.Size / 2);

        // Create new texture with white detection circle blended in
        FogImage.BlendRect(circleImage, (Rect2I)circleRect, (Vector2I)dissolvePosition);
        UpdateFogImageTexture();
    }

    private void OnPlayerMove(Vector2 playerPosition)
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
            GD.Print("Game Over!");
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


}
