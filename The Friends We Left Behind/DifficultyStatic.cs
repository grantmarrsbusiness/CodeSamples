using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyStatic : MonoBehaviour
{
	
	static public int difficulty = 0;
	public int getDifficulty = 0;
	
	static public string mode;
	public string getMode;
	
	public LevelChanger levelChanger;
	
	static public int cutscene = 1;
	public int getCutscene = 1;
	
	static public int changedDifficulty = 0;
	
	static public bool lethalUnlocked = false;
	public bool getLethalUnlocked = false;
	
	static public bool firstSettings = false;
	public bool getFirstSettings = false;
	
	static public bool continueAvailable = false;
	public bool getContinueAvailable = false;
	
    // Start is called before the first frame update
    void Start()
    {
        getDifficulty = difficulty;
		getMode = mode;
		getCutscene = cutscene;
		getLethalUnlocked = lethalUnlocked;
		getFirstSettings = firstSettings;
		getContinueAvailable = continueAvailable;
    }

    // Update is called once per frame
    void Update()
    {
        getDifficulty = difficulty;
		getMode = mode;
		getCutscene = cutscene;
		getLethalUnlocked = lethalUnlocked;
		getFirstSettings = firstSettings;
		getContinueAvailable = continueAvailable;
    }
	
	public void SetDifficulty (int dif)
	{
		difficulty = dif;
	}
	
	public int GetDifficulty ()
	{
		return difficulty;
	}
	
	public string GetMode ()
	{
		return mode;
	}
	
	public void SetCutscene (int scene)
	{
		cutscene = scene;
	}
	
	public int GetCutscene ()
	{
		return cutscene;
	}
	
	public void IncreaseDifficulty ()
	{
		if (mode == "StoryModePassive")
		{
			cutscene++;
			difficulty = -1;
			levelChanger.SetLevel("Cutscene " + cutscene);
		}
		else if (mode == "StoryModeCasual")
		{
			cutscene++;
			difficulty = 0;
			levelChanger.SetLevel("Cutscene " + cutscene);
		}
		else if (difficulty < 3)
		{
			difficulty++;
		}
		//after beating insane mode
		else if (levelChanger != null)
		{
			//move to next cutscene in story mode
			if (mode == "StoryMode")
			{
				cutscene++;
				difficulty = 0;
				levelChanger.SetLevel("Cutscene " + cutscene);
			}
			//if unlocked, move to lethal mode outisde
			else if (lethalUnlocked)
			{
				if (difficulty < 4)
				{
					difficulty++;
				}
				else
				{
					levelChanger.SetLevel("Main Menu");
					difficulty = 0;
				}
			}
			//if not unlocked, do nothing
			else
			{
				
			}
		}
	}
	
	public void SetMode (string mod)
	{
		mode = mod;
	}
	
	public void SetLethalUnlocked (bool leth)
	{
		lethalUnlocked = leth;
		PlayerPrefs.SetInt("lethalUnlocked", (lethalUnlocked ? 1 : 0));
		
	}
	
	public bool GetLethalUnlocked ()
	{
		return lethalUnlocked;
	}
	
	public void SetFirstSettings (bool sett)
	{
		firstSettings = sett;
		PlayerPrefs.SetInt("firstSettings", (firstSettings ? 1 : 0));
		//SaveSystem.SavePlayer(this);
	}
	
	public bool GetFirstSettings ()
	{
		return firstSettings;
	}
	
	public void SetContinueAvailable (bool cont)
	{
		continueAvailable = cont;
		PlayerPrefs.SetInt("continueAvailable", (continueAvailable ? 1 : 0));
	}
	
	public bool GetContinueAvailable ()
	{
		return continueAvailable;
	}
	
	public void IncreaseChangedDifficulty ()
	{
		changedDifficulty++;
	}
	public void SetChangedDifficulty (int change)
	{
		changedDifficulty = change;
	}
	public int GetChangedDifficulty ()
	{
		return changedDifficulty;
	}
	
	public void NewGame ()
	{
		if (mode == "StoryModePassive")
		{
			difficulty = -1;
		}
		else {
			difficulty = 0;
		}
		changedDifficulty = 0;
		cutscene = 1;
		continueAvailable = false;
		//lethalUnlocked = data.lethalUnlocked;
		levelChanger.SetLevel("Cutscene " + cutscene);
		//SaveSystem.SavePlayer(this);
		SavePlayer();
	}
	public void SavePlayer ()
	{
		//SetContinueAvailable(true);
		//SAVE DATA
		//cutscene
		PlayerPrefs.SetInt("cutscene", cutscene);
		//difficulty
		PlayerPrefs.SetInt("difficulty", difficulty);
		//changed difficulty
		PlayerPrefs.SetInt("changedDifficulty", changedDifficulty);
		//continue available
		PlayerPrefs.SetInt("continueAvailable", (continueAvailable ? 1 : 0));
		//lethal mode
		PlayerPrefs.SetInt("lethalUnlocked", (lethalUnlocked ? 1 : 0));
		//SaveSystem.SavePlayer(this);
	}
	public void LoadPlayer ()
	{
		//PlayerData data = SaveSystem.LoadPlayer();
		
		//difficulty = data.difficulty;
		difficulty = PlayerPrefs.GetInt("difficulty");
		//cutscene = data.cutscene;
		cutscene = PlayerPrefs.GetInt("cutscene");
		levelChanger.SetLevel("Cutscene " + cutscene);
		changedDifficulty = PlayerPrefs.GetInt("changedDifficulty");
		//lethalUnlocked = data.lethalUnlocked;
		lethalUnlocked = (PlayerPrefs.GetInt("lethalUnlocked") != 0);
		//continueAvailable = (PlayerPrefs.GetInt("continueAvailable") != 0);
		//firstSettings = (PlayerPrefs.GetInt("firstSettings") != 0);
	}
}
