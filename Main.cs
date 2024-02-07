using Godot;
using System;

public partial class Main : Node2D
{
    private ImageTexture CircleTexture  { get; set; } = new ImageTexture();
    public Image FogImage { get; set; }
    public Sprite2D Fog { get; set; }
    public ImageTexture FogTexture { get; set; } = new ImageTexture();
    
    public override void _Ready()
    {
        // Fill map out with darkness. 
        Fog = GetNode<Sprite2D>("Fog");
        FogImage = Image.Create(900, 900, false, Image.Format.Rgbah);
        FogImage.Fill(Colors.Black);
        FogTexture.SetImage(FogImage);
        Fog.Texture = FogTexture;

        // Load image we'll use to reveal the map
        Image CircleImage = ImageLoad("res://circle-128.png");
        CircleImage.Resize(CircleImage.GetWidth(), CircleImage.GetHeight());
        CircleImage.Convert(Image.Format.Rgbah);
        CircleTexture = ImageTexture.CreateFromImage(CircleImage);        
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
        Fog = GetNode<Sprite2D>("Fog");
        FogTexture = ImageTexture.CreateFromImage(FogImage);
        Fog.Texture = FogTexture;
    }

    // Helper function to avoid image load warning
    public static Image ImageLoad(String filepath)
    {
        var image = new Image();
        image = Image.LoadFromFile(ProjectSettings.GlobalizePath(filepath));
        return image;
    }


}
