using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    private PopUp popup;

    void Start()
    {
        popup = GetComponent<PopUp>();
        popup.Create(new Vector3(0.0f, 0.0f, 0.0f), 200, PopUp.TypePopUp.LIFE, true, 1f);
        popup.Create(new Vector3(2.0f, 0.0f, 0.0f), 200, PopUp.TypePopUp.DAMAGE, false, 0.5f);
        popup.Create(new Vector3(4.0f, 0.0f, 0.0f), 200, PopUp.TypePopUp.MONEY, true, 0.75f);
    }

}
