using Godot;

namespace Ludum48.Core.UI
{
    public class ReloadBar : Node2D
    {
        private Sprite _barSlider;

        private Tween _tween = new Tween();

        [Signal]
        public delegate void ReloadFinished();

        public override void _Ready()
        {
            _barSlider = GetNode<Sprite>("BarSlider");
            _tween.Connect("tween_completed", this, nameof(OnTweenCompleted));
            AddChild(_tween);
        }

        public void InterruptReloading()
        {
            Visible = false;
            _tween.Stop(_barSlider);
        }

        public void StartReloading(float duration)
        {
            Visible = true;
            _tween.InterpolateProperty(_barSlider, "position:x", -28, 28, duration);
            _tween.Start();
        }

        private void OnTweenCompleted(Object obj, NodePath key)
        {
            Visible = false;
            EmitSignal(nameof(ReloadFinished));
        }
    }
}