using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    public void ExitAnimation(string _boolName) => anim.SetBool(_boolName, false);
}
