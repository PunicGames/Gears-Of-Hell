using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltMoveAnimation : MonoBehaviour
{
    [SerializeField] Material belt;
    [SerializeField] float speed;
    float offset;
    void Update()
    {
        offset = Time.time * speed;
        belt.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
