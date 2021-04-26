using Godot;

namespace Ludum48.Core.UI
{
    public class Clock : TextureRect
    {
        private Label _label;

        public override void _Ready()
        {
            _label = GetNode<Label>("Label");
        }

        public void UpdateTime(float percent, int depth)
        {
            var material = Material as ShaderMaterial;

            material.SetShaderParam("rotation", -2 * Mathf.Pi * percent);
            material.SetShaderParam("depth", depth / 50f);
        }

        public void UpdateTimeSpeed(float speed)
        {
            _label.Text = "Time Speed: x" + speed.ToString();
        }
    }
}