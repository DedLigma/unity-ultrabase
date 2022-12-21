using NTC.Global.Cache;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoCache
{
    //add or remove an InteractionEvent component to this gameobject
    public bool useEvents;
    //message to player when looking at an interactable
    public string promptMessage;


    public void BaseInteract ()
    {
        if (useEvents)
        {
            Get<InteractionEvent>().onInteract.Invoke();
        }
        Interact();
    }


    protected virtual void Interact ()
    {

    }
}
