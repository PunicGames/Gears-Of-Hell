using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    private PopUp popup;

    void Start()
    {
        popup = GetComponent<PopUp>();
        popup.Create(new Vector3(0.0f, 2.0f, 0.0f), 200, PopUp.TypePopUp.LIFE);
        popup.Create(new Vector3(2.0f, 2.0f, 0.0f), 200, PopUp.TypePopUp.DAMAGE);
        popup.Create(new Vector3(4.0f, 2.0f, 0.0f), 200, PopUp.TypePopUp.MONEY);
    }

}
