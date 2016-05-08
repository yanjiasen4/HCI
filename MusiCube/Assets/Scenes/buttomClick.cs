using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class buttomClick : MonoBehaviour {

    public RawImage rImage;
    public Texture texture;

	// Use this for initialization
	void Start () {
     
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void change()
    {
        rImage.texture = (Texture2D)Resources.Load("AU821HWIYU7[0DFQAIB1KX"); ;
    }

    public void changeDiff()
    {
        Debug.Log(PlayerPrefs.GetInt("nowDiff"));
        PlayerPrefs.SetInt("nowDiff", PlayerPrefs.GetInt("nowDiff") + 1);
        Debug.Log(PlayerPrefs.GetInt("nowDiff"));
    }
}
