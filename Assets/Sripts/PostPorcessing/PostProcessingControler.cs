using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingControler : MonoBehaviour
{

    Bloom bloomEffect;
    PostProcessVolume volume;
    ColorGrading colorGrading;

    private void Awake()
    {
        volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloomEffect);
        volume.profile.TryGetSettings(out colorGrading);
    }

    private void Start()
    {
        // Bloom
        if (PlayerPrefs.GetInt("bloomEffect") == 0)
            bloomEffect.active = false;
        else
            bloomEffect.active = true;

        // Color Grading
        if (PlayerPrefs.GetInt("colorGrading") == 0)
            colorGrading.active = false;
        else
            colorGrading.active = true;

    }

    public void UpdateBloom(bool opt) {
        bloomEffect.active = opt;
    }

    public void UpdateColorGrading(bool opt) {
        colorGrading.active = opt;
    }
}
