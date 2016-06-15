using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusiCube;

public class GameManager : MonoBehaviour {

    public MagiCube mc;

    public enum hitStatus
    {
        incoming, hitable, missed
    }

    public class Hit
    {
        public float hitTime;
        public Note nt;
        public hitStatus hs;
    }

    private enum hitGrade
    {
        miss, normal, good, perfect
    }

    public List<Hit> currentHits;
    public int hitTimeCount = 0;

	// Use this for initialization
	void Start () {
        currentHits = new List<Hit>();
	}
	
	// Update is called once per frame
	void Update () {
        // end of beatmap
        if (hitTimeCount >= mc.timeStamp.Count)
            return;
        if (hitTimeCount == mc.noteCount)
        {
            updateCurrentHits();
            hitTimeCount++;
        }
       //printCurrentHits();
    }

    void updateCurrentHits()
    {
        int time = mc.timeStamp[mc.noteCount];
        foreach(Note nt in mc.currentNotes)
        {
            Hit ht = new Hit();
            ht.nt = nt;
            ht.hitTime = time;
            ht.hs = hitStatus.hitable;
            currentHits.Add(ht);
        }
    }

    // for Debug
    void printCurrentHits()
    {
        foreach(Hit ht in currentHits)
        {
            Debug.Log(ht.hitTime + " " + ht.nt.type.ToString() + " " + ht.nt.id.ToString() + " " + ht.hs);
        }
    }
}
