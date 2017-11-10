using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator
{
    public int Frame { get; private set; }
    public bool Finished { get; private set; }

    private float _lastFrameUpdate = 0f;
    private Sprite[] _timeline = null;
    private SpriteRenderer _renderer = null;
    private float _delay = 0f;
    private Dictionary<string, Sprite[]> _animationSeq = new Dictionary<string, Sprite[]>();

    public CustomAnimator(float frameRate, SpriteRenderer renderer)
    {
        _renderer = renderer;
        _delay = frameRate / 100;
        Frame = 0;
    }

    void Start()
    {
        Frame = 0;
    }

    public void Update(float deltaTime)
    {
        _lastFrameUpdate += deltaTime;
        if (_lastFrameUpdate > _delay)
        {
            ShowNextFrame();
            _lastFrameUpdate = 0f;
        }
    }

    private void ShowNextFrame()
    {
        if (_timeline == null)
        {
            return;
        }
        _renderer.sprite = _timeline[Frame];
        Frame++;
        if (Frame >= _timeline.Length)
        {
            _timeline = null;
            Frame = 0;
        }
    }

    public void AddAnimationSequence(string animationName, Sprite[] animationTimeline)
    {
        if (_animationSeq.ContainsKey(animationName))
        {
            throw new Exception("Animation with such name already added");
        }
        _animationSeq.Add(animationName, animationTimeline);
    }
    public void PlayOnce(string animationName)
    {
        if (!_animationSeq.ContainsKey(animationName))
        {
            Debug.LogError(animationName + " not found");
            return;
        }
        _timeline = _animationSeq[animationName];
        _renderer.sprite = _timeline[0];
        Frame = 1;
    }
    public void Reset()
    {
        Frame = 0;
        Finished = false;
    }
}
