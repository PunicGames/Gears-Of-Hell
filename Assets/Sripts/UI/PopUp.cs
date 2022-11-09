using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUp : MonoBehaviour
{
    [SerializeField] private GameObject popUpObject;

    public enum TypePopUp{ LIFE, DAMAGE, MONEY}

    public void Create(Vector3 position, int count, TypePopUp type, bool plus, float dissapearTime) {
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

        if(plus)
            textContainer.text = "+ " + count.ToString();
        else
            textContainer.text = "- " + count.ToString();

        popup.GetComponent<PopupBehaviour>().totalDisappearTime = dissapearTime;
    }
}
