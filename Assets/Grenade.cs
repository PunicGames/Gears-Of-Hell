using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    Rigidbody rb;
    public Vector3 target;
    public Vector3 origin;
    public float timeUntilExplosion = 0.5f;
    public int damage = 40;
    private float anim;

    private float speed = 1.8f;
    private bool animEnded = false;

    [SerializeField] ParticleSystem vfx;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        origin = transform.position;
        target = new Vector3(target.x, target.y+0.1f, target.z);
    }

    void Update()
    {
        if (animEnded)
            return;
        anim += Time.deltaTime;
        transform.position = MathParabola.Parabola(origin, target, 3.8f, anim / speed);

        if (!animEnded)
            if (transform.position.y <= target.y+0.1f)
            {
                Debug.Log("reached");
                animEnded = true;
                Invoke(nameof(Explode), timeUntilExplosion);
            }
    }

    private void Explode()
    {
        //vfx.Play();

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        vfx.Play();
    }

}
