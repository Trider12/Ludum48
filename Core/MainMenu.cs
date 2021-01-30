using System;
using GlobalGameJam2021.Core.Managers;
using Godot;
using Newtonsoft.Json;

namespace GlobalGameJam2021.Core
{
    public class MainMenu : Control
    {
        private const string SettingsFileName = "user://settings.save";

        private static readonly Vector2[] Resolutions =
        {
        new Vector2(1024, 576),
        new Vector2(1280, 720),
        new Vector2(1366, 768),
        new Vector2(1536, 864),
        new Vector2(1600, 900),
        new Vector2(1920, 1080)
    };

        private static readonly string[] ResolutionsNames;

        private CheckBox _fullscreenCheckBox;
        private ValueSlider _masterVolumeSlider;
        private OptionButton _resolutionsOption;
        private Settings _settings;
        private AcceptDialog _settingsWindow;

        static MainMenu()
        {
            ResolutionsNames = new string[Resolutions.Length];

            for (int i = 0; i < Resolutions.Length; i++)
            {
                ResolutionsNames[i] = (int)Resolutions[i].x + "x" + (int)Resolutions[i].y;
            }
        }

        public override void _EnterTree()
        {
            // call static constructors
            _ = GameManager.Instance;
            _ = SceneManager.Instance;
        }

        public void _on_PlayButton_pressed()
        {
            SceneManager.Instance.LoadDemoLevel();
        }

        public void _on_QuitButton_pressed()
        {
            GetTree().Quit();
        }

        public void _on_SettingsButton_pressed()
        {
            _settingsWindow.PopupCentered();
        }

        public void _on_SettingsWindow_confirmed()
        {
            _settings.Resolution = ResolutionsNames[_resolutionsOption.Selected];
            _settings.IsFullscreen = _fullscreenCheckBox.Pressed;
            _settings.MasterVolume = (int)(_masterVolumeSlider.Value);

            ApplySettings();

            var saveFile = new File();
            saveFile.Open(SettingsFileName, File.ModeFlags.Write);
            saveFile.StoreLine(JsonConvert.SerializeObject(_settings));
            saveFile.Close();
        }

        public override void _Ready()
        {
            _settingsWindow = GetNode<AcceptDialog>("SettingsWindow");
            _resolutionsOption = GetNode<OptionButton>("SettingsWindow/MarginContainer/GridContainer/OptionButton");
            _masterVolumeSlider = GetNode<ValueSlider>("SettingsWindow/MarginContainer/GridContainer/ValueSlider");
            _fullscreenCheckBox = GetNode<CheckBox>("SettingsWindow/MarginContainer/GridContainer/CheckBox");

            foreach (var res in ResolutionsNames)
            {
                _resolutionsOption.AddItem(res);
            }

            _resolutionsOption.Selected = 1;
            _masterVolumeSlider.Value = 50;
            _fullscreenCheckBox.Pressed = false;

            _settings = new Settings { Resolution = ResolutionsNames[0], MasterVolume = 50, IsFullscreen = false };

            var saveFile = new File();
            if (saveFile.FileExists(SettingsFileName))
            {
                saveFile.Open(SettingsFileName, File.ModeFlags.Read);
                _settings = JsonConvert.DeserializeObject<Settings>(saveFile.GetLine());
                saveFile.Close();

                _resolutionsOption.Selected = Array.IndexOf(ResolutionsNames, _settings.Resolution);
                _masterVolumeSlider.Value = _settings.MasterVolume;
                _fullscreenCheckBox.Pressed = _settings.IsFullscreen;
            }

            ApplySettings();
        }

        private void ApplySettings()
        {
            OS.WindowFullscreen = _settings.IsFullscreen;

            var resolution = Resolutions[Array.IndexOf(ResolutionsNames, _settings.Resolution)];
            // window res
            OS.WindowSize = resolution;
            // viewport res
            GetViewport().Size = resolution;
            GetViewport().SizeOverrideStretch = true;

            OS.CenterWindow();

            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), GD.Linear2Db(_settings.MasterVolume / 100f));
        }
    }
}