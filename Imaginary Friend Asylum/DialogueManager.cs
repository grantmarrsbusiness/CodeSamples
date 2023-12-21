using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
	//the step
	public int step;
	public bool debugStep;
	
	//level changer
	public GameObject levelChanger;
	
	//the player and camera
	public Transform player;
	public Transform camera;
	
	//UI elements
	public GameObject clickToInteract;
	public GameObject clickToEnter;
	public GameObject clickToTake;
	public GameObject clickToExamine;
	public GameObject clickToTakePills;
	public GameObject dialogueSystem;
	
	//dialogue settings
	public string dialogueText;
	public NPCDialogue currentNPC;
	public float currentNPCHeight;
	public bool noRotation;
	
	//current dialogue happening
	public int actionNum;
	public int dialogueID;
	public int optionNum;
	public bool talking;
	
	//option texts
	public string option1Text;
	public string option2Text;
	public string option3Text;
	public string option4Text;
	
	//dialogue system components
	public TMP_Text dialogue;
	public TMP_Text option1;
	public TMP_Text option2;
	public TMP_Text option3;
	public TMP_Text option4;
	
	
    // Start is called before the first frame update
    void OnEnable()
    {
        if (!debugStep)
		{
			step = (int)PlayerPrefs.GetFloat("step");
		}
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (Time.timeScale == 1)
		{
			
			if (talking)
			{
				
				//setting the option texts
				//if 1 option
				if (optionNum == 1)
				{
					//show option 1
					option1.transform.parent.gameObject.SetActive(true);
					option1.SetText(currentNPC.GetOptionText(0));
					//hide other options
					option2.transform.parent.gameObject.SetActive(false);
					option3.transform.parent.gameObject.SetActive(false);
					option4.transform.parent.gameObject.SetActive(false);
				}
				//if 2 options
				else if (optionNum == 2)
				{
					//show option 1
					option1.transform.parent.gameObject.SetActive(true);
					option1.SetText(currentNPC.GetOptionText(0));
					//show option 2
					option2.transform.parent.gameObject.SetActive(true);
					option2.SetText(currentNPC.GetOptionText(1));
					//hide other options
					option3.transform.parent.gameObject.SetActive(false);
					option4.transform.parent.gameObject.SetActive(false);
				}
				//if 3 options
				else if (optionNum == 3)
				{
					//show option 1
					option1.transform.parent.gameObject.SetActive(true);
					option1.SetText(currentNPC.GetOptionText(0));
					//show option 2
					option2.transform.parent.gameObject.SetActive(true);
					option2.SetText(currentNPC.GetOptionText(1));
					//show option 3
					option3.transform.parent.gameObject.SetActive(true);
					option3.SetText(currentNPC.GetOptionText(2));
					//hide other options
					option4.transform.parent.gameObject.SetActive(false);
				}
				//if 4 options
				else if (optionNum == 4)
				{
					//show option 1
					option1.transform.parent.gameObject.SetActive(true);
					option1.SetText(currentNPC.GetOptionText(0));
					//show option 2
					option2.transform.parent.gameObject.SetActive(true);
					option2.SetText(currentNPC.GetOptionText(1));
					//show option 3
					option3.transform.parent.gameObject.SetActive(true);
					option3.SetText(currentNPC.GetOptionText(2));
					//show option 4
					option4.transform.parent.gameObject.SetActive(true);
					option4.SetText(currentNPC.GetOptionText(3));
				}
				
				//diabling the camera movement
				player.GetComponent<PlayerController>().enabled = false;
				camera.GetComponent<CameraController>().enabled = false;
				camera.GetChild(0).GetComponent<HeadBobController>().enabled = false;
				//enabling the mouse again
				Cursor.lockState = CursorLockMode.None;
				
				//making the camera look at npc
				Quaternion _lookRotation = 
				Quaternion.LookRotation(((currentNPC.transform.position + new Vector3(0,currentNPCHeight,0)) - camera.transform.position).normalized);
				//over time
				camera.transform.rotation = 
				Quaternion.Slerp(camera.transform.rotation, _lookRotation, Time.deltaTime * 4);
				
				//making the npc look at camera
				Quaternion _lookRotation2 = 
				Quaternion.LookRotation((new Vector3(camera.transform.position.x, currentNPC.transform.position.y, camera.transform.position.z) - currentNPC.transform.position).normalized);
				if (!noRotation)
				{
					//over time
					currentNPC.transform.rotation = 
					Quaternion.Slerp(currentNPC.transform.rotation, _lookRotation2, Time.deltaTime * 4);
				}
				
				//show dialogue
				dialogueSystem.SetActive(true);
			}
			
		}
    }
	
	//Increase the current step
	public void IncreaseStep()
	{
		step++;
	}
	//Set the current step
	public void SetStep(int stp)
	{
		step = stp;
	}
	//Get the current step
	public int GetStep()
	{
		return step;
	}
	
	
	//Set whether we are currently talking
	public void SetTalking(bool talk)
	{
		talking = talk;
	}
	//Get whether we are currently talking
	public bool GetTalking()
	{
		return talking;
	}
	
	
	//Set the current NPC we are talking to
	public void SetNPC(NPCDialogue npc, float height, int act, int id, int opt)
	{
		currentNPC = npc;
		currentNPCHeight = height;
		actionNum = act;
		dialogueID = id;
		optionNum = opt;
		
	}
	
	
	//Set which action the current NPC is on
	public void SetActionNum(int act)
	{
		actionNum = act;
	}
	//Get which action the current NPC is on
	public int GetActionNum()
	{
		return actionNum;
	}
	
	
	//Set the current dialogue ID
	public void SetDialogueID(int id)
	{
		dialogueID = id;
	}
	//Get the current dialogue ID
	public int GetDialogueID()
	{
		return dialogueID;
	}
	
	//Set the dialogue text
	public void SetDialogueText(string text, string name)
	{
		//disabling to prep typewriter effect
		dialogue.gameObject.SetActive(false);
		
		//setting the dialogue text
		dialogue.GetComponent<UITextTypeWriter>().story = text;
		dialogue.SetText(name + "");
		if ((name + text).Length <= 42)
		{
			dialogue.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -17, 0);
		}
		else if ((name + text).Length <= 74)
		{
			dialogue.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -8, 0);
		}
		else if ((name + text).Length <= 95)
		{
			dialogue.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -8, 0);
		}
		else
		{
			dialogue.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 2, 0);
		}
		
		//enabling to activate typewriter effect
		dialogue.gameObject.SetActive(true);
	}
	
	//Set the rotation
	public void SetRotation(bool noRot)
	{
		//checking if npc should rotate
		noRotation = noRot;
	}
	
	
	//Set how many options the current dialogue has
	public void SetOptionNum(int opt)
	{
		optionNum = opt;
	}
	//Get how many options the current dialogue has
	public int GetOptionNum()
	{
		return optionNum;
	}
	
	
	//Reactivate camera after exiting dialogue
	public void ReactivateCam ()
	{
		//enabling the camera movement
		player.GetComponent<PlayerController>().enabled = true;
		camera.GetComponent<CameraController>().enabled = true;
		camera.GetChild(0).GetComponent<HeadBobController>().enabled = true;
	}
	
	
	//activate the events of dialogue option 1
	public void InvokeOption1()
	{
		currentNPC.InvokeOption(0);
	}
	//activate the events of dialogue option 2
	public void InvokeOption2()
	{
		currentNPC.InvokeOption(1);
	}
	//activate the events of dialogue option 3
	public void InvokeOption3()
	{
		currentNPC.InvokeOption(2);
	}
	//activate the events of dialogue option 4
	public void InvokeOption4()
	{
		currentNPC.InvokeOption(3);
	}
	
	public void FadeBlack()
	{
		levelChanger.GetComponent<Animator>().CrossFade("Fade_Out_Instructions", 0f, -1, 0f);
	}
	
	public void FreezePlayer()
	{
		camera.transform.GetComponent<CameraController>().enabled = false;
		camera.transform.GetChild(0).GetComponent<HeadBobController>().enabled = false;
		player.GetComponent<PlayerController>().enabled = false;
	}
	
	public void UnfreezePlayer()
	{
		camera.transform.GetComponent<CameraController>().enabled = true;
		camera.transform.GetChild(0).GetComponent<HeadBobController>().enabled = true;
		player.GetComponent<PlayerController>().enabled = true;
	}
	
	public void UnlockCursor()
	{
		Cursor.lockState = CursorLockMode.None;
	}
	
}
