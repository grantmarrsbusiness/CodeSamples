using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TwisteeAI : MonoBehaviour
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
	
	private Transform[] pos;
	public int spot;
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
	
	IEnumerator co;
	public string ballZone = "";
	public string targetBallZone = "";
	public bool hitWithBall = false;
	public bool chasingBall = false;
	
	private GameObject smudge;
	private bool looking = false;
	
	private Animator animator; //the "Animator" component of the script holder
	
	AudioSource audioSource;
	public AudioClip hallways;
	public AudioClip vent;
	public AudioClip ventCrawl;
	public AudioClip airduct;
	public AudioClip airductDrop;
	public AudioClip lDoor;
	public AudioClip running;
	public AudioClip holes;
	public AudioClip closet;
	public AudioClip suspense;
	
	private bool audioPlaying;
	private float lastVolume = 1;
    
	private float timer = 0.0f;		//counting timer
	public bool jumpScare = false;
	public bool jumpScareActivate = false;
	
    // Start is called before the first frame update
    void Start() 
	{
		audioSource = GetComponent<AudioSource>();
		pos = GameObject.Find("TwisteePositions").GetComponentsInChildren<Transform>();
		animator = GetComponent<Animator>();
		spot = 17;
		ballZone = "LHallway";
		co = waiter(); // create an IEnumerator object
		MoveSpot();
		smudge = GameObject.Find("Smudge");
		
		if (difficultySelector != null)
		{
			if (difficultySelector.GetComponent<DifficultyStatic>().GetDifficulty() == 0)
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
        MoveSpot();
    }
	
	public void StartMoving()
	{
		if (!jumpScare && spot != 7 && spot != 8 && spot != 11 && spot != 12 && spot != 13 && spot != 9 && spot != 10 && spot != 14 && (targetBallZone != ballZone || !chasingBall))
		{
			//canMove = true;
			//foreach (Renderer rend in GetComponentsInChildren<Renderer>())
				//rend.enabled = false;
			
			if (!passive || spot != 13)
			{
				StopCoroutine(co);
				//animator.CrossFade("Idle1", 0f, -1, 0f);
				//MoveSpot();
				waitTime = 0;
				StartCoroutine(co);
			}
		}
	}
	
	void MoveSpot()
	{
		if (ballZone != "")
		{
			//100% chance to chase the ball
			targetBallZone = ballZone;
			chasingBall = true;
		}
		
		if (spot == 1) //LHallwayDown
		{
			if (targetBallZone == "LHallway")
				{targetBallZone = ""; chasingBall = false;}
			
			if (ballZone != "")
			{
				int perc = Random.Range(1,101);
				if (perc <= 25)
				{
					targetBallZone = ballZone;
					chasingBall = true;
				}
				else if (perc <= 50)
				{
					if (ballZone == "LDoor" || ballZone == "Vent" || ballZone == "Closet" || ballZone == "RDoor")
					{
						targetBallZone = ballZone;
						chasingBall = true;
					}
				}
			}
			if (chasingBall)
			{
				ballZone = targetBallZone;
			}
			
			
			int rand = Random.Range(1,6);
			//checking ball zone
			if (ballZone == "LBackWall" || chasingBall && targetBallZone == "Closet")
				spot = 17; //LBackWall
			else if (ballZone == "RHallway" || chasingBall && (targetBallZone == "RDoor" || targetBallZone == "RFrontWall"))
				spot = 6; //RHallwayUp
			else if (ballZone == "LDoor" || chasingBall && (targetBallZone == "Vent"))
				spot = 16; //LHallwayUp
			else
				spot = 17; //LBackWall
			
			/*
			//if no ball zone, choose randomly
			else if (rand == 1 || rand == 2)
				spot = 17; //LBackWall
			else if (rand == 3 || rand == 4)
				spot = 6; //RHallwayUp
			else if (rand == 5)
				spot = 16; //LHallwayUp
			*/
		}
		else if (spot == 2) //LDoor
		{
			if (targetBallZone == "LDoor")
				{targetBallZone = ""; chasingBall = false;}
			
			if (ballZone != "")
			{
				int perc = Random.Range(1,101);
				if (perc <= 25)
				{
					targetBallZone = ballZone;
					chasingBall = true;
				}
				else if (perc <= 50)
				{
					if (ballZone == "LBackWall" || ballZone == "RHallway" || ballZone == "Closet" || ballZone == "RDoor")
					{
						targetBallZone = ballZone;
						chasingBall = true;
					}
				}
			}
			if (chasingBall)
			{
				ballZone = targetBallZone;
			}
			
			
			int rand = Random.Range(1,4);
			//checking ball zone
			if (ballZone == "LHallway" || chasingBall && (targetBallZone == "Closet" || targetBallZone == "LBackWall"))
				spot = 1; //LHallwayDown
			else if (ballZone == "RFrontWall" || chasingBall && (targetBallZone == "RHallway" || targetBallZone == "RDoor"))
				spot = 5; //RFrontWall
			else if (ballZone == "Vent")
				spot = 8; //Vent
			else
				spot = 5; //RFrontWall
			
			/*
			//if no ball zone, choose randomly
			else if (rand == 1)
				spot = 1; //LHallwayDown
			else if (rand == 2)
				spot = 5; //RFrontWall
			else if (rand == 3)
				spot = 8; //Vent
			*/
		}
		else if (spot == 3) //RDoor
		{
			if (targetBallZone == "RDoor")
				{targetBallZone = ""; chasingBall = false;}
			
			if (ballZone != "")
			{
				int perc = Random.Range(1,101);
				if (perc <= 25)
				{
					targetBallZone = ballZone;
					chasingBall = true;
				}
				else if (perc <= 50)
				{
					if (ballZone == "LDoor" || ballZone == "LHallway" || ballZone == "Closet" || ballZone == "LBackWall" || ballZone == "Vent")
					{
						targetBallZone = ballZone;
						chasingBall = true;
					}
				}
			}
			if (chasingBall)
			{
				ballZone = targetBallZone;
			}
			
			
			if ((ballZone == "RHallway" || chasingBall && (targetBallZone == "LHallway" || targetBallZone == "LBackWall" || targetBallZone == "Closet")) && !hitWithBall)
				spot = 15; //RHallwayDown
			else if ((ballZone == "RFrontWall" || chasingBall && (targetBallZone == "Vent" || targetBallZone == "LDoor")) && !hitWithBall)
				spot = 5; //RFrontWall
			else
				spot = 9; //RDoorRun
		}
		else if (spot == 4) //Closet
		{
			if (targetBallZone == "Closet")
				{targetBallZone = ""; chasingBall = false;}
			
			if (ballZone != "")
			{
				int perc = Random.Range(1,101);
				if (perc <= 25)
				{
					targetBallZone = ballZone;
					chasingBall = true;
				}
				else if (perc <= 50)
				{
					if (ballZone == "LDoor" || ballZone == "LHallway" || ballZone == "RFrontWall" || ballZone == "RHallway" || ballZone == "RDoor" || ballZone == "Vent")
					{
						targetBallZone = ballZone;
						chasingBall = true;
					}
				}
			}
			if (chasingBall)
			{
				ballZone = targetBallZone;
			}
			
			
			if ((ballZone == "LBackWall" || chasingBall) && !hitWithBall)
				spot = 17; //LBackWall
			else
				spot = 10; //ClosetRun
		}
		else if (spot == 5) //RFrontWall
		{
			if (targetBallZone == "RFrontWall")
				{targetBallZone = ""; chasingBall = false;}
			
			if (ballZone != "")
			{
				int perc = Random.Range(1,101);
				if (perc <= 25)
				{
					targetBallZone = ballZone;
					chasingBall = true;
				}
				else if (perc <= 50)
				{
					if (ballZone == "LBackWall" || ballZone == "LHallway" || ballZone == "RHallway" || ballZone == "RDoor" || ballZone == "Closet")
					{
						targetBallZone = ballZone;
						chasingBall = true;
					}
				}
			}
			if (chasingBall)
			{
				ballZone = targetBallZone;
			}
			
			
			int rand = Random.Range(1,3);
			//checking ball zone
			if (ballZone == "RHallway" || chasingBall && (targetBallZone == "LBackWall" || targetBallZone == "Closet"))
				spot = 15; //RHallwayDown
			else if (ballZone == "RDoor" || chasingBall && (targetBallZone == "LBackWall" || targetBallZone == "Closet"))
				spot = 3; //RDoor
			else if (ballZone == "LDoor" || chasingBall && (targetBallZone == "LHallway"))
				spot = 2; //LDoor
			else if (ballZone == "Vent")
				spot = 8; //Vent
			else
				spot = 3; //RDoor
			
			/*
			//if no ball zone, choose randomly
			else if (rand == 1)
				spot = 3; //RDoor
			else if (rand == 2)
				spot = 8; //Vent
			*/
			/*
			//if no ball zone, choose randomly
			else if (rand == 1)
				spot = 15; //RHallwayDown
			else if (rand == 2)
				spot = 3; //RDoor
			else if (rand == 3)
				spot = 2; //LDoor
			else if (rand == 4)
				spot = 8; //Vent
			*/
		}
		else if (spot == 6) //RHallwayUp
		{
			if (targetBallZone == "RHallway")
				{targetBallZone = ""; chasingBall = false;}
			
			if (ballZone != "")
			{
				int perc = Random.Range(1,101);
				if (perc <= 25)
				{
					targetBallZone = ballZone;
					chasingBall = true;
				}
				else if (perc <= 50)
				{
					if (ballZone == "Vent" || ballZone == "LDoor" || ballZone == "RHallway" || ballZone == "LHallway" || ballZone == "LBackWall")
					{
						targetBallZone = ballZone;
						chasingBall = true;
					}
				}
			}
			if (chasingBall)
			{
				ballZone = targetBallZone;
			}
			
			
			int rand = Random.Range(1,6);
			//checking ball zone
			if (ballZone == "RFrontWall" || chasingBall && (targetBallZone == "Vent" || targetBallZone == "LDoor"))
				spot = 5; //RFrontWall
			else if (ballZone == "RDoor")
				spot = 3; //RDoor
			else if (ballZone == "LBackWall" || chasingBall && (targetBallZone == "LHallway"))
				spot = 15; //RHallwayDown
			else
				spot = 3; //RDoor
			/*
			//if no ball zone, choose randomly
			else if (rand == 1 || rand == 2)
				spot = 5; //RFrontWall
			else if (rand == 3 || rand == 4)
				spot = 3; //RDoor
			else if (rand == 5)
				spot = 15; //RHallwayDown
			*/
		}
		else if (spot == 7) //Airduct
		{
			{targetBallZone = ""; chasingBall = false;}
			spot = 12; //AirductDrop
		}
		else if (spot == 8) //Vent
		{
			{targetBallZone = ""; chasingBall = false;}
			spot = 11; //VentCrawl
			
			/*
			if (ballZone == "LHallway")
				spot = 1; //LHallwayDown
			else if (ballZone == "LDoor")
				spot = 2; //LDoor
			else if (ballZone == "RFrontWall")
				spot = 5; //RFrontWall
			else
				spot = 11; //VentCrawl
			*/
			/*
			int rand = Random.Range(1,7);
			if (rand == 1 || rand == 2 || rand == 3)
				spot = 11; //VentCrawl
			else if (rand == 4)
				spot = 1; //LHallwayDown
			else if (rand == 5)
				spot = 2; //LDoor
			else if (rand == 6)
				spot = 5; //RFrontWall
			*/
		}
		else if (spot == 9) //RDoorRun
		{
			audioSource.Stop();
			jumpScareActivate = true;
			spot = 16; //LHallwayUp
		}
		else if (spot == 10) //ClosetRun
		{
			audioSource.Stop();
			jumpScareActivate = true;
			spot = 16; //LHallwayUp
		}
		else if (spot == 11) //VentCrawl
		{
			spot = 7; //Airduct
			{targetBallZone = ""; chasingBall = false;}
		}
		else if (spot == 12) //AirductDrop
		{
			spot = 13; //Main
			
			/*
			if (ballZone == "RDoor")
				spot = 3; //RDoor
			else if (ballZone == "RHallway")
				spot = 15; //RHallwayDown
			else
				spot = 13; //Main
			*/
			/*
			int rand = Random.Range(1,6);
			if (rand == 1 || rand == 2 || rand == 3)
				spot = 13; //Main
			else if (rand == 4 || ballZone == "RDoor")
				spot = 3; //RDoor
			else if (rand == 5 || ballZone == "RHallway")
				spot = 15; //RHallwayDown
			*/
		}
		else if (spot == 13) //Main
		{
			//targetBallZone = ""; chasingBall = false;
			
			if (ballZone != "")
			{
				int perc = Random.Range(1,101);
				if (perc <= 25)
				{
					targetBallZone = ballZone;
					chasingBall = true;
				}
				else if (perc <= 50)
				{
					if (ballZone == "Vent" || ballZone == "LDoor" || ballZone == "RFrontWall" || ballZone == "LHallway" || ballZone == "LBackWall" || ballZone == "Closet")
					{
						targetBallZone = ballZone;
						chasingBall = true;
					}
				}
			}
			if (chasingBall)
			{
				ballZone = targetBallZone;
			}
			
			
			if ((ballZone == "RDoor" || chasingBall && (targetBallZone == "RFrontWall" || targetBallZone == "Vent")) && !hitWithBall)
				spot = 3; //RDoor
			else if ((ballZone == "RHallway" || chasingBall && (targetBallZone == "LHallway" || targetBallZone == "LBackWall" || targetBallZone == "Closet")) && !hitWithBall)
				spot = 15; //RHallwayDown
			else
				spot = 14; //MainWalk
		}
		else if (spot == 14) //MainWalk
		{
			audioSource.Stop();
			jumpScareActivate = true;
			spot = 16; //LHallwayUp
		}
		else if (spot == 15) //RHallwayDown
		{
			if (targetBallZone == "RHallway")
				{targetBallZone = ""; chasingBall = false;}
			
			if (ballZone != "")
			{
				int perc = Random.Range(1,101);
				if (perc <= 25)
				{
					targetBallZone = ballZone;
					chasingBall = true;
				}
				else if (perc <= 50)
				{
					if (ballZone == "LDoor" || ballZone == "RDoor" || ballZone == "RFrontWall" || ballZone == "Vent" || ballZone == "Closet")
					{
						targetBallZone = ballZone;
						chasingBall = true;
					}
				}
			}
			if (chasingBall)
			{
				ballZone = targetBallZone;
			}
			
			
			int rand = Random.Range(1,6);
			//checking ball zone
			if (ballZone == "LBackWall" || chasingBall && (targetBallZone == "Closet"))
				spot = 17; //LBackWall
			else if (ballZone == "LHallway" || chasingBall && (targetBallZone == "LDoor" || targetBallZone == "Vent"))
				spot = 16; //LHallwayUp
			else if (ballZone == "RDoor" || chasingBall && (targetBallZone == "RFrontWall"))
				spot = 6; //RHallwayUp
			else
				spot = 17; //LBackWall
			
			/*
			//if no ball zone, choose randomly
			else if (rand == 1 || rand == 2)
				spot = 17; //LBackWall
			else if (rand == 3 || rand == 4)
				spot = 16; //LHallwayUp
			else if (rand == 7)
				spot = 6; //RHallwayUp
			*/
		}
		else if (spot == 16) //LHallwayUp
		{
			if (targetBallZone == "LHallway")
				{targetBallZone = ""; chasingBall = false;}
			
			if (ballZone != "")
			{
				int perc = Random.Range(1,101);
				if (perc <= 25)
				{
					targetBallZone = ballZone;
					chasingBall = true;
				}
				else if (perc <= 50)
				{
					if (ballZone == "RHallway" || ballZone == "RDoor" || ballZone == "RFrontWall" || ballZone == "LBackWall" || ballZone == "Closet")
					{
						targetBallZone = ballZone;
						chasingBall = true;
					}
				}
			}
			if (chasingBall)
			{
				ballZone = targetBallZone;
			}
			
			
			int rand = Random.Range(1,3);
			//checking ball zone
			if (ballZone == "Vent")
				spot = 8; //Vent
			else if (ballZone == "RFrontWall" || chasingBall && (targetBallZone == "RDoor" || targetBallZone == "RHallway"))
				spot = 5; //RFrontWall
			else if (ballZone == "LDoor")
				spot = 2; //LDoor
			else if (ballZone == "LBackWall" || chasingBall && (targetBallZone == "Closet"))
				spot = 1; //LHallwayDown
			//if no ball zone, choose randomly
			else if (rand == 1)
				spot = 5; //RFrontWall
			else if (rand == 2)
				spot = 2; //LDoor
			
			
			
			/*
			//if no ball zone, choose randomly
			else if (rand == 1)
				spot = 8; //Vent
			else if (rand == 2)
				spot = 5; //RFrontWall
			else if (rand == 3)
				spot = 2; //LDoor
			else if (rand == 4)
				spot = 1; //LHallwayDown
			*/
		}
		else if (spot == 17) //LBackWall
		{
			if (targetBallZone == "LBackWall")
				{targetBallZone = ""; chasingBall = false;}
			
			if (ballZone != "")
			{
				int perc = Random.Range(1,101);
				if (perc <= 25)
				{
					targetBallZone = ballZone;
					chasingBall = true;
				}
				else if (perc <= 50)
				{
					if (ballZone == "Vent" || ballZone == "LDoor" || ballZone == "RFrontWall" || ballZone == "RDoor")
					{
						targetBallZone = ballZone;
						chasingBall = true;
					}
				}
			}
			if (chasingBall)
			{
				ballZone = targetBallZone;
			}
			
			
			int rand = Random.Range(1,6);
			//checking ball zone
			if (ballZone == "Closet")
				spot = 4; //Closet
			else if (ballZone == "LHallway" || chasingBall && (targetBallZone == "LDoor" || targetBallZone == "Vent"))
				spot = 16; //LHallwayUp
			else if (ballZone == "RHallway" || chasingBall && (targetBallZone == "RDoor" || targetBallZone == "RFrontWall"))
				spot = 6; //RHallwayUp
			else
				spot = 4; //Closet
			
			/*
			//if no ball zone, choose randomly
			else if (rand == 1 || rand == 2 || rand == 3)
				spot = 4; //Closet
			else if (rand == 4)
				spot = 16; //LHallwayUp
			else if (rand == 5)
				spot = 6; //RHallwayUp
			*/
		}
		
		//reset ball zone if not in airduct
		if (spot != 7)
			ballZone = "";
		
		
		//spot = RandomRangeExcept(1, 8, spot);
		
		//transform.position = pos[spot].position;
		//transform.rotation = pos[spot].rotation;
		
		foreach (Renderer rend in GetComponentsInChildren<Renderer>())
			rend.enabled = false;
		
		if (spot == 1) //LHallwayDown
		{
			//audioSource.PlayOneShot(hallways, 1.0f);
			audioSource.Stop();
			audioSource.clip = hallways;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = 7;
			animator.CrossFade("Walk1", 0f, -1, 0f);
		}
		else if (spot == 2) //LDoor
		{
			//audioSource.PlayOneShot(lDoor, 0.3f);
			audioSource.Stop();
			audioSource.clip = lDoor;
			audioSource.volume = 0.3f;
			audioSource.Play();
			waitTime = Random.Range (minWait, maxWait)/2;
			//waitTime = 2;
			animator.CrossFade("InjuredIdle", 0f, -1, 0f);
		}
		else if (spot == 3) //RDoor
		{
			//audioSource.PlayOneShot(suspense, 1.0f);
			if (!passive)
				waitTime = Random.Range (minWait, maxWait);
			else
				waitTime = 30;
			animator.CrossFade("Idle1", 0f, -1, 0f);
		}
		else if (spot == 4) //Closet
		{
			//audioSource.PlayOneShot(closet, 1.0f);
			//audioSource.PlayOneShot(suspense, 1.0f);
			if (!passive)
				waitTime = Random.Range (minWait, maxWait);
			else
				waitTime = 30;
			animator.CrossFade("Idle1", 0f, -1, 0f);
		}
		else if (spot == 5) //RFrontWall
		{
			//audioSource.PlayOneShot(holes, 0.5f);
			audioSource.Stop();
			audioSource.clip = holes;
			audioSource.volume = 0.5f;
			audioSource.Play();
			waitTime = Random.Range (minWait, maxWait)/2;
			//waitTime = 2;
			animator.CrossFade("InjuredIdle 0", 0f, -1, 0f);
		}
		else if (spot == 6) //RHallwayUp
		{
			//audioSource.PlayOneShot(hallways, 1.0f);
			audioSource.Stop();
			audioSource.clip = hallways;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = 5.3f;
			animator.CrossFade("Walk1", 0f, -1, 0f);
		}
		else if (spot == 7) //Airduct
		{
			audioSource.Stop();
			audioSource.clip = airduct;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = Random.Range (minWait, maxWait);
			animator.CrossFade("CrouchingIdle", 0f, -1, 0f);
		}
		else if (spot == 8) //Vent
		{
			//audioSource.PlayOneShot(vent, 1.0f);
			audioSource.Stop();
			audioSource.clip = vent;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = Random.Range (minWait, maxWait);
			animator.CrossFade("CrouchingIdle", 0f, -1, 0f);
		}
		else if (spot == 9) //RDoorRun
		{
			audioSource.PlayOneShot(suspense, 1.0f);
			audioSource.PlayOneShot(running, 1.0f);
			waitTime = 0.6f;
			animator.CrossFade("PickUpItem", 0f, -1, 0f);
		}
		else if (spot == 10) //ClosetRun
		{
			audioSource.PlayOneShot(suspense, 1.0f);
			audioSource.PlayOneShot(running, 1.0f);
			waitTime = 0.45f;
			animator.CrossFade("PickUpItem", 0f, -1, 0f);
		}
		else if (spot == 11) //VentCrawl
		{
			//audioSource.PlayOneShot(ventCrawl, 1.0f);
			audioSource.Stop();
			audioSource.clip = ventCrawl;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = 5;
			animator.CrossFade("LowCrawl", 0f, -1, 0f);
		}
		else if (spot == 12) //AirductDrop
		{
			//audioSource.PlayOneShot(airductDrop, 1.0f);
			audioSource.Stop();
			audioSource.clip = airductDrop;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = 2;
			animator.CrossFade("JumpingDown", 0f, -1, 0f);
		}
		else if (spot == 13) //Main
		{
			//audioSource.PlayOneShot(closet, 1.0f);
			audioSource.Stop();
			audioSource.clip = closet;
			audioSource.volume = 1.0f;
			audioSource.Play();
			if (!passive)
				waitTime = Random.Range (minWait, maxWait);
			else
				waitTime = 30;
			animator.CrossFade("Idle1", 0f, -1, 0f);
		}
		else if (spot == 14) //MainWalk
		{
			//audioSource.PlayOneShot(suspense, 1.0f);
			audioSource.Stop();
			audioSource.clip = suspense;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = 0.8f;
			animator.CrossFade("Walk1", 0f, -1, 0f);
		}
		else if (spot == 15) //RHallwayDown
		{
			//audioSource.PlayOneShot(hallways, 1.0f);
			audioSource.Stop();
			audioSource.clip = hallways;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = 5.3f;
			animator.CrossFade("Walk1", 0f, -1, 0f);
		}
		else if (spot == 16) //LHallwayUp
		{
			//audioSource.PlayOneShot(hallways, 1.0f);
			audioSource.Stop();
			audioSource.clip = hallways;
			audioSource.volume = 1.0f;
			audioSource.Play();
			waitTime = 7;
			animator.CrossFade("Walk1", 0f, -1, 0f);
		}
		else if (spot == 17) //LBackWall
		{
			//audioSource.PlayOneShot(holes, 0.5f);
			audioSource.Stop();
			audioSource.clip = holes;
			audioSource.volume = 0.5f;
			audioSource.Play();
			waitTime = Random.Range (minWait, maxWait);
			animator.CrossFade("InjuredIdle", 0f, -1, 0f);
		}
		
		transform.position = pos[spot].position;
		transform.rotation = pos[spot].rotation;
		
		foreach (Renderer rend in GetComponentsInChildren<Renderer>())
			rend.enabled = true;
		
		if (audioSource.volume != 0 && mic.GetComponent<Mic>().on)
			lastVolume = audioSource.volume;
		
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
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Level 1 Jumpscares");
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
			
			if (!flashlight.GetComponent<Flashlight>().on && !cam.GetComponent<Cam>().on && cam.GetComponent<Light>().intensity <= 0.01f || !onScreen || spot == 2 && ballZone == "Vent"){
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
			
			/*
			Vector3 screenPoint2 = Camera.main.WorldToViewportPoint(transform.position);
			bool onScreen2 = screenPoint2.z > 0 && screenPoint.x > 1 - 0.85f && screenPoint2.x < 0.85f && screenPoint2.y > 1 - 0.85f && screenPoint2.y < 0.85f;
			
			if (spot == 13 && (cam.GetComponent<Cam>().on) && onScreen2)
			{
				canMove = true;
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = false;
				
				ballZone = "RDoor";
				StopCoroutine(co);
				MoveSpot();
			}
			*/
			
			if (hitWithBall && spot != 9 && spot != 10 && spot != 14)
			{
				canMove = true;
				/*
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = false;
				*/
				
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
			//Running at you
			if (!jumpScare && (spot == 9 || spot == 10 || spot == 14) && !smudge.GetComponent<SmudgeAI>().jumpScare)
			{
				jumpScare = true;
			}
			if (jumpScare)
			{
				jumpScareSelector.GetComponent<JumpscareStatic>().SetScareNum(1);
				
				pauseMenu.GetComponent<PauseMenu>().locked = true;
				
				canMove = true;
				/*
				foreach (Renderer rend in GetComponentsInChildren<Renderer>())
					rend.enabled = true;
				foreach (CapsuleCollider col in GetComponentsInChildren<CapsuleCollider>())
					col.enabled = true;
				*/
				
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
					Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,1,0)) - Camera.main.transform.position);
					//lookRotation.y = 12;
					
					Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 20 * Time.deltaTime);
				}
				else if (timer > waitTime - 0.1f){
					flashlight.GetComponent<Light>().intensity = 0;
					//flashlight.GetComponent<Light>().enabled = false;
					flashlight.SetActive(false);
					cam.GetComponent<Light>().intensity = 0;
					Quaternion lookRotation = Quaternion.LookRotation((transform.position + new Vector3(0,20,0)) - Camera.main.transform.position);
					//lookRotation.y = 12;
					
					Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, 3 * Time.deltaTime);
				}
				timer += Time.deltaTime;
				
			}
			
			if (smudge.GetComponent<SmudgeAI>().spot == 4 && spot == 13)
			{
				if (!looking)
				{
					animator.CrossFade("Looking", 0f, -1, 0f);
				}
				looking = true;
			}
			else
			{
				if (looking && spot == 13)
				{
					animator.CrossFade("Idle1", 0f, -1, 0f);
				}
				looking = false;
			}
			
			//Mic is enabled/disabled
			if (mic.GetComponent<Mic>().on)
				audioSource.volume = Mathf.Lerp(audioSource.volume, lastVolume, 10 * Time.deltaTime);
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
