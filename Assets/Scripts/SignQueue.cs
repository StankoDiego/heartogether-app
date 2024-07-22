using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignQueue : MonoBehaviour
{
 
  public Animator animator;
  public Dictionary<string, string> animations;

  void Start()
  {
    animations = new Dictionary<string, string>
    {
      { "Idle", "IdleState" },
      { "Hola", "HolaState" },
      { "A", "AState" },
      { "B", "BState" }
    };
  }

  public void StartAnimationQueue(string[] animationNames)
  {
    StartCoroutine(PlayAnimations(animationNames));
  }

  private IEnumerator PlayAnimations(string[] animationNames)
  {
    foreach (var animationName in animationNames)
    {
      if (animations.TryGetValue(animationName, out string animationState))
      {
        animator.Play(animationState);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
      }
      else
      {
        Debug.LogWarning($"Animation '{animationName}' not found in animation's dictionary");
      }
    }

    animator.Play("IDLE");
  }

}