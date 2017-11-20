using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadAnimationComponent : MonoBehaviour
{
    CustomAnimator animator;
    public Sprite[] hp100Animation;
    public Sprite[] hp80Animation;
    public Sprite[] hp60Animation;
    public Sprite[] hp40Animation;
    public Sprite[] hp20Animation;
    public Sprite[] hp0Animation;
    // Use this for initialization
    void Start()
    {
        animator = new CustomAnimator(2, GetComponent<SpriteRenderer>());
        animator.AddAnimationSequence("hp100", hp100Animation);
        animator.AddAnimationSequence("hp80", hp80Animation);
        animator.AddAnimationSequence("hp60", hp60Animation);
        animator.AddAnimationSequence("hp40", hp40Animation);
        animator.AddAnimationSequence("hp20", hp20Animation);
        animator.AddAnimationSequence("hp0", hp0Animation);
        animator.Play("hp100");
    }

    public void UpdateAnimation(int health)
    {
        var animToPlay = "hp0";
        if (health > 80)
        {
            animToPlay = "hp100";
        }
        else if (health > 60)
        {
            animToPlay = "hp80";
        }
        else if (health > 40)
        {
            animToPlay = "hp60";
        }
        else if (health > 20)
        {
            animToPlay = "hp40";
        }
        else if (health > 0)
        {
            animToPlay = "hp20";
        }
        else
        {
            animToPlay = "hp0";
        }
        if (animator != null)
        {
            animator.Play(animToPlay);
        }
    }

    void Update()
    {
        animator.Update(Time.deltaTime);
    }
}
