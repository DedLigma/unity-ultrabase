using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTest : Interactable
{
    [SerializeField]
    private GameObject door;

    private bool open = true;

    protected override void Interact ()
    {
        open = !open;
        door.SetActive(open);
    }
}
