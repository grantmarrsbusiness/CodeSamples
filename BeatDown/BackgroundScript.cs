using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
	private GameObject stage;
	
	//Tempo
	public float tempo = 128;		//adjustable tempo variable
	public float barCount = 1;		//adjustable bar count (usually 1, 2, or 4)
	private float stageTimer = 0.0f;		//counting timer
	private float bpm = 0;			//beats per minute that have passed
	private float stageSpeed;
	private float oldZ;
	
    // Start is called before the first frame update
    void Start()
    {
		oldZ = transform.localPosition.z;
		if (stage == null){
            stage = GameObject.FindWithTag("Stage");
			tempo = stage.GetComponent<StageSong>().tempo;
			barCount = stage.GetComponent<StageSong>().barCount;
		}
    }

    // Update is called once per frame
    void Update()
    {
        stageTimer += Time.deltaTime;
		bpm = tempo * (stageTimer/60);
		
		stageSpeed = -StageSong.instance.loopPositionInBeats * 2;
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, oldZ + stageSpeed);
		
		oldZ = transform.localPosition.z - stageSpeed;	
    }
}
