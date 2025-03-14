using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShootAnim()
    {
        animator.SetTrigger("Shoot");
    }
    public void OnShootAnimationComplete()
    {
        animator.SetTrigger("Shoot");
        
    }
}
