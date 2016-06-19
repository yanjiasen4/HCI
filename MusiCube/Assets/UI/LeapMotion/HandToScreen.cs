using UnityEngine;
using System.Collections;

public class HandToScreen : MonoBehaviour {

    public Camera c;
    public Transform t2;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 ScreenPos = c.WorldToScreenPoint(GameObject.Find("index/bone3").GetComponent<Transform>().position);
        Vector3 GUIPos = new Vector3(ScreenPos.x, ScreenPos.y, 0);
        t2.position = GUIPos;
    }
}
