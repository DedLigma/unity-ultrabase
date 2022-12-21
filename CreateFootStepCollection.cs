using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New footstep collection", menuName = "Create New FootStep Collection")]
public class CreateFootStepCollection : ScriptableObject
{
    public string collectionName = null;
    public List<AudioClip> walkClips = new List<AudioClip>();
    public List<AudioClip> sprintClips = new List<AudioClip>();
}
