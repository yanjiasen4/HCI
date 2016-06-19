using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHand : MonoBehaviour {

    private List<GameObject> handHiden = new List<GameObject>();

	// Use this for initialization
	void Start () {
        handHiden.Add(GameObject.Find("MinimalHand(Clone)"));
        handHiden.Add(GameObject.Find("RigidRoundHand(Clone)/middle"));
        handHiden.Add(GameObject.Find("RigidRoundHand(Clone)/pinky"));
        handHiden.Add(GameObject.Find("RigidRoundHand(Clone)/ring"));
        handHiden.Add(GameObject.Find("RigidRoundHand(Clone)/palm"));
        handHiden.Add(GameObject.Find("RigidRoundHand(Clone)/forearm"));
    }
	
	// Update is called once per frame
	void Update () {
        GetHandHiden();
        // 在UI时，Hand只剩下index（食指）
        foreach (GameObject obj in handHiden)
            SetActiveFalse(obj);
    }

    void GetHandHiden()
    {
        handHiden.Add(GameObject.Find("MinimalHand(Clone)"));
        handHiden.Add(GameObject.Find("RigidRoundHand(Clone)/middle"));
        handHiden.Add(GameObject.Find("RigidRoundHand(Clone)/pinky"));
        handHiden.Add(GameObject.Find("RigidRoundHand(Clone)/ring"));
        handHiden.Add(GameObject.Find("RigidRoundHand(Clone)/palm"));
        handHiden.Add(GameObject.Find("RigidRoundHand(Clone)/forearm"));
    }

    void SetActiveFalse(GameObject obj)
    {
        if (obj != null)
            obj.SetActive(false);
    }
}
