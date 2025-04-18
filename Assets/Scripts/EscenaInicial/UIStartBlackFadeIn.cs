using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;

public class WakeUpEffect : MonoBehaviour
{
    public CanvasGroup fadeCanvas;
    public Volume postProcessVolume;
    public Transform cameraTarget;
    public StarterAssets.FirstPersonController playerController;

    private Vignette vignette;
    private DepthOfField depthOfField;

    void Start()
    {
        fadeCanvas.gameObject.SetActive(true);
        
        if (postProcessVolume.profile.TryGet(out vignette) && postProcessVolume.profile.TryGet(out depthOfField))
        {
            StartCoroutine(WakeUpSequence());
        }
        else
        {
            Debug.LogError("No se encontraron efectos de viñeta o desenfoque en el volumen de post-procesado.");
        }
    }

    IEnumerator WakeUpSequence()
    {
        playerController.enabled = false;

        fadeCanvas.alpha = 1;
        depthOfField.active = true;
        depthOfField.focusDistance.Override(0.1f);
        depthOfField.focalLength.Override(200f); // 🎯 Focal Length inicia en 200
        vignette.intensity.Override(0.45f);

        float t = 0;
        float duration = 6.5f; // ⏳ Duración total de 6.5 segundos

        while (t < duration)
        {
            // 🔄 Fade In
            fadeCanvas.alpha = Mathf.Lerp(1, 0, t / duration);

            // 📸 Desenfoque progresivo
            depthOfField.focusDistance.Override(Mathf.Lerp(0.1f, 10f, t / duration));

            // 🎯 Cambio del Focal Length (de 200 a 60)
            depthOfField.focalLength.Override(Mathf.Lerp(500f, 60f, t / duration));

            // 🤢 Movimiento de péndulo más natural
            float tiltAmount = Mathf.Lerp(15f, 0, t / duration);
            float pendulum = Mathf.Sin(Time.time * 2) * tiltAmount * 0.7f; // Movimiento más suave
            cameraTarget.localRotation = Quaternion.Euler(tiltAmount, pendulum, 0);

            // 🌫️ Viñeta desapareciendo progresivamente
            vignette.intensity.Override(Mathf.Lerp(0.45f, 0, t / duration));

            t += Time.deltaTime;
            yield return null;
        }

        // 🔄 Desactiva los efectos cuando termine
        vignette.active = false;
        depthOfField.active = false;
        playerController.enabled = true;
    }
}