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

    // 节拍
    int beatSnapDivisor = 3;
    int[] divisorArray =
    {
        1,2,3,4,6,8,12
    };

    int timeSlice = 150; // default time slice
    int startTime = 0;
    int sliceCount = 0;
    int maxSlice;

    bool isSelected = false;
    int selectedBlockID = -1;

    //bool isPlaying = false;

	// Use this for initialization
	void Start () {
        bpmInput.gameObject.SetActive(false);
       // CreateSong();
        mc = GetComponentInChildren<MagiCube>();
        mc.state = MagiCube.GameState.Edit;
        mc.music.Pause();
        StartCoroutine(SetMusic());
        if(mc.bm.GetBpm() == 0f)
        {
            bpmInput.gameObject.SetActive(true);
        }
        startTime = mc.bm.GetOffset();
        timeSlice = CalculateTimeSlice();
    }

    IEnumerator SetMusic()
    {
        while(!mc.isDone)
        {
            yield return null;
        }
        tl.minValue = 0;
        tl.maxValue = mc.GetSongLength();
        maxSlice = (int)((mc.GetSongLength() * 1000 - mc.bm.GetOffset()) / timeSlice) + 1;
    }
	
	// Update is called once per frame
	void Update () {
        SetUIText();
        UpdateSliceCount();
        // Music play or pause
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mc.isPaused)
            {
                //Time.timeScale = 1;
                mc.isPaused = false;
                mc.music.Play();
            }
            else
            {
                //Time.timeScale = 0;
                mc.isPaused = true;
                mc.music.Pause();
            }
        }

        // go to next slice 
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            beatForward();
        }
        // trace back to last slice
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            beatBack();
        }

        // select block to edit
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                // 处于未选择状态
                if (isSelected == false)
                {
                    isSelected = true;
                    selectedBlockID = int.Parse(hit.collider.name);
                    mc.PlayBlockSelectedAnimation(selectedBlockID); 
                }
                // 处于选择状态
                else
                {
                    // 选中了其他方块
                    if(int.Parse(hit.collider.name) != selectedBlockID)
                    {
                        mc.PlayBlockIdleAnimation(selectedBlockID);
                        selectedBlockID = int.Parse(hit.collider.name);
                        mc.PlayBlockSelectedAnimation(selectedBlockID);
                    }
                    // 选中了之前的方块
                    else if(int.Parse(hit.collider.name) == selectedBlockID)
                    {
                        isSelected = false;
                        mc.PlayBlockIdleAnimation(selectedBlockID);
                        selectedBlockID = -1;
                    }
                }
            }
            // 选中了方块以外的物体
            else
            {
                if(isSelected == true)
                {
                    isSelected = false;
                    mc.PlayBlockIdleAnimation(selectedBlockID);
                    selectedBlockID = -1;
                }
            }
        }

        if(isSelected)
        {
            if (Input.GetKey(KeyCode.X))
            {
                print(selectedBlockID + " x");
                mc.bm.addNote(GetTimeMs(), selectedBlockID, Axis.x);
            }
            else if (Input.GetKey(KeyCode.Y))
            {
                print(selectedBlockID + " y");
                mc.bm.addNote(GetTimeMs(), selectedBlockID, Axis.y);
            }
            else if (Input.GetKey(KeyCode.Z))
            {
                print(selectedBlockID + " z");
                mc.bm.addNote(GetTimeMs(), selectedBlockID, Axis.z);
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
        int realTime = (int)System.Math.Ceiling((double)(t*1000));
        if ((int)realTime % timeSlice != 0)
            realTime -= 1;
        t = (float)(realTime) / 1000;

        if (!mc.isPaused)
        {
            // 必须在修改TimeLine值之前SetTime
            mc.SetTime(t);
            tl.value = t;
        }

        int minute = (int)(t) / 60;
        int second = (int)(t) % 60;
        int millis = realTime % 1000;
        timeText.text = minute.ToString() + ":" + second.ToString() + ":" + millis.ToString();

        // set divisor
        divisorText.text = ("BeatSnapDivisor: 1/" + divisorArray[beatSnapDivisor].ToString()); 
    }

    public void SetTimeSlice(float t)
    {
        float currTime = mc.GetTime();
        mc.SetTime(t);
        // // 如果滑条被拖动，设置音乐播放的进度
        if (t > currTime + Time.deltaTime || t < currTime - Time.deltaTime)
        {
            mc.music.time = t;
        }
    }
    public void SetBeatSnapDivisor(float divisor)
    {
        int currDivisor = (int)divisor;
        beatSnapDivisor = currDivisor;
    }
    public void UserSaveBeatMap()
    {
        // for test
        mc.bm.writeToFile("test.txt");
    }


    void UserSetBpm(float bpm)
    {
        mc.bm.SetBpm(bpm);
    }
    int CalculateTimeSlice()
    {
        return (int)((60 / mc.bm.GetBpm() / divisorArray[beatSnapDivisor])*1000);
    }
    // 由于拖动时间轴滑条，必须时时更新sliceCount参数
    void UpdateSliceCount()
    {
        int t = GetTimeMs();
        int currTime = timeSlice * sliceCount;

        if (t > currTime-1)
        {
            while ((sliceCount + 1) * timeSlice < t)
            {
                sliceCount++;
            }
        }
        else if (t < currTime-1)
        {
            while (sliceCount * timeSlice > t)
            {
                sliceCount--;
            }
        }
    }
    int GetTimeMs()
    {
        float t = mc.GetTime();
        // 修复由于音频文件采样精度导致的时间误差
        int realTime = (int)System.Math.Ceiling((t * 1000));
        if ((int)realTime % timeSlice != 0)
            realTime -= 1;
        return realTime;
    }
    void beatForward()
    {
        int tms = GetTimeMs();
        print(sliceCount);
        print(maxSlice);
        if (sliceCount + 1 < maxSlice)
        {
            sliceCount++;
        }
        tms = sliceCount * timeSlice;
        float t = ((float)(tms)) / 1000;
        mc.SetTime(t);
        mc.music.time = t;
        tl.value = t;
    }
    void beatBack()
    {
        int tms = GetTimeMs();
        if (sliceCount > 0 && tms == timeSlice*sliceCount)
        {
            sliceCount--;
        }
        tms = sliceCount * timeSlice;
        float t = ((float)(tms)) / 1000;
        mc.SetTime(t);
        mc.music.time = t;
        tl.value = t;
    }

}
