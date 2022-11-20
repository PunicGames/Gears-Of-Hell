using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingControler : MonoBehaviour
{

    Bloom bloomEffect;
    PostProcessVolume volume;

    private void Awake()
    {
        volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloomEffect);
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("bloomEffect") == 0)
            bloomEffect.active = false;
        else
            bloomEffect.active = true;
    }

    public void UpdateBloom(bool opt) {
        bloomEffect.active = opt;
    }
}
