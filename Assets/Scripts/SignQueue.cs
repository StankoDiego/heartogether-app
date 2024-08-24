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
      { "IDLE", "IdleState" },
      { "HOLA", "HolaState" },
      { "A", "AState" },
      { "B", "BState" },
      { "C", "CState" },
      { "D", "DState" },
      { "E", "EState" },
      { "F", "FState" },
      { "G", "GState" },
      { "H", "HState" },
      { "I", "IState" },
      { "J", "JState" },
      { "K", "KState" },
      { "L", "LState" },
      { "M", "MState" },
      { "N", "NState" },
      { "O", "OState" },
      { "P", "PState" },
      { "Q", "QState" },
      { "R", "RState" },
      { "S", "SState" },
      { "T", "TState" },
      { "U", "UState" },
      { "V", "VState" },
      { "W", "WState" },
      { "X", "XState" },
      { "Y", "YState" },
      { "Z", "ZState" },
      { "CHAU", "ChauState" },
      { "COMO", "ComoState" },
      { "COMO_ESTAS", "ComoEstasState" },
      { "CUAL", "CualState" },
      { "CUANDO", "CuandoPregState" },
      { "CUANTO", "CuantoState" },
      { "DONDE", "DondeState" },
      { "EL", "ElState" },
      { "ELLOS", "EllosState" },
      { "MIO", "MioState" },
      { "NOSOTROS", "NosotrosState" },
      { "OTRO", "OtroState" },
      { "PORFAVOR", "PorfavorState" },
      { "PORQUE", "PorQueState" },
      { "QUE", "QueState" },
      { "QUIEN", "QuienState" },
      { "TU", "TuState" },
      { "TUYO", "TuyoState" },
      { "USTEDES", "UstedesState" },
      { "YO", "YoState" },
      { "APELLIDO", "ApellidoState"}
    };
  }

  public void StartAnimationQueue(string[] animationNames, System.Action onComplete)
  {
    StartCoroutine(PlayAnimations(animationNames, onComplete));
  }

  private IEnumerator PlayAnimations(string[] animationNames, System.Action onComplete)
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
    onComplete?.Invoke();
  }

}