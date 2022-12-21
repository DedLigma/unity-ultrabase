using NTC.Global.Cache;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class FootStepParser : MonoCache
{
    [SerializeField]
    private float checkDistance = 1f;

    [SerializeField]
    private string collectionName;

    [SerializeField]
    private List<AudioClip> walkSteps;

    [SerializeField]
    private List<AudioClip> sprintSteps;

    [SerializeField]
    private const string defaultCollectionName = "defaultCollection";

    [SerializeField]
    private List<AudioClip> defaultWalkSteps;

    [SerializeField]
    private List<AudioClip> defaultSprintSteps;

    private AudioSource audioSource;

    private void Start ()
    {
        audioSource = GetCached<AudioSource>();
        EnableDefaultFootsteps();
    }

   

    private void SwapFootStepCollections (List<AudioClip> walkList, List<AudioClip> sprintList)
    {
        walkSteps.Clear();
        sprintSteps.Clear();
        for(int i = 0; i < walkList.Count; i++) walkSteps.Add(walkList[i]);
        for(int i = 0; i < sprintList.Count; i++) sprintSteps.Add(sprintList[i]);
    }

    private AudioClip GetRandomClip (List<AudioClip> audioList)
    {
        int index = Random.Range(0, audioList.Count);
        return audioList[index];
    }

    public void WalkStep ()
    {
        AudioClip clip = GetRandomClip(walkSteps);
        audioSource.PlayOneShot(clip);
    }


    public void SprintStep ()
    {
        AudioClip clip = GetRandomClip(sprintSteps);
        audioSource.PlayOneShot(clip);
    }

    private void EnableDefaultFootsteps ()
    {
        SwapFootStepCollections(defaultWalkSteps, defaultSprintSteps);
        collectionName = defaultCollectionName;
    }

    public void CheckGround ()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z),
            Vector3.down, out hit, checkDistance))
        {
            if (hit.transform.GetComponent<FootStepCollection>() != null)
            {
                FootStepCollection StepCollection = hit.transform.GetComponent<FootStepCollection>();
                if(StepCollection.collection != null) 
                { 
                    if (StepCollection.collection.collectionName != collectionName)
                    {
                        SwapFootStepCollections(StepCollection.collection.walkClips, StepCollection.collection.sprintClips);
                        collectionName = StepCollection.collection.collectionName;
                    }
                } 
                else EnableDefaultFootsteps();
            }
            else EnableDefaultFootsteps();
        }
    }
}
