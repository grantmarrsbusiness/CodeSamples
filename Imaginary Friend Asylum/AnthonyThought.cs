using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AnthonyThought : MonoBehaviour
{
	
	public string thought;
	public float time = 3;
	public AnthonyDialogueManager anthonyDialogueManager;
	public DialogueManager dialogueManager;
	
	private bool interactable;
	private float timer;
	
	private RaycastHit hit = new RaycastHit(); //information on the hit point of a raycast
	public LayerMask collisionLayers = ~(1 << 2); //the layers that the detectors (raycasts/linecasts) will collide with
	private float latestStep;
	public bool disabled;
	//private bool isInFront;
	
	[System.Serializable]
	public class Actions
	{
		public int step;
		public bool disabled = false;
		//public int setStep = 0;
		//public UnityEvent unityEvent;
	}
	public List<Actions> actions = new List<Actions>();
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		
		for (int i = 0; i < actions.Count; i++) {
			//if we are on the correct step
			if (actions[i].step <= dialogueManager.GetStep() && actions[i].step >= latestStep)
			{
				latestStep = actions[i].step;
				disabled = actions[i].disabled;
			}
		}
		
		if (transform.childCount > 0)
		{
			if (!disabled)
			{
				gameObject.transform.GetChild(0).gameObject.SetActive(true);
			}
			else
			{
				gameObject.transform.GetChild(0).gameObject.SetActive(false);
			}
		}
		
		//If player is close enough to interact
		if ((Physics.Linecast(dialogueManager.camera.transform.position, dialogueManager.camera.transform.position + dialogueManager.camera.transform.forward*2f, out hit, collisionLayers))
		&& hit.collider.name == gameObject.name && !disabled)
		{
			interactable = true;
			
		}
		//we cannot interact so hide "Click To Interact"
		else if (interactable)
		{
			interactable = false;
		}
		
		//If "Click To Interact" has appeared
		if (interactable)
		{
			for (int i = 0; i < actions.Count; i++) {
				//if we are on the correct step
				if (actions[i].step == latestStep)
				{
					anthonyDialogueManager.ActivateDialogue(thought, time);
				}
			}
			
		}
		
		
        
    }
	
}
