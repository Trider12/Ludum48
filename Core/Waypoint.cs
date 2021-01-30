using System.Collections.Generic;
using Godot;

namespace GlobalGameJam2021.Core
{
    public class Waypoint : Node2D
    {
        [Signal]
        public delegate void Clicked(Waypoint waypoint);

        [Export]
        public string ExitLevelName { get; set; } = string.Empty;

        public List<Waypoint> Waypoints { get; set; } = new List<Waypoint>();

        [Export]
        private List<NodePath> WaypointsNodePaths { get; set; } = new List<NodePath>();

        public override void _Ready()
        {
            GetNode<Area2D>("Area2D").Connect("input_event", this, nameof(OnInputEvent));

            foreach (var path in WaypointsNodePaths)
            {
                var node = GetNode<Waypoint>(path);
                Waypoints.Add(node);
            }
        }

        public void OnInputEvent(Object viewport, InputEvent @event, int shapeIdx)
        {
            if (@event is InputEventMouseButton eventKey)
                if (eventKey.Pressed && eventKey.ButtonIndex == (int)ButtonList.Left)
                    EmitSignal(nameof(Clicked), this);
        }
    }
}