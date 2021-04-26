using Godot;
using Ludum48.Core.Managers;
using System;

namespace Ludum48.Core.UI
{
    public class Eye : StaticBody2D
    {
        private const float Radius = 30f;

        private Vector2 _center;

        private float _maxDelay = 0.3f;
        private float _minDelay = 0.2f;

        private Timer _timer = new Timer();
        private Tween _tween = new Tween();

        public override void _Ready()
        {
            _center = GlobalPosition;

            AddChild(_tween);
            _tween.Connect("tween_completed", this, nameof(OnTweenCompleted));

            AddChild(_timer);
            _timer.OneShot = true;
            _timer.Connect("timeout", this, nameof(OnTimer));

            StartTween(RandomDuration());
        }

        private void OnTimer()
        {
            StartTween(RandomDuration());
        }

        private void OnTweenCompleted(Godot.Object obj, NodePath key)
        {
            //if (new Random().Next() % 3 == 0)
            //{
                StartTween(RandomDuration());
            //}
            //else
            //{
            //    _timer.Start(RandomDuration() * 2);
            //}
        }

        private float RandomDuration()
        {
            return (float)new Random().NextDouble() * (_maxDelay - _minDelay) + _minDelay;
        }

        private void StartTween(float duration)
        {
            _tween.InterpolateProperty(this, "global_position", GlobalPosition, _center + _center.DirectionTo(GameManager.Instance.Player.GlobalPosition) * Radius, duration);
            _tween.Start();
        }
    }
}