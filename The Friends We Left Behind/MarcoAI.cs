using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MarcoAI : MonoBehaviour
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
	
	IEnumerator co;
	IEnumerator soundCo;
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
	
	public Vector3 chaseSpot;
	public bool chasing;
	
	public Transform duck;
	public Transform duckPos;
	public GameObject dizzyDucks;
	
	public GameObject explosion;
	public float diveTimer;
	
	public bool up = false;
	public float upTimer = 0;
	public bool goingUp;
	
	public Transform door1;
	public Transform door2;
	public Transform door3;
	
	public Transform polo;
	
	private Animator animator; //the "Animator" component of the script holder
	
	private bool audioPlaying;
	private bool audioPlaying2;
	private bool audioPlaying3;
	private bool audioPlaying4;
	private bool keepPlaying = true;
	private bool audioStarted = false;
	
	public bool knockedOut;
	
	AudioSource audioSource;
	public AudioClip scribble;
	public AudioClip marcoSound;
	public AudioClip poloSound;
	public AudioClip waterUp;
	public AudioClip waterDown;
	public AudioClip splash;
	public AudioClip impactSound;
    
	private float timer = 0.0f;		//counting timer
	public bool jumpScare = false;
	public bool jumpScareActivate = false;
	
    // Start is called before the first frame update
    void Start()
    {
        
		audioSource = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		int rand = Random.Range(0,3);
		spot = rand;
		co = waiter(); // create an IEnumerator object
		MoveSpot();
		
		soundCo = SoundOut();
		StartCoroutine (soundCo);
		
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
		
    }
	
	IEnumerator SoundOut()
	{
		
		if (!knockedOut && audioStarted && !transform.GetChild(15).GetComponent<AudioSource>().isPlaying && !polo.GetChild(13).GetComponent<AudioSource>().isPlaying)
		{
			transform.GetChild(15).GetComponent<AudioSource>().clip = marcoSound;
			transform.GetChild(15).GetComponent<AudioSource>().Play();
			transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().clip = waterUp;
			transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().Play();
		}
		audioStarted = true;
		yield return new WaitForSeconds(5);
		if (!knockedOut && !polo.GetComponent<PoloAI>().chasingFlush && !transform.GetChild(15).GetComponent<AudioSource>().isPlaying && !polo.GetChild(13).GetComponent<AudioSource>().isPlaying)
		{
			polo.GetChild(13).GetComponent<AudioSource>().clip = poloSound;
			polo.GetChild(13).GetComponent<AudioSource>().Play();
		}
		yield return new WaitForSeconds(5);
		
	}
	
	IEnumerator waiter()
    {
        yield return new WaitForSeconds (waitTime);
		while(!canMove && up && goingUp) yield return null;
        MoveSpot();
    }
	
	public void PassiveJump (int poloSpot)
	{
		if (poloSpot == 7)
		{
			spot = 0;
		}
		else if (poloSpot == 8)
		{
			spot = 1;
		}
		else if (poloSpot == 9)
		{
			spot = 2;
		}
		StopCoroutine(co);
		transform.position = pos.GetChild(spot).position - new Vector3(0,0.6f,0);
		duck.transform.position = duckPos.GetChild(spot).position;
		if (!chasing)
		{
			transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().clip = waterDown;
			transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().Play();
		}
		//chaseSpot = playerPos.GetChild(spot + 2).GetChild(2).transform.position;
		chaseSpot = Camera.main.transform.position;
		chasing = true;
		//transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,transform.eulerAngles.z);
		//transform.position = new Vector3(transform.position.x, 0, transform.position.z - 0.5f);
		//transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
		animator.SetBool("Jumping", true);
		jumpScare = true;
	}
	
	void MoveSpot()
	{
		/*
		int rand2 = Random.Range(1,6);
		if (rand2 == 1 && !up)
		{
			up = true;
		}
		else
		{
			up = false;
		}
		*/
		
		if (!jumpScare)
		{
			
			if (!up)
			{
				/*
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = false;
				*/
				
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
					animator.CrossFade("Crouching", 0f, -1, 0f);
				}
				else if (spot == 1)
				{
					audioSource.Stop();
					//audioSource.PlayOneShot(walk, 1.0f);
					waitTime = Random.Range (minWait, maxWait);
					animator.CrossFade("Crouching", 0f, -1, 0f);
				}
				else if (spot == 2)
				{
					audioSource.Stop();
					//audioSource.PlayOneShot(walk, 1.0f);
					waitTime = Random.Range (minWait, maxWait);
					animator.CrossFade("Crouching", 0f, -1, 0f);
				}
				
			}
			else
			{
				/*
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = true;
					
				up = false;
				upTimer = 0;
				
				*/
				
			}
			
			
			duck.transform.position = duckPos.GetChild(spot).position;
			transform.position = pos.GetChild(spot).position - new Vector3(0,0.6f,0);
			
		
			co = waiter();
			StartCoroutine (co);
		}
	}
	
	public bool resetPos;
	public void ResetPos ()
	{
		resetPos = true;
		animator.CrossFade("Running", 0.1f, -1, 0f);
		jumpScare = true;
		diveTimer = 0;
		StartCoroutine(ResetSplash());
		chaseSpot = pos.GetChild(spot).position;
		Vector3 lookAtRotation = Quaternion.LookRotation(chaseSpot - transform.position).eulerAngles;
		transform.rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0)));
		animator.SetBool("Jumping", false);
		chasing = false;
	}
	
	IEnumerator ResetSplash()
	{
		yield return new WaitForSeconds (0.8f);
		transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().clip = splash;
		transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().Play();
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
			if (!audioPlaying2)
			{
				transform.GetChild(15).GetComponent<AudioSource>().Play();
				audioPlaying2 = true;
			}
			if (!audioPlaying3)
			{
				polo.GetChild(13).GetComponent<AudioSource>().Play();
				audioPlaying3 = true;
			}
			if (!audioPlaying4)
			{
				transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().Play();
				audioPlaying4 = true;
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
					
					if (!up && upTimer > 10)
					{
						StopCoroutine(co);
						goingUp = true;
						StopCoroutine(soundCo);
						soundCo = SoundOut();
						StartCoroutine (soundCo);
						up = true;
					}
					upTimer += Time.deltaTime;
					
					
					if (up)
					{
						if (goingUp)
						{
							transform.position = Vector3.Lerp(transform.position, pos.GetChild(spot).position, 1 * Time.deltaTime);
							if (Vector3.Distance(transform.position, pos.GetChild(spot).position) < 0.02f)
							{
								transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().clip = waterDown;
								transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().Play();
								goingUp = false;
							}
						}
						else
						{
							transform.position = Vector3.Lerp(transform.position, pos.GetChild(spot).position - new Vector3(0,0.6f,0), 1 * Time.deltaTime);
							if (Vector3.Distance(transform.position, pos.GetChild(spot).position - new Vector3(0,0.6f,0)) < 0.05f)
							{
								goingUp = false;
								up = false;
								upTimer = 0;
								MoveSpot();
							}
						}
					}
					
					if (up && transform.position.y > -0.1f && playerPos.GetChild(spot + 2).GetChild(0).GetComponent<OpenStall>().on && playerPos.GetChild(spot + 2).GetChild(0).GetComponent<OpenStall>().open) // && !playerPos.GetChild(spot + 2).GetChild(1).GetComponent<LayDownText>().layingDown && !playerPos.GetChild(spot + 2).GetChild(1).GetComponent<LayDownText>().down
					{
						if (playerPos.GetChild(spot + 2).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot + 2).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot + 2).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot + 2).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder)
						{
							if (!chasing)
							{
								transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().clip = waterDown;
								transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().Play();
							}
							//chaseSpot = playerPos.GetChild(spot + 2).GetChild(2).transform.position;
							chaseSpot = Camera.main.transform.position;
							chasing = true;
							//transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,transform.eulerAngles.z);
							//transform.position = new Vector3(transform.position.x, 0, transform.position.z - 0.5f);
							//transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
							animator.SetBool("Jumping", true);
							jumpScare = true;
						}
					}
					
				}
				
				
				//Running at you
				if (jumpScare)
				{
					if (Vector3.Distance(new Vector3(chaseSpot.x, transform.position.y, chaseSpot.z), transform.position) > 1.5f){
						Vector3 lookAtRotation = Quaternion.LookRotation(chaseSpot - transform.position).eulerAngles;
						transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0))), 20 * Time.deltaTime);
						
						up = false;
						goingUp = false;
						upTimer = 0;
						if (chasing)
						{
							transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 0, transform.position.z), 2f * Time.deltaTime);
							transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), 0.5f * Time.deltaTime);
						}
						
						//ifLight.GetComponent<Light>().intensity = 2;
						//flashlight.GetComponent<Light>().shadowStrength = 0;
						//cam.GetComponent<Light>().shadowStrength = 0;
						
						/*
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
						
						*/
					}
					else {
						if (chasing)
						{
							if (playerPos.GetChild(spot + 2).GetChild(0).GetComponent<OpenStall>().on && playerPos.GetChild(spot + 2).GetChild(0).GetComponent<OpenStall>().open
							&& (playerPos.GetChild(spot + 2).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot + 2).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot + 2).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot + 2).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder))
							{
								if (player.transform.position.y < -0.1f)
								{
									//transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), 1f * Time.deltaTime);
									transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), 0.5f * Time.deltaTime);
									if (!animator.GetBool("Down"))
									{
										animator.CrossFade("Running", 0.1f, -1, 0f);
									}
									animator.SetBool("Down", true);
								}
								else
								{
									animator.SetBool("Down", false);
								}
								jumpScareActivate = true;
								jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(2);
								pauseMenu.GetComponent<PauseMenu>().locked = true;
								ifLight.GetComponent<Light>().intensity = 2;
								
								flashlight.GetComponent<Light>().intensity = 0;
								cam.GetComponent<Light>().intensity = 0;
								flashlight.GetComponent<Light>().shadowStrength = 0;
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
								
								Camera.main.GetComponent<CameraController>().enabled = false;
								//Camera.main.transform.LookAt(transform);
								Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,1,0)) - Camera.main.transform.position);
								//lookRotation.y = 12;
								
								Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 20 * Time.deltaTime);
								
								/*
								Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,20,0)) - Camera.main.transform.position);
								//lookRotation.y = 12;
								
								Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 3 * Time.deltaTime);
								*/
							}
							else
							{
								if (polo.GetComponent<PoloAI>().spot < 7 || polo.GetComponent<PoloAI>().spot == 7 && spot != 0 || polo.GetComponent<PoloAI>().spot == 8 && spot != 1 || polo.GetComponent<PoloAI>().spot == 9 && spot != 2)
								{
									if (chasing)
									{
										transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().clip = splash;
										transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().Play();
									}
									chaseSpot = pos.GetChild(spot).position;
									Vector3 lookAtRotation = Quaternion.LookRotation(chaseSpot - transform.position).eulerAngles;
									transform.rotation = Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0,1,0)));
									animator.SetBool("Jumping", false);
									chasing = false;
								}
								else
								{
									transform.position = polo.GetComponent<PoloAI>().pos.GetChild(polo.GetComponent<PoloAI>().spot).position;
									polo.transform.position = new Vector3(polo.GetComponent<PoloAI>().pos.GetChild(polo.GetComponent<PoloAI>().spot).position.x, 0.74f, -2.259f);
									chaseSpot = pos.GetChild(spot).position;
									transform.eulerAngles = Vector3.zero;
									polo.transform.eulerAngles = new Vector3(-17.031f, 0.671f, 177.71f);
									
									animator.CrossFade("Sitting", 0f, -1, 0f);
									polo.GetComponent<Animator>().CrossFade("Sitting", 0f, -1, 0f);
									
									dizzyDucks.SetActive(true);
									
									polo.GetComponent<AudioSource>().clip = impactSound;
									polo.GetComponent<AudioSource>().Play();
									Instantiate(explosion, new Vector3(-0.537f + polo.GetComponent<PoloAI>().pos.GetChild(polo.GetComponent<PoloAI>().spot).position.x, 0, -1.261f), Quaternion.identity);
									
									chasing = false;
									jumpScare = false;
									jumpScareActivate = false;
									polo.GetComponent<PoloAI>().knockedOut = true;
									polo.GetChild(0).gameObject.SetActive(false);
									polo.GetChild(1).gameObject.SetActive(false);
									polo.GetChild(2).gameObject.SetActive(false);
									polo.GetChild(3).gameObject.SetActive(false);
									polo.GetChild(4).gameObject.SetActive(false);
									polo.GetChild(5).gameObject.SetActive(true);
									polo.GetChild(6).gameObject.SetActive(false);
									polo.GetChild(11).gameObject.SetActive(false);
									polo.GetChild(12).gameObject.SetActive(false);
									knockedOut = true;
								}
							}
						}
						else
						{
							animator.SetBool("Dive", true);
							diveTimer += Time.deltaTime;
							if (diveTimer > 1)
							{
								transform.position = pos.GetChild(spot).position;
								//upTimer = 10.1f;
								animator.CrossFade("Crouching", 0f, -1, 0f);
								transform.position = pos.GetChild(spot).position - new Vector3(0,0.6f,0);
								transform.eulerAngles = new Vector3(-26.247f, 180, 0);
								animator.SetBool("Dive", false);
								diveTimer = 0;
								jumpScare = false;
								jumpScareActivate = false;
								StopCoroutine(co);
								MoveSpot();
							}
							
						}
					}
					
					if (playerPos.GetChild(spot + 2).GetChild(0).GetComponent<OpenStall>().open && !(playerPos.GetChild(spot + 2).GetChild(0).GetComponent<OpenStall>().on
					&& playerPos.GetChild(spot + 2).GetChild(3).gameObject.activeSelf && playerPos.GetChild(spot + 2).GetChild(4).gameObject.activeSelf && !playerPos.GetChild(spot + 2).GetChild(3).GetComponent<CrawlUnderText>().crawlingUnder && !playerPos.GetChild(spot + 2).GetChild(4).GetComponent<CrawlUnderText>().crawlingUnder))
					{
						if (chasing)
						{
							chaseSpot = playerPos.GetChild(spot + 2).GetChild(2).transform.position;
						}
					}
					
					//transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), 0.5f * Time.deltaTime);
					
					if (Vector3.Distance(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), transform.position) <= 0.4f)
					{
						//jumpScareActivate = true;
						polo.GetComponent<PoloAI>().jumpScareActivate = true;
					}
					
				}
				
				
			}
			
			if (resetPos)
			{
				animator.SetBool("Dive", true);
				diveTimer += Time.deltaTime;
				if (diveTimer > 1)
				{
					transform.position = pos.GetChild(spot).position;
					//upTimer = 10.1f;
					animator.CrossFade("Crouching", 0f, -1, 0f);
					transform.position = pos.GetChild(spot).position - new Vector3(0,0.6f,0);
					transform.eulerAngles = new Vector3(-26.247f, 180, 0);
					animator.SetBool("Dive", false);
					diveTimer = 0;
					jumpScare = false;
					jumpScareActivate = false;
					StopCoroutine(co);
					MoveSpot();
					resetPos = false;
				}
			}
			
			
			//Mic is enabled/disabled
			if (mic.GetComponent<Mic>().on)
			{
				audioSource.volume = Mathf.Lerp(audioSource.volume, 1f, 10 * Time.deltaTime);
				transform.GetChild(15).GetComponent<AudioSource>().volume = Mathf.Lerp(transform.GetChild(15).GetComponent<AudioSource>().volume, 0.1f, 10 * Time.deltaTime);
				polo.GetChild(13).GetComponent<AudioSource>().volume = Mathf.Lerp(polo.GetChild(13).GetComponent<AudioSource>().volume, 0.1f, 10 * Time.deltaTime);
				transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().volume = Mathf.Lerp(transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().volume, 1f, 10 * Time.deltaTime);
			}
			else
			{
				audioSource.volume = Mathf.Lerp(audioSource.volume, 0, 10 * Time.deltaTime);
				transform.GetChild(15).GetComponent<AudioSource>().volume = Mathf.Lerp(transform.GetChild(15).GetComponent<AudioSource>().volume, 0f, 10 * Time.deltaTime);
				polo.GetChild(13).GetComponent<AudioSource>().volume = Mathf.Lerp(polo.GetChild(13).GetComponent<AudioSource>().volume, 0f, 10 * Time.deltaTime);
				transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().volume = Mathf.Lerp(transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().volume, 0f, 10 * Time.deltaTime);
			}
			
			
			
		}
		else
		{
			if (audioSource.isPlaying)
				audioPlaying = false;
			audioSource.Pause();
			//
			if (transform.GetChild(15).GetComponent<AudioSource>().isPlaying)
				audioPlaying2 = false;
			transform.GetChild(15).GetComponent<AudioSource>().Pause();
			//
			if (polo.GetChild(13).GetComponent<AudioSource>().isPlaying)
				audioPlaying3 = false;
			polo.GetChild(13).GetComponent<AudioSource>().Pause();
			//
			if (transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().isPlaying)
				audioPlaying4 = false;
			transform.GetChild(15).transform.GetChild(0).GetComponent<AudioSource>().Pause();
			
		}
		
	}
}
