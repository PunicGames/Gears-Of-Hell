using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBotSceneManager : MonoBehaviour
{
    [SerializeField] GameObject gearPrefab;
    [SerializeField] GameObject healPrefab;
    [SerializeField] GameObject ammoPrefab;

    [Space]

    [SerializeField] GameObject workerBot;

    [Space]

    [SerializeField] Transform pos1;
    [SerializeField] Transform pos2;
    [SerializeField] Transform pos3;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            print("gear creado");
            instanceObject(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print("heal creado");
            instanceObject(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("ammo creado");
            instanceObject(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {

        }
    }

    void instanceObject(int number)
    {
        float posX = Random.Range(pos1.position.x, pos2.position.x);
        float posY = pos1.position.y;
        float posZ = Random.Range(pos2.position.z, pos3.position.z); ;

        switch(number){
            case 1:
                Instantiate(gearPrefab, new Vector3(posX, posY, posZ), Quaternion.identity);
                break;

            case 2:
                Instantiate(healPrefab, new Vector3(posX, posY, posZ), Quaternion.identity);
                break;

            case 3:
                Instantiate(ammoPrefab, new Vector3(posX, posY, posZ), Quaternion.identity);
                break;
        }
    }
}
