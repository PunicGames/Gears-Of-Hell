using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bodyController : MonoBehaviour
{
    Vector3 velocity;
    Vector3 lastVelocity = Vector3.one;
    Vector3 lastSpiderPosition;
    Vector3[] legPositions;
    Vector3[] legOriginalPositions; // Posicion de arranque para las interpolaciones (cuando se mueve la pata de una posición a la nueva calculada)
    List<int> nextIndexToMove = new List<int>();
    List<int> IndexMoving = new List<int>();
    Vector3 lastBodyUp;
    List<int> oppositeLeg = new List<int>(); // Una araña podrá mover a la vez una pata que sea la opuesta a la que ya se mueve
    bool currentLeg = true;

    [Space(10)]
    [Header("GameObject Assignment")]
    [Space(10)]

    public GameObject spider;       
    public GameObject[] legTargets; // Posicion de las patas
    public GameObject[] legCubes;   // Controladores de posicion

    [Space(10)]
    [Header("Rotation of Body and Movement of leg")]
    [Space(10)]

    public bool enableBodyRotation = false;
    public bool enableMovementRotation = false;
    public bool rigidBodyController;

    [Space(10)]
    [Header("Values for leg Movement")]
    [Space(10)]

    public float moveDistance = 0.7f;
    public float stepHeight = .15f;
    public float spiderJitterCutOff = 0f;
    public int waitTimeBetweenEveryStep = 0;
    public float LegSmoothness = 8;
    public float BodySmoothness = 8;
    public float OverStepMultiplier = 4;



    void Start()
    {
        lastBodyUp = transform.up;

        legPositions = new Vector3[legTargets.Length];
        legOriginalPositions = new Vector3[legTargets.Length];

        for (int i = 0; i < legTargets.Length; i++)
        {
            legPositions[i] = legTargets[i].transform.position;
            legOriginalPositions[i] = legTargets[i].transform.position;

            // con 4 patas oppositeLeg almacenará 1,0,3,2 que son los indices de las patas opuestas 0,1,2,3
            if (currentLeg) { oppositeLeg.Add(i + 1); currentLeg = false; }
            else if (!currentLeg) { oppositeLeg.Add(i - 1); currentLeg = true; }
        }

        lastSpiderPosition = spider.transform.position;

        rotateBody();
    }


    void FixedUpdate()
    {
        // El cálculo de la nueva posicion de la pata está influenciado por la velocidad de la propia araña
        velocity = spider.transform.position - lastSpiderPosition;
        velocity = (velocity + BodySmoothness * lastVelocity) / (BodySmoothness + 1f);

        moveLegs();
        //rotateBody();


        lastSpiderPosition = spider.transform.position;
        lastVelocity = velocity;
    }

    void moveLegs()
    {
        if (!enableMovementRotation) return;
        for (int i = 0; i < legTargets.Length; i++)
        {
            // Si la distancia entre la pata y el controlador que la gobierna es el suficiente, la pata deberá moverse (por lo que se añade a la lista)
            if (Vector3.Distance(legTargets[i].transform.position, legCubes[i].transform.position) >= moveDistance)
            {
                // La añade en caso de que no esté ya añadida a la lista o se esté procesando su movimiento
                if (!nextIndexToMove.Contains(i) && !IndexMoving.Contains(i)) nextIndexToMove.Add(i);
            }
            else if (!IndexMoving.Contains(i)) // En el caso contrario de que no deba moverse, ni lo esté haciendo, la pata se queda quieta en su posicion 
            {
                legTargets[i].transform.position = legOriginalPositions[i];
            }

        }

        if (nextIndexToMove.Count == 0 || IndexMoving.Count != 0) return; // Si no hay ninguna pata esperando a ser movida o ya se está moviendo una, salimos del método
        Vector3 targetPosition = legCubes[nextIndexToMove[0]].transform.position + Mathf.Clamp(velocity.magnitude * OverStepMultiplier, 0.0f, 1.5f) * (legCubes[nextIndexToMove[0]].transform.position - legTargets[nextIndexToMove[0]].transform.position) + velocity * OverStepMultiplier;
        StartCoroutine(step(nextIndexToMove[0], targetPosition, false));
    }

    IEnumerator step(int index, Vector3 moveTo, bool isOpposite)
    {
        if (!isOpposite) moveOppisteLeg(oppositeLeg[index]);

        // Quitamos el indice de la lista ya que empieza a mover la pata
        if (nextIndexToMove.Contains(index)) nextIndexToMove.Remove(index);

        // Añadimos el indice de la pata a que se está moviendo
        if (!IndexMoving.Contains(index)) IndexMoving.Add(index);

        Vector3 startPos = legOriginalPositions[index];

        for (int i = 1; i <= LegSmoothness; ++i)
        {
            // La interpolacion de la pata entre posiciones es una interpolacion sinusoidal. Dependiendo del stepHeight la función sinusoidal tendrá una diferente amplitud.
            legTargets[index].transform.position = Vector3.Lerp(startPos, moveTo + new Vector3(0, Mathf.Sin(i / (float)(LegSmoothness + spiderJitterCutOff) * Mathf.PI) * stepHeight, 0), (i / LegSmoothness + spiderJitterCutOff));
            yield return new WaitForFixedUpdate();
        }

        // Actualiza su nueva posicion raiz
        legOriginalPositions[index] = moveTo;

        // Añade un offset de tiempo entre movimiento de patas
        for (int i = 1; i <= waitTimeBetweenEveryStep; ++i) yield return new WaitForFixedUpdate();

        if (IndexMoving.Contains(index)) IndexMoving.Remove(index);

    }

    void moveOppisteLeg(int index)
    {
        Vector3 targetPosition = legCubes[index].transform.position + Mathf.Clamp(velocity.magnitude * OverStepMultiplier, 0.0f, 1.5f) * (legCubes[index].transform.position - legTargets[index].transform.position) + velocity * OverStepMultiplier;
        StartCoroutine(step(index, targetPosition, true));
    }

    void rotateBody()
    {
        if (!enableBodyRotation) return;

        Vector3 v1 = legTargets[0].transform.position - legTargets[1].transform.position;
        Vector3 v2 = legTargets[2].transform.position - legTargets[3].transform.position;
        Vector3 normal = Vector3.Cross(v1, v2).normalized;
        Vector3 up = Vector3.Lerp(lastBodyUp, normal, 1f / (float)(BodySmoothness));
        transform.up = up;
        if (!rigidBodyController) transform.rotation = Quaternion.LookRotation(transform.parent.forward, up);
        lastBodyUp = transform.up;
    }
}
