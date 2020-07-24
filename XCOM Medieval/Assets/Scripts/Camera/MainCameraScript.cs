using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the main gameplay camera, that will hover in isometric view, used during making moves by the player. Action camera will be a different script
/// The two cameras will switch between. This camera only follows a target.
/// </summary>
public class MainCameraScript : MonoBehaviour
{
    public Transform Target;
    public Vector3 TargetCamPosition;
    public bool freeCam;                 // if camera can move away from target.
    public float smoothValue = 0.3f;


    [Header("Camera Offsets")]
    public float height;
    public float xOffset;
    public float zOffset;

    Vector3 velocity = Vector3.zero;

    [Header("Movement Offsets")]
    public float moveSpeed;

    Transform CamParent;

    private void Start()
    {
        CamParent = transform.parent;
    }

    public void SetTarget(Transform target, bool freeCamAllowed)
    {
        Target = target;
        TargetCamPosition = Target.position;
        if (freeCamAllowed)
            freeCam = true;
        else
            freeCam = false;

        ResetCamOnTarget();
    }

    private void Update()
    {
        CamControl();
        InputControl();
    }


    void CamControl()
    {
        Vector3 pos = new Vector3();
        pos.x = TargetCamPosition.x + xOffset;
        pos.y = TargetCamPosition.y + height;
        pos.z = TargetCamPosition.z + zOffset;

        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothValue);
    }

    void InputControl()
    {
        if (freeCam)
        {
            if (Input.GetKey(KeyCode.D))
                TargetCamPosition.z += moveSpeed;
            if (Input.GetKey(KeyCode.A))
                TargetCamPosition.z += -1f*moveSpeed;
            if (Input.GetKey(KeyCode.W))
                TargetCamPosition.x += -1*moveSpeed;
            if (Input.GetKey(KeyCode.S))
                TargetCamPosition.x += moveSpeed;
        }

        if (Input.GetKeyDown(KeyCode.R))         // reset
            ResetCamOnTarget();

        // rotations
        //if (Input.GetKey(KeyCode.E))   // rotate Right
        //    CamParent.Rotate(new Vector3(0, 1, 0));
    }

    public void ResetCamOnTarget() { TargetCamPosition = Target.position; }
}
