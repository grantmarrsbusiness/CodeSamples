using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DollyAI : MonoBehaviour
{
	public bool passive = false;
	public bool easy = true;
	public bool normal = false;
	public bool hard = false;
	public bool insane = false;
	public bool lethal = false;
	private float speed1;
	private float speed2;
	
	public GameObject difficultySelector;
	public GameObject jumpScareSelector;
	public GameObject pauseMenu;
	
	public GameObject flashlight;
	public GameObject cam;
	
	public GameObject flashlightObject;
	public GameObject camObject;
	public GameObject micObject;
	public GameObject ifLight;
	
	public float offTimer;
	
	public float distanceModifierFloat;
	public Vector3 distanceModifier = new Vector3();
	
	public bool started = false;
	public bool dropping = false;
	public bool climbing = false;
	public bool dollUp = false;
	
	public bool dollMode = true;
	public int oldGrateNum;
	
	public Transform grates;
	public GameObject paths;
	public GameObject pathsEasy;
	public GameObject pathsNormal;
	public GameObject pathsHard;
	public GameObject pathsInsane;
	public Transform doll;
	
	private RaycastHit hit = new RaycastHit(); //information on the hit point of a raycast
	public LayerMask collisionLayers = ~((1 << 2) | (1 << 4)); //the layers that the detectors (raycasts/linecasts) will collide with
	public LayerMask collisionLayers2 = ~((1 << 2) | (1 << 4)); //the layers that the detectors (raycasts/linecasts) will collide with
	public LayerMask collisionLayers3 = ~((1 << 2) | (1 << 4)); //the layers that the detectors (raycasts/linecasts) will collide with
	
	private Animator animator; //the "Animator" component of the script holder
	
	AudioSource audioSource;
	
	public Vector3 startPos;
	public Vector3 startRot;
	public Vector3 hidePos;
	public Vector3 lockPos;
	public Vector3 unlockingPos;
	
	private Vector3 dollStartPos;
	private Vector3 dollStartRot;
    
	private float timer = 0.0f;		//counting timer
	public bool jumpScare = false;
	public bool jumpScareActivate = false;
	
	private Vector3 dollHitPoint;
	private Vector3 dollHitPoint2;
	private Vector3 dollHitPointTransition;
	
	private Vector3 dollyHitPoint;
	private Vector3 dollyHitPoint2;
	private Vector3 dollyHitPointTransition;
	public float dollySpeed = 0;
	public float dollSpeed = 0;
	public bool dollTouchingGrate;
	
	public Vector3 normalDollPos = new Vector3(0.02889633f, -1.101f, 0);
	public Vector3 dollRestrainedPos = new Vector3(-1.57f, -1.131f, 7.11f);
	public GameObject straps;
	
	public Vector3 oldHitPoint = new Vector3();
	
	public bool ceilingOnScreen;
	
	public bool restrainable = false;
	public bool dollRestrained = false;
	public bool unlocking = false;
	
	public bool activateDollMode = false;
	public bool controllingDolly = false;
	
	private bool audioPlaying;
	
	public List<int> oldGratesList =  new List<int>{0, 0, 0};
	public List<int> currentGratesList =  new List<int>{1, 2, 3};
	
	public bool startGrates = true;
	public float gratesHidden = 0;
	public float lastGrateHid = -1;
	public int finalGrate = -1;
	public int finalRand = -1;
	
	public Vector3 lastPos;
	
	public float beforeDropTimer = 0;
	public float afterDropTimer = 0;
	
	public AudioClip suspense;
	public AudioClip running;
	public AudioClip metalGrate;
	public AudioClip dropSound;
	public AudioClip unlockPull;
	public AudioClip unlockSound;
	public AudioClip ropeSound;
	public AudioClip spiderClimb;
	public bool soundPlayed;
	
	public float viewDistance = 1;
	private float unlockTimer = 0.0f;		//counting timer
	
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		
		//setting where doll spawns
		dollStartPos = doll.transform.position;
		dollStartRot = doll.transform.eulerAngles;
		dollHitPoint = dollStartPos;
		dollHitPoint2 = dollStartPos;
		dollHitPointTransition = dollStartPos;
		distanceModifier = doll.transform.position;
		dollSpeed = 0;
		
		//setting where dolly spawns
		dollyHitPoint = startPos;
		dollyHitPoint2 = startPos;
		dollyHitPointTransition = startPos;
		dollySpeed = 0;
		
		//hiding dolly at start
		transform.position = hidePos;
		
		grates.gameObject.SetActive(true);
		
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
			speed1 = 1;
			speed2 = 3;
			paths = pathsEasy.gameObject;
		}
		else if (normal)
		{
			speed1 = 2;
			speed2 = 2;
			paths = pathsNormal.gameObject;
		}
		else if (hard)
		{
			speed1 = 3;
			speed2 = 1;
			paths = pathsHard.gameObject;
		}
		else if (insane)
		{
			speed1 = 4;
			speed2 = 0.1f;
			paths = pathsInsane.gameObject;
		}
		else if (lethal)
		{
			speed1 = 6;
			speed2 = 0.1f;
			paths = pathsInsane.gameObject;
		}
		
		paths.gameObject.SetActive(false);
		
		//Start loading the jumpscare Scene asynchronously
		StartCoroutine(LoadScene());
		
    }
	
	IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Level 3 Jumpscares");
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
			
			if (!started)
			{
				if (dollMode)
				{
					Vector3 dollScreenPoint = Camera.main.WorldToViewportPoint(new Vector3(doll.transform.position.x, 0, doll.transform.position.z));
					bool dollOnScreen = dollScreenPoint.z > 0 && dollScreenPoint.x > 1 - viewDistance && dollScreenPoint.x < viewDistance && dollScreenPoint.y > 1 - viewDistance && dollScreenPoint.y < viewDistance;
					if (dollOnScreen && flashlight.GetComponent<Flashlight>().on)
					{
						started = true;
					}
				}
				else
				{
					Vector3 dollyScreenPoint = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x, 0, transform.position.z));
					bool dollyOnScreen = dollyScreenPoint.z > 0 && dollyScreenPoint.x > 1 - viewDistance && dollyScreenPoint.x < viewDistance && dollyScreenPoint.y > 1 - viewDistance && dollyScreenPoint.y < viewDistance;
					if (dollyOnScreen && flashlight.GetComponent<Flashlight>().on)
					{
						started = true;
					}
				}
			}
			
			animator.SetBool("Unlocking", unlocking);
			animator.SetBool("Climbing", climbing);
			
			
			if (!dollMode && !unlocking && !climbing && !jumpScare)
			{
				//finishing doll's movement
				doll.transform.position = Vector3.Lerp(doll.transform.position, dollHitPointTransition, 1f * Time.deltaTime);
				dollHitPoint = doll.transform.position;
				dollHitPoint2 = doll.transform.position;
				dollSpeed = Mathf.Lerp(dollSpeed, 12, 1 * Time.deltaTime);
				doll.transform.GetChild(1).GetComponent<Animator>().SetFloat("Speed", dollSpeed);
			}
			
			//if flashlight was shined on doll
			if (started && !jumpScare)
			{
				
				//dolly unlocking doll
				if (Physics.Linecast (transform.position, transform.position + transform.up*10, out hit, collisionLayers3) && hit.collider.name.Contains("Bed") && dollRestrained)
				{
					transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, startRot, 4f * Time.deltaTime);
					
					Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
					bool onScreen = screenPoint.z > 0 && screenPoint.x > -1 && screenPoint.x < 2 && screenPoint.y > -1 && screenPoint.y < 2;
					if (cam.GetComponent<Cam>().on && onScreen && !climbing){
						jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(3);
						jumpScareActivate = true;
						//SceneManager.LoadScene("Level 1 Win");
					}
					if ((unlockTimer >= 5 - speed1 && !lethal || unlockTimer >= 6.5f - speed1 && lethal) && !passive || passive && unlockTimer >= 30)
					{
						dollUp = false;
						climbing = true;
						if (unlocking)
						{
							audioSource.Stop();
							audioSource.PlayOneShot(unlockSound, 1f);
							//audioSource.PlayOneShot(spiderClimb, 1f);
							audioSource.clip = spiderClimb;
							audioSource.volume = 1.0f;
							audioSource.Play();
						}
						unlocking = false;
						doll.transform.GetChild(1).transform.localPosition = normalDollPos;
						straps.SetActive(false);
						doll.transform.GetChild(1).GetComponent<Animator>().CrossFade("DollIdle", 0f, -1, 0f);
					}
					else
					{
						if (!unlocking)
						{
							//audioSource.PlayOneShot(unlockPull, 1f);
							audioSource.clip = unlockPull;
							audioSource.volume = 1.0f;
							audioSource.Play();
						}
						unlocking = true;
						transform.position = Vector3.Lerp(transform.position, unlockingPos, 4f * Time.deltaTime);
					}
					unlockTimer += Time.deltaTime;
					
				}
				
				if (climbing)
				{
					float dollHidingSpotY = 7.85f;
					//dolly climbing up
					transform.position = Vector3.Lerp(transform.position, new Vector3(doll.transform.position.x, transform.position.y, doll.transform.position.z) - transform.forward/4, 7f * Time.deltaTime);
					transform.position += (transform.up/15) * (Time.deltaTime/0.0164452f);
					//pulling doll up
					if (!dollUp)
					{
						//doll.transform.position = Vector3.Lerp(doll.transform.position, new Vector3(doll.transform.position.x, dollHidingSpotY, doll.transform.position.z), 0.25f * Time.deltaTime);
						doll.transform.position += (doll.transform.up/25) * (Time.deltaTime/0.0164452f);
						if (doll.transform.position.y >= dollHidingSpotY)
						{
							doll.transform.position = new Vector3(dollStartPos.x, dollHidingSpotY, dollStartPos.z);
							//audioSource.PlayOneShot(ropeSound, 1f);
							audioSource.clip = ropeSound;
							audioSource.volume = 1.0f;
							audioSource.Play();
							dollUp = true;
						}
					}
					//putting doll down
					else
					{
						doll.transform.eulerAngles = dollStartRot;
						if (Vector3.Distance(doll.transform.position, dollStartPos) <= 0.01f)
						{
							
							//hiding dolly
							transform.position = hidePos;
							//resetting and spawning doll
							dollHitPoint = dollStartPos;
							dollHitPoint2 = dollStartPos;
							dollHitPointTransition = dollStartPos;
							doll.transform.position = dollStartPos;
							
							afterDropTimer += Time.deltaTime;
							if (afterDropTimer > Time.deltaTime)
							{
								//beforeDropTimer = 0;
								afterDropTimer = 0;
								unlockTimer = 0;
								gratesHidden = 0;
								lastGrateHid = -1;
								activateDollMode = true;
								dollMode = true;
								doll.GetComponent<DollScript>().grateNum = 0;
								dollUp = false;
								climbing = false;
							}
							
							started = false;
						}
						else {
							//waiting to drop
							//if (beforeDropTimer > speed2)
							//{
								doll.transform.position = Vector3.Lerp(doll.transform.position, dollStartPos, 4* Time.deltaTime);
							//}
						}
						//beforeDropTimer += Time.deltaTime;
						
					}
				}

				if (!unlocking && !climbing)
				{
					
					//if dolly made it to doll
					if (doll.GetComponent<DollScript>().grateNum == 100)
					{
						/*
						//hiding dolly
						transform.position = hidePos;
						//resetting and spawning doll
						dollHitPoint = dollStartPos;
						dollHitPoint2 = dollStartPos;
						dollHitPointTransition = dollStartPos;
						doll.transform.position = dollStartPos;
						transform.eulerAngles = dollStartRot;
						gratesHidden = 0;
						lastGrateHid = -1;
						activateDollMode = true;
						dollMode = true;
						doll.GetComponent<DollScript>().grateNum = 0;
						*/
						dollUp = false;
						//audioSource.PlayOneShot(spiderClimb, 1f);
						audioSource.clip = spiderClimb;
						audioSource.volume = 1.0f;
						audioSource.Play();
						climbing = true;
					}
					//if dolly hit the edge of the maze
					else if (!dollMode && Physics.Linecast (transform.position, transform.position + transform.up*10, out hit, collisionLayers3) && !hit.collider.name.Contains("Bed")) {
						if (!passive)
						{
							animator.CrossFade("JumpscareRun", 0f, -1, 0f);
							audioSource.Stop();
							audioSource.PlayOneShot(suspense, 1.0f);
							audioSource.PlayOneShot(running, 1.0f);
							jumpScare = true;
							//jumpScareActivate = true;
						}
						else
						{
							dollyHitPoint2 = new Vector3(lastPos.x, transform.position.y, lastPos.z);
							dollyHitPointTransition = new Vector3(lastPos.x, transform.position.y, lastPos.z);
							transform.position = Vector3.Lerp(transform.position, new Vector3(lastPos.x, transform.position.y, lastPos.z), 1 * Time.deltaTime);
						}
					}
					
					if (!dollMode && !Physics.Linecast (transform.position, transform.position + transform.up*10, out hit, collisionLayers3))
					{
						lastPos = transform.position;
					}
					
					
					//Doll Movement
					if (dollMode)
					{
						offTimer = 0;
						
						dollyHitPoint = startPos;
						dollyHitPoint2 = startPos;
						dollyHitPointTransition = startPos;
						
						if (flashlight.GetComponent<Flashlight>().on && Physics.Linecast (flashlight.transform.position, flashlight.transform.position + flashlight.transform.forward*10, out hit, collisionLayers2)) {
							//moving doll toward flashlight
							dollHitPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
							
							Vector3 dollZeroY = new Vector3(doll.transform.position.x, 0, doll.transform.position.z);
							
							distanceModifierFloat = 5/((1 + (Vector3.Distance(dollZeroY, dollHitPoint))*3));
							float fastMovement = 1 - Vector3.Distance(oldHitPoint, hit.point)*4;
							if (fastMovement <= 0)
							{
								fastMovement = 0;
							}
							distanceModifier = (dollZeroY + (distanceModifierFloat*fastMovement)*(dollHitPoint - dollZeroY));
							
							if (!passive)
							{
								dollHitPoint2 = Vector3.Lerp(dollHitPoint2, new Vector3(distanceModifier.x, doll.transform.position.y, distanceModifier.z), 1*(0.75f + speed1/4) * Time.deltaTime);
							}
							else
							{
								if (!dollTouchingGrate)
								{
									
									dollHitPoint2 = Vector3.Lerp(dollHitPoint2, new Vector3(distanceModifier.x, doll.transform.position.y, distanceModifier.z), 1*(0.75f + speed1/4) * Time.deltaTime);
								}
								else
								{
									//dollHitPoint2 = doll.transform.position - doll.transform.forward;
									dollHitPoint2 = new Vector3(grates.GetChild(oldGrateNum).transform.position.x, dollHitPoint2.y, grates.GetChild(oldGrateNum).transform.position.z);
								}
							}
							
							doll.transform.position = Vector3.Lerp(doll.transform.position, dollHitPoint2, 4*(0.75f + speed1/4) * Time.deltaTime);
							
							
							if (Vector3.Distance(dollHitPoint, new Vector3(doll.transform.position.x, dollHitPoint.y, doll.transform.position.z)) >= 0.25f)
							{
								dollHitPointTransition = dollHitPoint2;
								Vector3 lookAtRotationDoll = Quaternion.LookRotation(dollHitPointTransition - doll.transform.position).eulerAngles;
							doll.transform.rotation = Quaternion.Slerp(doll.transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotationDoll, new Vector3(0,1,0))), (1f + Vector3.Distance(dollHitPoint, new Vector3(doll.transform.position.x, dollHitPoint.y, doll.transform.position.z)))*(0.75f + speed1/4) * Time.deltaTime);
							
							}
							else
							{
								//dollHitPointTransition = Vector3.Lerp(dollHitPointTransition, new Vector3(Camera.main.transform.position.x, doll.transform.position.y, Camera.main.transform.position.z), 6f * Time.deltaTime);
							}
							
							
							
							dollSpeed = Mathf.Lerp(dollSpeed, Vector3.Distance(dollHitPoint, doll.transform.position)*distanceModifierFloat*(0.75f + speed1/4), 8 * Time.deltaTime);
							//
							doll.transform.GetChild(1).GetComponent<Animator>().SetFloat("Speed", dollSpeed);
							
							oldHitPoint = hit.point;
						}
						else {
							//finishing doll's movement and moving toward camera
							if (passive)
							{
								if (!dollTouchingGrate)
								{
									
									dollHitPointTransition = Vector3.Lerp(dollHitPointTransition, new Vector3(Camera.main.transform.position.x, doll.transform.position.y, Camera.main.transform.position.z), speed1/15 * Time.deltaTime);
								}
								else
								{
									//dollHitPoint2 = doll.transform.position - doll.transform.forward;
									dollHitPointTransition = new Vector3(grates.GetChild(oldGrateNum).transform.position.x, dollHitPointTransition.y, grates.GetChild(oldGrateNum).transform.position.z);
								}
							}
							
							
							dollHitPointTransition = Vector3.Lerp(dollHitPointTransition, new Vector3(Camera.main.transform.position.x, doll.transform.position.y, Camera.main.transform.position.z), speed1/15 * Time.deltaTime);
							doll.transform.position = Vector3.Lerp(doll.transform.position, dollHitPointTransition, 4f * Time.deltaTime);
							dollHitPoint = doll.transform.position;
							dollHitPoint2 = doll.transform.position;
							distanceModifier = doll.transform.position;
							
							Vector3 lookAtRotationDoll = Quaternion.LookRotation(dollHitPointTransition - doll.transform.position).eulerAngles;
							doll.transform.rotation = Quaternion.Slerp(doll.transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotationDoll, new Vector3(0,1,0))), 3 * Time.deltaTime);
							
							dollSpeed = Mathf.Lerp(dollSpeed, 1, 3 * Time.deltaTime);
							doll.transform.GetChild(1).GetComponent<Animator>().SetFloat("Speed", dollSpeed);
						}
					}
					//Dolly Movement
					else
					{
						/*
						//finishing doll's movement
						doll.transform.position = Vector3.Lerp(doll.transform.position, dollHitPointTransition, 1f * Time.deltaTime);
						dollHitPoint = doll.transform.position;
						dollHitPoint2 = doll.transform.position;
						dollSpeed = Mathf.Lerp(dollSpeed, 12, 1 * Time.deltaTime);
						doll.transform.GetChild(1).GetComponent<Animator>().SetFloat("Speed", dollSpeed);
						*/
						
						if (!dropping)
						{
						
							//starting dolly's movement
							if (flashlight.GetComponent<Flashlight>().on && Physics.Linecast (flashlight.transform.position, flashlight.transform.position + flashlight.transform.forward*10, out hit, collisionLayers2)) {
								offTimer = 0;
								
								//moving dolly toward flashlight
								dollyHitPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
								
								Vector3 dollyZeroY = new Vector3(transform.position.x, 0, transform.position.z);
								
								distanceModifierFloat = 5/((1 + (Vector3.Distance(dollyZeroY, dollyHitPoint))*3));
								float fastMovement = 1 - Vector3.Distance(oldHitPoint, hit.point)*4;
								if (fastMovement <= 0)
								{
									fastMovement = 0;
								}
								distanceModifier = (dollyZeroY + (distanceModifierFloat*fastMovement)*(dollyHitPoint - dollyZeroY));
								
								dollyHitPoint2 = Vector3.Lerp(dollyHitPoint2, new Vector3(distanceModifier.x, transform.position.y, distanceModifier.z), 1*(0.75f + speed1/4) * Time.deltaTime);
								
								transform.position = Vector3.Lerp(transform.position, dollyHitPoint2, 4*(0.75f + speed1/4) * Time.deltaTime);
								
								oldHitPoint = hit.point;
								
								/*
								dollyHitPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
								dollyHitPoint2 = Vector3.Lerp(dollyHitPoint2, dollyHitPoint, 1 * Time.deltaTime);
								transform.position = Vector3.Lerp(transform.position, dollyHitPoint2, 4 * Time.deltaTime);
								*/
								
								if (Vector3.Distance(dollyHitPoint, transform.position) >= 0.5f)
								{
									dollyHitPointTransition = dollyHitPoint2;
								}
								else
								{
									dollyHitPointTransition = Vector3.Lerp(dollyHitPointTransition, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), 6f * Time.deltaTime);
								}
								
								
								
								
								Vector3 lookAtRotation = Quaternion.LookRotation(dollyHitPointTransition - transform.position).eulerAngles;
								dollySpeed = Mathf.Lerp(dollySpeed, Vector3.Distance(dollyHitPoint, transform.position)*distanceModifierFloat*(0.75f + speed1/4), 8 * Time.deltaTime);
								transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0))), (1f + Vector3.Distance(dollyHitPoint, transform.position))*(0.75f + speed1/4) * Time.deltaTime);
								//
								animator.SetFloat("Speed", dollySpeed);
								controllingDolly = true;
							}
							else {
								
								if (!passive)
								{
									if (controllingDolly)
									{
										dollyHitPointTransition = transform.position;
										controllingDolly = false;
									}
									//finishing dolly's movement and moving toward camera
									dollyHitPointTransition = Vector3.Lerp(dollyHitPointTransition, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), speed1/15 * Time.deltaTime);
									transform.position = Vector3.Lerp(transform.position, dollyHitPointTransition, 4f * Time.deltaTime);
									//
									Vector3 lookAtRotation = Quaternion.LookRotation(dollyHitPointTransition - transform.position).eulerAngles;
									dollySpeed = Mathf.Lerp(dollySpeed, 1, 3 * Time.deltaTime);
									transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0))), 3 * Time.deltaTime);
									//
									animator.SetFloat("Speed", dollySpeed);
									dollyHitPoint = transform.position;
									dollyHitPoint2 = transform.position;
								}
								else
								{
									offTimer += Time.deltaTime;
									if (!dollMode && Physics.Linecast (transform.position, transform.position + transform.up*10, out hit, collisionLayers3) && !hit.collider.name.Contains("Bed")) {
										/*
										animator.CrossFade("JumpscareRun", 0f, -1, 0f);
										audioSource.Stop();
										audioSource.PlayOneShot(suspense, 1.0f);
										audioSource.PlayOneShot(running, 1.0f);
										jumpScare = true;
										*/
										dollyHitPoint2 = new Vector3(lastPos.x, transform.position.y, lastPos.z);
										dollyHitPointTransition = new Vector3(lastPos.x, transform.position.y, lastPos.z);
										transform.position = Vector3.Lerp(transform.position, new Vector3(lastPos.x, transform.position.y, lastPos.z), 1 * Time.deltaTime);
									}
									else
									{
										if (controllingDolly)
										{
											dollyHitPointTransition = transform.position;
											controllingDolly = false;
										}
										//finishing dolly's movement and moving toward camera
										dollyHitPointTransition = Vector3.Lerp(dollyHitPointTransition, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), speed1/15 * Time.deltaTime);
										transform.position = Vector3.Lerp(transform.position, dollyHitPointTransition, 4f * Time.deltaTime);
										
										dollyHitPoint = transform.position;
										dollyHitPoint2 = transform.position;
									}
									
									//
									Vector3 lookAtRotation = Quaternion.LookRotation(dollyHitPointTransition - transform.position).eulerAngles;
									dollySpeed = Mathf.Lerp(dollySpeed, 1, 3 * Time.deltaTime);
									transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0))), 3 * Time.deltaTime);
									//
									animator.SetFloat("Speed", dollySpeed);
								}
							}
						}
						else
						{
							transform.eulerAngles = startRot;
							//transform.position = Vector3.Lerp(transform.position, startPos, 8f * Time.deltaTime);
							if (Vector3.Distance(startPos, transform.position) <= 0.01f || transform.position.y <= startPos.y)
							{
								transform.position = startPos;
								
								//if (afterDropTimer > speed2/2)
								//{
									animator.speed = 1;
									beforeDropTimer = 0;
									//afterDropTimer = 0;
									dropping = false;
									started = false;
								//}
								//afterDropTimer += Time.deltaTime;
							}
							else {
								//waiting to drop
								beforeDropTimer += Time.deltaTime;
								if (beforeDropTimer <= Time.deltaTime)
								{
									//audioSource.PlayOneShot(dropSound, 1f);
									audioSource.clip = dropSound;
									audioSource.volume = 1.0f;
									audioSource.Play();
								}
								//if (beforeDropTimer > speed2)
								//{
									transform.position -= (transform.up/10) * (Time.deltaTime/0.0164452f);
								//}
								//else {
									//animator.CrossFade("Landing", 0f, -1, 0f);
									//animator.speed = 0.66f + (speed1/3);
								//}
								dollySpeed = 0;
							}
							//beforeDropTimer += Time.deltaTime;
							
						}
					}
					
					if (passive && offTimer > 30)
					{
						animator.CrossFade("JumpscareRun", 0f, -1, 0f);
						audioSource.Stop();
						audioSource.PlayOneShot(suspense, 1.0f);
						audioSource.PlayOneShot(running, 1.0f);
						jumpScare = true;
						offTimer = 0;
					}
					
					
					
					//Hiding grates randomly
					
					Vector3 screenPoint1 = Camera.main.WorldToViewportPoint(grates.GetChild(0).transform.position);
					bool ceilingOnScreen1 = screenPoint1.z > 0 && screenPoint1.x > 0 && screenPoint1.x < 1 && screenPoint1.y > 0 && screenPoint1.y < 1;
					Vector3 screenPoint2 = Camera.main.WorldToViewportPoint(grates.GetChild(3).transform.position);
					bool ceilingOnScreen2 = screenPoint2.z > 0 && screenPoint2.x > 0 && screenPoint2.x < 1 && screenPoint2.y > 0 && screenPoint2.y < 1;
					Vector3 screenPoint3 = Camera.main.WorldToViewportPoint(grates.GetChild(12).transform.position);
					bool ceilingOnScreen3 = screenPoint3.z > 0 && screenPoint3.x > 0 && screenPoint3.x < 1 && screenPoint3.y > 0 && screenPoint3.y < 1;
					Vector3 screenPoint4 = Camera.main.WorldToViewportPoint(grates.GetChild(15).transform.position);
					bool ceilingOnScreen4 = screenPoint4.z > 0 && screenPoint4.x > 0 && screenPoint4.x < 1 && screenPoint4.y > 0 && screenPoint4.y < 1;
					
					if (ceilingOnScreen1 || ceilingOnScreen2 || ceilingOnScreen3 || ceilingOnScreen4)
						ceilingOnScreen = true;
					else
						ceilingOnScreen = false;
					
					//Controlling the doll
					if (startGrates || (dollMode && (cam.GetComponent<Cam>().on || cam.GetComponent<Light>().intensity > 0.01f) && ceilingOnScreen || dollMode && activateDollMode)){
						
						grates.gameObject.SetActive(true);
						paths.gameObject.SetActive(false);
						
						if (!soundPlayed)
						{
							//audioSource.PlayOneShot(metalGrate, 1f);
							audioSource.clip = metalGrate;
							audioSource.volume = 1.0f;
							audioSource.Play();
							soundPlayed = true;
						}
						
						if (gratesHidden == 0)
						{
							
							oldGratesList.Clear();
							oldGratesList.Add(currentGratesList[0]);
							oldGratesList.Add(currentGratesList[1]);
							if (currentGratesList.Count > 2)
								oldGratesList.Add(currentGratesList[2]);
							currentGratesList.Clear();
							
							foreach (Renderer rend in grates.GetComponentsInChildren<Renderer>())
								rend.enabled = true;
							
							//if doll is not over bed
							if (doll.GetComponent<DollScript>().grateNum != 200 && (grates.GetChild(15).GetComponent<Renderer>().enabled || !dollRestrained))
							{
								grates.GetChild(doll.GetComponent<DollScript>().grateNum).GetComponent<Renderer>().enabled = false;
							}
							//if doll is over bed
							else
							{
								grates.GetChild(15).GetComponent<Renderer>().enabled = false;
							}
							gratesHidden++;
						}
						
						if (gratesHidden < 3 && gratesHidden > 0)
						{
							//Row 1
							
							//1_1
							if (!grates.GetChild(0).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 0)
							{
								currentGratesList.Add(0);
								int rand = Random.Range(1,3);
								if (rand == 1)
								{
									if (grates.GetChild(1).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(1).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 0;
										currentGratesList.Add(1);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(4).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(4).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 0;
										currentGratesList.Add(4);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
							}
							
							//1_2
							else if (!grates.GetChild(1).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 1)
							{
								currentGratesList.Add(1);
								int rand = Random.Range(1,4);
								if (rand == 1)
								{
									if (grates.GetChild(0).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(0).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 1;
										currentGratesList.Add(0);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(2).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(2).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 1;
										currentGratesList.Add(2);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(5).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(5).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 1;
										currentGratesList.Add(5);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
							}
							
							//1_3
							else if (!grates.GetChild(2).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 2)
							{
								currentGratesList.Add(2);
								int rand = Random.Range(1,4);
								if (rand == 1)
								{
									if (grates.GetChild(1).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(1).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 2;
										currentGratesList.Add(1);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(3).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(3).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 2;
										currentGratesList.Add(3);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(6).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(6).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 2;
										currentGratesList.Add(6);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
								
							}
							
							//1_4
							if (!grates.GetChild(3).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 3)
							{
								currentGratesList.Add(3);
								int rand = Random.Range(1,3);
								if (rand == 1)
								{
									if (grates.GetChild(2).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(2).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 3;
										currentGratesList.Add(2);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(7).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(7).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 3;
										currentGratesList.Add(7);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
							}
							
							//Row 2
							
							//2_1
							else if (!grates.GetChild(4).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 4)
							{
								currentGratesList.Add(4);
								int rand = Random.Range(1,4);
								if (rand == 1)
								{
									if (grates.GetChild(0).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(0).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 4;
										currentGratesList.Add(0);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(5).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(5).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 4;
										currentGratesList.Add(5);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(8).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(8).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 4;
										currentGratesList.Add(8);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
								
							}
							
							//2_2
							else if (!grates.GetChild(5).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 5)
							{
								currentGratesList.Add(5);
								int rand = Random.Range(1,5);
								if (rand == 1)
								{
									if (grates.GetChild(1).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(1).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 5;
										currentGratesList.Add(1);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(4).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(4).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 5;
										currentGratesList.Add(4);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(6).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(6).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 5;
										currentGratesList.Add(6);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
								if (rand == 4)
								{
									if (grates.GetChild(9).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(9).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 5;
										currentGratesList.Add(9);
										gratesHidden++;
									}
									else
									{
										rand = 5;
									}
								}
								
							}
							
							//2_3
							else if (!grates.GetChild(6).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 6)
							{
								currentGratesList.Add(6);
								int rand = Random.Range(1,5);
								if (rand == 1)
								{
									if (grates.GetChild(2).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(2).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 6;
										currentGratesList.Add(2);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(5).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(5).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 6;
										currentGratesList.Add(5);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(7).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(7).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 6;
										currentGratesList.Add(7);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
								if (rand == 4)
								{
									if (grates.GetChild(10).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(10).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 6;
										currentGratesList.Add(10);
										gratesHidden++;
									}
									else
									{
										rand = 5;
									}
								}
								
							}
							
							//2_4
							else if (!grates.GetChild(7).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 7)
							{
								currentGratesList.Add(7);
								int rand = Random.Range(1,4);
								if (rand == 1)
								{
									if (grates.GetChild(3).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(3).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 7;
										currentGratesList.Add(3);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(6).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(6).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 7;
										currentGratesList.Add(6);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(11).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(11).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 7;
										currentGratesList.Add(11);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
								
							}
							
							//Row 3
							
							//3_1
							else if (!grates.GetChild(8).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 8)
							{
								currentGratesList.Add(8);
								int rand = Random.Range(1,4);
								if (rand == 1)
								{
									if (grates.GetChild(4).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(4).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 8;
										currentGratesList.Add(4);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(9).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(9).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 8;
										currentGratesList.Add(9);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(12).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(12).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 8;
										currentGratesList.Add(12);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
								
							}
							
							//3_2
							else if (!grates.GetChild(9).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 9)
							{
								currentGratesList.Add(9);
								int rand = Random.Range(1,5);
								if (rand == 1)
								{
									if (grates.GetChild(5).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(5).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 9;
										currentGratesList.Add(5);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(8).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(8).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 9;
										currentGratesList.Add(8);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(10).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(10).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 9;
										currentGratesList.Add(10);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
								if (rand == 4)
								{
									if (grates.GetChild(13).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(13).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 9;
										currentGratesList.Add(13);
										gratesHidden++;
									}
									else
									{
										rand = 5;
									}
								}
								
							}
							
							//3_3
							else if (!grates.GetChild(10).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 10)
							{
								currentGratesList.Add(10);
								int rand = Random.Range(1,5);
								if (rand == 1)
								{
									if (grates.GetChild(6).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(6).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 10;
										currentGratesList.Add(6);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(9).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(9).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 10;
										currentGratesList.Add(9);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(11).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(11).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 10;
										currentGratesList.Add(11);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
								if (rand == 4)
								{
									if (grates.GetChild(14).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(14).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 10;
										currentGratesList.Add(14);
										gratesHidden++;
									}
									else
									{
										rand = 5;
									}
								}
								
							}
							
							//3_4
							else if (!grates.GetChild(11).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 11)
							{
								currentGratesList.Add(11);
								int rand = Random.Range(1,4);
								if (rand == 1)
								{
									if (grates.GetChild(7).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(7).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 11;
										currentGratesList.Add(7);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(10).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(10).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 11;
										currentGratesList.Add(10);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(15).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(15).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 11;
										currentGratesList.Add(15);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
								
							}
							
							//Row 4
							
							//4_1
							if (!grates.GetChild(12).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 12)
							{
								currentGratesList.Add(12);
								int rand = Random.Range(1,3);
								if (rand == 1)
								{
									if (grates.GetChild(8).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(8).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 12;
										currentGratesList.Add(8);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(13).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(13).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 12;
										currentGratesList.Add(13);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
							}
							
							//4_2
							else if (!grates.GetChild(13).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 13)
							{
								currentGratesList.Add(13);
								int rand = Random.Range(1,4);
								if (rand == 1)
								{
									if (grates.GetChild(12).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(12).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 13;
										currentGratesList.Add(12);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(9).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(9).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 13;
										currentGratesList.Add(9);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(14).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(14).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 13;
										currentGratesList.Add(14);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
							}
							
							//4_3
							else if (!grates.GetChild(14).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 14)
							{
								currentGratesList.Add(14);
								int rand = Random.Range(1,4);
								if (rand == 1)
								{
									if (grates.GetChild(13).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(13).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 14;
										currentGratesList.Add(13);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(10).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(10).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 14;
										currentGratesList.Add(10);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								if (rand == 3)
								{
									if (grates.GetChild(15).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(15).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 14;
										currentGratesList.Add(15);
										gratesHidden++;
									}
									else
									{
										rand = 4;
									}
								}
								
							}
							
							//4_4
							else if (!grates.GetChild(15).GetComponent<Renderer>().enabled && gratesHidden < 3 && lastGrateHid != 15)
							{
								currentGratesList.Add(15);
								int rand = Random.Range(1,3);
								if (rand == 1)
								{
									if (grates.GetChild(14).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(14).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 15;
										currentGratesList.Add(14);
										gratesHidden++;
									}
									else
									{
										rand = 2;
									}
								}
								if (rand == 2)
								{
									if (grates.GetChild(11).GetComponent<Renderer>().enabled)
									{
										grates.GetChild(11).GetComponent<Renderer>().enabled = false;
										lastGrateHid = 15;
										currentGratesList.Add(11);
										gratesHidden++;
									}
									else
									{
										rand = 3;
									}
								}
								
							}
							
							
						}
						
						currentGratesList = currentGratesList.Distinct().ToList();
						//Resetting grates if the same combo was picked
						if (gratesHidden == 3 && currentGratesList.Count >= 3)
						{
							if (oldGratesList.Contains(currentGratesList[0]) && oldGratesList.Contains(currentGratesList[1]) && oldGratesList.Contains(currentGratesList[2]))
							{
								gratesHidden = 0;
							}
							else
							{
								
								startGrates = false;
							}
						}
						
						if (gratesHidden == 3)
							activateDollMode = false;
					}
					else {
						soundPlayed = false;
						gratesHidden = 0;
						lastGrateHid = -1;
						
					}
					
					//doll touched the edge of maze
					if (dollMode && !activateDollMode && doll.GetComponent<DollScript>().grateNum != 100 && doll.GetComponent<DollScript>().grateNum != 200 && (grates.GetChild(15).GetComponent<Renderer>().enabled || !dollRestrained) && grates.GetChild(doll.GetComponent<DollScript>().grateNum).GetComponent<Renderer>().enabled)
					{
						if (!passive)
						{
							/*
							transform.position = startPos;
							transform.eulerAngles = startRot;
							*/
							animator.CrossFade("Landing", 0f, -1, 0f);
							//animator.speed = 0.66f + (speed1/3);
							if (doll.GetComponent<DollScript>().grateNum != 15)
								dollHitPointTransition = new Vector3(grates.GetChild(doll.GetComponent<DollScript>().grateNum).transform.position.x, dollHitPointTransition.y, grates.GetChild(doll.GetComponent<DollScript>().grateNum).transform.position.z);
							else
								dollHitPointTransition = new Vector3(grates.GetChild(doll.GetComponent<DollScript>().grateNum).transform.position.x - 0.3f, dollHitPointTransition.y, grates.GetChild(doll.GetComponent<DollScript>().grateNum).transform.position.z + 0.7f);
							dropping = true;
							dollSpeed = 12;
							doll.transform.GetChild(1).GetComponent<Animator>().SetFloat("Speed", dollSpeed);
							dollMode = false;
						}
						else
						{
							dollTouchingGrate = true;
						}
					}
					else
					{
						oldGrateNum = doll.GetComponent<DollScript>().grateNum;
						dollTouchingGrate = false;
					}
					
					//if doll is over the bed
					if ((!dollTouchingGrate || !passive) && (doll.GetComponent<DollScript>().grateNum == 15 || !grates.GetChild(15).GetComponent<Renderer>().enabled))
					{
						restrainable = true;
					}
					else
					{
						restrainable = false;
					}
					//restraining doll
					if (restrainable)
					{
						if (dollRestrained && dollMode && !activateDollMode)
						{
							/*
							transform.position = startPos;
							transform.eulerAngles = startRot;
							*/
							dropping = true;
							animator.CrossFade("Landing", 0f, -1, 0f);
							doll.transform.position = lockPos;
							doll.transform.GetChild(1).transform.localPosition = dollRestrainedPos;
							doll.transform.GetChild(1).GetComponent<Animator>().CrossFade("DollSit", 0f, -1, 0f);
							doll.transform.eulerAngles = new Vector3(doll.transform.eulerAngles.x, 341.54f, doll.transform.eulerAngles.z);
							dollHitPoint = lockPos;
							dollHitPoint2 = lockPos;
							dollHitPointTransition = lockPos;
							dollMode = false;
						}
						
					}
					
					//setting Dolly path
					if (!dollMode)
					{
						grates.gameObject.SetActive(false);
						paths.gameObject.SetActive(true);
						
						if (finalGrate == -1)
						{
							//if doll is not over bed
							if (doll.GetComponent<DollScript>().grateNum != 200 && (grates.GetChild(15).GetComponent<Renderer>().enabled || !dollRestrained))
							{
								int childrenNum = paths.transform.GetChild(doll.GetComponent<DollScript>().grateNum).transform.childCount;
								int rand = Random.Range(0,childrenNum);
								
								paths.transform.GetChild(doll.GetComponent<DollScript>().grateNum).GetChild(rand).gameObject.SetActive(true);
								
								finalRand = rand;
								finalGrate = doll.GetComponent<DollScript>().grateNum;
							}
							//if doll is over bed
							else
							{
								int childrenNum = paths.transform.GetChild(15).transform.childCount;
								int rand = Random.Range(0,childrenNum);
								
								paths.transform.GetChild(15).GetChild(rand).gameObject.SetActive(true);
								
								finalRand = rand;
								finalGrate = 15;
							}
						}
						
						
						
						
					}
					else
					{
						if (finalGrate != -1 && finalRand != -1)
						{
							paths.transform.GetChild(finalGrate).GetChild(finalRand).gameObject.SetActive(false);
							grates.gameObject.SetActive(true);
							paths.gameObject.SetActive(false);
						}
						finalRand = -1;
						finalGrate = -1;
					}
				
				}
			}
			
			if (jumpScare)
			{
				pauseMenu.GetComponent<PauseMenu>().locked = true;
				
				if (Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), transform.position) > 0.8f){
					dollySpeed += 0.1f;
					animator.SetFloat("Speed", dollySpeed);
					Vector3 lookAtRotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position).eulerAngles;
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0))), 20 * Time.deltaTime);
					
					ifLight.GetComponent<Light>().intensity = 2;
					//flashlight.GetComponent<Light>().intensity = 0;
					flashlight.GetComponent<Light>().shadowStrength = 0;
					//cam.GetComponent<Light>().intensity = 0;
					cam.GetComponent<Light>().shadowStrength = 0;
					
					flashlightObject.transform.parent = Camera.main.transform;
					flashlightObject.GetComponent<CameraController>().enabled = false;
					flashlightObject.transform.localPosition = Vector3.Lerp(flashlightObject.transform.localPosition, new Vector3(flashlightObject.transform.localPosition.x, -4, flashlightObject.transform.localPosition.z), 2 * Time.deltaTime);
					
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
				else {
					flashlight.GetComponent<Light>().intensity = 0;
					cam.GetComponent<Light>().intensity = 0;
					Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,20,0)) - Camera.main.transform.position);
					//lookRotation.y = 12;
					
					Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 3 * Time.deltaTime);
				}
				timer += Time.deltaTime;
				
				transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), 5.5f * Time.deltaTime);
				
				if (Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), transform.position) <= 0.4f)
				{
					jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(1);
					jumpScareActivate = true;
				}
				
			}
		
		}
		else
		{
			if (audioSource.isPlaying)
				audioPlaying = false;
			audioSource.Pause();
		}
		
    }
}
