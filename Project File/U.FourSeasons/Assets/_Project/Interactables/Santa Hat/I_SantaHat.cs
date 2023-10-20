using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class I_SantaHat : MonoBehaviour
{
    public PostProcessVolume volume;   // Reference to the Post Processing Volume
    private ColorGrading colorGrading;
    
    public static event Action OnSantaHatTriggered = delegate { };  // Declare the event

    private void Awake()
    {
        // Try to get the ColorGrading settings from the referenced PostProcessVolume
        if (volume.profile.TryGetSettings(out colorGrading) == false)
        {
            Debug.LogError("No ColorGrading found in the PostProcessVolume.");
        }
        volume.gameObject.SetActive(true);   // Deactivate the Post Processing Volume
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger");
        // Start the coroutine to lerp the saturation value
        OnSantaHatTriggered.Invoke();  // Invoke the event
        StartCoroutine(LerpSaturation());
    }


    IEnumerator LerpSaturation()
    {
        float startSaturation = colorGrading.saturation.value;
        float elapsedTime = 0f;
        float lerpDuration = 5f;   // Duration in seconds for the lerp

        while (elapsedTime < lerpDuration)
        {
            // Lerp the saturation value
            float newSaturation = Mathf.Lerp(startSaturation, 0f, elapsedTime / lerpDuration);
            colorGrading.saturation.value = newSaturation;

            elapsedTime += Time.deltaTime;
            yield return null;   // Wait for the next frame
        }

        colorGrading.saturation.value = 0f;   // Ensure saturation is set to 0 at the end
        volume.gameObject.SetActive(false);   // Deactivate the Post Processing Volume

        Destroy(gameObject);  // Destroy the current game object
    }

}