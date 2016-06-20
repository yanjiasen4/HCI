using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIEditChoose : MonoBehaviour {

    public ChooseSongUI.scrolltest s;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (PlayerPrefs.GetFloat("ClickedTime") > 2 && PlayerPrefs.GetInt("Buttoned") == 0)
        {
            PlayerPrefs.SetString("songName", s.getNow().songName);
            PlayerPrefs.SetString("diffName", s.getNow().diffs[s.getNowDiff()].difficultyName);
            PlayerPrefs.SetString("audioName", s.getNow().audioFileName);
            SceneManager.LoadScene(2);
        }
    }
}
