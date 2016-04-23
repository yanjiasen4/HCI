using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using MusiCube;

public class MapMaker : MonoBehaviour {

    public Slider tl; // timeline controll slider
    public Slider bsd; // beatSnapDivisor controll slider
    public InputField bpmInput; // Input for bpm

    public Text timeText; // show current timestap
    public Text divisorText; // show current divisor

    public MagiCube mc;

    int beatSnapDivisor = 3;
    int[] divisorArray =
    {
        1,2,3,4,6,8,12
    };

    float timeSlice = 0.15f; // default time slice
    float startTime = 0f;
    int sliceCount = 0;
    int maxSlice;

    //bool isPlaying = false;

	// Use this for initialization
	void Start () {
        bpmInput.gameObject.SetActive(false);
       // CreateSong();
        mc = GetComponentInChildren<MagiCube>();
        mc.state = MagiCube.GameState.Edit;
        mc.music.Pause();
        tl.minValue = 0;
        tl.maxValue = mc.GetSongLength();
        if(mc.bm.GetBpm() == 0f)
        {
            bpmInput.gameObject.SetActive(true);
        }
        startTime = mc.bm.GetOffset();
        timeSlice = CalculateTimeSlice();
        maxSlice = (int)((mc.GetSongLength() - mc.bm.GetOffset()) / timeSlice)+1;
        print(mc.GetSongLength());
        print(mc.bm.GetOffset());
        print(timeSlice);
        print(maxSlice);
    }
	
	// Update is called once per frame
	void Update () {
        SetUIText();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mc.isPaused)
            {
                Time.timeScale = 1;
                mc.isPaused = false;
                mc.music.Play();
            }
            else
            {
                Time.timeScale = 0;
                mc.isPaused = true;
                mc.music.Pause();
            }
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            print("forward");
            beatForward();
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            print("back");
            beatBack();
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.collider.name);
                Debug.DrawLine(ray.origin, hit.point);
            }
        }
    }

    void FixedUpdate()
    {
        
    }

    private void SetUIText()
    {
        // set timeline
        float t = mc.music.time;
        tl.value = t;
        int minute = (int)(t) / 60;
        int second = (int)(t) % 60;
        int millis = (int)(1000 * (t - (int)t));
        timeText.text = minute.ToString() + ":" + second.ToString() + ":" + millis.ToString();

        // set divisor
        divisorText.text = ("BeatSnapDivisor: 1/" + divisorArray[beatSnapDivisor].ToString()); 
    }

    /*
    [DllImport("System.Windows.Forms.dll")]
    private static extern void OpenFileDialog();

    private void CreateSong()
    {
        System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
        ofd.Title = "请选择音乐";
        ofd.Filter = "所有文件(*.*)(*.*)";
        if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            string filename = ofd.FileName;
            print(filename);
        }
    }
    */

    public void SetTime(float t)
    {
        float currTime = mc.GetTime();
        print(t);
        print(currTime);
        if (t > currTime)
        {
            while ((sliceCount+1) * timeSlice < t)
            {
                sliceCount++;
            }
        }
        else if(t < currTime)
        {
            while(sliceCount * timeSlice > t)
            {
                sliceCount--;
            }
        }
        mc.SetTime(t);
        // 如果滑条被拖动，设置音乐播放的进度
        if (t > currTime + Time.deltaTime || t < currTime - Time.deltaTime)
            mc.music.time = t;
    }
    public void SetBeatSnapDivisor(float divisor)
    {
        int currDivisor = (int)divisor;
        beatSnapDivisor = currDivisor;
    }

    void UserSetBpm(float bpm)
    {
        mc.bm.SetBpm(bpm);
    }
    float CalculateTimeSlice()
    {
        return 60 / mc.bm.GetBpm() / beatSnapDivisor;
    }
    void beatForward()
    {
        float t = mc.GetTime();
        print(sliceCount);
        if (sliceCount + 1 < maxSlice)
        {
            sliceCount++;
        }
        t = sliceCount * timeSlice;
        mc.SetTime(t);
        mc.music.time = t;
    }
    void beatBack()
    {
        float t = mc.GetTime();
        print(sliceCount);
        if (sliceCount > 0 && t == timeSlice*sliceCount)
        {
            sliceCount--;
        }
        t = sliceCount * timeSlice;
        mc.SetTime(t);
        mc.music.time = t;
    }
}
