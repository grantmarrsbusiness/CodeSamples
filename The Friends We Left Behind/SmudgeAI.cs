using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SmudgeAI : MonoBehaviour
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
	
	public GameObject peekaboo;
	public GameObject iseeyou;
	public GameObject imclose;
	public GameObject lightscary;
	public GameObject alonesolong;
	public GameObject beourfriend;
	public GameObject play;
	
	private Transform[] pos;
	public Transform[] writing;
	public Transform currentWrit;
	public bool written;
	public bool writingSeen = true;
	public int spot;
	
	IEnumerator co;
	public float waitTime;
	public GameObject flashlight;
	public GameObject cam;
	public GameObject mic;
	public GameObject flashlightObject;
	public GameObject camObject;
	public GameObject micObject;
	public GameObject ballObject;
	public GameObject ballBasket;
	public GameObject ifLight;
	public bool canMove = true;
	public bool inView = false;
	
	private Animator animator; //the "Animator" component of the script holder
	
	private GameObject twistee;
	private bool looking = false;
	
	public bool movedByCam = false;
	public bool hitWithBall = false;
	
	private bool audioPlaying;
	AudioSource audioSource;
	public AudioClip scribble;
    
	private float timer = 0.0f;		//counting timer
	public bool jumpScare = false;
	public bool jumpScareActivate = false;
	
    // Start is called before the first frame update
    void Start()
    {
        
		audioSource = GetComponent<AudioSource>();
		pos = GameObject.Find("SmudgePositions").GetComponentsInChildren<Transform>();
		writing = GameObject.Find("SmudgeWriting").GetComponentsInChildren<Transform>();
		animator = GetComponent<Animator>();
		twistee = GameObject.Find("Twistee");
		spot = 0;
		co = waiter(); // create an IEnumerator object
		MoveSpot();
		
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
		
		foreach (Transform writ in writing)
		{
			if (writ.GetComponent<Renderer>())
				writ.GetComponent<Renderer>().enabled = false;
		}
		
    }
	
	IEnumerator waiter()
    {
        
        yield return new WaitForSeconds (waitTime);
		if (jumpScare)
			twistee.GetComponent<TwisteeAI>().jumpScareActivate = true;
		if (written && !twistee.GetComponent<TwisteeAI>().jumpScare)
		{
			jumpScare = true;
		}
		while(!canMove) yield return null;
        MoveSpot();
    }
	
	void MoveSpot()
	{
		
		if (!jumpScare)
		{
			
			
			if (Random.Range (1, 6) == 1 && !written && waitTime != 0 || Random.Range (1, 9) == 1 && movedByCam && !written && spot != 0)
			{
				audioSource.clip = scribble;
				audioSource.Play();
				writingSeen = false;
				written = true;
			}
			else
			{
				if (written)
				{
					foreach (Transform writ in writing)
					{
						if (writ.GetComponent<Renderer>())
							writ.GetComponent<Renderer>().enabled = false;
					}
				}
				written = false;
			}
			
			if (!passive && !written)
				waitTime = Random.Range (minWait, maxWait);
			else
				waitTime = 30;
			if (!written)
			{
				//waitTime = Random.Range (minWait, maxWait);
				if ((easy && Random.Range (1, 6) == 1 || normal && Random.Range (1, 7) == 1 || hard && Random.Range (1, 8) == 1) && spot != 4 && movedByCam || passive)
				{
					spot = 4;
					movedByCam = false;
				}
				else
					spot = RandomRangeExcept(1, 7, spot);
			}
			else
			{
				//waitTime = Random.Range (4, 10);
				spot += 7;
			}
			
			foreach (Renderer rend in GetComponentsInChildren<Renderer>())
				rend.enabled = false;
			foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
				col.enabled = false;
			
			//transform.position = pos[spot].position;
			//transform.rotation = pos[spot].rotation;
			
			if (!written)
			{
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = false;
			}
			
			if (spot <= 7)
			{
				currentWrit = writing[spot];
			}
			else
			{
				currentWrit = writing[spot - 7];
			}
			
			if (spot == 1 || spot == 2)
			{
				animator.CrossFade("Sit", 0f, -1, 0f);
			}
			else if (spot == 3 || spot == 5 || spot == 7)
			{
				animator.CrossFade("Crouching Idle", 0f, -1, 0f);
			}
			else
			{
				animator.CrossFade("Idle", 0f, -1, 0f);
			}
			
			transform.position = pos[spot].position;
			transform.rotation = pos[spot].rotation;
			
			if (!written)
			{
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = true;
			}
					
		}
		else
		{
			animator.CrossFade("Walking", 0f, -1, 0f);
			waitTime = 0.35f;
		}
		
		co = waiter();
		StartCoroutine (co);
	}
	
	int RandomRangeExcept (int min, int max, int except) {
		int newSpot = 0;
		do {
			newSpot = Random.Range (min, max + 1);
		} while (newSpot == except || newSpot == 4 && !movedByCam);
		movedByCam = false;
		return newSpot;
	}

    // Update is called once per frame
    void Update()
    {
		
		/*
		if (difficultySelector != null)
		{
			if (difficultySelector.GetComponent<DifficultyStatic>().getDifficulty == 0)
			{
				easy = true;
				normal = false;
				hard = false;
				insane = false;
				lethal = false;
			}
			if (difficultySelector.GetComponent<DifficultyStatic>().getDifficulty == 1)
			{
				easy = false;
				normal = true;
				hard = false;
				insane = false;
				lethal = false;
			}
			if (difficultySelector.GetComponent<DifficultyStatic>().getDifficulty == 2)
			{
				easy = false;
				normal = false;
				hard = true;
				insane = false;
				lethal = false;
			}
			if (difficultySelector.GetComponent<DifficultyStatic>().getDifficulty == 3)
			{
				easy = false;
				normal = false;
				hard = false;
				insane = true;
				lethal = false;
			}
			if (difficultySelector.GetComponent<DifficultyStatic>().getDifficulty == 4)
			{
				easy = false;
				normal = false;
				hard = false;
				insane = false;
				lethal = true;
			}
		}
		
		if (easy)
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
		*/
		
		if (Time.timeScale == 1)
		{
			
			if (!audioPlaying)
			{
				audioSource.Play();
				audioPlaying = true;
			}
			
			Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
			bool onScreen = screenPoint.z > 0 && screenPoint.x > -1 && screenPoint.x < 2 && screenPoint.y > -1 && screenPoint.y < 2;
			//bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
			
			Vector3 screenPoint2 = Camera.main.WorldToViewportPoint(new Vector3((transform.position.x + twistee.transform.position.x)/2, (transform.position.y + twistee.transform.position.y)/2, (transform.position.z + twistee.transform.position.z)/2));
			bool onScreen2 = screenPoint2.z > 0 && screenPoint.x > 1 - 0.85f && screenPoint2.x < 0.85f && screenPoint2.y > 1 - 0.85f && screenPoint2.y < 0.85f;
			
			if (writingSeen)
			{
				if (!flashlight.GetComponent<Flashlight>().on && !cam.GetComponent<Cam>().on && cam.GetComponent<Light>().intensity <= 0.01f || !onScreen){
					canMove = true;
				}
				else{
					canMove = false;
					
				}
			}
			if (written && !writingSeen)
			{
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = false;
				foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
					col.enabled = false;
			}
			else
			{
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = true;
				foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
					col.enabled = true;
			}
			
			if (written)
			{
				Vector3 screenPointWrit = Camera.main.WorldToViewportPoint(currentWrit.transform.position);
				bool onScreenWrit = screenPointWrit.z > 0 && screenPointWrit.x > -1 && screenPointWrit.x < 2 && screenPointWrit.y > -1 && screenPointWrit.y < 2;
				
				//if (!flashlight.GetComponent<Flashlight>().on || !onScreenWrit){
				if (!onScreenWrit){
					currentWrit.GetComponent<Renderer>().enabled = false;
				}
				else{
					canMove = false;
					writingSeen = true;
					currentWrit.GetComponent<Renderer>().enabled = true;
				}
				
				if ((cam.GetComponent<Cam>().on || cam.GetComponent<Light>().intensity > 0.01f) && onScreen && writingSeen){
					canMove = true;
					foreach (Renderer rend in GetComponentsInChildren<Renderer>())
						rend.enabled = false;
					foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
							col.enabled = false;
					
					spot = 0;
					waitTime = 0;
					written = false;
					movedByCam = true;
					StopCoroutine(co);
					MoveSpot();
				}
				
			}
			else
			{
				if ((cam.GetComponent<Cam>().on) && onScreen){
					if (twistee.GetComponent<TwisteeAI>().spot != 13 || spot != 4 || !onScreen2){
						canMove = true;
						foreach (Renderer rend in GetComponentsInChildren<Renderer>())
							rend.enabled = false;
						foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
							col.enabled = false;
						
						waitTime = 0;
						movedByCam = true;
						StopCoroutine(co);
						MoveSpot();
					}
					else {
						jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(3);
						twistee.GetComponent<TwisteeAI>().jumpScareActivate = true;
						//SceneManager.LoadScene("Level 1 Win");
					}
				}
			}
			
			
			if (hitWithBall && waitTime != 0.35f && !twistee.GetComponent<TwisteeAI>().jumpScare)
			{
				if (!jumpScare)
				{
					canMove = true;
					foreach (Renderer rend in GetComponentsInChildren<Renderer>())
						rend.enabled = false;
					foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
							col.enabled = false;
					
					StopCoroutine(co);
					
					
					if (spot <= 7)
						spot += 7;
					
					animator.CrossFade("Idle", 0f, -1, 0f);
					transform.position = pos[spot].position;
					transform.rotation = pos[spot].rotation;
					
					jumpScare = true;
				}
				MoveSpot();
				
			}
			
			
			//Running at you
			if (jumpScare)
			{
				pauseMenu.GetComponent<PauseMenu>().locked = true;
				jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(2);
				
				canMove = true;
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = true;
				foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
					col.enabled = true;
				
				if (timer > waitTime - 0.4f && timer <= waitTime - 0.1f){
					ifLight.GetComponent<Light>().intensity = 2;
					
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
					
					ballObject.transform.parent = Camera.main.transform;
					ballObject.GetComponent<CameraController>().enabled = false;
					ballObject.transform.localPosition = Vector3.Lerp(ballObject.transform.localPosition, new Vector3(ballObject.transform.localPosition.x, -4, ballObject.transform.localPosition.z), 2 * Time.deltaTime);
					ballBasket.GetComponent<BallBasket>().enabled = false;
					ballObject.transform.GetChild(0).GetComponent<Ball>().enabled = false;
					
					Camera.main.GetComponent<CameraController>().enabled = false;
					//Camera.main.transform.LookAt(transform);
					Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,0.85f,0)) - Camera.main.transform.position);
					//lookRotation.y = 12;
					
					Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 10 * Time.deltaTime);
				}
				else if (timer > waitTime - 0.1f){
					flashlight.GetComponent<Light>().intensity = 0;
					//flashlight.GetComponent<Light>().enabled = false;
					flashlight.SetActive(false);
					cam.GetComponent<Light>().intensity = 0;
					Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,10,0)) - Camera.main.transform.position);
					//lookRotation.y = 12;
					
					Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 3 * Time.deltaTime);
				}
				timer += Time.deltaTime;
				
			}
			
			
			if (twistee.GetComponent<TwisteeAI>().spot == 13 && spot == 4)
			{
				if (!looking)
				{
					animator.CrossFade("Looking", 0f, -1, 0f);
				}
				looking = true;
			}
			else
			{
				if (looking && spot == 4)
				{
					animator.CrossFade("Idle", 0f, -1, 0f);
				}
				looking = false;
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
}
