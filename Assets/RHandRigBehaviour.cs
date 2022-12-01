using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHandRigBehaviour : MonoBehaviour
{
    [SerializeField] Transform[] targetPos;
    [SerializeField] Transform[] hintPos;
    [SerializeField] Transform hint;

    // Start is called before the first frame update
   
    public void ChangeTargetRigPos(int n)
    {
        transform.localPosition = targetPos[n].localPosition;
        hint.localPosition = hintPos[n].localPosition;
    }
}
