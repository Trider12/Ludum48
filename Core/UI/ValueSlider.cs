using Godot;

namespace Ludum48.Core.UI
{
	public class ValueSlider : HBoxContainer
	{
		private Label _label;

		private HSlider _slider;

		public double Value
		{
			get { return _slider.Value; }

			set { _slider.Value = value; }
		}

		public void _on_HSlider_value_changed(float value)
		{
			_label.Text = value.ToString();
		}

		public override void _Ready()
		{
			_slider = GetNode<HSlider>("HSlider");
			_slider.Connect("value_changed", this, nameof(_on_HSlider_value_changed));
			_label = GetNode<Label>("Label");
			_label.Text = _slider.Value.ToString();
		}
	}
}
