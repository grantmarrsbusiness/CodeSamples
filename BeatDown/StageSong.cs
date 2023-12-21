using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageSong : MonoBehaviour
{
	
		
	//Song beats per minute
	//This is determined by the song you're trying to sync up to
	public float songBpm;

	//The number of seconds for each song beat
	public float secPerBeat;

	//Current song position, in seconds
	public float songPosition;

	//Current song position, in beats
	public float songPositionInBeats;

	//How many seconds have passed since the song started
	public float dspSongTime;

	//an AudioSource attached to this GameObject that will play the music.
	public AudioSource musicSource;
	
	//The offset to the first beat of the song in seconds
	public float firstBeatOffset;
	
	//the number of beats in each loop
	public float beatsPerLoop;

	//the total number of loops completed since the looping clip first started
	public int completedLoops = 0;

	//The current position of the song within the loop in beats.
	public float loopPositionInBeats;
	
	//The current relative position of the song within the loop measured between 0 and 1.
	public float loopPositionInAnalog;
	
	//Health bar timer
	public TextMeshProUGUI healthBarTimer;
	public float songTimer = 0f;
	public int timeLeft;
	public int minutes;
	public int seconds;
	public bool musicStart = false;

	//Conductor instance
	public static StageSong instance;
	
	public float startDelay = 4.0f;
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		//Load the AudioSource attached to the Conductor GameObject
		//musicSource = GetComponent<AudioSource>();

		//Calculate the number of seconds in each beat
		secPerBeat = 60f / songBpm;

		//Record the time when the music starts
		dspSongTime = (float)AudioSettings.dspTime;
		
		Invoke("PlayAudio", startDelay);
		
		
	}
	
	void PlayAudio()
	{
		//Start the music
		musicSource.Play();
		musicStart = true;
	}
	
	void Update()
	{
		//determine how many seconds since the song started
		songPosition = (float)(AudioSettings.dspTime - dspSongTime - (firstBeatOffset + startDelay));

		//determine how many beats since the song started
		songPositionInBeats = songPosition / secPerBeat;
		
		//calculate the loop position
		if (songPositionInBeats >= (completedLoops + 1) * beatsPerLoop)
			completedLoops++;
		loopPositionInBeats = songPositionInBeats - completedLoops * beatsPerLoop;
		
		loopPositionInAnalog = loopPositionInBeats / beatsPerLoop;
		
		//updating health bar timer
		if (musicStart){
			songTimer += Time.deltaTime;
		}
		timeLeft = (int)(musicSource.clip.length - songTimer);
		minutes = (int)(timeLeft / 60);
		
		if (minutes > 0){
			seconds = timeLeft % (60*minutes);
			if (seconds >= 10)
				healthBarTimer.SetText(minutes + ":" + seconds);
			else
				healthBarTimer.SetText(minutes + ":0" + seconds);
		}
		else {
			seconds = timeLeft;
			healthBarTimer.SetText("" + seconds);
		}
	}
	
	
	//Tempo
	[HideInInspector]
	public float tempo = 128;		//adjustable tempo variable
	[HideInInspector]
	public float barCount = 1;		//adjustable bar count (usually 1, 2, or 4)
	[HideInInspector]
	public float timer = 0.0f;		//counting timer
	[HideInInspector]
	public float bpm = 0;			//beats per minute that have passed
	
	/*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		
    }
	
	*/
}
