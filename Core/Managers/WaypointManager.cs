using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GlobalGameJam2021.Core.Managers
{
    public class WaypointManager : Node2D
    {
        private Waypoint _currentWaypoint;

        private Player _player;
        private List<Waypoint> _waypoints = new List<Waypoint>();

        public WaypointManager()
        {
            GameManager.Instance.WaypointManager = this;
        }

        [Export]
        private NodePath PlayerNodePath { get; set; }

        public override void _Draw()
        {
            if (OS.IsDebugBuild())
            {
                foreach (var waypoint in _waypoints)
                {
                    foreach (var point in waypoint.Waypoints)
                    {
                        DrawLine(waypoint.Position, point.Position, Colors.Green);
                        var vec = point.Position - waypoint.Position;
                        var dir = vec.Normalized();
                        DrawTriangle(point.Position - vec / 10f, dir, 10, Colors.Green);
                    }
                }
            }
        }

        public override void _Ready()
        {
            _player = GetNode<Player>(PlayerNodePath);

            foreach (var child in GetChildren())
            {
                if (child is Waypoint)
                {
                    var point = child as Waypoint;
                    point.Connect(nameof(Waypoint.Clicked), this, nameof(OnWaypointClicked));
                    _waypoints.Add(point);
                }
            }

            _currentWaypoint = _waypoints.First();
            _player.GlobalPosition = _currentWaypoint.GlobalPosition;
        }

        public void OnWaypointClicked(Waypoint waypoint)
        {
            if (GameManager.Instance.InventoryManager.HoldingItem != null)
            {
                return;
            }

            if (_currentWaypoint.Waypoints.Contains(waypoint))
            {
                MovePlayer(waypoint);
            }
        }

        private void DrawTriangle(Vector2 pos, Vector2 dir, float size, Color color)
        {
            var a = pos + dir * size;
            var b = pos + dir.Rotated(2 * Mathf.Pi / 3) * size;
            var c = pos + dir.Rotated(4 * Mathf.Pi / 3) * size;
            DrawPolygon(new[] { a, b, c, }, new[] { color, color, color });
        }

        private void MovePlayer(Waypoint waypoint)
        {
            if (_currentWaypoint == waypoint)
            {
                return;
            }

            if (_player.MoveTo(waypoint.GlobalPosition))
            {
                _currentWaypoint = waypoint;
            }
        }
    }
}