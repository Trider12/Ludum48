using Godot;

namespace GlobalGameJam2021.Core
{
    public class VideoSprite : Sprite
    {
        public override void _Ready()
        {
            var viewportSize = GetViewport().Size;
            var scale = viewportSize / Texture.GetSize();
            Position = viewportSize / 2;
            Scale = scale;
        }
    }
}