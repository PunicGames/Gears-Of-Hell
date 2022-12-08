using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderTabController : MonoBehaviour
{
    [SerializeField] Animator pivotAnimator;


    public void Interact() 
    {
        pivotAnimator.SetTrigger("Interact");
    }
}
