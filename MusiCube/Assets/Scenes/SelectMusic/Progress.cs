using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MusiCube;

public class Progress : MonoBehaviour {

    public Image img;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void playCircle(float time)
    {
        Sprite[] circleSeq = CircleSource.instance.circleSequence;
        int idx = (int)(time * circleSeq.Length) % circleSeq.Length;
        img.sprite = circleSeq[idx];
    }
}
