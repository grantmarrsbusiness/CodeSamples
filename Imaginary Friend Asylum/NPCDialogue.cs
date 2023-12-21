using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NPCDialogue : MonoBehaviour
{
	
	public bool interactable = false;
	
	public DialogueManager dialogueManager;
	
	private RaycastHit hit = new RaycastHit(); //information on the hit point of a raycast
	public LayerMask collisionLayers = ~(1 << 2); //the layers that the detectors (raycasts/linecasts) will collide with
	
	public int actionNum;
	public int dialogueNum;
	public int optionNum;
	
	public float npcHeight = 1;
	
	private float latestStep;
	private bool disabled = false;
	
	private Quaternion baseRotation;
	public bool rotationActivated;
	
	private bool baseRotSet;
	
	private float activeTimer;
	
	
	[System.Serializable]
	public class NpcActions
	{
		public int step;
		public bool disabled = false;
		
		[System.Serializable]
		public class Dialogue
		{
			public int id;
			public string dialogueText = "";
			public string dialogueName = "<b>NAME:</b> ";
			public bool noRotate;
			
			[System.Serializable]
			public class Options
			{
				public string optionText = "";
				public AudioClip soundEffect;
				public bool reloadAndRemove = false;
				public bool exit = false;
				public int goToId = 0;
				public int setStep = 0;
				public UnityEvent unityEvent;
			}
			public List<Options> options = new List<Options>();
		}
		public List<Dialogue> dialogue = new List<Dialogue>();
	}
	public List<NpcActions> npcActions = new List<NpcActions>();
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		
		if (Time.timeScale == 1)
		{
			
			for (int i = 0; i < npcActions.Count; i++) {
				//if we are on the correct step
				if (npcActions[i].step <= dialogueManager.GetStep() && npcActions[i].step >= latestStep)
				{
					latestStep = npcActions[i].step;
					disabled = npcActions[i].disabled;
				}
			}
			
			for (int i = 0; i < npcActions.Count; i++) {
				//if we are on the correct step
				if (npcActions[i].step == latestStep)
				{
					
					//setting which action this is
					actionNum = i;
					
					//checking if we are on the right dialogue id
					for (int i2 = 0; i2 < npcActions[i].dialogue.Count; i2++) {
						//if talking hasn't started, set id to 0
						if (!dialogueManager.GetTalking())
						{
							//setting dialogue id to 0
							dialogueNum = 0;
							//setting number of options in this step
							optionNum = npcActions[i].dialogue[0].options.Count;
						}
						//if talking has started, check options for current id
						else if (npcActions[i].dialogue[i2].id == dialogueManager.GetDialogueID())
						{
							//setting dialogue id to current id
							dialogueNum = dialogueManager.GetDialogueID();
							//setting number of options in this step
							optionNum = npcActions[i].dialogue[i2].options.Count;
						}
					}
					
				}				
			}
			
			//Debug.DrawLine(dialogueManager.camera.transform.position, dialogueManager.camera.transform.position + dialogueManager.camera.transform.forward*2f, Color.red);
			//Debug.DrawLine(dialogueManager.camera.transform.position - dialogueManager.camera.transform.right/6, dialogueManager.camera.transform.position + dialogueManager.camera.transform.forward*2f - dialogueManager.camera.transform.right/6, Color.red);
			//Debug.DrawLine(dialogueManager.camera.transform.position + dialogueManager.camera.transform.right/6, dialogueManager.camera.transform.position + dialogueManager.camera.transform.forward*2f + dialogueManager.camera.transform.right/6, Color.red);
			
			//if interaction is not currently happening
			if (!dialogueManager.GetTalking() && !disabled)
			{
				if (Quaternion.Angle(baseRotation, transform.rotation) < 0.01f)
				{
					rotationActivated = false;
				}
				
				//If player is close enough to interact
				if ((Physics.Linecast(dialogueManager.camera.transform.position, dialogueManager.camera.transform.position + dialogueManager.camera.transform.forward*2f, out hit, collisionLayers)
				|| Physics.Linecast(dialogueManager.camera.transform.position - dialogueManager.camera.transform.right/6, dialogueManager.camera.transform.position + dialogueManager.camera.transform.forward*2f - dialogueManager.camera.transform.right/6, out hit, collisionLayers)
				|| Physics.Linecast(dialogueManager.camera.transform.position + dialogueManager.camera.transform.right/6, dialogueManager.camera.transform.position + dialogueManager.camera.transform.forward*2f + dialogueManager.camera.transform.right/6, out hit, collisionLayers))
				&& hit.collider.name == gameObject.name)
				{
					//show "Click To Interact"
					if (!dialogueManager.clickToInteract.activeSelf)
					{
						interactable = true;
						dialogueManager.clickToInteract.SetActive(true);
					}
				}
				//we cannot interact so hide "Click To Interact"
				else if (interactable && dialogueManager.clickToInteract.activeSelf)
				{
					if (!rotationActivated)
						baseRotation = transform.rotation;
					interactable = false;
					dialogueManager.clickToInteract.SetActive(false);
				}
				else
				{
					if (!rotationActivated)
						baseRotation = transform.rotation;
				}
				
				//If "Click To Interact" has appeared
				if (interactable && dialogueManager.clickToInteract.activeSelf && activeTimer > 0)
				{
					//If clicked, send values to dialogue manager
					if (Input.GetButtonDown("Fire1"))
					{
						
						Clicked();
						
					}
				}
				
				transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation, 4 * Time.deltaTime);
			}
			//we cannot interact so hide "Click To Interact"
			else if (interactable && dialogueManager.clickToInteract.activeSelf)
			{
				interactable = false;
				dialogueManager.clickToInteract.SetActive(false);
			}
			
			if (!baseRotSet)
			{
				baseRotation = transform.rotation;
				baseRotSet = true;
			}
			
			if (dialogueManager.clickToInteract.activeSelf)
			{
				activeTimer += Time.deltaTime;
			}
			else
			{
				activeTimer = 0;
			}
			
		}
		
    }
	
	//perform actions if clicked
	public void Clicked()
	{
		//We are now talking
		dialogueManager.SetTalking(true);
		rotationActivated = true;
		
		//Send values of current dialogue, IDs, etc
		dialogueManager.SetNPC(transform.GetComponent<NPCDialogue>(), npcHeight, actionNum, dialogueNum, optionNum);
		dialogueManager.SetDialogueText(npcActions[actionNum].dialogue[dialogueNum].dialogueText, npcActions[actionNum].dialogue[dialogueNum].dialogueName);
		dialogueManager.SetRotation(npcActions[actionNum].dialogue[dialogueNum].noRotate);
		
		//Showing dialogue system and hiding click to interact prompt
		//dialogueManager.dialogueSystem.SetActive(true);
		dialogueManager.clickToInteract.SetActive(false);
		
		
		//option1.GetComponent<Button>().Select();
	}
	
	//get the option texts
	public string GetDialogueText()
	{
		return npcActions[actionNum].dialogue[dialogueNum].dialogueText;
	}
	
	//get the option texts
	public string GetOptionText(int opt)
	{
		return npcActions[actionNum].dialogue[dialogueNum].options[opt].optionText;
	}
	
	//activate the events of dialogue option
	public void InvokeOption(int opt)
	{
		//setting the step
		if (npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].options[opt].setStep > 0)
		{
			dialogueManager.SetStep(npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].options[opt].setStep);
		}
		
		//activating events
		npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].options[opt].unityEvent.Invoke();
		
		//playing the sound
		if (npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].options[opt].soundEffect != null)
		{
			dialogueManager.GetComponent<AudioSource>().PlayOneShot(npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].options[opt].soundEffect);
		}
		
		//exiting
		if (npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].options[opt].exit)
		{
			//Hiding dialogue system and click to interact prompt
			dialogueManager.dialogueSystem.SetActive(false);
			dialogueManager.clickToInteract.SetActive(false);
			//Turning off interaction
			interactable = false;
			dialogueManager.SetTalking(false);
			//Reactivating the camera
			dialogueManager.ReactivateCam();
		}
		//remove option and reload
		else if (npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].options[opt].reloadAndRemove)
		{
			//removing option
			npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].options.RemoveAt(opt);
			//updating the option num
			optionNum -= 1;
			dialogueManager.SetOptionNum(optionNum);
			//reloading the dialogue text
			dialogueManager.SetDialogueText(npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].dialogueText, npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].dialogueName);
		}
		//go to next dialogue id
		else
		{
			//setting the dialogue id
			dialogueManager.SetDialogueID(npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].options[opt].goToId);
			//updating the option num
			optionNum = npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].options.Count;
			dialogueManager.SetOptionNum(optionNum);
			//reloading the dialogue text
			dialogueManager.SetDialogueText(npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].dialogueText, npcActions[dialogueManager.GetActionNum()].dialogue[dialogueManager.GetDialogueID()].dialogueName);
		}
		
	}
	
}
