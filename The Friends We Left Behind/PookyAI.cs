using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PookyAI : MonoBehaviour
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
	
	public Transform pos;
	public int playerSpot;
	public int gateSpot;
	public int spot;
	public int lastSpot = 6;
	public float waitTime;
	public GameObject cam;
	public GameObject mic;
	public GameObject flashlight;
	public GameObject flashlightObject;
	public GameObject camObject;
	public GameObject micObject;
	public GameObject distractionObject;
	public GameObject ifLight;
	public GameObject ifLight2;
	public bool canMove = true;
	public bool inView = false;
	public bool scared = false;
	public float addY = 0;
	public Transform fadeObject;
	
	public Transform leftHand;
	public Transform rightHand;
	
	public Transform gateControls;
	public bool chasing = false;
	public bool chasingNotStarted = true;
	public bool chasingGate = false;
	public bool chasingGateNotStarted = true;
	
	public bool runAway = false;
	
	public bool stuckInGate = false;
	public bool stuckInGateAdjustments = false;
	public bool reachedCenter = false;
	
	public bool playerMoved = false;
	
	IEnumerator co;
	public string ballZone = "";
	public bool hitWithBall = false;
	
	public Vector3 gatePosition = new Vector3(2.76f, 0.22f, -0.115f);
	public Vector3 gateRotation = new Vector3(22.66f, 90.346f, 0f);
	
	public float walkTimer;
	public bool reachedCenterWalk;
	public bool oneSpotAway;
	public bool twoSpotsAway;
	
	private bool looking = false;
	
	private Animator animator; //the "Animator" component of the script holder
	
	AudioSource audioSource;
	public AudioClip walk;
	public AudioClip loudWalk;
	public AudioClip door;
	public AudioClip stuck;
	public AudioClip climb;
	public AudioClip drop;
	public AudioClip curtainDrop;
	public AudioClip running;
	public AudioClip suspense;
	
	private float stuckTimer = 0.0f;
    
	private float timer = 0.0f;		//counting timer
	public bool jumpScare = false;
	public bool jumpScareActivate = false;
	
	private RaycastHit hit = new RaycastHit(); //information on the hit point of a raycast
	public LayerMask collisionLayers = ~((1 << 2) | (1 << 4)); //the layers that the detectors (raycasts/linecasts) will collide with
	
    // Start is called before the first frame update
    void Start() 
	{
		audioSource = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		//MoveSpot();
		
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
			//minWait = 13;
			//maxWait = 20;
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
	
	
	IEnumerator waiter()
    {
		yield return new WaitForSeconds (waitTime);
		while(!canMove || !fadeObject.GetComponent<FadeInOut>().clickable) yield return null;
		MoveSpot();
    }
	
	void MoveSpot()
	{
		
		if (playerMoved && !chasing && !chasingGate)
		{
			int oldSpot = spot;
			
			if (spot == 6) //Center
			{
				/*
				int rand = Random.Range(0,7);
				spot = rand;
				*/
				spot = playerSpot;
			}
			else if (spot == 0) //Porch
			{
				if (playerSpot == 1 || playerSpot == 2)
				{
					spot = 1; //RightBushes
				}
				if (playerSpot == 5 || playerSpot == 4)
				{
					spot = 5; //LeftBushes
				}
				if (playerSpot == 3)
				{
					int rand = Random.Range(1,3);
					if (rand == 1)
						spot = 1; //RightBushes
					else if (rand == 2)
						spot = 5; //LeftBushes
				}
				if (playerSpot == 6) //Center
				{
					spot = 6; //Center
				}
			}
			else if (spot == 1) //RightBushes
			{
				if (playerSpot == 2 || playerSpot == 3)
				{
					spot = 2; //Sign
				}
				if (playerSpot == 0 || playerSpot == 5)
				{
					spot = 0; //Porch
				}
				if (playerSpot == 4)
				{
					int rand = Random.Range(1,3);
					if (rand == 1)
						spot = 2; //Sign
					else if (rand == 2)
						spot = 0; //Porch
				}
				if (playerSpot == 6) //Center
				{
					spot = 6; //Center
				}
			}
			else if (spot == 2) //Sign
			{
				if (playerSpot == 3 || playerSpot == 4)
				{
					spot = 3; //HidingBush
				}
				if (playerSpot == 1 || playerSpot == 0)
				{
					spot = 1; //RightBushes
				}
				if (playerSpot == 5)
				{
					int rand = Random.Range(1,3);
					if (rand == 1)
						spot = 3; //HidingBush
					else if (rand == 2)
						spot = 1; //RightBushes
				}
				if (playerSpot == 6) //Center
				{
					spot = 6; //Center
				}
			}
			else if (spot == 3) //HidingBush
			{
				if (playerSpot == 4 || playerSpot == 5)
				{
					spot = 4; //Fence
				}
				if (playerSpot == 2 || playerSpot == 1)
				{
					spot = 2; //Sign
				}
				if (playerSpot == 0)
				{
					int rand = Random.Range(1,3);
					if (rand == 1)
						spot = 4; //Fence
					else if (rand == 2)
						spot = 2; //Sign
				}
				if (playerSpot == 6) //Center
				{
					spot = 6; //Center
				}
			}
			else if (spot == 4) //Fence
			{
				if (playerSpot == 5 || playerSpot == 0)
				{
					spot = 5; //LeftBushes
				}
				if (playerSpot == 3 || playerSpot == 2)
				{
					spot = 3; //HidingBush
				}
				if (playerSpot == 1)
				{
					int rand = Random.Range(1,3);
					if (rand == 1)
						spot = 5; //LeftBushes
					else if (rand == 2)
						spot = 3; //HidingBush
				}
				if (playerSpot == 6) //Center
				{
					spot = 6; //Center
				}
			}
			else if (spot == 5) //LeftBushes
			{
				if (playerSpot == 0 || playerSpot == 1)
				{
					spot = 0; //Porch
				}
				if (playerSpot == 4 || playerSpot == 3)
				{
					spot = 4; //Fence
				}
				if (playerSpot == 2)
				{
					int rand = Random.Range(1,3);
					if (rand == 1)
						spot = 0; //Porch
					else if (rand == 2)
						spot = 4; //Fence
				}
				if (playerSpot == 6) //Center
				{
					spot = 6; //Center
				}
			}
			
			//spot = RandomRangeExcept(1, 8, spot);
			
			//if (!jumpScare)
			//{
			
			if (spot == playerSpot && passive)
			{
				spot = oldSpot;
			}
			
			
			if (spot == 0) //Porch
			{
				audioSource.Stop();
				//audioSource.PlayOneShot(walk, 1.0f);
				waitTime = Random.Range (minWait, maxWait);
				//animator.CrossFade("Catwalk", 0f, -1, 0f);
				if (playerSpot == 0)
				{
					animator.CrossFade("Run", 0f, -1, 0f);
					audioSource.PlayOneShot(suspense, 1.0f);
					audioSource.PlayOneShot(running, 1.0f);
					addY = 1;
					jumpScare = true;
				}
				else
				{
					animator.CrossFade("Idle", 0f, -1, 0f);
				}
			}
			else if (spot == 1) //RightBushes
			{
				audioSource.Stop();
				//audioSource.PlayOneShot(walk, 1.0f);
				waitTime = Random.Range (minWait, maxWait);
				//animator.CrossFade("Catwalk", 0f, -1, 0f);
				if (playerSpot == 1)
				{
					animator.CrossFade("Run", 0f, -1, 0f);
					audioSource.PlayOneShot(suspense, 1.0f);
					audioSource.PlayOneShot(running, 1.0f);
					jumpScare = true;
				}
				else
				{
					animator.CrossFade("Idle", 0f, -1, 0f);
				}
			}
			else if (spot == 2) //Sign
			{
				audioSource.Stop();
				//audioSource.PlayOneShot(walk, 1.0f);
				waitTime = Random.Range (minWait, maxWait);
				//animator.CrossFade("Catwalk", 0f, -1, 0f);
				if (playerSpot == 2)
				{
					animator.CrossFade("Run", 0f, -1, 0f);
					audioSource.PlayOneShot(suspense, 1.0f);
					audioSource.PlayOneShot(running, 1.0f);
					jumpScare = true;
				}
				else
				{
					animator.CrossFade("Idle", 0f, -1, 0f);
				}
			}
			else if (spot == 3) //HidingBush
			{
				audioSource.Stop();
				//audioSource.PlayOneShot(walk, 1.0f);
				waitTime = Random.Range (minWait, maxWait);
				//animator.CrossFade("Catwalk", 0f, -1, 0f);
				if (playerSpot == 3)
				{
					animator.CrossFade("Run", 0f, -1, 0f);
					audioSource.PlayOneShot(suspense, 1.0f);
					audioSource.PlayOneShot(running, 1.0f);
					jumpScare = true;
				}
				else
				{
					animator.CrossFade("Idle", 0f, -1, 0f);
				}
			}
			else if (spot == 4) //Fence
			{
				audioSource.Stop();
				//audioSource.PlayOneShot(walk, 1.0f);
				waitTime = Random.Range (minWait, maxWait);
				//animator.CrossFade("Catwalk", 0f, -1, 0f);
				if (playerSpot == 4)
				{
					animator.CrossFade("Run", 0f, -1, 0f);
					audioSource.PlayOneShot(suspense, 1.0f);
					audioSource.PlayOneShot(running, 1.0f);
					jumpScare = true;
				}
				else
				{
					animator.CrossFade("Idle", 0f, -1, 0f);
				}
			}
			else if (spot == 5) //LeftBushes
			{
				audioSource.Stop();
				//audioSource.PlayOneShot(walk, 1.0f);
				waitTime = Random.Range (minWait, maxWait);
				//animator.CrossFade("Catwalk", 0f, -1, 0f);
				if (playerSpot == 5)
				{
					animator.CrossFade("Run", 0f, -1, 0f);
					audioSource.PlayOneShot(suspense, 1.0f);
					audioSource.PlayOneShot(running, 1.0f);
					jumpScare = true;
				}
				else
				{
					animator.CrossFade("Idle", 0f, -1, 0f);
				}
			}
			else if (spot == 6) //Center
			{
				audioSource.Stop();
				//audioSource.PlayOneShot(walk, 1.0f);
				waitTime = Random.Range (minWait, maxWait);
				//animator.CrossFade("Catwalk", 0f, -1, 0f);
				if (playerSpot == 6)
				{
					animator.CrossFade("Run", 0f, -1, 0f);
					audioSource.PlayOneShot(suspense, 1.0f);
					audioSource.PlayOneShot(running, 1.0f);
					jumpScare = true;
				}
				else
				{
					animator.CrossFade("Idle", 0f, -1, 0f);
				}
			}
			
			if (!jumpScare)
			{
				transform.position = pos.GetChild(spot).position;
			}
			//}
		
			co = waiter();
			StartCoroutine (co);
		}
	}
	
	public void IllegalPickUp ()
	{
		if (!passive)
		{
			audioSource.PlayOneShot(suspense, 1.0f);
			audioSource.PlayOneShot(running, 1.0f);
			if (!oneSpotAway)
				spot = 6;
			chasing = false;
			StopCoroutine (co);
			MoveSpot();
		}
		else
		{
			walkTimer = 90;
		}
		/*
		transform.position = pos.GetChild(6).position;
		audioSource.Stop();
		waitTime = Random.Range (minWait, maxWait);
		animator.CrossFade("Run", 0f, -1, 0f);
		
		jumpScare = true;
		
		co = waiter();
		StartCoroutine (co);
		*/
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
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Level 5 Jumpscares");
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
	
	public float timey;
	public bool clickedToRunAway = false;

    // Update is called once per frame
    void Update()
    {
		timey = Time.deltaTime;
		if (Time.timeScale == 1)
		{
			
			if (!clickedToRunAway && passive && playerSpot == 7 && gateControls.GetComponent<GateControls>().percentage < 0.01f && !stuckInGate && !runAway)
			{
				pauseMenu.GetComponent<PauseMenu>().locked = false;
				reachedCenterWalk = false;
				chasing = true;
				transform.position = new Vector3(pos.GetChild(6).position.x, transform.position.y, pos.GetChild(6).position.z);
				if (jumpScare)
				{
					animator.CrossFade("LookingAround", 0f, -1, 0f);
					animator.SetBool("Looking", true);
				}
				audioSource.Stop();
				jumpScare = false;
				jumpScareActivate = false;
				spot = 6;
				walkTimer = 0;
				
				
				playerMoved = false;
				
				
				ifLight.GetComponent<Light>().intensity = 0;
				ifLight2.GetComponent<Light>().intensity = 0;
				
				Camera.main.GetComponent<CameraController>().enabled = true;
				
				flashlightObject.transform.parent = rightHand;
				flashlightObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
				flashlightObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
				flashlightObject.GetComponent<CameraController>().enabled = true;
				
				camObject.transform.parent = rightHand;
				camObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
				camObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
				camObject.GetComponent<CameraController>().enabled = true;
				cam.SetActive(true);
				
				micObject.transform.parent = leftHand;
				micObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
				micObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
				micObject.GetComponent<CameraController>().enabled = true;
				
				distractionObject.transform.parent = leftHand;
				distractionObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
				distractionObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
				distractionObject.transform.GetChild(0).GetComponent<Distraction>().enabled = true;
				distractionObject.GetComponent<CameraController>().enabled = true;
				
				
				
			}
			
			if (chasing)
			{
				if (chasingNotStarted)
				{
					animator.CrossFade("Walk", 0f, -1, 0f);
					//audioSource.PlayOneShot(suspense, 1.0f);
					//audioSource.PlayOneShot(running, 1.0f);
					animator.SetBool("Looking", false);
					walkTimer = 0;
					
					chasingNotStarted = false;
				}
				
				
				
				if (Vector3.Distance(new Vector3(pos.GetChild(6).position.x, transform.position.y, pos.GetChild(6).position.z), transform.position) < 0.3f)
				{
					if (!oneSpotAway && !reachedCenterWalk)
					{
						spot = 6;
						if (spot == playerSpot && !jumpScare && (!chasing || spot == 6) && canMove && fadeObject.GetComponent<FadeInOut>().clickable)
						{
							jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(1);
							jumpScareActivate = true;
						}
					}
					reachedCenterWalk = true;
				}
				
				if (!twoSpotsAway && Mathf.Abs(leftHand.GetComponent<LeftHand2>().currentDistractionSpot - spot) <= 1 || leftHand.GetComponent<LeftHand2>().currentDistractionSpot == 5 && spot == 0 || leftHand.GetComponent<LeftHand2>().currentDistractionSpot == 0 && spot == 5)
				{
					//oneSpotAway = true;
				}
				else
				{
					oneSpotAway = false;
				}
				
				//if reached center or one spot away
				if (reachedCenterWalk && (walkTimer > minWait || insane || lethal) || oneSpotAway || passive && playerSpot == 6 && reachedCenterWalk || passive && playerSpot == 7 && gateControls.GetComponent<GateControls>().percentage >= 0.01f) //
				{
					animator.SetBool("Looking", false);
					//spot = leftHand.GetComponent<LeftHand2>().currentDistractionSpot;
					
					if (!oneSpotAway)
					{
						spot = leftHand.GetComponent<LeftHand2>().currentDistractionSpot;
						if (leftHand.GetComponent<LeftHand2>().currentDistractionSpot == 0)
						{
							animator.speed += 0.00f * (Time.deltaTime/0.0164452f);
						}
						else if (leftHand.GetComponent<LeftHand2>().currentDistractionSpot == 1)
						{
							animator.speed += 0.011f * (Time.deltaTime/0.0164452f);
						}
						else if (leftHand.GetComponent<LeftHand2>().currentDistractionSpot == 2)
						{
							animator.speed += 0.008f * (Time.deltaTime/0.0164452f);
						}
						else if (leftHand.GetComponent<LeftHand2>().currentDistractionSpot == 3)
						{
							animator.speed += 0.005f * (Time.deltaTime/0.0164452f);
						}
						else if (leftHand.GetComponent<LeftHand2>().currentDistractionSpot == 4)
						{
							animator.speed += 0.011f * (Time.deltaTime/0.0164452f);
						}
						else if (leftHand.GetComponent<LeftHand2>().currentDistractionSpot == 5)
						{
							animator.speed += 0.013f * (Time.deltaTime/0.0164452f);
						}
						
						if (passive && playerSpot == 7)
						{
							animator.speed += 0.001f * (Time.deltaTime/0.0164452f);
						}
					}
					else
					{
						animator.speed = 1.0f;
					}
					
					
					Vector3 lookAtRotation = Quaternion.LookRotation(pos.GetChild(leftHand.GetComponent<LeftHand2>().currentDistractionSpot).position - transform.position).eulerAngles;
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0))), 5 * Time.deltaTime);
				}
				else
				{
					twoSpotsAway = true;
					animator.speed = 1.0f;
					if (!reachedCenterWalk)
					{
						Vector3 lookAtRotation = Quaternion.LookRotation(pos.GetChild(6).position - transform.position).eulerAngles;
						transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0))), 5 * Time.deltaTime);
					}
					else
					{
						//transform.position = pos.GetChild(6).position;
						spot = 6;
						animator.SetBool("Looking", true);
						walkTimer += Time.deltaTime;
					}
				}
				
				//transform.position = Vector3.Lerp(transform.position, new Vector3(pos.GetChild(leftHand.GetComponent<LeftHand2>().currentDistractionSpot).position.x, transform.position.y, pos.GetChild(leftHand.GetComponent<LeftHand2>().currentDistractionSpot).position.z), 1f * Time.deltaTime);
				if (Physics.Linecast (transform.position + transform.up/2, transform.position - transform.up*5, out hit, collisionLayers))
				{
					transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hit.point.y, transform.position.z), 8f * Time.deltaTime);
				}
				
				if (Vector3.Distance(new Vector3(pos.GetChild(leftHand.GetComponent<LeftHand2>().currentDistractionSpot).position.x, transform.position.y, pos.GetChild(leftHand.GetComponent<LeftHand2>().currentDistractionSpot).position.z), transform.position) <= 1f)
				{
					spot = leftHand.GetComponent<LeftHand2>().currentDistractionSpot;
					transform.position = pos.GetChild(spot).position;
					
					animator.CrossFade("Idle", 0f, -1, 0f);
					
					waitTime = Random.Range (minWait, maxWait);
					playerMoved = true;
					co = waiter(); // create an IEnumerator object
					StartCoroutine (co);
					
					chasingNotStarted = true;
					chasing = false;
				}
				
				
				if (spot == playerSpot && !jumpScare && (!chasing || spot == 6) && canMove && fadeObject.GetComponent<FadeInOut>().clickable)
				{
					jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(1);
					jumpScareActivate = true;
				}
				
			}
			else
			{
				animator.speed = 1f;
				reachedCenterWalk = false;
				walkTimer = 0;
				twoSpotsAway = false;
				oneSpotAway = false;
				
				if (!runAway)
				{
					if (!jumpScare)
					{
						if (playerSpot == 7)
						{
							playerMoved = false;
							if (co != null)
								StopCoroutine(co);
							lastSpot = 7;
						}
						
						if (gateControls.GetComponent<GateControls>().percentage > 0 && (playerSpot != 7 || gateControls.GetComponent<GateControls>().open && gateControls.GetComponent<GateControls>().percentage < 1))
						{
							if (!passive || passive && playerSpot == 7 && gateControls.GetComponent<GateControls>().percentage >= 0.01f)
								chasingGate = true;
						}
						if (chasingGate)
						{
							if (chasingGateNotStarted)
							{
								if (playerSpot == 7)
								{
									//transform.position = new Vector3(pos.GetChild(0).position.x, pos.GetChild(0).position.y, pos.GetChild(6).position.z);
									//Vector3 lookAtRotation2 = Quaternion.LookRotation(Camera.main.transform.position - transform.position).eulerAngles;
									//transform.rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation2, new Vector3(0,1,0)));
								}
								animator.CrossFade("Run", 0f, -1, 0f);
								audioSource.PlayOneShot(suspense, 1.0f);
								audioSource.PlayOneShot(running, 1.0f);
								jumpScare = true;
								/*
								animator.CrossFade("Run", 0f, -1, 0f);
								gateSpot = playerSpot;
								//audioSource.PlayOneShot(suspense, 1.0f);
								//audioSource.PlayOneShot(running, 1.0f);
								chasingGateNotStarted = false;
								*/
							}
							
							
							Vector3 lookAtRotation = Quaternion.LookRotation(pos.GetChild(gateSpot).position - transform.position).eulerAngles;
							transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0))), 20 * Time.deltaTime);
							//transform.position = Vector3.Lerp(transform.position, new Vector3(pos.GetChild(leftHand.GetComponent<LeftHand2>().currentDistractionSpot).position.x, transform.position.y, pos.GetChild(leftHand.GetComponent<LeftHand2>().currentDistractionSpot).position.z), 1f * Time.deltaTime);
							if (Physics.Linecast (transform.position + transform.up/2, transform.position - transform.up*5, out hit, collisionLayers))
							{
								transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hit.point.y, transform.position.z), 8f * Time.deltaTime);
							}
							
							/*
							if (Vector3.Distance(new Vector3(pos.GetChild(gateSpot).position.x, transform.position.y, pos.GetChild(gateSpot).position.z), transform.position) <= 1f)
							{
								spot = gateSpot;
								transform.position = pos.GetChild(spot).position;
								
								animator.CrossFade("Idle", 0f, -1, 0f);
								
								waitTime = Random.Range (minWait, maxWait);
								playerMoved = true;
								co = waiter(); // create an IEnumerator object
								StartCoroutine (co);
								
								chasingGateNotStarted = true;
								chasingGate = false;
							}
							*/
							
						}
					}
					
					if (playerSpot != lastSpot && !playerMoved)
					{
						waitTime = Random.Range (minWait, maxWait);
						co = waiter(); // create an IEnumerator object
						StartCoroutine (co);
						playerMoved = true;
					}
					
					if (spot == playerSpot && !jumpScare && canMove && fadeObject.GetComponent<FadeInOut>().clickable)
					{
						jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(1);
						jumpScareActivate = true;
					}
					
					//Running at you
					/*
					if (!jumpScare && (spot == 11 || spot == 12 || spot == 13 || spot == 14))
					{
						jumpScare = true;
					}
					*/
					if (jumpScare && !stuckInGate)
					{
						pauseMenu.GetComponent<PauseMenu>().locked = true;
						
						canMove = true;
						foreach (Renderer rend in GetComponentsInChildren<Renderer>())
							rend.enabled = true;
						foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
							col.enabled = true;
						
						if (Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), transform.position) > 1.2f){
							//dollySpeed += 0.1f;
							//animator.SetFloat("Speed", dollySpeed);
							if (chasingGate && playerSpot == 7)
							{
								ifLight2.GetComponent<Light>().intensity = 2;
							}
							
							if (!chasingGate || playerSpot != 7 || reachedCenter)
							{
								Vector3 lookAtRotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position).eulerAngles;
								transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0))), 20 * Time.deltaTime);
							}
							else
							{
								if (!reachedCenter)
								{
									Vector3 lookAtRotation = Quaternion.LookRotation(pos.GetChild(6).position - transform.position).eulerAngles;
									transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0))), 20 * Time.deltaTime);
								}
							}
							
							ifLight.GetComponent<Light>().intensity = 2;
							//flashlight.GetComponent<Light>().intensity = 0;
							//flashlight.GetComponent<Light>().shadowStrength = 0;
							//cam.GetComponent<Light>().intensity = 0;
							//cam.GetComponent<Light>().shadowStrength = 0;
							
							flashlightObject.transform.parent = Camera.main.transform;
							flashlightObject.GetComponent<CameraController>().enabled = false;
							flashlightObject.transform.localPosition = Vector3.Lerp(flashlightObject.transform.localPosition, new Vector3(flashlightObject.transform.localPosition.x, -4, flashlightObject.transform.localPosition.z), 2 * Time.deltaTime);
							
							camObject.transform.parent = Camera.main.transform;
							camObject.GetComponent<CameraController>().enabled = false;
							camObject.transform.localPosition = Vector3.Lerp(camObject.transform.localPosition, new Vector3(camObject.transform.localPosition.x, -4, camObject.transform.localPosition.z), 2 * Time.deltaTime);
							cam.SetActive(false);
							
							micObject.transform.parent = Camera.main.transform;
							micObject.GetComponent<CameraController>().enabled = false;
							micObject.transform.localPosition = Vector3.Lerp(distractionObject.transform.localPosition, new Vector3(distractionObject.transform.localPosition.x, -4, distractionObject.transform.localPosition.z), 2 * Time.deltaTime);
							
							distractionObject.transform.parent = Camera.main.transform;
							distractionObject.GetComponent<CameraController>().enabled = false;
							distractionObject.transform.GetChild(0).GetComponent<Distraction>().enabled = false;
							distractionObject.transform.localPosition = Vector3.Lerp(distractionObject.transform.localPosition, new Vector3(distractionObject.transform.localPosition.x, -4, distractionObject.transform.localPosition.z), 2 * Time.deltaTime);
							
							Camera.main.GetComponent<CameraController>().enabled = false;
							//Camera.main.transform.LookAt(transform);
							Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,1,0)) - Camera.main.transform.position);
							//lookRotation.y = 12;
							
							Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 20 * Time.deltaTime);
						}
						else {
							flashlight.GetComponent<Light>().intensity = 0;
							cam.GetComponent<Light>().intensity = 0;
							Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,20,0)) - Camera.main.transform.position);
							//lookRotation.y = 12;
							
							Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 3.5f * Time.deltaTime);
						}
						//timer += Time.deltaTime;
						
						if (!chasingGate || playerSpot != 7 || reachedCenter)
						{
							transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), 3f * Time.deltaTime);
						}
						else
						{
							if (!reachedCenter)
							{
								transform.position = Vector3.Lerp(transform.position, new Vector3(pos.GetChild(6).position.x, transform.position.y, pos.GetChild(6).position.z), 3f * Time.deltaTime);
							}
							if (Vector3.Distance(new Vector3(pos.GetChild(6).position.x, transform.position.y, pos.GetChild(6).position.z), transform.position) < 0.3f)
							{
								transform.position = pos.GetChild(6).position;
								Vector3 lookAtRotation2 = Quaternion.LookRotation(Camera.main.transform.position - transform.position).eulerAngles;
								transform.rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation2, new Vector3(0,1,0)));
								reachedCenter = true;
							}
						}
						transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, Camera.main.transform.parent.parent.position.y, transform.position.z), 8f * Time.deltaTime);
						
						if (Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), transform.position) <= 0.8f)
						{
							jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(1);
							jumpScareActivate = true;
						}
						
					}
					else
					{
						pauseMenu.GetComponent<PauseMenu>().locked = false;
						reachedCenter = false;
					}
					
					if (stuckInGate)
					{
						transform.position = Vector3.Lerp(transform.position, gatePosition, 10 * Time.deltaTime);
						//transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(gateRotation.x, gateRotation.y, transform.eulerAngles.z), 10 * Time.deltaTime);
						transform.eulerAngles = new Vector3(gateRotation.x, gateRotation.y, transform.eulerAngles.z);
						
						playerMoved = false;
						StopCoroutine(co);
						lastSpot = 7;
						
						if (!stuckInGateAdjustments)
						{
							animator.CrossFade("Stuck", 0f, -1, 0f);
							
							ifLight.GetComponent<Light>().intensity = 0;
							ifLight2.GetComponent<Light>().intensity = 0;
							
							Camera.main.GetComponent<CameraController>().enabled = true;
							
							flashlightObject.transform.parent = rightHand;
							flashlightObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
							flashlightObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
							flashlightObject.GetComponent<CameraController>().enabled = true;
							
							camObject.transform.parent = rightHand;
							camObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
							camObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
							camObject.GetComponent<CameraController>().enabled = true;
							cam.SetActive(true);
							
							micObject.transform.parent = leftHand;
							micObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
							micObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
							micObject.GetComponent<CameraController>().enabled = true;
							
							distractionObject.transform.parent = leftHand;
							distractionObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
							distractionObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
							distractionObject.transform.GetChild(0).GetComponent<Distraction>().enabled = true;
							distractionObject.GetComponent<CameraController>().enabled = true;
							
							flashlight.GetComponent<Light>().intensity = 3.3f;
							//cam.GetComponent<Light>().intensity = 0;
							
							audioSource.Stop();
							audioSource.PlayOneShot(stuck, 1.0f);
							
							stuckInGateAdjustments = true;
						}
						
						if (stuckTimer > minWait && !passive || passive && stuckTimer > 30)
						{
							audioSource.Stop();
							audioSource.PlayOneShot(stuck, 1.0f);
							gateControls.GetComponent<GateControls>().hitGate = true;
							gateControls.GetComponent<GateControls>().wasStuckInGate = true;
							animator.SetBool("Escaped", true);
							timer = 0;
							runAway = true;
							stuckInGate = false;
						}
						
						stuckTimer += Time.deltaTime;
						
						Vector3 screenPointWin = Camera.main.WorldToViewportPoint(transform.position);
						bool onScreenWin = screenPointWin.z > 0 && screenPointWin.x > -1 && screenPointWin.x < 2 && screenPointWin.y > -1 && screenPointWin.y < 2;
						if (cam.GetComponent<Cam>().on && onScreenWin){
							jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(3);
							jumpScareActivate = true;
							//SceneManager.LoadScene("Level 1 Win");
						}
						
					}
					else
					{
						animator.SetBool("Escaped", false);
						stuckTimer = 0;
						stuckInGateAdjustments = false;
					}
				}
				
				
				//ran into gate
				if (!runAway && !stuckInGate && !gateControls.GetComponent<GateControls>().open && jumpScare && playerSpot == 7 && Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, transform.position.z), transform.position) > 1.2f && (gateControls.GetComponent<GateControls>().percentage > 0 && Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, transform.position.z), transform.position) <= 3f || Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, transform.position.z), transform.position) <= 2.5f))
				{
					//transform.position = gatePosition;
					//transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(gateRotation.x, gateRotation.y, transform.eulerAngles.z), 10 * Time.deltaTime);
					transform.eulerAngles = new Vector3(gateRotation.x, gateRotation.y, transform.eulerAngles.z);
					
					playerMoved = false;
					StopCoroutine(co);
					lastSpot = 7;
					
					
					ifLight.GetComponent<Light>().intensity = 0;
					ifLight2.GetComponent<Light>().intensity = 0;
					
					Camera.main.GetComponent<CameraController>().enabled = true;
					
					flashlightObject.transform.parent = rightHand;
					flashlightObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
					flashlightObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
					flashlightObject.GetComponent<CameraController>().enabled = true;
					
					camObject.transform.parent = rightHand;
					camObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
					camObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
					camObject.GetComponent<CameraController>().enabled = true;
					cam.SetActive(true);
					
					micObject.transform.parent = leftHand;
					micObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
					micObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
					micObject.GetComponent<CameraController>().enabled = true;
					
					distractionObject.transform.parent = leftHand;
					distractionObject.transform.localPosition = new Vector3(0.73f, 1, -1.07f);
					distractionObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
					distractionObject.transform.GetChild(0).GetComponent<Distraction>().enabled = true;
					distractionObject.GetComponent<CameraController>().enabled = true;
					
					
					audioSource.Stop();
					audioSource.PlayOneShot(stuck, 1.0f);
					gateControls.GetComponent<GateControls>().percentage = 0;
					gateControls.GetComponent<GateControls>().hitGate = true;
					animator.SetBool("Escaped", true);
					timer = 0;
					runAway = true;
					stuckInGate = false;
				}
				
				
				
				if (runAway)
				{
				
					canMove = true;
					foreach (Renderer rend in GetComponentsInChildren<Renderer>())
						rend.enabled = true;
					foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
						col.enabled = true;
					
						
					/*
					if (chasingGate && playerSpot == 7)
					{
						ifLight2.GetComponent<Light>().intensity = 2;
					}
					*/
					
					if (timer > 1)
					{
						Vector3 lookAtRotation = Quaternion.LookRotation(pos.GetChild(0).position - transform.position).eulerAngles;
						transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0))), 7 * Time.deltaTime);
						animator.SetBool("Escaped", false);
					}
					timer += Time.deltaTime;
					
					//ifLight.GetComponent<Light>().intensity = 2;
					
					if (Vector3.Distance(new Vector3(pos.GetChild(0).position.x, transform.position.y, pos.GetChild(0).position.z), transform.position) >= 3f)
					{
						//transform.position = Vector3.Lerp(transform.position, new Vector3(pos.GetChild(0).position.x, transform.position.y, pos.GetChild(0).position.z), 0.5f * Time.deltaTime);
					}
					else
					{
						transform.position = pos.GetChild(0).position;
						Vector3 lookAtRotation2 = Quaternion.LookRotation(Camera.main.transform.position - transform.position).eulerAngles;
						transform.rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation2, new Vector3(0,1,0)));
						animator.CrossFade("Idle", 0f, -1, 0f);
						
						jumpScare = false;
						jumpScareActivate = false;
						stuckInGate = false;
						chasing = false;
						chasingNotStarted = true;
						chasingGate = false;
						chasingGateNotStarted = true;
						gateControls.GetComponent<GateControls>().hitGate = false;
						
						//gateControls.GetComponent<GateControls>().open = true;
						gateControls.GetComponent<GateControls>().percentage = 0;
						
						spot = 0;
						runAway = false;
					}
					transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, Camera.main.transform.parent.parent.position.y + 0.14f, transform.position.z), 8f * Time.deltaTime);
					
				}
				
			}
			
			//Mic is enabled/disabled
			if (mic.GetComponent<Mic>().on)
				audioSource.volume = Mathf.Lerp(audioSource.volume, 1f, 10 * Time.deltaTime);
			else
				audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, 10 * Time.deltaTime);
			
		}
		
    }
	
	public bool onScreen;
	
	void FixedUpdate(){
		
		if (!chasing && !chasingGate)
		{
			Vector3 lookAtRotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position).eulerAngles;
			transform.rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0)));
			
			Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
			onScreen = screenPoint.z > 0 && screenPoint.x > -1 && screenPoint.x < 2 && screenPoint.y > -1 && screenPoint.y < 2;
			//bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
			
			if (!cam.GetComponent<Cam>().on && cam.GetComponent<Light>().intensity <= 0.01f || !onScreen){
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
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = true;
				foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
					col.enabled = true;
			}
		}
		
	}
}
