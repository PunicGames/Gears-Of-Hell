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

    public void UpdateBloom(bool opt) {
        bloomEffect.active = opt;
    }

    public void UpdateColorGrading(bool opt) {
        colorGrading.active = opt;
    }
}
