using Godot;

namespace Ludum48.Core.Managers
{
    public class SoundManager : Control
    {
        private AudioStreamPlayer _clickPlayer;

        public override void _Ready()
        {
            base._Ready();

            _clickPlayer = GetNode<AudioStreamPlayer>("ClickSoundPlayer");
        }

        public void PlayClickSound()
        {
            _clickPlayer.Play();
        }
    }
}