using System;
using System.Collections.Generic;

using Godot;

using Ludum48.Core.Managers;

namespace Ludum48.Core.Time
{
    public enum TimeState { Replay, Rewind, Normal }

    public abstract class TimeObject : KinematicBody2D
    {
        private LinkedListNode<BaseTimeFrame> _currentFrameNode = null;

        private LinkedListNode<BaseTimeFrame> _nextFrameNode = null;

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

        protected LinkedList<BaseTimeFrame> SavedFrames { get; set; } = new LinkedList<BaseTimeFrame>();

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

        public virtual void EraseFromFuture()
        {
            var node = _currentFrameNode;

            while (node != null)
            {
                node.Value.IsActive = false;
                node = node.Next;
            }
        }

        public abstract void PhysicsProcess(float delta);

        public abstract void Process(float delta);

        public void StartNormal()
        {
            TimeState = TimeState.Normal;
            Depth = 0;
        }

        public virtual void StartReplay()
        {
            if (TimeState == TimeState.Replay)
            {
                return;
            }

            TimeState = TimeState.Replay;

            _currentFrameNode = _nextFrameNode;
        }

        public virtual void StartRewind()
        {
            if (TimeState == TimeState.Rewind)
            {
                return;
            }

            TimeState = TimeState.Rewind;
        }

        protected virtual void ApplyFrame(BaseTimeFrame frame)
        {
            Position = frame.Position;
            Rotation = frame.Rotation;
            IsActive = frame.IsActive;
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

        protected virtual BaseTimeFrame GetTimeFrame()
        {
            return new BaseTimeFrame { Position = Position, Rotation = Rotation, IsActive = IsActive };
        }

        private void Normal(float delta)
        {
            SavedFrames.AddLast(GetTimeFrame());
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
                    var value = SavedFrames.Last.Value.Clone();
                    value.IsActive = false;
                    SavedFrames.AddLast(value);
                    _currentFrameNode = SavedFrames.Last;
                }

                ApplyFrame(_currentFrameNode.Value);

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
                    if (SavedFrames.Count > 0)
                    {
                        var value = SavedFrames.First.Value.Clone();
                        value.IsActive = false;
                        SavedFrames.AddFirst(value);
                    }
                    else  // rewind before first frame happened
                    {
                        SavedFrames.AddFirst(GetTimeFrame());
                    }

                    _currentFrameNode = SavedFrames.First;
                }

                ApplyFrame(_currentFrameNode.Value);

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