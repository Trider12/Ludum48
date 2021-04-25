using Godot;

namespace Ludum48.Core.UI
{
    public class Clock : TextureRect
    {
        public void UpdateTime(float percent, int depth)
        {
            var material = Material as ShaderMaterial;

            material.SetShaderParam("rotation", -2 * Mathf.Pi * percent);
            material.SetShaderParam("depth", depth / 50f);
        }
    }
}