using Godot;
using Ludum48.Core.Managers;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Ludum48.Core.UI
{
	public class MainMenu : Control
	{
		private const string SettingsFileName = "user://settings.save";

		private static readonly string[] DisplayModesNames = { "Windowed", "Borderless Fullscreen", "Fullscreen" };

		private static readonly DynamicFont Font = new DynamicFont();

		private static readonly Vector2[] Resolutions =
		{
			new Vector2(1024, 576),
			new Vector2(1280, 720),
			new Vector2(1366, 768),
			new Vector2(1536, 864),
			new Vector2(1600, 900),
			new Vector2(1920, 1080)
		};

		private static readonly string[] ResolutionsNames = Resolutions.Select(x => $"{(int)x.x}x{(int)x.y}").ToArray();

		private OptionButton _displayModeOption;

		private ValueSlider _masterVolumeSlider;

		private OptionButton _resolutionsOption;

		private Settings _settings;

		private ConfirmationDialog _settingsWindow;

		static MainMenu()
		{
			Font.FontData = ResourceLoader.Load<DynamicFontData>("res://Assets/Fonts/Roboto/Roboto-Regular.ttf");
			Font.Size = 10;
		}

		public void _on_PlayButton_pressed()
		{
			GameManager.Instance.SceneManager.LoadDemoLevel();
		}

		public void _on_QuitButton_pressed()
		{
			GetTree().Quit();
		}

		public void _on_SettingsButton_pressed()
		{
			_settingsWindow.PopupCentered();
		}

		public void _on_SettingsWindow_canceled()
		{
			_resolutionsOption.Selected = Array.IndexOf(ResolutionsNames, _settings.Resolution);
			_displayModeOption.Selected = Array.IndexOf(DisplayModesNames, _settings.DisplayMode);
			_masterVolumeSlider.Value = _settings.MasterVolume;
		}

		public void _on_SettingsWindow_confirmed()
		{
			_settings.Resolution = ResolutionsNames[_resolutionsOption.Selected];
			_settings.DisplayMode = DisplayModesNames[_displayModeOption.Selected];
			_settings.MasterVolume = (int)(_masterVolumeSlider.Value);

			ApplySettings();

			var saveFile = new File();
			saveFile.Open(SettingsFileName, File.ModeFlags.Write);
			saveFile.StoreLine(JsonConvert.SerializeObject(_settings));
			saveFile.Close();
		}

		public override void _Ready()
		{
			_ = GameManager.Instance;

			_settingsWindow = GetNode<ConfirmationDialog>("SettingsWindow");

			_settingsWindow.Connect("confirmed", this, nameof(_on_SettingsWindow_confirmed));
			_settingsWindow.GetCancel().Connect("pressed", this, nameof(_on_SettingsWindow_canceled));
			_settingsWindow.GetCloseButton().Connect("pressed", this, nameof(_on_SettingsWindow_canceled));

			_settingsWindow.GetOk().Text = "Apply";
			_settingsWindow.GetLabel().AddFontOverride("font", Font);
			_settingsWindow.GetOk().AddFontOverride("font", Font);
			_settingsWindow.GetCancel().AddFontOverride("font", Font);
			_settingsWindow.GetCloseButton().AddFontOverride("font", Font);

			_resolutionsOption = GetNode<OptionButton>("SettingsWindow/MarginContainer/GridContainer/ResolutionOptionButton");
			_displayModeOption = GetNode<OptionButton>("SettingsWindow/MarginContainer/GridContainer/DisplayModeOptionButton");
			_masterVolumeSlider = GetNode<ValueSlider>("SettingsWindow/MarginContainer/GridContainer/MasterVolumeValueSlider");

			foreach (var res in ResolutionsNames)
			{
				_resolutionsOption.AddItem(res);
			}

			foreach (var mode in DisplayModesNames)
			{
				_displayModeOption.AddItem(mode);
			}

			_resolutionsOption.Selected = 1;
			_displayModeOption.Selected = 0;
			_masterVolumeSlider.Value = 50;

			_settings = new Settings { Resolution = ResolutionsNames[1], DisplayMode = DisplayModesNames[0], MasterVolume = 50 };

			var saveFile = new File();

			if (saveFile.FileExists(SettingsFileName))
			{
				saveFile.Open(SettingsFileName, File.ModeFlags.Read);
				_settings = JsonConvert.DeserializeObject<Settings>(saveFile.GetLine());
				saveFile.Close();

				_resolutionsOption.Selected = Array.IndexOf(ResolutionsNames, _settings.Resolution);
				_displayModeOption.Selected = Array.IndexOf(DisplayModesNames, _settings.DisplayMode);
				_masterVolumeSlider.Value = _settings.MasterVolume;
			}

			ApplySettings();
		}

		private void ApplySettings()
		{
			OS.WindowBorderless = _settings.DisplayMode == DisplayModesNames[1];
			OS.WindowFullscreen = _settings.DisplayMode == DisplayModesNames[2];

			Vector2 resolution = Resolutions[0];

			try
			{
				resolution = Resolutions[Array.IndexOf(ResolutionsNames, _settings.Resolution)];
			}
			catch
			{
			}

			// window res
			OS.WindowSize = _settings.DisplayMode == DisplayModesNames[1] ? OS.GetScreenSize() : resolution;
			// viewport res
			//GetViewport().Size = resolution;
			//GetViewport().SizeOverrideStretch = true;

			OS.CenterWindow();

			GD.Print("Window Resolution: " + OS.WindowSize + "; Viewport Resolution: " + GetViewport().Size);

			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), GD.Linear2Db(_settings.MasterVolume / 100f));
		}
	}
}
