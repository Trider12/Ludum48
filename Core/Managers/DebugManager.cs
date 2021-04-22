using Godot;

namespace Ludum48.Core.Managers
{
	public class DebugManager : CanvasLayer
	{
		private Control _debugOverlay;
		private int _minArgumentToConsoleCount = 2;

		public override void _Input(InputEvent @event)
		{
			if (Input.IsKeyPressed((int)KeyList.F1) && @event.IsPressed() && !@event.IsEcho())
			{
				_debugOverlay.Visible = !_debugOverlay.Visible;
			}
		}

		public override void _Ready()
		{
			SetProcessInput(OS.IsDebugBuild());

			_debugOverlay = GetNode<Control>("DebugOverlay");
			GetNode<LineEdit>("DebugOverlay/ConsoleInput").Connect("text_entered", this, nameof(OnConsoleInput));
		}

		public void OnConsoleInput(string text)
		{
			GD.Print(text);

			string[] splittedText = text.Split(" ");

			if (splittedText.Length >= _minArgumentToConsoleCount)
			{
				switch (splittedText[0])
				{
					case "lvl":
					{
						string levelName = splittedText[1];
						try
						{
							GameManager.Instance.SceneManager.LoadLevel(levelName);
						}
						catch
						{
							throw;
						}
						break;
					}

					default:
						break;
				}
			}
		}
	}
}
