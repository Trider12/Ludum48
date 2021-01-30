using Godot;

namespace GlobalGameJam2021.Core
{
    public class VideoPlayer : Godot.VideoPlayer
    {
        [Export]
        private bool Loop { get; set; } = false;

        public override void _Notification(int what)
        {
            switch (what)
            {
                case NotificationPaused:
                {
                    Paused = true;
                    break;
                }
                case NotificationUnpaused:
                {
                    Paused = false;
                    break;
                }
            }
        }

        public void _on_Finished()
        {
            if (Loop)
            {
                Play();
            }
        }

        public override void _Ready()
        {
            Connect("finished", this, nameof(_on_Finished));
        }
    }
}