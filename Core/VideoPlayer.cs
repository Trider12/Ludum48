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

    public void _on_Finished()
    {
        Play();
    }

    public override void _Ready()
    {
        Connect("finished", this, nameof(_on_Finished));
    }
}