using Newtonsoft.Json.Bson;
using NTC.Global.Cache;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShake : MonoCache
{
    [Header("Movement")]
    [SerializeField]
    private AnimationCurve camCurveY;

    [SerializeField]
    private AnimationCurve camCurveX;

    [SerializeField]
    private float amplitude = 1f;

    [SerializeField]
    private float walkFrequance = 1f;

    [SerializeField]
    private float sprintFreaquance = 2f;

    [SerializeField]
    private float crouchFrequance = 0.5f;

    [SerializeField]
    private float resetCameraPosSpeed = 1f;

    [SerializeField]
    private Transform cam;

    [SerializeField]
    private Transform handItems;

    private float expiredTime;

    public void MoveShake (bool isMove, bool isSprint, bool isCrouch)
    {
        if (isMove)
        {
            if (isSprint) Shaker(sprintFreaquance);
            else if (isCrouch) Shaker(crouchFrequance);
            else Shaker(walkFrequance);
        }
        else if (cam.localPosition != Vector3.zero || handItems.localPosition != Vector3.zero)
        {
            cam.localPosition = Vector3.MoveTowards(cam.localPosition, Vector3.zero, resetCameraPosSpeed);
            handItems.localPosition = Vector3.MoveTowards(handItems.localPosition, Vector3.zero, resetCameraPosSpeed);
        }
        else
        {
            expiredTime = 0f;
        }
    }

    private void Shaker (float frequance)
    {
        expiredTime += Time.deltaTime / frequance;
        if (expiredTime > 1f)
            expiredTime = 0f;

        cam.localPosition = new Vector3(camCurveX.Evaluate(expiredTime) * amplitude,
            camCurveY.Evaluate(expiredTime) * amplitude, 0);
        handItems.localPosition = new Vector3(-camCurveX.Evaluate(expiredTime) * amplitude,
            -camCurveY.Evaluate(expiredTime) * amplitude, 0);
    }
}
