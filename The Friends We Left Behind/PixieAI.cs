using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PixieAI : MonoBehaviour
{
	public bool passive = false;
	public bool easy = true;
	public bool normal = false;
	public bool hard = false;
	public bool insane = false;
	public bool lethal = false;
	private float minWait;
	private float maxWait;
	
	public GameObject difficultySelector;
	public GameObject jumpScareSelector;
	public GameObject pauseMenu;
	
	public GameObject flashlightOff;
	public GameObject flashlightSinging;
	public GameObject flashlightWalking;
	public GameObject flashlightWalkingAudience;
	public bool flashOn;
	
	private Transform[] pos;
	public int spot;
	public int lastSpot;
	public float waitTime;
	public GameObject cam;
	public GameObject mic;
	public GameObject camObject;
	public GameObject micObject;
	public GameObject ifLight;
	public bool canMove = true;
	public bool inView = false;
	public bool scared = false;
	
	public GameObject record;
	public GameObject curtain;
	public bool recordPlaying;
	public bool curtainOpen;
	
	private bool audioPlaying;
	
	IEnumerator co;
	public string ballZone = "";
	public bool hitWithBall = false;
	
	private bool looking = false;
	public float walkUpTimer;
	
	private Animator animator; //the "Animator" component of the script holder
	
	AudioSource audioSource;
	public AudioClip walk;
	public AudioClip loudWalk;
	public AudioClip door;
	public AudioClip climb;
	public AudioClip drop;
	public AudioClip curtainDrop;
	public AudioClip running;
	public AudioClip suspense;
	
	public float cameraTimer = 0.0f;
    
	private float timer = 0.0f;		//counting timer
	public bool jumpScare = false;
	public bool jumpScareActivate = false;
	
    // Start is called before the first frame update
    void Start() 
	{
		audioSource = GetComponent<AudioSource>();
		pos = GameObject.Find("PixiePositions").GetComponentsInChildren<Transform>();
		animator = GetComponent<Animator>();
		spot = 0;
		co = waiter(); // create an IEnumerator object
		MoveSpot();
		spot = 0;
		
		if (difficultySelector != null)
		{
			if (difficultySelector.GetComponent<DifficultyStatic>().GetDifficulty() == -1)
			{
				passive = true;
				easy = false;
				normal = false;
				hard = false;
				insane = false;
				lethal = false;
			}
			if (difficultySelector.GetComponent<DifficultyStatic>().GetDifficulty() == 0)
			{
				passive = false;
				easy = true;
				normal = false;
				hard = false;
				insane = false;
				lethal = false;
			}
			if (difficultySelector.GetComponent<DifficultyStatic>().GetDifficulty() == 1)
			{
				passive = false;
				easy = false;
				normal = true;
				hard = false;
				insane = false;
				lethal = false;
			}
			if (difficultySelector.GetComponent<DifficultyStatic>().GetDifficulty() == 2)
			{
				passive = false;
				easy = false;
				normal = false;
				hard = true;
				insane = false;
				lethal = false;
			}
			if (difficultySelector.GetComponent<DifficultyStatic>().GetDifficulty() == 3)
			{
				passive = false;
				easy = false;
				normal = false;
				hard = false;
				insane = true;
				lethal = false;
			}
			if (difficultySelector.GetComponent<DifficultyStatic>().GetDifficulty() == 4)
			{
				passive = false;
				easy = false;
				normal = false;
				hard = false;
				insane = false;
				lethal = true;
			}
		}
		
		if (easy || passive)
		{
			minWait = 10;
			maxWait = 15;
		}
		else if (normal)
		{
			minWait = 7;
			maxWait = 12;
		}
		else if (hard)
		{
			minWait = 3;
			maxWait = 10;
		}
		else if (insane)
		{
			minWait = 2;
			maxWait = 4;
		}
		else if (lethal)
		{
			minWait = 1;
			maxWait = 2;
		}
		
		//Start loading the jumpscare Scene asynchronously
		StartCoroutine(LoadScene());
	}

	
	public float flashTimer = 0;
	IEnumerator waiter()
    {
        if (recordPlaying)
		{
			flashOn = false;
			flashTimer = 0;
		}
		else
		{
			flashOn = false;
		}
		
        yield return new WaitForSeconds (waitTime);
		while(!canMove && !flashOn) yield return null;
		flashlightSinging.transform.GetChild(5).gameObject.SetActive(false);
		flashlightWalking.transform.GetChild(5).gameObject.SetActive(false);
		flashlightWalkingAudience.transform.GetChild(5).gameObject.SetActive(false);
        MoveSpot();
    }
	
	public bool goLeft = false;
	void MoveSpot()
	{
		recordPlaying = record.GetComponent<RecordControls>().playing;
		curtainOpen = curtain.GetComponent<CurtainControls>().open;
		
		if (spot == 0) //Starting
		{
			spot = 2; //AudienceLeft
		}
		else if (spot == 1) //AudienceBack
		{
			spot = 3; //AudienceFront
			
		}
		else if (spot == 2) //AudienceLeft
		{
			if (curtainOpen)
			{
				spot = 3; //AudienceFront
			}
			else
			{
				spot = 16; //AudienceLeft2
			}
		}
		else if (spot == 3) //AudienceFront
		{
			
			if (recordPlaying)
			{
				if (curtainOpen)
				{
					spot = 15; //AudienceSinging
					
					/*
					if (goLeft)
					{
						spot = 16; //AudienceLeft2
						goLeft = false;
					}
					else
					{
						spot = 17; //AudienceRight2
						goLeft = true;
					}
					*/
				}
				else
				{
					//spot = 18; //CurtainClimbOutside
					
					if (goLeft)
					{
						spot = 16; //AudienceLeft2
						goLeft = false;
					}
					else
					{
						spot = 17; //AudienceRight2
						goLeft = true;
					}
					
				}
			}
			else
			{
				if (curtainOpen)
				{
					if (curtain.GetComponent<CurtainControls>().percentage < 0.4f)
					{
						spot = 5; //StageFront
					}
					else {
						if (goLeft)
						{
							spot = 16; //AudienceLeft2
							goLeft = false;
						}
						else
						{
							spot = 17; //AudienceRight2
							goLeft = true;
						}
					}
				}
				else
				{
					//spot = 18; //CurtainClimbOutside
					
					if (goLeft)
					{
						spot = 16; //AudienceLeft2
						goLeft = false;
					}
					else
					{
						spot = 17; //AudienceRight2
						goLeft = true;
					}
					
				}
			}
			
		}
		else if (spot == 4) //AudienceRight
		{
			if (curtainOpen)
			{
				spot = 3; //AudienceFront
			}
			else
			{
				spot = 17; //AudienceRight2
			}
			
		}
		else if (spot == 5) //StageFront
		{
			
			if (recordPlaying)
			{
				if (curtainOpen)
				{
					if (goLeft)
					{
						spot = 16; //AudienceLeft2
						goLeft = false;
					}
					else
					{
						spot = 17; //AudienceRight2
						goLeft = true;
					}
				}
				else
				{
					if (goLeft)
					{
						spot = 16; //AudienceLeft2
						goLeft = false;
					}
					else
					{
						spot = 17; //AudienceRight2
						goLeft = true;
					}
				}
			}
			else
			{
				spot = 6; //StageCenter
			}
			
		}
		else if (spot == 6) //StageCenter
		{
			
			if (recordPlaying)
			{
				if (curtainOpen)
				{
					if (goLeft)
					{
						spot = 16; //AudienceLeft2
						goLeft = false;
					}
					else
					{
						spot = 17; //AudienceRight2
						goLeft = true;
					}
				}
				else
				{
					//jumpScare
					spot = 13; //StageCenterRun
				}
			}
			else
			{
				//jumpScare
				spot = 13; //StageCenterRun
			}
			
		}
		else if (spot == 7) //StageLeft
		{
			
			if (recordPlaying)
			{
				if (curtainOpen)
				{
					spot = 2; //AudienceLeft
				}
				else
				{
					//jumpScare
					spot = 11; //StageLeftRun
				}
			}
			else
			{
				//jumpScare
				spot = 11; //StageLeftRun
			}
			
		}
		else if (spot == 8) //StageRight
		{
			
			if (recordPlaying)
			{
				if (curtainOpen)
				{
					spot = 4; //AudienceRight
				}
				else
				{
					//jumpScare
					spot = 12; //StageRightRun
				}
			}
			else
			{
				//jumpScare
				spot = 12; //StageRightRun
			}
			
		}
		else if (spot == 9) //CurtainClimb
		{
			
			if (recordPlaying)
			{
				if (curtainOpen)
				{
					spot = 19; //CurtainFall
				}
				else
				{
					spot = 6; //StageCenter
				}
			}
			else
			{
				if (curtainOpen)
				{
					spot = 19; //CurtainFall
					//jumpScare
					//spot = 14; //CurtainTopRun
				}
				else
				{
					spot = 6; //StageCenter
				}
			}
			
		}
		else if (spot == 10) //CurtainTop
		{
			
			if (recordPlaying)
			{
				if (curtainOpen)
				{
					//jumpScare
					spot = 14; //CurtainTopRun
				}
				else
				{
					//jumpScare
					//spot = 14; //CurtainTopRun
					spot = 9; //CurtainClimb
				}
			}
			else
			{
				if (curtainOpen)
				{
					//jumpScare
					spot = 14; //CurtainTopRun
				}
				else
				{
					//jumpScare
					//spot = 14; //CurtainTopRun
					spot = 9; //CurtainClimb
				}
			}
			
		}
		else if (spot == 11) //StageLeftRun
		{
			audioSource.Stop();
			jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(1);
			jumpScareActivate = true;
			spot = 0; //Starting
		}
		else if (spot == 12) //StageRightRun
		{
			audioSource.Stop();
			jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(1);
			jumpScareActivate = true;
			spot = 0; //Starting
		}
		else if (spot == 13) //StageCenterRun
		{
			audioSource.Stop();
			jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(1);
			jumpScareActivate = true;
			spot = 0; //Starting
		}
		else if (spot == 14) //CurtainTopRun
		{
			audioSource.Stop();
			jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(1);
			jumpScareActivate = true;
			spot = 0; //Starting
		}
		else if (spot == 15) //AudienceSinging
		{
			
			if (recordPlaying)
			{
				if (curtainOpen)
				{
					if (goLeft)
					{
						spot = 16; //AudienceLeft2
						goLeft = false;
					}
					else
					{
						spot = 17; //AudienceRight2
						goLeft = true;
					}
				}
				else
				{
					spot = 18; //CurtainClimbOutside
					/*
					if (goLeft)
					{
						spot = 16; //AudienceLeft2
						goLeft = false;
					}
					else
					{
						spot = 17; //AudienceRight2
						goLeft = true;
					}
					*/
				}
			}
			else
			{
				if (curtainOpen)
				{
					if (curtain.GetComponent<CurtainControls>().percentage < 0.4f)
					{
						spot = 5; //StageFront
					}
					else {
						if (goLeft)
						{
							spot = 16; //AudienceLeft2
							goLeft = false;
						}
						else
						{
							spot = 17; //AudienceRight2
							goLeft = true;
						}
					}
				}
				else
				{
					spot = 18; //CurtainClimbOutside
					/*
					if (goLeft)
					{
						spot = 16; //AudienceLeft2
						goLeft = false;
					}
					else
					{
						spot = 17; //AudienceRight2
						goLeft = true;
					}
					*/
				}
			}
			
		}
		else if (spot == 16) //AudienceLeft2
		{
			lastSpot = 16;
			if (recordPlaying)
			{
				if (curtainOpen)
				{
					spot = 2; //AudienceLeft
				}
				else
				{
					//spot = 2; //AudienceLeft
					spot = 7; //StageLeft
				}
			}
			else
			{
				spot = 7; //StageLeft
			}
		}
		else if (spot == 17) //AudienceRight2
		{
			lastSpot = 17;
			if (recordPlaying)
			{
				if (curtainOpen)
				{
					spot = 4; //AudienceRight
				}
				else
				{
					//spot = 4; //AudienceRight
					spot = 8; //StageRight
				}
			}
			else
			{
				spot = 8; //StageRight
			}
		}
		else if (spot == 18) //CurtainClimbOutside
		{
			
			if (recordPlaying)
			{
				if (curtainOpen)
				{
					spot = 10; //CurtainTop
					//spot = 4; //AudienceRight
				}
				else
				{
					spot = 10; //CurtainTop
				}
			}
			else
			{
				spot = 10; //CurtainTop
			}
			
		}
		else if (spot == 19) //CurtainFall
		{
			lastSpot = 19;
			spot = 6; //StageCenter
			
		}
		
		
		//spot = RandomRangeExcept(1, 8, spot);
		
		//if (!jumpScare)
		//{
		
		//transform.position = pos[spot].position;
		//transform.rotation = pos[spot].rotation;
		
		if (spot != 18)
		{
			foreach (Renderer rend in GetComponentsInChildren<Renderer>())
				rend.enabled = false;
		}
		
		if (spot == 1) //AudienceBack
		{
			flashlightOff.SetActive(false);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(true);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			//audioSource.PlayOneShot(walk, 1.0f);
			audioSource.clip = walk;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = 0;
			animator.CrossFade("Catwalk", 0f, -1, 0f);
		}
		else if (spot == 2) //AudienceLeft
		{
			flashlightOff.SetActive(false);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(false);
			flashlightWalkingAudience.SetActive(true);
			audioSource.Stop();
			//audioSource.PlayOneShot(walk, 1.0f);
			audioSource.clip = walk;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = Random.Range (minWait, maxWait);
			animator.CrossFade("Catwalk", 0f, -1, 0f);
		}
		else if (spot == 3) //AudienceFront
		{
			flashlightOff.SetActive(false);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(true);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			//audioSource.PlayOneShot(walk, 1.0f);
			audioSource.clip = walk;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = Random.Range (minWait, maxWait);
			animator.CrossFade("Catwalk", 0f, -1, 0f);
		}
		else if (spot == 4) //AudienceRight
		{
			flashlightOff.SetActive(false);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(true);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			//audioSource.PlayOneShot(walk, 1.0f);
			audioSource.clip = walk;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = Random.Range (minWait, maxWait);
			animator.CrossFade("Catwalk", 0f, -1, 0f);
		}
		else if (spot == 5) //StageFront
		{
			flashlightOff.SetActive(false);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(true);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			//audioSource.PlayOneShot(loudWalk, 1.0f);
			audioSource.clip = loudWalk;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = Random.Range (2, 4);
			animator.CrossFade("Catwalk", 0f, -1, 0f);
		}
		else if (spot == 6) //StageCenter
		{
			flashlightOff.SetActive(true);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(false);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			//audioSource.PlayOneShot(drop, 1.0f);
			audioSource.clip = drop;
			audioSource.volume = 1.0f;
			audioSource.Play();
			if (!passive)
				waitTime = Random.Range (minWait, maxWait);
			else
				waitTime = 30;
			lastSpot = 6;
			scared = false;
			animator.CrossFade("Idle", 0f, -1, 0f);
		}
		else if (spot == 7) //StageLeft
		{
			flashlightOff.SetActive(true);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(false);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			if (!passive)
				waitTime = Random.Range (minWait, maxWait);
			else
				waitTime = 30;
			animator.CrossFade("Idle", 0f, -1, 0f);
		}
		else if (spot == 8) //StageRight
		{
			flashlightOff.SetActive(true);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(false);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			//audioSource.PlayOneShot(vent, 1.0f);
			if (!passive)
				waitTime = Random.Range (minWait, maxWait);
			else
				waitTime = 30;
			animator.CrossFade("Idle", 0f, -1, 0f);
		}
		else if (spot == 9) //CurtainClimb
		{
			flashlightOff.SetActive(true);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(false);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			//audioSource.PlayOneShot(climb, 1.0f);
			audioSource.clip = climb;
			audioSource.volume = 1.0f;
			audioSource.Play();
			if (passive)
			{
				waitTime = 8;
			}
			else if (easy)
			{
				waitTime = 4.75f;
			}
			else if (normal)
			{
				waitTime = 4f;
			}
			else if (hard)
			{
				waitTime = 3;
			}
			else if (insane)
			{
				waitTime = 2;
			}
			else if (lethal)
			{
				waitTime = 1;
			}
			//waitTime = Random.Range (2, 4);
			animator.CrossFade("Climb", 0f, -1, 0f);
		}
		else if (spot == 10) //CurtainTop
		{
			flashlightOff.SetActive(true);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(false);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			//audioSource.PlayOneShot(suspense, 1.0f);
			waitTime = Random.Range (minWait, maxWait);
			animator.CrossFade("Crouch", 0f, -1, 0f);
		}
		else if (spot == 11) //StageLeftRun
		{
			audioSource.PlayOneShot(suspense, 1.0f);
			audioSource.PlayOneShot(running, 1.0f);
			waitTime = 0.6f;
			animator.CrossFade("Run", 0f, -1, 0f);
		}
		else if (spot == 12) //StageRightRun
		{
			audioSource.PlayOneShot(suspense, 1.0f);
			audioSource.PlayOneShot(running, 1.0f);
			waitTime = 0.45f;
			animator.CrossFade("Run 1", 0f, -1, 0f);
		}
		else if (spot == 13) //StageCenterRun
		{
			audioSource.PlayOneShot(suspense, 1.0f);
			audioSource.PlayOneShot(running, 1.0f);
			waitTime = 0.3f;
			animator.CrossFade("Run 0 1", 0f, -1, 0f);
		}
		else if (spot == 14) //CurtainTopRun
		{
			audioSource.PlayOneShot(suspense, 1.0f);
			audioSource.PlayOneShot(running, 1.0f);
			waitTime = 0.45f;
			animator.CrossFade("Run 0 1 1", 0f, -1, 0f);
		}
		else if (spot == 15) //AudienceSinging
		{
			flashlightOff.SetActive(false);
			flashlightSinging.SetActive(true);
			flashlightWalking.SetActive(false);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			//audioSource.PlayOneShot(closet, 1.0f);
			if (!passive)
				waitTime = Random.Range (minWait, maxWait);
			else
				waitTime = 90;
			animator.CrossFade("Singing", 0f, -1, 0f);
		}
		else if (spot == 16) //AudienceLeft2
		{
			flashlightOff.SetActive(false);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(false);
			flashlightWalkingAudience.SetActive(true);
			audioSource.Stop();
			//audioSource.PlayOneShot(loudWalk, 1.0f);
			audioSource.clip = loudWalk;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = Random.Range (2, 4);
			animator.CrossFade("Catwalk", 0f, -1, 0f);
		}
		else if (spot == 17) //AudienceRight2
		{
			flashlightOff.SetActive(false);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(false);
			flashlightWalkingAudience.SetActive(true);
			audioSource.Stop();
			//audioSource.PlayOneShot(loudWalk, 1.0f);
			audioSource.clip = loudWalk;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = Random.Range (2, 4);
			animator.CrossFade("Catwalk", 0f, -1, 0f);
		}
		else if (spot == 18) //CurtainClimbOutside
		{
			flashlightOff.SetActive(true);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(false);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			//audioSource.PlayOneShot(climb, 1.0f);
			audioSource.clip = climb;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = Random.Range (2, 4);
			animator.CrossFade("Climb", 0f, -1, 0f);
		}
		else if (spot == 19) //CurtainFall
		{
			flashlightOff.SetActive(true);
			flashlightSinging.SetActive(false);
			flashlightWalking.SetActive(false);
			flashlightWalkingAudience.SetActive(false);
			audioSource.Stop();
			//audioSource.PlayOneShot(curtainDrop, 1.0f);
			audioSource.clip = curtainDrop;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = minWait/2;
			animator.CrossFade("Fall", 0f, -1, 0f);
		}
		
		transform.position = pos[spot].position;
		transform.rotation = pos[spot].rotation;
		
		if (spot != 18)
		{
			foreach (Renderer rend in GetComponentsInChildren<Renderer>())
				rend.enabled = true;
		}
		
		//}
		
		co = waiter();
		StartCoroutine (co);
		
		
	}
	
	int RandomRangeExcept (int min, int max, int except) {
		int newSpot = 0;
		do {
			newSpot = Random.Range (min, max + 1);
		} while (newSpot == except);
		return newSpot;
	}
	
	IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Level 2 Jumpscares");
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                //Wait to you press the space key to activate the Scene
                if (jumpScareActivate)
                    //Activate the Scene
                    asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
	

    // Update is called once per frame
    void Update()
    {
		
		if (Time.timeScale == 1)
		{
			if (!audioPlaying)
			{
				audioSource.Play();
				audioPlaying = true;
			}
			
			recordPlaying = record.GetComponent<RecordControls>().playing;
			curtainOpen = curtain.GetComponent<CurtainControls>().open;
			
			if (passive)
			{
				if (spot == 10)
				{
					if (curtain.GetComponent<CurtainControls>().percentage > 0)
					{
						StopCoroutine(co);
						MoveSpot();
					}
				}
				if (spot == 9)
				{
					animator.speed = 0.722f;
				}
				else
				{
					animator.speed = 1;
				}
			}
			
			if (spot == 3) //singing when record is playing
			{
				if (walkUpTimer > 7 && recordPlaying && curtainOpen)
				{
					flashlightSinging.transform.GetChild(5).gameObject.SetActive(false);
					flashlightWalking.transform.GetChild(5).gameObject.SetActive(false);
					flashlightWalkingAudience.transform.GetChild(5).gameObject.SetActive(false);
					StopCoroutine(co);
					MoveSpot();
				}
				walkUpTimer += Time.deltaTime;
			}
			else
			{
				walkUpTimer = 0;
			}
			
			if (spot == 15 && !curtainOpen)
			{
				StopCoroutine(co);
				MoveSpot();
			}
			
			if (spot == 19) //CurtainFall
			{
				transform.position = Vector3.Lerp(transform.position, pos[6].position, 3 * Time.deltaTime);
				//transform.rotation = pos[spot].rotation;
				
				if (cameraTimer > 0.5f)
				{
					scared = true;
				}
				cameraTimer += Time.deltaTime;
				
				if (scared) //trapped Pixie and ready to take picture
				{
					if (cam.GetComponent<Cam>().on && onScreen){
						jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(3);
						jumpScareActivate = true;
						//SceneManager.LoadScene("Level 1 Win");
					}
				}
			}
			else
			{
				scared = false;
				cameraTimer = 0;
			}
			
			if (spot == 9 && curtainOpen)
			{
				StopCoroutine(co);
				MoveSpot();
			}
			
			if (spot != 6) //StageCenter
			{
				
				flashlightOff.transform.GetChild(5).gameObject.SetActive(false);
			}
			
			//Pixie leaves stage when music turns on
			if ((spot == 5 || spot == 6 || spot == 7 || spot == 8) && !scared)
			{
				recordPlaying = record.GetComponent<RecordControls>().playing;
				curtainOpen = curtain.GetComponent<CurtainControls>().open;
				
				if (recordPlaying && (spot == 5 || curtainOpen))
				{
					StopCoroutine(co);
					if (spot == 5)
						spot = 6;
					MoveSpot();
				}
			}
			
			if (spot == 18)
			{
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = false;
			}
			else
			{
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = true;
			}
			
			/*
			Vector3 screenPoint2 = Camera.main.WorldToViewportPoint(transform.position);
			bool onScreen2 = screenPoint2.z > 0 && screenPoint.x > 1 - 0.85f && screenPoint2.x < 0.85f && screenPoint2.y > 1 - 0.85f && screenPoint2.y < 0.85f;
			
			if (spot == 13 && (cam.GetComponent<Cam>().on) && onScreen2)
			{
				canMove = true;
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					//rend.enabled = false;
				
				ballZone = "RDoor";
				StopCoroutine(co);
				MoveSpot();
			}
			
			
			if (hitWithBall && spot != 9 && spot != 10 && spot != 14)
			{
				canMove = true;
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					//rend.enabled = false;
				
				StopCoroutine(co);
				if (spot == 1 || spot == 2 || spot == 4 || spot == 15 || spot == 16 || spot == 17)
				{
					spot = 4; //Closet
				}
				else if (spot == 13)
				{
					spot = 13; //Main
				}
				else
				{
					spot = 3; //RDoor
				}
				animator.CrossFade("Idle1", 0f, -1, 0f);
				MoveSpot();
			}
			
			
			//Walking or crawling for set amount of time so needs to move
			if (spot == 1 || spot == 6 || spot == 11 || spot == 15 || spot == 16)
			{
				canMove = true;
			}
			*/
			
			//Running at you
			if (!jumpScare && (spot == 11 || spot == 12 || spot == 13 || spot == 14))
			{
				jumpScare = true;
			}
			if (jumpScare)
			{
				pauseMenu.GetComponent<PauseMenu>().locked = true;
				
				canMove = true;
				/*
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = true;
				foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
					col.enabled = true;
				*/
				
				if (timer > waitTime - 0.4f && timer <= waitTime - 0.1f){
					ifLight.GetComponent<Light>().intensity = 4;
					
					camObject.transform.parent = Camera.main.transform;
					camObject.GetComponent<CameraController>().enabled = false;
					camObject.transform.localPosition = Vector3.Lerp(camObject.transform.localPosition, new Vector3(camObject.transform.localPosition.x, -4, camObject.transform.localPosition.z), 2 * Time.deltaTime);
					cam.SetActive(false);
					
					micObject.transform.parent = Camera.main.transform;
					micObject.GetComponent<CameraController>().enabled = false;
					micObject.transform.localPosition = Vector3.Lerp(micObject.transform.localPosition, new Vector3(micObject.transform.localPosition.x, -4, micObject.transform.localPosition.z), 2 * Time.deltaTime);
					
					Camera.main.GetComponent<CameraController>().enabled = false;
					//Camera.main.transform.LookAt(transform);
					Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,1,0)) - Camera.main.transform.position);
					//lookRotation.y = 12;
					
					Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 20 * Time.deltaTime);
				}
				else if (timer > waitTime - 0.1f){
					cam.GetComponent<Light>().intensity = 0;
					Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,20,0)) - Camera.main.transform.position);
					//lookRotation.y = 12;
					
					Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 3 * Time.deltaTime);
				}
				timer += Time.deltaTime;
				
			}
			
			//Mic is enabled/disabled
			if (mic.GetComponent<Mic>().on)
				audioSource.volume = Mathf.Lerp(audioSource.volume, 1f, 10 * Time.deltaTime);
			else
				audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, 10 * Time.deltaTime);
			
		}
		else
		{
			if (audioSource.isPlaying)
				audioPlaying = false;
			audioSource.Pause();
		}
		
    }
	
	public bool onScreen;
	
	void FixedUpdate(){
		
		Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
		onScreen = screenPoint.z > 0 && screenPoint.x > -1 && screenPoint.x < 2 && screenPoint.y > -1 && screenPoint.y < 2;
		//bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		
        if (!flashOn && !scared && (!cam.GetComponent<Cam>().on && cam.GetComponent<Light>().intensity <= 0.01f || !onScreen)){
			canMove = true;
			/*
			foreach (Renderer rend in GetComponentsInChildren<Renderer>())
				rend.enabled = false;
			foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
				col.enabled = false;
			*/
		}
		else{
			canMove = false;
			/*
			foreach (Renderer rend in GetComponentsInChildren<Renderer>())
				rend.enabled = true;
			foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
				col.enabled = true;
			*/
		}
		
		if (flashTimer >= 0.6f && recordPlaying)
		{
			flashOn = true;
		}
		//turning flashlight on/ff
		if (flashOn)
		{
			flashlightSinging.transform.GetChild(5).gameObject.SetActive(true);
			flashlightWalking.transform.GetChild(5).gameObject.SetActive(true);
			flashlightWalkingAudience.transform.GetChild(5).gameObject.SetActive(true);
		}
		else
		{
			flashlightSinging.transform.GetChild(5).gameObject.SetActive(false);
			flashlightWalking.transform.GetChild(5).gameObject.SetActive(false);
			flashlightWalkingAudience.transform.GetChild(5).gameObject.SetActive(false);
		}
		flashTimer += Time.deltaTime;
		
		if (spot == 6 && scared)
		{
			flashlightOff.transform.GetChild(5).gameObject.SetActive(true);
		}
		else
		{
			flashlightOff.transform.GetChild(5).gameObject.SetActive(false);
		}
		
	}
}
