using UnityEngine;
using System.Collections;
using MusiCube;

public class Test : MonoBehaviour {

    public BeatMap bm = new BeatMap();
    // Use this for initialization
    void Start () {
        /*
        Random.seed = (int)System.DateTime.Now.Ticks;
        for(int i = 0; i < 10; i++)
        {
            Note nt = new Note();
            nt.type = NoteType.Note;
            nt.id = getRandomInt(0, 26);
            nt.dir = (Direction)getRandomInt(0, 5);
            nt.duration = 0;
            bm.addNote(0.5 * i, nt);
        }
        bm.difficultyName = "Easy";
        bm.ar = 8.0f;
        bm.od = 6.5f;
        bm.writeToFile("test.txt");
        */
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    int getRandomInt(int min, int max)
    {
        float num = Random.Range((float)min, (float)max + 1.0f);
        int ans = (int)num;
        if (ans > max)
        {
            ans = max;
        }
        return ans;
    }
}
