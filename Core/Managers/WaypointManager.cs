using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GlobalGameJam2021.Core.Managers
{
    public class WaypointManager : Node2D
    {
        private Waypoint _currentWaypoint;

        private MovableEntity _player;
        private List<Waypoint> _waypoints = new List<Waypoint>();

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
                        DrawTriangle(point.Position - vec / 10f, vec.Normalized(), 10, Colors.Green);

                        if (!string.IsNullOrEmpty(waypoint.ExitLevelName))
                        {
                            DrawRect(new Rect2(waypoint.Position - new Vector2(10, 10), new Vector2(20, 20)), Colors.Red);
                        }
                    }
                }
            }
        }

        public override void _Ready()
        {
            _player = GetNode<MovableEntity>(PlayerNodePath);

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

            GameManager.Instance.WaypointManager = this;
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

        private void ChangeLevel(string levelName)
        {
            _player.Disconnect(nameof(MovableEntity.FinishedMovement), this, nameof(ChangeLevel));

            GameManager.Instance.SceneManager.LoadLevel(levelName);
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

                if (!string.IsNullOrEmpty(_currentWaypoint.ExitLevelName))
                {
                    _player.Connect(nameof(MovableEntity.FinishedMovement), this, nameof(ChangeLevel), new Godot.Collections.Array { _currentWaypoint.ExitLevelName });
                }
            }
        }
    }
}