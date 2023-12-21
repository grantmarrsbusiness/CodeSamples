using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class BeatCPU : MonoBehaviour
{
	
	public float health = 100f;
	public Image healthBarImage;
	public TextMeshProUGUI healthBarPercentage;
	
	public Image scoreBarImage;
	public TextMeshProUGUI comboCount;
	public TextMeshProUGUI pointsCount;
	
	public float moveSpeed = 20f;
	
	public GameObject hazard1 = null;
	public GameObject hazard2 = null;
	
	public float smoothXPos = 3f;
	public float smoothSpeed = 5f;
	public float xPos = 3f;
	
	private GameObject note;
	private GameObject stage;
	public GameObject[] sounds;
	public GameObject[] trails;
	
	public bool hittingTrail = false;
	public bool hittingNote = false;
	
	public string currentTarget = "T3";
	public float targetNum = 3;
	public float oldTargetNum = 3;
	
	public bool dodging = false;
	public bool chasing = false;
	public bool switching = false;
	public bool plantingHazard = false;
	public bool hazardLock = false;
	public GameObject soundTarget = null;
	public GameObject soundTarget1 = null;
	public GameObject soundTarget2 = null;
	public bool safeToPlace = false;
	public float safeToPlace2;
	public bool currentlyPlacing = false;
	public float overlappingSound = 0;
	public bool frickedUp = false;
	
	public bool canPlaceHazards = false;
	public bool canPlaceHazards2 = false;
	
	private bool jumping = false;
	private bool jumpPressed = false;
	private bool jumpInitiated = false;
	private float jumpTimer = 0.0f;		//counting timer
	private float jumpHeight = 0.0f;		//counting timer
	public float jHeight = 4.5f;
	public float jHeight2 = -5f;
	public float jTime = 2f;
	public float jTime2 = 2f;
	
	public GameObject specialAttack1;
	public GameObject specialCharge;
	public bool specialAttackStarted = false;
	public float specialAttackTimer = 10f;
	public float specialAttackNum = 11f;
	public float specialAttackTarget = 3f;
	
	public float t1Tracker;
	public float t2Tracker;
	public float t3Tracker;
	public float t4Tracker;
	public float t5Tracker;
	
	private float timer = 0.0f;		//counting timer
	
	public float combo = 0f;
	public float totalCombo = 0f;
	public float totalPoints = 0f;
	
	public float nextUpCount = 0f;
	
    // Start is called before the first frame update
    void Start()
    {
        if (note == null)
            note = GameObject.FindWithTag("Note");
		
		if (stage == null)
            stage = GameObject.FindWithTag("Stage");
		
		sounds = GameObject.FindGameObjectsWithTag("Sound");
		trails = GameObject.FindGameObjectsWithTag("Trail");
    }

    // Update is called once per frame
    void Update()
    {
		
		//sounds.RemoveAll( x => !x);
		
		t1Tracker = 0;
		t2Tracker = 0;
		t3Tracker = 0;
		t4Tracker = 0;
		t5Tracker = 0;
		
		if (timer > 0.5f){
			hazardLock = false;
		}
		
		//Checking if location is safe to place hazard
		if (sounds != null)
			safeToPlace2 = Mathf.Abs(GameObject.Find("Board").transform.localPosition.z % 1);
		if (safeToPlace2 > 0.9f || safeToPlace2 < 0.1f){
			safeToPlace = true;
		}
		else {
			safeToPlace = false;
		}
		if (Mathf.Abs(xPos - transform.localPosition.x) >= 0.2f){
			overlappingSound = 0;
		}
        foreach (GameObject sound in sounds) {
			
			//Making sure we are not placing hazards over a note
			if (sound != null && sound.GetComponent<SoundScript>().currentTarget == currentTarget
			&& sound.transform.localPosition.z <= 10f
			&& sound.transform.localPosition.z >= 9f){
				overlappingSound++;
			}
			if (!canPlaceHazards && canPlaceHazards2){
				if (sound != null && sound.transform.localPosition.z < 10f){
					canPlaceHazards = true;
				}
			}
			
		}
		foreach (GameObject trail in trails) {
			
			//Making sure we are not placing hazards over a note
			if (trail != null && trail.GetComponent<TrailScript>().currentTarget == currentTarget
			&& trail.transform.localPosition.z <= 10f
			&& trail.transform.localPosition.z >= 9f){
				overlappingSound++;
			}
		}
		
		//Dodging
		/*if (!dodging){
			foreach (GameObject sound in sounds) {
			
				if (sound.GetComponent<SoundScript>().currentTarget == currentTarget
				&& sound.GetComponent<SoundScript>().noteScored
				&& sound.transform.localPosition.z < 11f && sound.transform.localPosition.z >= 0.4f){
					
					float rand = Random.Range(0.0f, 10.0f);
					//dodge jump
					if ((rand < 2.0f || (chasing && Mathf.Abs(sound.transform.localPosition.x - transform.localPosition.x) < 0.5f)) && !sound.GetComponent<SoundScript>().beatScored)
					{
						jumpInitiated = true;
						
						//Jump button pressed
						timer = 0;
						jumpPressed = true;
					}
					//dodge left/right
					else if (transform.localPosition.z % 1 < 0.5f && currentTarget != "T5")
					{
						xPos = targetNum + 1;
					}
					else {
						xPos = targetNum - 1;
					}
					
					chasing = false;
					plantingHazard = false;
					switching = false;
					dodging = true;
				}
				
				
			}
		}*/
		
		if (canPlaceHazards){
			//Chasing
			if (!chasing){
				
				if (!hittingTrail){
					foreach (GameObject sound in sounds) {
						
						if (sound != null && sound.transform.localPosition.z <= 2f && sound.transform.localPosition.z > 0.5f){
							nextUpCount++;
							hittingTrail = false;
							hittingNote = true;
							soundTarget = sound;
							chasing = true;
							plantingHazard = false;
							currentlyPlacing = false;
							//hazardLock = false;
							dodging = false;
							switching = false;
							//if there is more than one sound orb at once
							if (nextUpCount == 1)
								soundTarget1 = sound;
							if (nextUpCount == 2)
								soundTarget2 = sound;
						}
					}
				}
				if (!hittingNote){
					foreach (GameObject trail in trails) {
						
						if (trail != null && trail.GetComponent<TrailScript>().beatScoredBefore && !trail.GetComponent<TrailScript>().beatScored && trail.transform.localPosition.z >= 0.49f){
							nextUpCount++;
							hittingTrail = true;
							hittingNote = false;
							soundTarget = trail;
							chasing = true;
							plantingHazard = false;
							hazardLock = true;
							currentlyPlacing = false;
							//hazardLock = false;
							dodging = false;
							switching = false;
							//if there is more than one sound orb at once
							if (nextUpCount == 1)
								soundTarget1 = trail;
							if (nextUpCount == 2)
								soundTarget2 = trail;
						}
					}
				}
				
			}
			
			
			//Planting Hazards
			if (!switching || plantingHazard){
				frickedUp = false;
			}
			if (!dodging && !chasing){
				if (!plantingHazard && !hazardLock){
					foreach (GameObject sound in sounds) {
						
						if (sound.transform.localPosition.z < 9f
						&& sound.transform.localPosition.z >= 0.4f){
							
							
							if (sound.GetComponent<SoundScript>().currentTarget == "T1"){
								t1Tracker++;
							}
							if (sound.GetComponent<SoundScript>().currentTarget == "T2"){
								t2Tracker++;
							}
							if (sound.GetComponent<SoundScript>().currentTarget == "T3"){
								t3Tracker++;
							}
							if (sound.GetComponent<SoundScript>().currentTarget == "T4"){
								t4Tracker++;
							}
							if (sound.GetComponent<SoundScript>().currentTarget == "T5"){
								t5Tracker++;
							}
							
							if (t1Tracker > t2Tracker && t1Tracker > t3Tracker && t1Tracker > t4Tracker && t1Tracker > t5Tracker){
							xPos = 1;
							}
							else if (t2Tracker > t1Tracker && t2Tracker > t3Tracker && t2Tracker > t4Tracker && t2Tracker > t5Tracker){
								xPos = 2;
							}
							else if (t3Tracker > t1Tracker && t3Tracker > t2Tracker && t3Tracker > t4Tracker && t1Tracker > t5Tracker){
								xPos = 3;
							}
							else if (t4Tracker > t1Tracker && t4Tracker > t2Tracker && t4Tracker > t3Tracker && t1Tracker > t5Tracker){
								xPos = 4;
							}
							else {
								xPos = 5;
							}
							chasing = false;
							plantingHazard = true;
						}
						
					}
				}
				if (plantingHazard && !frickedUp){
					foreach (GameObject sound in sounds) {
						
						if (sound.transform.localPosition.z <= 3f && sound.transform.localPosition.z >= 2f && Mathf.Abs(sound.transform.localPosition.x - xPos) >= 3){
							if (xPos < sound.transform.localPosition.x){
								xPos++;
							}
							else {
								xPos--;
							}
							frickedUp = true;
						}
						
						
					}
				}
			}
			
			
			//Switching
			if (!plantingHazard && !dodging && !chasing){
				
				if (!switching){
					foreach (GameObject sound in sounds) {
						int randChoice = Random.Range(1,6);
						
						xPos = randChoice;
						switching = true;
					}
					frickedUp = false;
				}
				if (switching && !frickedUp){
					foreach (GameObject sound in sounds) {
						
						if (sound.transform.localPosition.z <= 3f && sound.transform.localPosition.z >= 2f && Mathf.Abs(sound.transform.localPosition.x - xPos) >= 3){
							if (xPos < sound.transform.localPosition.x){
								xPos++;
							}
							else {
								xPos--;
							}
							frickedUp = true;
						}
						
						
					}
				}
				
			}
			
			if (chasing && soundTarget != null) {
				plantingHazard = false;
				switching = false;
				
				if (Mathf.Abs(transform.localPosition.y - soundTarget.transform.localPosition.y) > 1)
				{
					jumpInitiated = true;
					
					//Jump button pressed
					timer = 0;
					jumpPressed = true;
				}
				
				if (nextUpCount <= 1){
				
					xPos = soundTarget.transform.localPosition.x;
					
					
					if (Mathf.Abs(transform.localPosition.x - soundTarget.transform.localPosition.x) < 0.5f && Mathf.Abs(transform.localPosition.y - soundTarget.transform.localPosition.y) <= 1){
						
						if (hittingNote){
							if (soundTarget.GetComponent<SoundScript>().currentTarget == currentTarget){
								float zPos = soundTarget.GetComponent<SoundScript>().zPos;
								float rand = Random.Range(0.0f, 10.0f);
								
								if (!soundTarget.GetComponent<SoundScript>().beatScored && !soundTarget.GetComponent<SoundScript>().hit){
									if (zPos >= 0.85f && zPos <= 1.15f){
										soundTarget.GetComponent<SoundScript>().explode = true;
										//PERFECT
									}
									else if (zPos >= 0.7f && zPos <= 1.3f){
										soundTarget.GetComponent<SoundScript>().explode = true;
										//GREAT
									}
									else if (zPos >= 0.4f && zPos <= 1.6f){
										soundTarget.GetComponent<SoundScript>().explode = true;
										//OKAY
									}
									else if (zPos < 0.4f) {
										//health -= 5;
										//MISS
									}
								}
								soundTarget = null;
								hittingTrail = false;
								hittingNote = false;
								nextUpCount = 0;
							}
						}
						else {
							if (soundTarget.GetComponent<TrailScript>().currentTarget == currentTarget){
								float zPos = soundTarget.GetComponent<TrailScript>().zPos;
								float rand = Random.Range(0.0f, 10.0f);
								
								if (!soundTarget.GetComponent<TrailScript>().beatScored && !soundTarget.GetComponent<TrailScript>().hit){
									if (zPos >= 0.5f && zPos <= 1.5f){
										soundTarget.GetComponent<TrailScript>().explode = true;
										//PERFECT
									}
								}
								soundTarget = null;
								hittingTrail = false;
								hittingNote = false;
								nextUpCount = 0;
							}
						}
						
						chasing = false;
					}
					
				}
				else if (nextUpCount == 2 && soundTarget1 != null && soundTarget2 != null){
					
					xPos = (soundTarget1.transform.localPosition.x + soundTarget2.transform.localPosition.x)/2;
					
					if (Mathf.Abs(transform.localPosition.x - xPos) < 0.5f && Mathf.Abs(transform.localPosition.y - soundTarget.transform.localPosition.y) <= 1){
						
						if (hittingNote){
							if ((soundTarget1.GetComponent<SoundScript>().targetNum + soundTarget2.GetComponent<SoundScript>().targetNum)/2 == targetNum){
								float zPos = (soundTarget1.transform.localPosition.z + soundTarget2.transform.localPosition.z)/2;
								float rand = Random.Range(0.0f, 10.0f);
								
								if (!soundTarget1.GetComponent<SoundScript>().beatScored && !soundTarget1.GetComponent<SoundScript>().hit && !soundTarget2.GetComponent<SoundScript>().beatScored && !soundTarget2.GetComponent<SoundScript>().hit){
									if (zPos >= 0.85f && zPos <= 1.15f){
										soundTarget1.GetComponent<SoundScript>().explode = true;
										soundTarget2.GetComponent<SoundScript>().explode = true;
										//PERFECT
									}
									else if (zPos >= 0.7f && zPos <= 1.3f){
										soundTarget1.GetComponent<SoundScript>().explode = true;
										soundTarget2.GetComponent<SoundScript>().explode = true;
										//GREAT
									}
									else if (zPos >= 0.4f && zPos <= 1.6f){
										soundTarget1.GetComponent<SoundScript>().explode = true;
										soundTarget2.GetComponent<SoundScript>().explode = true;
										//OKAY
									}
									else if (zPos < 0.4f) {
										//health -= 5;
										//MISS
									}
								}
								soundTarget1 = null;
								soundTarget2 = null;
								hittingTrail = false;
								hittingNote = false;
								nextUpCount = 0;
							}
						}
						else {
							if ((soundTarget1.GetComponent<TrailScript>().targetNum + soundTarget2.GetComponent<TrailScript>().targetNum)/2 == targetNum){
								float zPos = (soundTarget1.transform.localPosition.z + soundTarget2.transform.localPosition.z)/2;
								float rand = Random.Range(0.0f, 10.0f);
								
								if (!soundTarget1.GetComponent<TrailScript>().beatScored && !soundTarget1.GetComponent<TrailScript>().hit && !soundTarget2.GetComponent<TrailScript>().beatScored && !soundTarget2.GetComponent<TrailScript>().hit){
									if (zPos >= 0.5f && zPos <= 1.5f){
										soundTarget1.GetComponent<TrailScript>().explode = true;
										soundTarget2.GetComponent<TrailScript>().explode = true;
										//PERFECT
									}
								}
								soundTarget1 = null;
								soundTarget2 = null;
								hittingTrail = false;
								hittingNote = false;
								nextUpCount = 0;
							}
						}
						
						chasing = false;
					}
					
				}
				else {
					nextUpCount = 0;
				}
				
			}
			
			if (plantingHazard){
				
				
				
				if (Mathf.Abs(xPos - transform.localPosition.x) < 0.2f){
					if (overlappingSound == 0){
						currentlyPlacing = true;
					}
				}
				
				switching = false;
				oldTargetNum = targetNum;
				//timer = 0f;
			}
			timer += Time.deltaTime;
		}
		
		if (dodging){
			chasing = false;
			plantingHazard = false;
			switching = false;
			if (Mathf.Abs(xPos - transform.localPosition.x) < 0.2f){
				dodging = false;
			}
		}
		
		if (switching){
			
			if ((note.GetComponent<PlayerMovement>().currentTarget == currentTarget || Mathf.Abs(xPos - transform.localPosition.x) < 0.2f) && !plantingHazard){
				plantingHazard = true;
				switching = false;
			}
			
			
			if (!hazardLock && !plantingHazard && Mathf.Abs(xPos - transform.localPosition.x) < 0.2f){
				switching = false;
			}
		}
		
		foreach (GameObject sound in sounds) {
			
			//Making sure we are not placing hazards over a note
			if (sound.GetComponent<SoundScript>().currentTarget == currentTarget
			&& sound.transform.localPosition.z < 9.3f
			&& sound.transform.localPosition.z > 8.7f){
				overlappingSound++;
			}
		}
		foreach (GameObject trail in trails) {
			
			//Making sure we are not placing hazards over a note
			if (trail != null && trail.GetComponent<TrailScript>().currentTarget == currentTarget
			&& trail.transform.localPosition.z <= 10f
			&& trail.transform.localPosition.z >= 9f){
				overlappingSound++;
			}
		}
		
		if (currentlyPlacing && overlappingSound == 0){
			if (safeToPlace && hazard1 != null && !hazardLock){
				GameObject obs = Instantiate(hazard1, new Vector3(0,hazard1.transform.position.y,0), Quaternion.identity) as GameObject; 
				obs.transform.parent = GameObject.Find("Hazards").transform;
				obs.transform.localPosition = new Vector3(targetNum, obs.transform.localPosition.y, 9f);
				plantingHazard = false;
				hazardLock = true;
				timer = 0f;
				currentlyPlacing = false;
			}
		}
		
		smoothXPos = Mathf.Lerp(smoothXPos, xPos, (smoothSpeed * Random.Range(0.5f, 1.5f)) * Time.deltaTime);
		transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3 (smoothXPos, transform.localPosition.y, transform.localPosition.z), moveSpeed * Time.deltaTime);
		
		if (jumpInitiated){
			Jump();
		}
		
		//Determining which target player is on
		Target();
		
		//Special Attack
		SpecialAttack();
		
		//Updating health bar
		healthBarImage.fillAmount = health/100f;
		
		//Updating score bar
		scoreBarImage.fillAmount = combo/25f;
		comboCount.SetText("" + totalCombo);
		pointsCount.SetText("" + totalPoints);
		
		canPlaceHazards2 = true;
		
    }
	
	private GameObject chg = null;
	private GameObject atk = null;
	
	void SpecialAttack()
	{
		
		if(combo >= 25 && !specialAttackStarted){
			
			
			chg = Instantiate(specialCharge, new Vector3(transform.position.x + specialCharge.transform.position.x,transform.position.y + specialCharge.transform.position.y + 2,specialCharge.transform.position.z), Quaternion.identity) as GameObject;
			
			chg.transform.parent = transform;
			chg.transform.localPosition = new Vector3(0, transform.localPosition.y, 8.5f);
			
			specialAttackTimer = 0f;
			specialAttackStarted = true;
		}
		
		if (specialAttackStarted){
			if (specialAttackTimer == 0){
				
					atk = Instantiate(specialAttack1, new Vector3(transform.position.x + specialAttack1.transform.position.x,specialAttack1.transform.position.y,-7), Quaternion.identity) as GameObject; 
					//atk.transform.position = transform.position;
					//atk.transform.parent = transform;
				
			}
			specialAttackTimer += Time.deltaTime;
			
			if (specialAttackTimer >= 2){
				
				if (atk != null){
					if (note.GetComponent<PlayerMovement>().targetNum == targetNum){
						note.GetComponent<PlayerMovement>().Damage(15);
						Damage(-15);
					}
					combo = 0;
					atk.transform.parent = null;
					//Destroy(atk);
					
				}
				
				if (chg != null){
					//Destroy(chg);
				}
				
				specialAttackStarted = false;
				
			}
			else {
				atk.transform.position = new Vector3(transform.position.x + specialAttack1.transform.position.x,specialAttack1.transform.position.y,-7);
			}
		}
		
	}
	
	void Jump()
	{
		
		//Jump button hasn't ended yet
		if (jumpPressed){
			jumpTimer += Time.deltaTime;
			if (transform.localPosition.y <= 0.1f){
				jumpHeight = jHeight;
				transform.localPosition = new Vector3 (transform.localPosition.x, 0, transform.localPosition.z);
				jumping = true;
				jumpPressed = false;
			}
			if (jumpTimer > 0.2f){
				jumpPressed = false;
			}
		}
		//Perform jump
		if (jumping){
			
			transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3 (transform.localPosition.x, jumpHeight, transform.localPosition.z), jTime * Time.deltaTime);
			jumpHeight = Mathf.Lerp(jumpHeight, jHeight2, jTime2 * Time.deltaTime);
			if (transform.localPosition.y <= 0.0f){
				jumpHeight = 0;
				transform.localPosition = new Vector3 (transform.localPosition.x, 0, transform.localPosition.z);
				jumpInitiated = false;
				jumping = false;
			}
		}
		
	}
	
	void Target()
	{
		if (transform.localPosition.x >= 4.5f) {
			currentTarget = "T5";
			targetNum = 5;
		}
		else if (transform.localPosition.x >= 3.5f) {
			currentTarget = "T4";
			targetNum = 4;
		}
		else if (transform.localPosition.x >= 2.5f) {
			currentTarget = "T3";
			targetNum = 3;
		}
		else if (transform.localPosition.x >= 1.5f) {
			currentTarget = "T2";
			targetNum = 2;
		}
		else if (transform.localPosition.x < 1.5f) {
			currentTarget = "T1";
			targetNum = 1;
		}
	}
	
	public void Damage(int points)
	{
		health -= points;
		health = Mathf.Clamp(health, 0f, 200f);
		healthBarPercentage.SetText(health + "%");
	}
	
}
