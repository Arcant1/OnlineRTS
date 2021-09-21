using UnityEngine;

public class CameraFacing : MonoBehaviour
{
#pragma warning disable CS0108
    UnityEngine.Camera camera;
#pragma warning restore CS0108
    private void Awake()
    {
        camera = Camera.main;
    }

    void LateUpdate()
    {
        transform.forward = camera.transform.forward;
    }
}

