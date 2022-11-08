using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUp : MonoBehaviour
{
    [SerializeField] private GameObject popUpObject;

    public enum TypePopUp{ LIFE, DAMAGE, MONEY}

    public GameObject Create(Vector3 position, int count, TypePopUp type) { 
        GameObject popup = Instantiate(popUpObject, position, Quaternion.identity);
        TextMeshPro textContainer = popup.GetComponent<TextMeshPro>();

        switch (type) { 
            case TypePopUp.DAMAGE:
                textContainer.color = Color.red;
                break;
            case TypePopUp.LIFE:
                textContainer.color = Color.green;
                break;
            case TypePopUp.MONEY:
                textContainer.color = Color.yellow;
                break;
            default:
                break;

        }

        textContainer.text = count.ToString();
        return popup;
    }
}
