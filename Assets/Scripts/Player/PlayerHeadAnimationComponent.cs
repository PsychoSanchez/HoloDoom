using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadAnimationComponent : MonoBehaviour
{
    CustomAnimator animator;
    public CustomAnimation hp100Animation;
    public CustomAnimation hp80Animation;
    public CustomAnimation hp60Animation;
    public CustomAnimation hp40Animation;
    public CustomAnimation hp20Animation;
    public CustomAnimation hp0Animation;
    // Use this for initialization
    void Start()
    {
        animator = new CustomAnimator(GetComponent<SpriteRenderer>());

        animator.Play(hp100Animation);
    }

    public void UpdateAnimation(int health)
    {
        CustomAnimation animToPlay = null;

        if (health > 80)
        {
            animToPlay = hp100Animation;
        }
        else if (health > 60)
        {
            animToPlay = hp80Animation;
        }
        else if (health > 40)
        {
            animToPlay = hp60Animation;
        }
        else if (health > 20)
        {
            animToPlay = hp40Animation;
        }
        else if (health > 0)
        {
            animToPlay = hp20Animation;
        }
        else
        {
            animToPlay = hp0Animation;
        }
        animator.Play(animToPlay);
    }

    void Update()
    {
        animator.Update(Time.deltaTime);
    }
}
