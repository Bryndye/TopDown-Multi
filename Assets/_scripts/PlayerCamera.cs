using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform t_camera;
    public Vector2 SensivityMouse = new Vector2(5, 5);
    public Vector2 LimitCameraView = new Vector2(-80, 80);
    private float rotY;
    private float rotX;

    public void LookMouse(InputAction.CallbackContext context)
    {
        Vector2 _inputMouse = context.ReadValue<Vector2>();
        float x = _inputMouse.x * Time.deltaTime * SensivityMouse.x;
        float y = _inputMouse.y * Time.deltaTime * SensivityMouse.y;

        rotY += x;
        rotX -= y;
        rotX = Mathf.Clamp(rotX, LimitCameraView.x, LimitCameraView.y);

        t_camera.rotation = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = Quaternion.Euler(0, rotY, 0);

        //rotY -= _inputMouse.y * Time.deltaTime * -SensivityMouse.y;
        //rotY = Mathf.Clamp(rotY, LimitCameraView.x, LimitCameraView.y);

        //t_camera.localRotation = Quaternion.Euler(-rotY, 0, 0);

        //transform.localEulerAngles += new Vector3(0, _inputMouse.x * Time.deltaTime * SensivityMouse.x, 0);
    }
}
