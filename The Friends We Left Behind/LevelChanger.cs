using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    
	public Animator animator;
	
	private string levelToLoad;
	static public string level;
	private bool fading = false;
	
	public DifficultyStatic difficultyStatic;
	public bool finalCutscene;
	
	void Start()
	{
		//AudioListener.volume = 1;
		StartCoroutine(FadeUpAudio());
	}
	
    // Update is called once per frame
    void Update()
    {
		
    }
	
	public void SetLevel (string levelName)
	{
		levelToLoad = levelName;
		level = levelToLoad;
	}
	
	public void FadeToLevel ()
	{
		if (!fading)
		{
			StartCoroutine(FadeAudio());
			//animator.SetTrigger("FadeOut");
			animator.CrossFade("Fade_Out", 0f, -1, 0f);
			fading = true;
		}
	}
	
	public void SmoothFadeToLevel ()
	{
		if (!fading)
		{
			StartCoroutine(FadeAudio());
			//animator.SetTrigger("FadeOut");
			animator.CrossFade("Fade_Out", 0.3f, -1, 0f);
			fading = true;
		}
	}
	
	public void FadeToInstructions ()
	{
		//StartCoroutine(FadeAudio());
		//animator.SetTrigger("FadeOut");
		if (difficultyStatic != null && (difficultyStatic.GetMode() == "StoryModeCasual" || difficultyStatic.GetMode() == "StoryModePassive" || difficultyStatic.GetChangedDifficulty() > 0) && finalCutscene)
		{
			difficultyStatic.NewGame();
			difficultyStatic.SetContinueAvailable(false);
			SetLevel("Main Menu");
			FadeToLevel();
		}
		else
		{
			animator.CrossFade("Fade_Out_Instructions", 0f, -1, 0f);
		}
	}
	
	public void OnFadeComplete ()
	{
		level = levelToLoad;
		//Cursor.lockState = CursorLockMode.None;
		SceneManager.LoadScene("Loading");
	}
	
 
    //load level after one sceond delay
    IEnumerator FadeAudio() {
 
        float elapsedTime = 0;
        float currentVolume = AudioListener.volume;
 
        while(elapsedTime < 1) {
            elapsedTime += Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(currentVolume, 0, elapsedTime / 1);
            yield return null;
        }
    }
	//load level after one sceond delay
    IEnumerator FadeUpAudio() {
 
        float elapsedTime = 0;
        float currentVolume = AudioListener.volume;
 
        while(elapsedTime < 1) {
            elapsedTime += Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(currentVolume, 1, elapsedTime / 1);
            yield return null;
        }
    }
	
	public string GetLevel ()
	{
		return level;
	}
	
	public void QuitLevel ()
	{
		Application.Quit();
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false; 
		#endif
	}
}
