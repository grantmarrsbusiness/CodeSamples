using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoloAI : MonoBehaviour
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
	public Transform playerPos;
	public int spot;
	
	public Transform player;
	
	public Transform marco;
	
	IEnumerator co;
	public float waitTime;
	public GameObject flashlight;
	public GameObject cam;
	public GameObject mic;
	public GameObject flashlightObject;
	public GameObject camObject;
	public GameObject micObject;
	public GameObject bucketObject;
	public GameObject ifLight;
	public bool canMove = true;
	public bool inView = false;
	
	public bool chasingFlush;
	
	public GameObject torso;
	public GameObject hands;
	
	public float lookTimer = 0;
	public bool reverse;
	
	public int scareSpot;
	public float scareTimer;
	
	public bool knockedOut;
	public float koTimer = 0;
	
	public Transform door1;
	public Transform door2;
	public Transform door3;
	
	public bool stallJumpscare;
	public bool jumpScareStart;
	public float jumpScareStartTimer;
	
	private Animator animator; //the "Animator" component of the script holder
	
	
	AudioSource audioSource;
	public AudioClip calmOpen;
	public AudioClip slamOpen;
	private bool audioPlaying;
    
	private float timer = 0.0f;		//counting timer
	public bool jumpScare = false;
	public bool jumpScareActivate = false;
	
    // Start is called before the first frame update
    void Start()
    {
        
		audioSource = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		animator.CrossFade("Walking", 0f, -1, 0f);
		
		//spot = 0;
		co = waiter(); // create an IEnumerator object
		scareSpot = spot;
		//MoveSpot();
		//spot = 0;
		
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
	
	IEnumerator waiter()
    {
        yield return new WaitForSeconds (waitTime);
		while(!canMove) yield return null;
        //MoveSpot();
    }
	
	void MoveSpot()
	{
		/*
		
		if (spot == 0)
		{
			int rand = Random.Range(1,3);
			if (rand == 1)
				spot = 1;
			else if (rand == 2)
				spot = 2;
			
		}
		else if (spot == 1)
		{
			int rand = Random.Range(1,3);
			if (rand == 1)
				spot = 0;
			else if (rand == 2)
				spot = 2;
			
		}
		else if (spot == 2)
		{
			int rand = Random.Range(1,3);
			if (rand == 1)
				spot = 0;
			else if (rand == 2)
				spot = 1;
			
		}
		
		if (spot == 0)
		{
			audioSource.Stop();
			//audioSource.PlayOneShot(walk, 1.0f);
			waitTime = Random.Range (minWait, maxWait);
			animator.CrossFade("Sitting", 0f, -1, 0f);
		}
		else if (spot == 1)
		{
			audioSource.Stop();
			//audioSource.PlayOneShot(walk, 1.0f);
			waitTime = Random.Range (minWait, maxWait);
			animator.CrossFade("Sitting", 0f, -1, 0f);
		}
		else if (spot == 2)
		{
			audioSource.Stop();
			//audioSource.PlayOneShot(walk, 1.0f);
			waitTime = Random.Range (minWait, maxWait);
			animator.CrossFade("Sitting", 0f, -1, 0f);
		}
		
		if (!jumpScare)
		{
			transform.position = pos.GetChild(spot).position;
		}
	
		co = waiter();
		StartCoroutine (co);
		*/
	}
	
	IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Level 4 Jumpscares");
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
	
	public void ResetPos()
	{
		animator.SetBool("Open", false);
		animator.SetBool("Look", false);
		animator.SetBool("Walk", true);
		animator.CrossFade("Walking", 0.1f, -1, 0f);
		if (resetFrozen)
		{
			float spot2Dist = Vector3.Distance(transform.position, new Vector3(pos.GetChild(2).position.x, 0, pos.GetChild(2).position.z));
			float spot4Dist = Vector3.Distance(transform.position, new Vector3(pos.GetChild(4).position.x, 0, pos.GetChild(4).position.z));
			float spot6Dist = Vector3.Distance(transform.position, new Vector3(pos.GetChild(6).position.x, 0, pos.GetChild(6).position.z));
			if (spot2Dist < spot4Dist && spot2Dist < spot6Dist)
			{
				spot = 2;
			}
			else if (spot4Dist < spot2Dist && spot4Dist < spot6Dist)
			{
				spot = 4;
			}
			else if (spot6Dist < spot2Dist && spot6Dist < spot4Dist)
			{
				spot = 6;
			}
			resetFrozen = false;
		}
		transform.position = new Vector3(pos.GetChild(spot).position.x, 0, pos.GetChild(spot).position.z);
		transform.eulerAngles = new Vector3(0, 0, 0);
		lookTimer = 0;
		if (spot == 7)
		{
			spot = 2;
		}
		else if (spot == 8)
		{
			spot = 4;
		}
		else if (spot == 9)
		{
			spot = 6;
		}
		//audioSource.PlayOneShot(calmOpen);
		audioSource.clip = calmOpen;
		audioSource.volume = 1.0f;
		audioSource.Play();
		playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open = true;
		chasingFlush = false;
		jumpScare = false;
		transform.GetChild(0).gameObject.SetActive(true);
		transform.GetChild(1).gameObject.SetActive(true);
		transform.GetChild(2).gameObject.SetActive(true);
		transform.GetChild(3).gameObject.SetActive(true);
		transform.GetChild(4).gameObject.SetActive(true);
		transform.GetChild(5).gameObject.SetActive(true);
		transform.GetChild(6).gameObject.SetActive(true);
		transform.GetChild(11).gameObject.SetActive(true);
		transform.GetChild(12).gameObject.SetActive(true);
		knockedOut = false;
	}
	
	public float stuckTimer = 0;
	public Vector3 oldPos;
	public Vector3 oldRot;
	public bool resetFrozen = false;
	
    // Update is called once per frame
    void Update()
    {
		
		if (Time.timeScale == 1)
		{
			
			
			if (oldPos == transform.position && oldRot == transform.eulerAngles && !animator.GetBool("Look") && !animator.GetBool("Walk") && !knockedOut && !jumpScare)
			{
				stuckTimer += Time.deltaTime;
			}
			else
			{
				stuckTimer = 0;
			}
			
			if (stuckTimer > 0.2f)
			{
				resetFrozen = true;
			}
			oldPos = transform.position;
			oldRot = transform.eulerAngles;
			
			if (resetFrozen)
			{
				ResetPos();
				//resetFrozen = false;
			}
			
			if (!audioPlaying)
			{
				audioSource.Play();
				audioPlaying = true;
			}
			
			if (!knockedOut)
			{
				
				Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
				bool onScreen = screenPoint.z > 0 && screenPoint.x > -1 && screenPoint.x < 2 && screenPoint.y > -1 && screenPoint.y < 2;
				
				/*
				if (!flashlight.GetComponent<Flashlight>().on && !cam.GetComponent<Cam>().on && cam.GetComponent<Light>().intensity <= 0.01f || !onScreen){
					canMove = true;
					foreach (Renderer rend in GetComponentsInChildren<Renderer>())
						rend.enabled = false;
					foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
							col.enabled = false;
				}
				else{
					canMove = false;
					foreach (Renderer rend in GetComponentsInChildren<Renderer>())
						rend.enabled = true;
					foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
							col.enabled = true;
				}
				*/
				
				
				if (!jumpScare)
				{
					
					//Making sure spot stays in bounds
					if (!chasingFlush)
					{
						if (spot <= 0)
						{
							spot = 0;
						}
						if (spot >= 6)
						{
							spot = 6;
						}
					}
					else
					{
						torso.GetComponent<Renderer>().enabled = true;
						hands.GetComponent<Renderer>().enabled = true;
					}
					
					
					//if (Vector3.Distance(new Vector3(pos.GetChild(spot).position.x, transform.position.y, pos.GetChild(spot).position.z), transform.position) < 0.5f)
					//{
						
						
						//if (scareSpot == 2 || scareSpot == 3 || scareSpot == 4)
						//{
							if (!jumpScareStart && !chasingFlush
							&& (Mathf.Abs(transform.localPosition.x - (-0.5f)) < 0.3f && playerPos.GetChild(2).GetChild(0).GetComponent<OpenStall>().open
							|| Mathf.Abs(transform.localPosition.x - (0.8f)) < 0.3f && playerPos.GetChild(3).GetChild(0).GetComponent<OpenStall>().open
							|| Mathf.Abs(transform.localPosition.x - (2.2f)) < 0.3f && playerPos.GetChild(4).GetChild(0).GetComponent<OpenStall>().open))
							{
								if (Mathf.Abs(Camera.main.transform.position.x - transform.position.x) < 0.2f)
								{
									animator.CrossFade("Running", 0.1f, -1, 0f);
									jumpScare = true;
								}
							}
						//}
						
						
					//}
					
					
					if (Vector3.Distance(new Vector3(pos.GetChild(spot).position.x, transform.position.y, pos.GetChild(spot).position.z), transform.position) < 0.05f)
					{
						
						
						//transform.position = new Vector3(pos.GetChild(spot).position.x, transform.position.y, pos.GetChild(spot).position.z);
						
						if (!chasingFlush)
						{
						
							if (spot <= 0)
							{
								reverse = false;
							}
							if (spot >= 6)
							{
								reverse = true;
							}
						
							
							//tubs
							if (spot == 1 || spot == 3 || spot == 5)
							{
								transform.position = new Vector3(pos.GetChild(spot).position.x, transform.position.y, pos.GetChild(spot).position.z);
								if (lookTimer > minWait)
								{
									animator.SetBool("Open", false);
									animator.SetBool("Look", false);
									animator.SetBool("Walk", true);
									
									if (!reverse)
									{
										spot++;
									}
									else
									{
										spot--;
									}
									lookTimer = 0;
								}
								else {
									if (spot == 1 || spot == 3 || spot == 5)
									{
										animator.SetBool("Open", false);
										animator.SetBool("Look", true);
										animator.SetBool("Walk", false);
									}
								}
								
							}
							//doors
							else if (spot == 2 || spot == 4 || spot == 6)
							{
								
								
								
								if (Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), transform.position) < 2f && playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open || playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().lea.y < -25 || spot == 1 || spot == 3 || spot == 5)
								{
									torso.GetComponent<Renderer>().enabled = true;
									hands.GetComponent<Renderer>().enabled = true;
								}
								else
								{
									torso.GetComponent<Renderer>().enabled = false;
									hands.GetComponent<Renderer>().enabled = false;
								}
								
								animator.SetBool("Walk", false);
								animator.SetBool("Look", false);
								animator.SetBool("Open", true);
								
								if (!playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open)
								{
									transform.position = new Vector3(pos.GetChild(spot).position.x, transform.position.y, pos.GetChild(spot).position.z);
									if (lookTimer > 1.5f)
									{
										animator.SetBool("Open", false);
										animator.SetBool("Look", false);
										animator.SetBool("Walk", true);
										scareSpot = spot/2 + 1;
										scareTimer = 0;
										if (!reverse)
										{
											spot += 2;
										}
										else
										{
											spot -= 2;
										}
										lookTimer = 0;
									}
									else if (lookTimer > 0.8f)
									{
										//audioSource.PlayOneShot(slamOpen);
										audioSource.clip = slamOpen;
										audioSource.volume = 1.0f;
										audioSource.Play();
										playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open = true;
										scareSpot = spot/2 + 1;
										scareTimer = 0;
										if (playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().on && playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open)
										{
											if (playerPos.GetChild(spot/2 + 1).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot/2 + 1).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot/2 + 1).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot/2 + 1).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
											{
												jumpScareStart = true;
											}
										}
									}
								}
								else
								{
									
									if (!jumpScareStart && playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().on && playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open)
									{
										if (playerPos.GetChild(spot/2 + 1).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot/2 + 1).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot/2 + 1).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot/2 + 1).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
										{
											animator.CrossFade("Running", 0.1f, -1, 0f);
											jumpScare = true;
										}
									}
									
									//transform.position = Vector3.Lerp(transform.position, new Vector3(pos.GetChild(spot).position.x, transform.position.y, pos.GetChild(spot).position.z), 5 * Time.deltaTime);
									animator.SetBool("Open", false);
									animator.SetBool("Look", false);
									animator.SetBool("Walk", true);
									scareSpot = spot/2 + 1;
									scareTimer = 0;
									if (!reverse)
									{
										spot += 2;
									}
									else
									{
										spot -= 2;
									}
									lookTimer = 0;
								}
								
								
								Vector3 lookAtRotation3 = Quaternion.LookRotation(playerPos.GetChild(spot/2 + 1).GetChild(0).position - transform.position).eulerAngles;
								transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation3, new Vector3(0,1,0))), 5 * Time.deltaTime);
								
							}
							//open stall
							else if (spot == 0)
							{
								
								if (playerPos.GetChild(spot/2 + 1).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot/2 + 1).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot/2 + 1).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot/2 + 1).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
								{
									animator.CrossFade("Running", 0.1f, -1, 0f);
									jumpScare = true;
								}
								
								transform.position = Vector3.Lerp(transform.position, new Vector3(pos.GetChild(spot).position.x, transform.position.y, pos.GetChild(spot).position.z), 5 * Time.deltaTime);
								animator.SetBool("Open", false);
								animator.SetBool("Look", false);
								animator.SetBool("Walk", true);
								scareSpot = spot/2 + 1;
								scareTimer = 0;
								if (!reverse)
								{
									spot += 2;
								}
								else
								{
									spot -= 2;
								}
								lookTimer = 0;
							}
							
							
							
						}
						else
						{
							transform.position = new Vector3(pos.GetChild(spot).position.x, transform.position.y, pos.GetChild(spot).position.z);
							
							if (spot == 7 || spot == 8 || spot == 9)
							{
								if (lookTimer > minWait)
								{
									animator.SetBool("Open", false);
									animator.SetBool("Look", false);
									animator.SetBool("Walk", true);
									
									if (spot == 7)
									{
										spot = 2;
										if (playerPos.GetChild(spot/2 + 1).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot/2 + 1).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot/2 + 1).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot/2 + 1).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
										{
											animator.CrossFade("Running", 0.1f, -1, 0f);
											Vector3 lookAtRotation2 = Quaternion.LookRotation(Camera.main.transform.position - transform.position).eulerAngles;
											transform.rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation2, new Vector3(0,1,0)));
											stallJumpscare = true;
											jumpScare = true;
										}
									}
									else if (spot == 8)
									{
										spot = 4;
										if (playerPos.GetChild(spot/2 + 1).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot/2 + 1).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot/2 + 1).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot/2 + 1).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
										{
											animator.CrossFade("Running", 0.1f, -1, 0f);
											Vector3 lookAtRotation2 = Quaternion.LookRotation(Camera.main.transform.position - transform.position).eulerAngles;
											transform.rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation2, new Vector3(0,1,0)));
											stallJumpscare = true;
											jumpScare = true;
										}
									}
									else if (spot == 9)
									{
										spot = 6;
										if (playerPos.GetChild(spot/2 + 1).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot/2 + 1).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot/2 + 1).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot/2 + 1).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
										{
											animator.CrossFade("Running", 0.1f, -1, 0f);
											Vector3 lookAtRotation2 = Quaternion.LookRotation(Camera.main.transform.position - transform.position).eulerAngles;
											transform.rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation2, new Vector3(0,1,0)));
											stallJumpscare = true;
											jumpScare = true;
										}
									}
									lookTimer = 0;
									if (!stallJumpscare)
									{
										//audioSource.PlayOneShot(calmOpen);
										audioSource.clip = calmOpen;
										audioSource.volume = 1.0f;
										audioSource.Play();
										playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open = true;
									}
									chasingFlush = false;
								}
								else {
									
									if (transform.eulerAngles.y < 180)
									{
										transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(transform.eulerAngles.x, 190, transform.eulerAngles.z), 10 * Time.deltaTime);
									}
									if (!animator.GetBool("Look"))
									{
										if (spot == 7)
										{
											//audioSource.PlayOneShot(calmOpen);
											audioSource.clip = calmOpen;
											audioSource.volume = 1.0f;
											audioSource.Play();
											playerPos.GetChild(2/2 + 1).GetChild(0).GetComponent<OpenStall>().open = false;
										}
										else if (spot == 8)
										{
											//audioSource.PlayOneShot(calmOpen);
											audioSource.clip = calmOpen;
											audioSource.volume = 1.0f;
											audioSource.Play();
											playerPos.GetChild(4/2 + 1).GetChild(0).GetComponent<OpenStall>().open = false;
										}
										else if (spot == 9)
										{
											//audioSource.PlayOneShot(calmOpen);
											audioSource.clip = calmOpen;
											audioSource.volume = 1.0f;
											audioSource.Play();
											playerPos.GetChild(6/2 + 1).GetChild(0).GetComponent<OpenStall>().open = false;
										}
									}
									animator.SetBool("Open", false);
									animator.SetBool("Look", true);
									animator.SetBool("Walk", false);
								}
							}
							else if (spot == 2)
							{
								//audioSource.PlayOneShot(slamOpen);
								audioSource.clip = slamOpen;
								audioSource.volume = 1.0f;
								audioSource.Play();
								playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open = true;
								if (playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().on && playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open)
								{
									if (playerPos.GetChild(spot/2 + 1).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot/2 + 1).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot/2 + 1).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot/2 + 1).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
									{
										animator.CrossFade("Running", 0.1f, -1, 0f);
										jumpScare = true;
									}
								}
								lookTimer = 0;
								spot = 7;
							}
							else if (spot == 4)
							{
								//audioSource.PlayOneShot(slamOpen);
								audioSource.clip = slamOpen;
								audioSource.volume = 1.0f;
								audioSource.Play();
								playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open = true;
								if (playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().on && playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open)
								{
									if (playerPos.GetChild(spot/2 + 1).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot/2 + 1).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot/2 + 1).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot/2 + 1).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
									{
										animator.CrossFade("Running", 0.1f, -1, 0f);
										jumpScare = true;
									}
								}
								lookTimer = 0;
								spot = 8;
							}
							else if (spot == 6)
							{
								//audioSource.PlayOneShot(slamOpen);
								audioSource.clip = slamOpen;
								audioSource.volume = 1.0f;
								audioSource.Play();
								playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open = true;
								if (playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().on && playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open)
								{
									if (playerPos.GetChild(spot/2 + 1).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot/2 + 1).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot/2 + 1).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot/2 + 1).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
									{
										animator.CrossFade("Running", 0.1f, -1, 0f);
										jumpScare = true;
									}
								}
								lookTimer = 0;
								spot = 9;
							}
							
							if (passive)
							{
								if (!marco.GetComponent<MarcoAI>().chasing)
									marco.GetComponent<MarcoAI>().PassiveJump(spot);
							}
						}
						
						lookTimer += Time.deltaTime;
						
					}
					else
					{
						if (!reverse)
						{
							animator.SetFloat("Reverse", 0);
						}
						else
						{
							animator.SetFloat("Reverse", 1);
						}
						if (!chasingFlush)
						{
							Vector3 lookAtRotation2 = Quaternion.LookRotation(pos.GetChild(spot).position - transform.position).eulerAngles;
							transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation2, new Vector3(0,1,0))), 5 * Time.deltaTime);
						}
						else
						{
							Vector3 lookAtRotation2 = Quaternion.LookRotation(pos.GetChild(spot).position - transform.position).eulerAngles;
							//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation2, new Vector3(0,1,0))), 20 * Time.deltaTime);
							transform.rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation2, new Vector3(0,1,0)));
						}
					}
					
					
					if (!torso.GetComponent<Renderer>().enabled || !hands.GetComponent<Renderer>().enabled)
					{
						if (!animator.GetBool("Open") || Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), transform.position) < 2f && playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open || playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().lea.y < -25 || spot == 1 || spot == 3 || spot == 5)
						{
							torso.GetComponent<Renderer>().enabled = true;
							hands.GetComponent<Renderer>().enabled = true;
						}
					}
					
					
					
					if (spot < 7 && Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, transform.position.z), transform.position) < 0.5f && !jumpScareStart && playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().on && playerPos.GetChild(spot/2 + 1).GetChild(0).GetComponent<OpenStall>().open)
					{
						if (playerPos.GetChild(spot/2 + 1).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot/2 + 1).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot/2 + 1).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot/2 + 1).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
						{
							animator.CrossFade("Running", 0.1f, -1, 0f);
							jumpScare = true;
						}
					}
					if (spot < 7 && scareTimer < 1 && !jumpScareStart && playerPos.GetChild(scareSpot).GetChild(0).GetComponent<OpenStall>().on && playerPos.GetChild(scareSpot).GetChild(0).GetComponent<OpenStall>().open)
					{
						if (playerPos.GetChild(scareSpot).GetChild(3).gameObject.activeSelf && playerPos.GetChild(scareSpot).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(scareSpot).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(scareSpot).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
						{
							animator.CrossFade("Running", 0.1f, -1, 0f);
							jumpScare = true;
						}
					}
					scareTimer += Time.deltaTime;
					
					if (jumpScareStart)
					{
						if (jumpScareStartTimer > 0.2f)
						{
							animator.CrossFade("Running", 0.4f, -1, 0f);
							Vector3 lookAtRotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position).eulerAngles;
							transform.rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0)));
							jumpScare = true;
						}
						jumpScareStartTimer += Time.deltaTime;
					}
					
				}
				
				//Running at you
				if (jumpScareStart || jumpScare)
				{
					torso.GetComponent<Renderer>().enabled = true;
					hands.GetComponent<Renderer>().enabled = true;
					
					if (stallJumpscare || Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), transform.position) > 0.8f){
						
						pauseMenu.GetComponent<PauseMenu>().locked = true;
						
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
						
						bucketObject.transform.parent = Camera.main.transform;
						bucketObject.GetComponent<CameraController>().enabled = false;
						bucketObject.transform.localPosition = Vector3.Lerp(bucketObject.transform.localPosition, new Vector3(bucketObject.transform.localPosition.x, -4, bucketObject.transform.localPosition.z), 2 * Time.deltaTime);
						
						if (stallJumpscare)
						{
							if (player.transform.position.y < -0.1f)
							{
								transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), 2f * Time.deltaTime);
								animator.SetBool("Down", true);
							}
							else
							{
								animator.SetBool("Down", false);
							}
						}
						
						Camera.main.GetComponent<CameraController>().enabled = false;
						//Camera.main.transform.LookAt(transform);
						Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,1,0)) - Camera.main.transform.position);
						//lookRotation.y = 12;
						
						Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 25 * Time.deltaTime);
					}
					else {
						
						if (player.transform.position.y < -0.1f)
						{
							transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), 2f * Time.deltaTime);
							animator.SetBool("Down", true);
						}
						else
						{
							animator.SetBool("Down", false);
						}
						
						flashlight.GetComponent<Light>().intensity = 0;
						cam.GetComponent<Light>().intensity = 0;
						Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,20,0)) - Camera.main.transform.position);
						//lookRotation.y = 12;
						
						Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 5 * Time.deltaTime);
					}
					timer += Time.deltaTime;
					
					//transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), 5.5f * Time.deltaTime);
					
					if (Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), transform.position) <= 0.4f)
					{
						if (marco.GetComponent<MarcoAI>().diveTimer > 0.5f || !marco.GetComponent<MarcoAI>().resetPos)
							jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(1);
						else
							jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(2);
						jumpScareActivate = true;
					}
					
				}
				
				/*
				//Mic is enabled/disabled
				if (mic.GetComponent<Mic>().on)
					audioSource.volume = 1;
				else
					audioSource.volume = 0;
				*/
			
			}
			else
			{
				
				Vector3 screenPoint2 = Camera.main.WorldToViewportPoint(transform.position);
				bool onScreen2 = screenPoint2.z > 0 && screenPoint2.x > -1 && screenPoint2.x < 2 && screenPoint2.y > -1 && screenPoint2.y < 2;
				if (Mathf.Abs(Camera.main.transform.position.x - transform.position.x) < 0.6f)
				{
					if (cam.GetComponent<Cam>().on && onScreen2){
						jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(3);
						jumpScareActivate = true;
						//SceneManager.LoadScene("Level 1 Win");
					}
				}
				
				if (koTimer > minWait && !passive || koTimer > 30 && passive)
				{
					ResetPos();
					marco.GetComponent<MarcoAI>().dizzyDucks.SetActive(false);
					marco.GetComponent<MarcoAI>().ResetPos();
					koTimer = 0;
				}
				
				koTimer += Time.deltaTime;
				
			}
			
			if (!animator.GetBool("Look") && chasingFlush && passive && !jumpScare)
			{
				animator.speed = 0.75f;
			}
			else
			{
				animator.speed = 1.0f;
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
