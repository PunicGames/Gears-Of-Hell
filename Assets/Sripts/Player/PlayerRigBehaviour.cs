using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerRigBehaviour : MonoBehaviour
{

    [Header("Right Hand")]
    [SerializeField] Rig r_rig;
    [SerializeField] Transform r_target;
    [SerializeField] Transform r_hint;
    [SerializeField] Transform[] r_targetPos;
    [SerializeField] Transform[] r_hintPos;

    [Header("Left Hand")]
    [SerializeField] Rig l_rig;
    [SerializeField] Transform l_target;
    [SerializeField] Transform l_hint;
    [SerializeField] Transform[] l_targetPos;
    [SerializeField] Transform[] l_hintPos;

    private void Start()
    {
        GetComponentInParent<Health>().onDeath += ActivateRRig;
        GetComponentInParent<Health>().onDeath += ActivateLRig;
    }

    public void ChangeRightTargetRigPos(int n)
    {
        r_target.localPosition = r_targetPos[n].localPosition;
        r_hint.localPosition = r_hintPos[n].localPosition;
    }
    public void ChangeLeftTargetRigPos(int n)
    {
        l_target.localPosition = l_targetPos[n].localPosition;
        l_hint.localPosition = l_hintPos[n].localPosition;
    }
    public void ActivateLRig(bool t)
    {
        if (t) l_rig.weight = 1; else l_rig.weight = 0;
    }
    public void ActivateRRig(bool t)
    {
        if (t) r_rig.weight = 1; else r_rig.weight = 0;
    }
    public void ActivateLRig(bool t, float time)
    {
        if (t) StartCoroutine(LLerping(time)); else StartCoroutine(LLerping(time));
    }
    public void ActivateRRig(float w,bool t, float time)
    {
        if (t) StartCoroutine(RLerping(time,w)); else StartCoroutine(RLerping(time,w));
    }
    IEnumerator RLerping(float t,float w)
    {
       
        float timeElapsed = 0;
        while (timeElapsed < t)
        {
            r_rig.weight = Mathf.Lerp(0, w, timeElapsed / t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        r_rig.weight = w;
    }
    IEnumerator LLerping(float t)
    {
        float timeElapsed = 0;
        while (timeElapsed < t)
        {
            l_rig.weight = Mathf.Lerp(0, 1, timeElapsed / t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        l_rig.weight = 1;
    }
    public void setRRigWeight(float w)
    {
        r_rig.weight = w;
    }
    public void setLRigWeight(float w)
    {
        l_rig.weight = w;
    }

}
