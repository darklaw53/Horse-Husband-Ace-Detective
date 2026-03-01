using System.Collections;
using UnityEngine;

public class CameraEffectsManager : MonoBehaviour
{
    Camera cam;

    [Header("Negative Effect")]
    public Material negativeMaterial; 
    private bool isNegative;

    [Header("Flash Effect")]
    public CanvasGroup flashOverlay;

    [Header("Wobble Effect")]
    public float wobbleDuration = 1f;
    public float wobbleMagnitude = 0.1f;
    public float wobbleFrequency = 10f;

    [Header("Spin Effect")]
    public float spinDuration = 0.5f;
    public float spinSpeed = 720f;

    [Header("Zoom Effect")]
    public float zoomFOV = 30f;
    public float zoomDuration = 0.5f;

    [Header("Shake Effect")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    Coroutine shake, zoom, spin, negative, wobble, flash;

    private void Awake()
    {
        cam = Camera.main;
    }

    // ====== SHAKE ======
    public void Shake()
    {
        if (shake != null)
        {
            StopCoroutine(shake);
        }

        shake = StartCoroutine(ShakeRoutine(shakeDuration, shakeMagnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 originalPos = cam.transform.position;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    // ====== DRAMATIC ZOOM ======
    public void DramaticZoom()
    {
        if (zoom != null)
        {
            StopCoroutine(zoom);
        }

        zoom = StartCoroutine(DramaticZoomRoutine(zoomFOV, zoomDuration));
    }

    private IEnumerator DramaticZoomRoutine(float zoomFOV, float duration)
    {
        float originalFOV = cam.fieldOfView;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            cam.fieldOfView = Mathf.Lerp(originalFOV, zoomFOV, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = zoomFOV;
        yield return new WaitForSeconds(0.2f);

        elapsed = 0f;
        while (elapsed < duration)
        {
            cam.fieldOfView = Mathf.Lerp(zoomFOV, originalFOV, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = originalFOV;
    }

    // ====== SPIN ======
    public void Spin()
    {
        if (spin != null)
        {
            StopCoroutine(spin);
        }

        spin = StartCoroutine(SpinRoutine(spinDuration, spinSpeed));
    }

    private IEnumerator SpinRoutine(float duration, float speed)
    {
        Quaternion originalRot = cam.transform.localRotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            cam.transform.Rotate(Vector3.forward, speed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localRotation = originalRot;
    }

    // ====== NEGATIVE ======
    public void Negative(float duration = 0.5f)
    {
        if (negativeMaterial == null) return;

        if (negative != null)
        {
            StopCoroutine(negative);
        }

        negative = StartCoroutine(NegativeRoutine(duration));
    }

    private IEnumerator NegativeRoutine(float duration)
    {
        isNegative = true;
        yield return new WaitForSeconds(duration);
        isNegative = false;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (isNegative && negativeMaterial != null)
            Graphics.Blit(src, dest, negativeMaterial);
        else
            Graphics.Blit(src, dest);
    }

    // ====== WOBBLE ======
    public void Wobble()
    {
        if (wobble != null)
        {
            StopCoroutine(wobble);
        }

        wobble = StartCoroutine(WobbleRoutine(wobbleDuration, wobbleMagnitude, wobbleFrequency));
    }

    private IEnumerator WobbleRoutine(float duration, float magnitude, float frequency)
    {
        Vector3 originalPos = cam.transform.position;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float offsetX = Mathf.Sin(Time.time * frequency) * magnitude;
            float offsetY = Mathf.Cos(Time.time * frequency) * magnitude;
            cam.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }

    // ====== FLASH ======
    public void Flash(float duration = 0.2f)
    {
        if (flashOverlay == null) return;

        if (flash != null)
        {
            StopCoroutine(flash);
        }

        flash = StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        float halfDuration = duration / 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (elapsed < halfDuration)
                flashOverlay.alpha = Mathf.Lerp(0f, 1f, elapsed / halfDuration);
            else
                flashOverlay.alpha = Mathf.Lerp(1f, 0f, (elapsed - halfDuration) / halfDuration);

            elapsed += Time.deltaTime;

            yield return null;
        }

        flashOverlay.alpha = 0f; 
    }
}