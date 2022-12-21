using NTC.Global.Cache;
using UniRx;
using UnityEngine;

public class PlayerInteract : MonoCache
{
    [SerializeField]
    private Transform cam;

    [SerializeField]
    private float interactRayDistance = 1f;

    [SerializeField]
    private LayerMask mask;

    

    public ReactiveProperty<string> interactText = new ReactiveProperty<string>(string.Empty);

    public void InteractChecker (bool interactTrigger)
    {
        Ray ray = new Ray(cam.position, cam.forward * interactRayDistance);
        Debug.DrawRay(cam.position, cam.forward * interactRayDistance, Color.red);

        RaycastHit hitinfo;
        if (Physics.Raycast(ray, out hitinfo, interactRayDistance))
        {
            if (Physics.Raycast(ray, interactRayDistance, mask))
            {
                if (hitinfo.collider.GetComponent<Interactable>() != null)
                {

                    Interactable interactable = hitinfo.collider.GetComponent<Interactable>();
                    interactText.Value = interactable.promptMessage;
                    if (interactTrigger) interactable.BaseInteract();
                }
            }
            else interactText.Value = string.Empty;
        }
        else interactText.Value = string.Empty;
    }
}
