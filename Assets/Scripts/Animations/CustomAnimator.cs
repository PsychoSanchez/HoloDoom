using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator
{
    public int Frame { get; private set; }
    public bool Finished { get; private set; }

    private float _lastFrameUpdate = 0f;
    public CustomAnimation animation { get; private set; }

    private SpriteRenderer _renderer = null;

    public CustomAnimator(SpriteRenderer renderer)
    {
        _renderer = renderer;
        Frame = 0;
    }

    void Start()
    {
        Frame = 0;
    }

    public void Update(float deltaTime)
    {
        if (animation == null)
        {
            return;
        }

        _lastFrameUpdate += deltaTime;

        if (_lastFrameUpdate > animation.delay)
        {
            ShowNextFrame();
            _lastFrameUpdate = 0f;
        }
    }

    private void ShowNextFrame()
    {
        _renderer.sprite = animation.timeline[Frame];
        Frame++;

        if (Frame >= animation.timeline.Length && animation.loop)
        {
            Frame = 0;
        }
    }

    public void Play(CustomAnimation animation)
    {
        this.animation = animation;
    }

    public void Stop()
    {
        this.animation = null;
    }
    public void Reset()
    {
        Frame = 0;
        Finished = false;
    }
}


[System.Serializable]
public class CustomAnimation
{
    public Sprite[] timeline { get; private set; }
    public float delay { get; private set; }
    public bool loop { get; private set; }
    public float duration { get; private set; }


    public CustomAnimation(Sprite[] timeline, float frameRate, bool loop)
    {
        this.timeline = timeline;
        this.delay = 1000 / frameRate;
        this.duration = this.delay * this.timeline.Length;
        this.loop = loop;
    }
}