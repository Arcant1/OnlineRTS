using UnityEngine;

public class CameraFacing : MonoBehaviour
{
#pragma warning disable CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva
    UnityEngine.Camera camera;
#pragma warning restore CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva
    private void Awake()
    {
        camera = Camera.main;
    }

    void LateUpdate()
    {
        transform.forward = camera.transform.forward;
    }
}

