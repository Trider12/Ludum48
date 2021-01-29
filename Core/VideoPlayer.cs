public class VideoPlayer : Godot.VideoPlayer
{
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
}