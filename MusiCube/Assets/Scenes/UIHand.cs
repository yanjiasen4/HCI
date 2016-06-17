using UnityEngine;
using System.Collections;

public class UIHand : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // 在UI时，Hand只剩下index（食指）
        GameObject.Find("MinimalHand(Clone)").SetActive(false);
        //GameObject.Find("RigidRoundHand(Clone)/thumb").SetActive(false);
        GameObject.Find("RigidRoundHand(Clone)/middle").SetActive(false);
        GameObject.Find("RigidRoundHand(Clone)/pinky").SetActive(false);
        GameObject.Find("RigidRoundHand(Clone)/ring").SetActive(false);
        GameObject.Find("RigidRoundHand(Clone)/palm").SetActive(false);
        GameObject.Find("RigidRoundHand(Clone)/forearm").SetActive(false);
    }
}
