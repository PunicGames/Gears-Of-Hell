using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheneryBehaviour : MonoBehaviour
{
    [SerializeField] GameObject floor;
    [SerializeField] Color[] colors;
    [HideInInspector] Material materialBckUp;
    [HideInInspector] GameObject player;
    [HideInInspector] AudioSource[] bossPhasesMusic;

    [SerializeField] bool transitionTrigger = false;
    private bool lerping = false;
    [SerializeField] float duration = 1.0f;
    private float speed = 0.02f;
    private float value = 0;
    [SerializeField] private float maxVolume = 0.9f;

    // stores the hp at we want to change the phase
    [SerializeField] int[] phases;

    [SerializeField] List<Vector3> spawnPoints;
    [SerializeField] float spawnLimit = 10.0f;
    [SerializeField] GameObject littleSpider;
    [SerializeField] public float waitTimeSinceNextSpider = 30.0f;

    // phase index
    private int phase = 0;
    private Vector2Int fromTo = Vector2Int.zero;

    #region MonoBehaviour

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bossPhasesMusic = gameObject.GetComponents<AudioSource>();
        bossPhasesMusic[0].volume = maxVolume;
        InitializeNewMaterial();
        InitializeSpawnPoints();
    }

    private void Start()
    {
        Invoke("SpawnLittleSpider", GetRandomTime(waitTimeSinceNextSpider));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PhaseTransition();
    }

    #endregion

    #region Functionality

    private void SpawnLittleSpider()
    {
        Instantiate(littleSpider, GetSpawnPoint(), Quaternion.identity);
        Invoke("SpawnLittleSpider", GetRandomTime(waitTimeSinceNextSpider));
    }


    private float GetRandomTime(float k)
    {
        var max = k * 1.05f;
        var min = k * 0.95f;
        return Random.Range(min, max);
    }

    private Vector3 GetSpawnPoint()
    {
        var playerPos = player.transform.position;
        var spwnPntsCopy = new List<Vector3>(spawnPoints);

        spawnPoints.RemoveAll(x =>
        {
            var dist = Vector3.Distance(playerPos, x);
            return dist <= spawnLimit;
        });

        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    private void InitializeSpawnPoints()
    {
        spawnPoints = new List<Vector3>();
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child.position);
            Destroy(child.gameObject);
        }
    }

    public void CheckPhase(int hp)
    {
        for (int i = phase; i < phases.Length; i++)
        {
            if (hp <= phases[i])
            {
                GoToPhase(i + 1);
                return;
            }
        }
    }

    private void GoToPhase(int i)
    {
        phase = i;
        fromTo = new Vector2Int(i - 1, i);
        bossPhasesMusic[i].Play();
        transitionTrigger = true;
    }

    private void SetPhase(int i)
    {
        switch (i)
        {
            case 1:
                waitTimeSinceNextSpider = 15.0f;
                break;
            case 2:
                waitTimeSinceNextSpider = 5.0f;
                break;
        }
    }

    private void PhaseTransition()
    {
        if (transitionTrigger)
        {
            transitionTrigger = false;
            lerping = true;
            value = 0;
        }

        if (lerping)
            FadeColor(fromTo.x, fromTo.y);
    }

    private void SetVolume(int index, float value)
    {
        bossPhasesMusic[index].volume = value * maxVolume;
    }

    private void FadeColor(int from, int to)
    {
        if (value <= 1)
        {
            SetVolume(from, 1 - value);
            SetVolume(to, value);
            SetColor(Color.Lerp(colors[from], colors[to], value));
            value += (speed / duration);
        }
        else
        {
            bossPhasesMusic[phase - 1].Pause();
            lerping = false;
        }
    }

    private void SetColor(Color color)
    {
        materialBckUp.SetColor("_EmissionColor", color);
    }

    private void InitializeNewMaterial()
    {
        materialBckUp = new Material(floor.GetComponent<MeshRenderer>().material);
        SetColor(colors[0]);
        floor.GetComponent<MeshRenderer>().material = materialBckUp;
    }

    #endregion
}
