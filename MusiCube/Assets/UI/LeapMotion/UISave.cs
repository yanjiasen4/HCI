using UnityEngine;
using System.Collections;

public class UISave : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(PlayerPrefs.GetFloat("ClickedTime") >2 && PlayerPrefs.GetInt("Buttoned") == 0)
        {

        }
	}
}
