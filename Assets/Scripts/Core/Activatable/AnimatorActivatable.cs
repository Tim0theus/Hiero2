using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorActivatable : Activatable {
    public EventHandler OnAnimationFinished;

    public Animator Animator;

    private void Reset() {
        Animator = gameObject.GetComponent<Animator>();
    }

    public override void Activate() {
        Animator.SetTrigger("Activate");
    }

    public override void DeActivate() {
        Animator.SetTrigger("DeActivate");
        AnimationFinished();
    }

    private void AnimationFinished() {
        if (OnAnimationFinished != null) OnAnimationFinished(null, null);
    }
}
