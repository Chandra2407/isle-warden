using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraControllerScript : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;

    public float orthographicSizeDefault = 7f;
    public float orthographicSizeMin = 3f;
    public float orthographicSizeMax = 10f;

    public float zoomSpeed = 0.02f;
    public float smoothTime = 0.15f;

    private float targetZoom;
    private float zoomVelocity;

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();

        targetZoom = orthographicSizeDefault;

        LensSettings lens = cinemachineCamera.Lens;
        lens.OrthographicSize = targetZoom;
        cinemachineCamera.Lens = lens;
    }

    private void Update()
    {
        HandleZoom();
    }

    private void OnDisable()
    {
        LensSettings lens = cinemachineCamera.Lens;
        lens.OrthographicSize = orthographicSizeDefault;
        cinemachineCamera.Lens = lens;
    }

    private void HandleZoom()
    {
        if (Mouse.current == null)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(
                targetZoom,
                orthographicSizeMin,
                orthographicSizeMax);
        }

        LensSettings lens = cinemachineCamera.Lens;

        lens.OrthographicSize = Mathf.SmoothDamp(
            lens.OrthographicSize,
            targetZoom,
            ref zoomVelocity,
            smoothTime);

        cinemachineCamera.Lens = lens;
    }
}