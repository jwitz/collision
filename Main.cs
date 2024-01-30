using Godot;
using System;

public partial class Main : Node
{
    private Image CircleImage;
    public Image FogImage;
    public int GridSize { get; set; } = 64;
    public Sprite2D Fog { get; set; }
    public ImageTexture FogTexture { get; set; } = new ImageTexture();
    
    public override void _Ready()
    {
        // Fill map out with darkness. Some of this code/ sizing might not be needed
        Fog = GetNode<Sprite2D>("Fog");
        FogImage = Image.Create(5000, 5000, false, Image.Format.Rgbah);
        FogImage.Fill(Colors.Black);
        FogTexture.SetImage(FogImage);
        Fog.Texture = FogTexture;
        Fog.GlobalScale *= GridSize;

        // Load image we'll use to reveal the map
        CircleImage = Image.LoadFromFile("res://circle-48.png"); 
        CircleImage.Convert(Image.Format.Rgbah);
        
    }

    // public void updateFog(detectionPoint)
    // {
    //     var circleRect = new Rect2(Vector2.Zero, CircleImage.GetWidth(), CircleImage.GetHeight());
    //     FogImage.BlendRect(CircleImage, circleRect, detectionPoint);
    // }



}
