using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	
	public float health = 100f;
	public Image healthBarImage;
	public TextMeshProUGUI healthBarPercentage;
	
	public Image scoreBarImage;
	public TextMeshProUGUI comboCount;
	public TextMeshProUGUI pointsCount;
	
	private float depth = 5.0f;
	private Vector3 wantedPos = new Vector3(0,0,0);
	public float correctedX2 = 3f;
	private bool jumping = false;
	private bool jumpPressed = false;
	private float timer = 0.0f;		//counting timer
	private float jumpHeight = 0.0f;		//counting timer
	
	public bool keyControl = true;
	
	public float moveSpeed = 20f;
	
	public float jHeight = 4.5f;
	public float jHeight2 = -5f;
	public float jTime = 2f;
	public float jTime2 = 2f;
	
	public string currentTarget = "T3";
	
	public GameObject beat;
	public GameObject[] sounds;
	public float nextSoundTarget;
	public float oldSoundTarget;
	public float olderSoundTarget;
	public bool switchingNextSoundTarget;
	public bool autoAdjust = false;
	
	public float targetNum = 3;
	public GameObject specialAttack1;
	public GameObject specialCharge;
	public bool specialAttackStarted = false;
	public float specialAttackTimer = 10f;
	public float specialAttackNum = 11f;
	public float specialAttackTarget = 3f;
	
	public float mouse0timer = 0;
	public bool mouse0Pressed = false;
	public float mouse1timer = 0;
	public bool mouse1Pressed = false;
	
	public float combo = 0f;
	public float totalCombo = 0f;
	public float totalPoints = 0f;
	
    // Start is called before the first frame update
    void Start()
    {
		if (beat == null)
            beat = GameObject.FindWithTag("Beat");
        sounds = GameObject.FindGameObjectsWithTag("Sound");
    }
	
	public float nextUpCount = 0f;
	public float moveX = 3f;
	public float hori = 0f;
	public float oldHori = 0f;
	public float switchingRange = 0.0f;
	public float switchingSpeed = 0.0f;
	public bool changedSides = false;
	public float desiredTarget = 3f;
	public float brakes;
	public float brakes2;
	
    // Update is called once per frame
    void Update()
    {
		
		autoAdjust = false;
		switchingNextSoundTarget = false;
		nextUpCount = 0f;
		olderSoundTarget = nextSoundTarget;
		//Making sure this note is next in line
		foreach (GameObject sound in sounds) {
			if (sound != null){
				
				if (sound.transform.localPosition.z < 6 && sound.transform.localPosition.z - transform.localPosition.z > -1f){
					autoAdjust = true;
				}
				
				//allowing the player to target a note that just exploded if they were aiming for it but just missed it, in order to prevent confusion
				if (sound.transform.localPosition.z <= 0.4f && sound.transform.localPosition.z > 0f){
					switchingNextSoundTarget = true;
				}
				
				if (!switchingNextSoundTarget && sound.GetComponent<SoundScript>().nextUp == 0 && !sound.GetComponent<SoundScript>().scored){
					nextUpCount++;
					if (Mathf.Abs(hori) > 0.2f)
						nextSoundTarget = sound.GetComponent<SoundScript>().targetNum;
					//if there is more than one sound orb at once
					if (nextUpCount >= 2)
						nextSoundTarget = (nextSoundTarget + oldSoundTarget)/2;
					oldSoundTarget = nextSoundTarget;
				}
			
			}
		}
		if (switchingNextSoundTarget)
			nextSoundTarget = olderSoundTarget;
		
		if ((Input.GetKey("left") || Input.GetKey(KeyCode.A)) && (Input.GetKey("right") || Input.GetKey(KeyCode.D)))			
			hori = Mathf.Lerp(hori, 0, moveSpeed * 0.5f * Time.deltaTime);
		else
			hori = Input.GetAxis("Horizontal");
		
		/*
		if (Mathf.Abs(hori) <= 0.6f)
			switchingRange = 0.0f;
		else
			switchingRange = 0.1f;
		*/
		if (Mathf.Abs(hori) >= 0.15f && Mathf.Abs(hori) < 0.5f)
			switchingRange = -0.2f + Mathf.Abs(hori)/2.5f;
		else if (Mathf.Abs(hori) >= 0.5f)
			switchingRange = 0.1f;
		else
			switchingRange = 0.0f;
		
		switchingSpeed = 1.5f - Mathf.Abs(hori)*0.4f;
		
		if(Input.GetMouseButtonDown(0)){
			keyControl = false;
		}
		if (hori != 0){
			if (!keyControl)
				desiredTarget = transform.localPosition.x;
			keyControl = true;
		}
		
		//Mouse Control
		if (!keyControl){
			//Left/right movement
			Vector3 mousePos = Input.mousePosition;
			Vector3 wantedPos = Camera.main.ScreenToWorldPoint (new Vector3 (mousePos.x, mousePos.y, depth));
			float correctedX = wantedPos.x;
			if (correctedX >= -1.35f && correctedX <= 1.35f){
				correctedX2 = 1 + (correctedX + 1.35f) * (4.0f/2.7f);
			}
			transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3 (correctedX2, transform.localPosition.y, transform.localPosition.z), moveSpeed * Time.deltaTime);
		}
		else {
			
			
				
			if (Mathf.Abs(hori) >= oldHori && Mathf.Abs(hori) != 0){
				changedSides = true;
				//desiredTarget += hori * 0.02f * (moveSpeed * (1.4f - Mathf.Abs(hori)*1f));
				desiredTarget += hori * 0.02f * (moveSpeed * (1.25f - Mathf.Abs(hori)*0.8f));
				brakes = 1f;
				correctedX2 = desiredTarget;
			}
			
			if (Mathf.Abs(hori) < oldHori){
				if (changedSides){
					if (hori > 0){
						if (transform.localPosition.x + switchingRange > targetNum && (!autoAdjust || autoAdjust && (nextSoundTarget != targetNum || hori < 0.3f)) || autoAdjust && nextSoundTarget == targetNum + 1){
							//auto adjusting one over
							correctedX2 = targetNum + 1;
							//slowing auto adjustment speed since it is a large leap
							if (Mathf.Abs(desiredTarget - correctedX2) >= 1){
								brakes = Mathf.Abs(transform.localPosition.x - correctedX2);
								brakes2++;
							}
						}
						else {
							correctedX2 = targetNum;
						}
						changedSides = false;
					}
					else if (hori < 0){
						if (transform.localPosition.x - switchingRange < targetNum && (!autoAdjust || autoAdjust && (nextSoundTarget != targetNum || hori > -0.3f)) || autoAdjust && nextSoundTarget == targetNum - 1){
							//auto adjusting one over
							correctedX2 = targetNum - 1;
							//slowing auto adjustment speed since it is a large leap
							if (Mathf.Abs(desiredTarget - correctedX2) >= 1){
								brakes = Mathf.Abs(transform.localPosition.x - correctedX2);
								brakes2++;
							}
						}
						else {
							correctedX2 = targetNum;
						}
						changedSides = false;
					}
				}
			}
			else {
				brakes = 1f;
			}
			
			//Snapping to target
			desiredTarget = Mathf.Lerp(desiredTarget, correctedX2, ((moveSpeed * switchingSpeed)/brakes) * Time.deltaTime);
			desiredTarget = Mathf.Clamp(desiredTarget, 1f, 5f);
			
			transform.localPosition = new Vector3(desiredTarget, transform.localPosition.y, transform.localPosition.z);
			
			// initially, the temporary vector should equal the player's position
			Vector3 clampedPosition = transform.localPosition;
			// Now we can manipulte it to clamp the y element
			clampedPosition.x = Mathf.Clamp(clampedPosition.x, 1f, 5f);
			// re-assigning the transform's position will clamp it
			transform.localPosition = clampedPosition;
			
			
		}
		oldHori = Mathf.Abs(hori);
		
		//Special Attack
		SpecialAttack();
		
		//Jumping
		Jump();
		
		//Determining which target player is on
		Target();
		
		//Updating health bar
		healthBarImage.fillAmount = health/100f;
		
		//Updating score bar
		scoreBarImage.fillAmount = combo/25f;
		comboCount.SetText("" + totalCombo);
		pointsCount.SetText("" + totalPoints);
		
		
    }
	
	private GameObject chg = null;
	private GameObject atk = null;
	
	void SpecialAttack()
	{
		
		//Left click pressed
		if((Input.GetMouseButtonDown(0) || Input.GetKeyDown("space") || Input.GetKeyDown("enter"))){
			mouse0timer = 0;
			mouse0Pressed = true;
		}
		//Left click hasn't ended yet
		if (mouse0Pressed){
			mouse0timer += Time.deltaTime;
			if (mouse0timer > 0.1f){
				mouse0Pressed = false;
			}
		}
		//Right click pressed
		if((Input.GetMouseButtonDown(1) || Input.GetAxis("Vertical") > 0)){
			mouse1timer = 0;
			mouse1Pressed = true;
		}
		//Right click hasn't ended yet
		if (mouse1Pressed){
			mouse1timer += Time.deltaTime;
			if (mouse1timer > 0.1f){
				mouse1Pressed = false;
			}
		}
		
		/*if(mouse0Pressed && mouse1Pressed){
			specialAttackTimer = 0f;
			specialAttackStarted = true;
			
				chg = Instantiate(specialCharge, new Vector3(transform.position.x + specialCharge.transform.position.x,transform.position.y + specialCharge.transform.position.y,specialCharge.transform.position.z), Quaternion.identity) as GameObject;
				//chg.transform.position = transform.position;
				chg.transform.parent = transform;
			
			
			mouse0Pressed = false;
			mouse1Pressed = false;
		}*/
		
		
		if(combo >= 25 && !specialAttackStarted){
			
			
			chg = Instantiate(specialCharge, new Vector3(transform.position.x + specialCharge.transform.position.x,transform.position.y + specialCharge.transform.position.y,specialCharge.transform.position.z), Quaternion.identity) as GameObject;
			//chg.transform.position = transform.position;
			chg.transform.parent = transform;
			
			specialAttackTimer = 0f;
			specialAttackStarted = true;
		}
		
		if (specialAttackStarted){
			if (specialAttackTimer == 0){
				
					atk = Instantiate(specialAttack1, new Vector3(transform.position.x + specialAttack1.transform.position.x,specialAttack1.transform.position.y,specialAttack1.transform.position.z), Quaternion.identity) as GameObject; 
					//atk.transform.position = transform.position;
					//atk.transform.parent = transform;
				
			}
			specialAttackTimer += Time.deltaTime;
			
			if (specialAttackTimer >= 2){
				
				if (atk != null){
					if (beat.GetComponent<BeatCPU>().targetNum == targetNum){
						beat.GetComponent<BeatCPU>().Damage(15);
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
				atk.transform.position = new Vector3(transform.position.x + specialAttack1.transform.position.x,specialAttack1.transform.position.y,specialAttack1.transform.position.z);
			}
		}
		
		//Extra Special Attack
		/*if(mouse0Pressed && mouse1Pressed){
			specialAttackTimer = 0f;
			specialAttackNum = 0f;
			specialAttackTarget = targetNum;
			mouse0Pressed = false;
			mouse1Pressed = false;
		}
		
		if(mouse0Pressed && mouse1Pressed && specialAttackNum >= 10){
			specialAttackTimer = 0f;
			specialAttackNum = 0f;
			specialAttackTarget = targetNum;
			mouse0Pressed = false;
			mouse1Pressed = false;
		}
		
		if (specialAttack1 != null && specialAttackNum < 10){
			
			specialAttackTimer += Time.deltaTime;
			if (specialAttackTimer > 0.03f){
				specialAttackTimer = 0f;
				specialAttackNum++;
			}
			for(int i = 0; i < 11; i++)
			{
				if (specialAttackNum == i){
					GameObject atk = Instantiate(specialAttack1, new Vector3(0,0,0), Quaternion.identity) as GameObject; 
					atk.transform.parent = GameObject.Find("Specials").transform;
					atk.transform.localPosition = new Vector3(specialAttackTarget, atk.transform.localPosition.y, i+1.5f);
				}
			}
			
		}*/
		
	}
	private IEnumerator Countdown2(int i) {
		while(true) {
			yield return new WaitForSeconds(0.25f); //wait 2 seconds
			
		}
	}
	
	void Jump()
	{
		//Jump button pressed
		if(Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)){
			timer = 0;
			jumpPressed = true;
		}
		//Jump button hasn't ended yet
		if (jumpPressed){
			timer += Time.deltaTime;
			if (transform.localPosition.y <= 0.1f){
				jumpHeight = jHeight;
				transform.localPosition = new Vector3 (transform.localPosition.x, 0, transform.localPosition.z);
				jumping = true;
				jumpPressed = false;
			}
			if (timer > 0.2f){
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
