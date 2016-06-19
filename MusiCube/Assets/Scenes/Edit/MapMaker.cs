using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using MusiCube;
using System.Windows.Forms;

public class MapMaker : MonoBehaviour {

    public Slider tl; // timeline controll slider
    public Slider bsd; // beatSnapDivisor controll slider
    public InputField bpmInput; // Input for bpm

    public Text timeText; // show current timestap
    public Text divisorText; // show current divisor

    public Graphic notesBar; // notes bar
    public UIIdxView ud;

    public MagiCube mc;

    public float seekContinuousTrigleTime;
    public float seekMinTime;
    public float seekTrigleCount = 0;
    public float seekCount = 0;

    public float playbackSpeed = 1f;

    // 节拍
    int beatSnapDivisor = 3;
    static int[] divisorArray =
    {
        1,2,3,4,6,8,12
    };
    int currDivide;

    float timeSlice = 150f; // default time slice
    float startTime = 0;
    float currTime = 0;
    int sliceCount = 0;
    int maxSlice;

    float beatWidth = 100f;
    float beatWidthScaleStep = 3f;
    int defaultSliderDuration = 0;

    bool isSelected = false;
    int selectedBlockID = -1;

    //bool isPlaying = false;

	// Use this for initialization
	void Start () {
        bpmInput.gameObject.SetActive(false);
        mc = GetComponentInChildren<MagiCube>();
        mc.state = MagiCube.GameState.Edit;
        mc.music.Pause();
        StartCoroutine(SetMusic());
        if(mc.bm.GetBpm() == 0f)
        {
            bpmInput.gameObject.SetActive(true);
        }

        seekContinuousTrigleTime = 0.2f;
        seekMinTime = 40f;
        currDivide = divisorArray[beatSnapDivisor];
        timeSlice = CalculateTimeSlice();
        startTime = CalculateStartTime();
        defaultSliderDuration = (int)((1000 * timeSlice) * currDivide);
        initNotesBar();
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
        int realTime = (int)System.Math.Ceiling((double)(currTime * 1000));
        if ((int)realTime % timeSlice != 0)
            realTime -= 1;
        notesBar.GetComponent<UIIdxView>().curTime = realTime;

        ud.divide = currDivide;
        ud.beatWidth = beatWidth;
        //UpdateSliceCount();
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

        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            beatWidth += beatWidthScaleStep;
        }
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            beatWidth -= beatWidthScaleStep;
        }

        // play finished
        if(!mc.isPaused && !mc.music.isPlaying)
        {
            mc.isPaused = true;
        }
        
        if(!mc.isPaused)
        {
            currTime += Time.deltaTime;
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
        if(Input.GetKey(KeyCode.RightArrow))
        {
            beatForwardContinuous();
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
        {
            beatBakcContinuous();
        }
        if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) )
        {
            seekTrigleCount = 0;
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
            if (Input.GetKeyUp(KeyCode.X))
            {
                print(selectedBlockID + " x");
                mc.addNote(GetTimeMsInt(), selectedBlockID, Axis.x);
                //mc.bm.addNote(GetTimeMsInt(), selectedBlockID, Axis.x);
            }
            else if (Input.GetKeyUp(KeyCode.Y))
            {
                print(selectedBlockID + " y");
                mc.addNote(GetTimeMsInt(), selectedBlockID, Axis.y);
                //mc.bm.addNote(GetTimeMsInt(), selectedBlockID, Axis.y);
            }
            else if (Input.GetKeyUp(KeyCode.Z))
            {
                print(selectedBlockID + " z");
                mc.addNote(GetTimeMsInt(), selectedBlockID, Axis.z);
                //mc.bm.addNote(GetTimeMsInt(), selectedBlockID, Axis.z);
            }
            // Silder
            else if(Input.GetKeyUp(KeyCode.Alpha1))
            {
                print(selectedBlockID / 9 + "xplus");
                mc.addSlider(GetTimeMsInt(), selectedBlockID, Direction.xplus, defaultSliderDuration);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                print(selectedBlockID / 9 + "xminus");
                mc.addSlider(GetTimeMsInt(), selectedBlockID, Direction.xminus, defaultSliderDuration);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                print(selectedBlockID / 9 + "yplus");
                mc.addSlider(GetTimeMsInt(), selectedBlockID, Direction.yplus, defaultSliderDuration);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                print(selectedBlockID / 9 + "yminus");
                mc.addSlider(GetTimeMsInt(), selectedBlockID, Direction.yminus, defaultSliderDuration);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                print(selectedBlockID / 9 + "zplus");
                mc.addSlider(GetTimeMsInt(), selectedBlockID, Direction.zplus,  defaultSliderDuration);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha6))
            {
                print(selectedBlockID / 9 + "zminus");
                mc.addSlider(GetTimeMsInt(), selectedBlockID, Direction.zminus, defaultSliderDuration);
            }

            if(Input.GetKeyUp(KeyCode.Delete) || Input.GetKeyUp(KeyCode.Backspace))
            {
                print("delete note: " + selectedBlockID.ToString());
                mc.deleteNote(GetTimeMsInt(), selectedBlockID);
            }
        }
    }

    void FixedUpdate()
    {

    }

    private void initNotesBar()
    {
        ud = notesBar.GetComponent<UIIdxView>();
        ud.curTime = 0;
        ud.bpm = mc.bm.GetBpm();
        ud.offSet = mc.bm.GetOffset();
        ud.divide = currDivide;
        ud.notes = mc.notes;
        ud.timestamps = mc.timeStamp;
    }

    private void SetUIText()
    {
        // set timeline
        float t = currTime;
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
        divisorText.text = ("BeatSnapDivisor: 1/" + currDivide.ToString()); 
    }

    public void SetTimeSlice(float t)
    {
        mc.SetTime(t);
        UpdateSliceCount();
        // // 如果滑条被拖动，设置音乐播放的进度
        if (t > currTime + Time.deltaTime || t < currTime - Time.deltaTime)
        {
            mc.music.time = t;
        }
        currTime = t;
        sliceCount = (int)Mathf.Round((currTime - startTime) / timeSlice);
    }
    public void SetBeatSnapDivisor(float divisor)
    {
        int currDivisor = (int)divisor;
        beatSnapDivisor = currDivisor;
        int lastDivide = currDivide;
        currDivide = divisorArray[beatSnapDivisor];
        timeSlice = ((60 / mc.bm.GetBpm() / currDivide));
        startTime = CalculateStartTime();
        sliceCount = sliceCount * currDivide / lastDivide;
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
    float CalculateTimeSlice()
    {
        return ((60 / mc.bm.GetBpm() / currDivide));
    }
    float CalculateStartTime()
    {
        float st = (float)(mc.bm.GetOffset())/1000;
        while(st > timeSlice)
        {
            st -= timeSlice;
        }
        return st;
    }
    // 由于拖动时间轴滑条，必须时时更新sliceCount参数
    void UpdateSliceCount()
    {
        float currSliceTime = timeSlice * sliceCount;

        if (currTime > currSliceTime)
        {
            while ((sliceCount + 1) * timeSlice < currTime)
            {
                sliceCount++;
            }
        }
        else if (currTime < currSliceTime)
        {
            while (sliceCount * timeSlice > currTime)
            {
                sliceCount--;
            }
        }
    }
    float GetTimeMs()
    {
        return currTime * 1000;
    }
    int GetTimeMsInt()
    {
        return (int)(currTime * 1000);
    }
    public void beatForward()
    {
        float t = currTime;
        if(t == 0)
        {
            t = startTime;
        }
        else if (sliceCount + 1 < maxSlice)
        {
            sliceCount++;
            t = timeSlice * sliceCount + startTime;
            //print(startTime);
        }
        currTime = t;
        mc.SetTime(t);
        mc.music.time = t;
        tl.value = t;
    }
    public void beatBack()
    {
        float t = currTime;
        float currTimePoint = timeSlice * sliceCount + startTime;
        if (t <= startTime)
        {
            t = 0;
        }
        else if (sliceCount > 0 && (t > currTime-0.001f && t < currTime+0.001f) )
        {
            sliceCount--;
            t = timeSlice * sliceCount + startTime;
        }
        else
        {
            t = timeSlice * sliceCount + startTime;
        }
        currTime = t;
        mc.SetTime(t);
        mc.music.time = t;
        tl.value = t;
    }
    void beatForwardContinuous()
    {
        if (seekTrigleCount < seekContinuousTrigleTime)
            seekTrigleCount += Time.deltaTime;
        else {
            if (seekCount >= seekMinTime)
            {
                beatForward();
                seekCount = 0;
            }
            else
                seekCount += 1000 * Time.deltaTime;
        }
    }
    void beatBakcContinuous()
    {
        if (seekTrigleCount < seekContinuousTrigleTime)
            seekTrigleCount += Time.deltaTime;
        else {
            if (seekCount >= seekMinTime)
            {
                beatBack();
                seekCount = 0;
            }
            else
                seekCount += 1000 * Time.deltaTime;
        }
    }
    public void SetPlaybackSpeed(float input)
    {
        playbackSpeed = input;
    }
}
