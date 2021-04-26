using Godot;
using Ludum48.Core.Managers;

namespace Ludum48.Core.UI
{
    public class ReplayTimer : ColorRect
    {
        private Timer _replayTimer = new Timer();

        private Label _replayTimerLabel;

        public ReplayTimer()
        {
            _replayTimer.OneShot = true;
            _replayTimer.Connect("timeout", this, nameof(OnReplayTimeout));
        }

        public override void _Process(float delta)
        {
            UpdateTimerLabel();
        }

        public override void _Ready()
        {
            _replayTimerLabel = GetNode<Label>("Label");

            AddChild(_replayTimer);

            SetProcess(false);
        }

        public void ConnectTimeout(Object target, string methodName)
        {
            _replayTimer.Connect("timeout", target, methodName);
        }

        public void Start(bool add = false)
        {
            Visible = true;

            _replayTimer.Start(GameManager.RewindTime * 2 + (add ? _replayTimer.TimeLeft : 0));

            UpdateTimerLabel();

            SetProcess(true);
        }

        private void OnReplayTimeout()
        {
            Visible = false;

            SetProcess(false);
        }

        private void UpdateTimerLabel()
        {
            _replayTimerLabel.Text = Mathf.Stepify(_replayTimer.TimeLeft, 0.1f).ToString();
        }
    }
}