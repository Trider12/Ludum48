using Godot;
using Ludum48.Core.Managers;
using System;
using System.Collections.Generic;

namespace Ludum48.Core
{
    public abstract class TimeObject : KinematicBody2D
    {
        private LinkedListNode<Tuple<Vector2, float, bool>> _currentFrameNode = null;

        private LinkedListNode<Tuple<Vector2, float, bool>> _nextFrameNode = null;

        private float _replayFramesToPlay = 0f;

        private float _rewindFramesToPlay = 0f;

        public int Depth { get; set; } = 0;

        public int FrameFactor { get; set; } = 1;

        public virtual bool IsActive
        {
            get { return Visible; }

            protected set { Visible = value; }
        }

        public TimeState TimeState { get; private set; } = TimeState.Normal;
        protected LinkedList<Tuple<Vector2, float, bool>> SavedFrames { get; set; } = new LinkedList<Tuple<Vector2, float, bool>>();

        private float TimeScale
        {
            get
            {
                switch (Depth)
                {
                    case 0:
                    {
                        return 1;
                    }

                    case 1:
                    {
                        return 0.5f;
                    }

                    case 2:
                    {
                        return 0.25f;
                    }

                    case 3:
                    {
                        return 0.125f;
                    }
                }

                return 0;
            }
        }

        public override sealed void _PhysicsProcess(float delta)
        {
            switch (TimeState)
            {
                case TimeState.Replay:
                {
                    Replay();
                }
                break;

                case TimeState.Rewind:
                {
                    Rewind();
                }
                break;

                case TimeState.Normal:
                {
                    Normal(delta);
                }
                break;
            }
        }

        public override sealed void _Process(float delta)
        {
            if (TimeState == TimeState.Normal)
            {
                Process(delta);
            }
        }

        public override void _Ready()
        {
            GameManager.Instance.Register(this);
        }

        public abstract void PhysicsProcess(float delta);

        public abstract void Process(float delta);

        public void StartNormal()
        {
            TimeState = TimeState.Normal;
            Depth = 0;
        }

        public void StartReplay()
        {
            if (TimeState == TimeState.Replay)
            {
                return;
            }

            TimeState = TimeState.Replay;

            _currentFrameNode = _nextFrameNode;
        }

        public void StartRewind()
        {
            if (TimeState == TimeState.Rewind)
            {
                return;
            }

            TimeState = TimeState.Rewind;
        }

        protected virtual void CustomNormalLogic()
        {
        }

        protected virtual void CustomReplayLogic()
        {
        }

        protected virtual void CustomRewindLogic()
        {
        }

        private void Normal(float delta)
        {
            SavedFrames.AddLast(new Tuple<Vector2, float, bool>(Position, Rotation, IsActive));
            _currentFrameNode = SavedFrames.Last;
            CustomNormalLogic();

            while (SavedFrames.Count > GameManager.MaxFramesStored)
            {
                SavedFrames.RemoveFirst();
            }

            PhysicsProcess(delta);
        }

        private void Replay()
        {
            _replayFramesToPlay += TimeScale;

            while (_replayFramesToPlay >= 1)
            {
                _replayFramesToPlay--;

                if (_currentFrameNode == null) // we don't have information about the object so late, so we duplicate, but make inactive
                {
                    var value = SavedFrames.Last.Value;
                    SavedFrames.AddLast(new Tuple<Vector2, float, bool>(value.Item1, value.Item2, false));
                    _currentFrameNode = SavedFrames.Last;
                }

                Position = _currentFrameNode.Value.Item1;
                Rotation = _currentFrameNode.Value.Item2;
                IsActive = _currentFrameNode.Value.Item3;

                _currentFrameNode = _currentFrameNode.Next;

                CustomReplayLogic();

                if (TimeState != TimeState.Replay)
                {
                    return;
                }
            }
        }

        private void Rewind()
        {
            var timeScale = TimeScale * Math.Max(1, GameManager.Instance.MainPlayer.Depth);

            _rewindFramesToPlay += timeScale;

            while (_rewindFramesToPlay >= 1)
            {
                _rewindFramesToPlay--;

                if (_currentFrameNode == null) // we don't have information about the object so early, so we duplicate, but make inactive
                {
                    var value = SavedFrames.First.Value;
                    SavedFrames.AddFirst(new Tuple<Vector2, float, bool>(value.Item1, value.Item2, false));
                    _currentFrameNode = SavedFrames.First;
                }

                Position = _currentFrameNode.Value.Item1;
                Rotation = _currentFrameNode.Value.Item2;
                IsActive = _currentFrameNode.Value.Item3;

                _nextFrameNode = _currentFrameNode;
                _currentFrameNode = _currentFrameNode.Previous;

                CustomRewindLogic();

                if (TimeState != TimeState.Rewind)
                {
                    return;
                }
            }
        }
    }
}