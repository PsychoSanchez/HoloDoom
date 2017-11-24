using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloDoom.Animation
{
    public enum AnimationState
    {
        Started,
        Playing,
        Over
    }

    public class AnimatorEventArgs : EventArgs
    {
        public string animationName;
        public AnimationState state;
    }


    struct AnimationInfo
    {
        public float FrameRate;
        public string AnimationName;
        public Sprite[] Timeline;
    }

    public class CustomAnimator
    {
        public event EventHandler<AnimatorEventArgs> AnimationOver;
        public int Frame { get; private set; }
        public bool Finished { get; private set; }
        public float DeafaultFrameRate { get; private set; }

        private float _lastFrameUpdate = 0f;
        private AnimationInfo? _shotAnimation = null;
        private AnimationInfo? _repeatAnimation = null;
        private SpriteRenderer _renderer = null;
        private float _delay = 0f;
        private Dictionary<string, AnimationInfo> _animationSeq = new Dictionary<string, AnimationInfo>();

        public CustomAnimator(float frameRate, SpriteRenderer renderer)
        {
            _renderer = renderer;
            DeafaultFrameRate = frameRate;
            _delay = 1 / DeafaultFrameRate;
            Frame = 0;
        }

        public CustomAnimator(SpriteRenderer renderer)
        {
            _renderer = renderer;
            DeafaultFrameRate = 60;
            _delay = 1 / DeafaultFrameRate;
            Frame = 0;
        }

        /// <summary>
        /// Custom update method
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            float delay;
            if (_shotAnimation.HasValue)
            {
                delay = _shotAnimation.GetValueOrDefault().FrameRate;
            }
            else if (_repeatAnimation.HasValue)
            {
                delay = _repeatAnimation.GetValueOrDefault().FrameRate;
            }
            else
            {
                delay = _delay;
            }
            _lastFrameUpdate += deltaTime;
            if (_lastFrameUpdate > delay)
            {
                ShowNextFrame();
                _lastFrameUpdate = 0f;
            }
        }

        /// <summary>
        /// Shows next frame
        /// </summary>
        private void ShowNextFrame()
        {
            if (_shotAnimation.HasValue)
            {
                PlayShotAnimation();
                return;
            }

            PlayRepeatAnimation();
        }

        private void PlayShotAnimation()
        {
            var animation = _shotAnimation.GetValueOrDefault();
            _renderer.sprite = animation.Timeline[Frame];
            Frame++;

            if (Frame >= animation.Timeline.Length)
            {
                // Reset animation
                _shotAnimation = null;
                Frame = 0;

                // Fire event
                var connectedEvent = AnimationOver;
                if (connectedEvent != null)
                {
                    connectedEvent(this, new AnimatorEventArgs()
                    {
                        state = AnimationState.Over,
                        animationName = animation.AnimationName
                    });
                }
            }
        }

        private void PlayRepeatAnimation()
        {
            if (_repeatAnimation.HasValue)
            {
                var animation = _repeatAnimation.GetValueOrDefault();
                _renderer.sprite = animation.Timeline[Frame];
                Frame++;

                if (Frame >= animation.Timeline.Length)
                {
                    Frame = 0;
                }
            }
        }

        /// <summary>
        /// Adding sprite array to dictionary with specific name and default frameRate  
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="animationTimeline"></param>
        public void AddAnimationSequence(string animationName, Sprite[] animationTimeline)
        {
            if (_animationSeq.ContainsKey(animationName))
            {
                throw new Exception("Animation with given name already added");
            }

            _animationSeq.Add(animationName, new AnimationInfo()
            {
                Timeline = animationTimeline,
                AnimationName = animationName,
                FrameRate = 1 / DeafaultFrameRate
            });
        }


        /// <summary>
        /// Adding sprite array to dictionary with specific name and default frameRate  
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="animationTimeline"></param>
        public void AddAnimationSequence(string animationName, Sprite[] animationTimeline, float frameRate)
        {
            if (_animationSeq.ContainsKey(animationName))
            {
                throw new Exception("Animation with given name already added");
            }

            _animationSeq.Add(animationName, new AnimationInfo()
            {
                Timeline = animationTimeline,
                AnimationName = animationName,
                FrameRate = 1 / frameRate
            });
        }

        /// <summary>
        /// Play animation once (Pauses repeat animation)
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayOnce(string animationName)
        {
            if (!_animationSeq.ContainsKey(animationName))
            {
                Debug.LogError(animationName + " not found");
                return;
            }
            _lastFrameUpdate = 0f;
            _shotAnimation = _animationSeq[animationName];
            _renderer.sprite = _shotAnimation.GetValueOrDefault().Timeline[0];
            Frame = 1;
        }

        /// <summary>
        /// Play animation sequence on reapeat
        /// </summary>
        /// <param name="animationName"></param>
        public void Play(string animationName)
        {
            if (!_animationSeq.ContainsKey(animationName))
            {
                Debug.LogError(animationName + " not found");
                return;
            }
            _lastFrameUpdate = 0f;
            _repeatAnimation = _animationSeq[animationName];
        }

        /// <summary>
        /// Stops repeat animation
        /// </summary>
        public void Stop()
        {
            _repeatAnimation = null;
        }
        public void Reset()
        {
            Frame = 0;
            Finished = false;
        }
    }

    public class CustomAnimation
    {
        Sprite[] _timeline;
        public int Frame { get; private set; }
        public bool Finished { get; private set; }

        private DateTime _lastFrame = DateTime.Now;
        private float _delay = 0f;
        public CustomAnimation(Sprite[] timeline, float frameRate)
        {
            _timeline = timeline;
            _delay = 1000 / frameRate;
            Frame = 0;
        }

        public Sprite GetNextFrame()
        {
            if (_timeline == null)
            {
                return null;
            }

            float timePassed = (DateTime.Now - _lastFrame).Milliseconds;
            if (timePassed > _delay)
            {
                Frame++;
                _lastFrame = DateTime.Now;
            }

            if (Frame >= _timeline.Length)
            {
                Frame = 0;
                Finished = true;
                return null;
            }

            return _timeline[Frame];
        }

        public void Reset()
        {
            Frame = 0;
            Finished = false;
        }
    }
}