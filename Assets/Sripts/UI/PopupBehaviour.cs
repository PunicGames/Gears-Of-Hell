using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupBehaviour : MonoBehaviour
{

    private float currentDisappearTimer;
    public float totalDisappearTime;
    private TextMeshPro textMesh;
    float upwardSpeed = 2.0f;

    private static int sortingOrder;

    private void Start()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        upwardSpeed = 2.0f;
        currentDisappearTimer = totalDisappearTime;

        // Hace que se superponga sobre los popups existentes
        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
    }

    private void Update()
    {
        transform.position += new Vector3(0, upwardSpeed) * Time.deltaTime;


        if (currentDisappearTimer > totalDisappearTime * 0.5f)
        {
            // First half of the popup lifetime
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else {
            // Second half of the popup lifetime
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }



        currentDisappearTimer -= Time.deltaTime;
        if (currentDisappearTimer < 0) {
            // Start dissapear
            float disappearSpeed = 3f;
            textMesh.alpha -= disappearSpeed * Time.deltaTime;
            if (textMesh.color.a < 0) {
                Destroy(gameObject);
            }
        }
    }
}
