using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusiCube;
using NAudio;
using NAudio.Wave;
using System;

public class MagiCube : MonoBehaviour
{
    public enum GameState
    {
        Play,
        Edit
    }
    public GameObject block;
    public GameObject layer;
    public float blockSize = 0.05f;
    public Material activeMt;
    public Material inActiveMt;
    public float timeSlice = 0.5f;
    public Face threeFace;
    BlockStatus[,,] squareBlock = new BlockStatus[3, 3, 3];
    Vector3[,,] initialBlockPosition = new Vector3[3, 3, 3];
    public float timeCount = 0;
    public int noteCount = 0;
    public SortedDictionary<int, List<Note>> notes = new SortedDictionary<int, List<Note>>();
    // 定义Slider为可比较类型并重载比较方法
    public class Slider : IComparable
    {
        public int start;
        public int end;
        public Note nt;
        public int CompareTo(object obj)
        {
            int res = 0;
            try
            {
                Slider sld = (Slider)obj;
                if (this.start > sld.start)
                    return 1;
                else if (this.start < sld.start)
                    return -1;
            }
            catch (Exception ex)
            {
                throw new Exception("Slider/CompareTo: Compare exception", ex.InnerException);
            }
            return res;
        }
    }
    public List<Slider> sliders = new List<Slider>();

    public List<int> timeStamp = new List<int>();
    public List<Note> currentNotes = new List<Note>();
    public List<Note> nextNotes = new List<Note>();
    private GameObject sliderAnimation;
    bool isOver = false;
    public GameState state;
    public bool isPaused = false;
    // 音乐控件
    public AudioSource music;
    float musicLength; // ms
    public int beatMapLength; // 最后一个note所在时间

    public Quaternion blockRotate;
    // 帮助加载音乐
    WWW www;
    public bool isDone = false;

    string songName;
    string songFullPathAndName;
    string beatmapFullPathAndName;
    string blockPath = "MagiCube/";
    public BeatMap bm;

    public float appTime; // 缩圈时间/ms
    public float judgeRange; // 判定范围/ms

    // 缩圈时间数组
    private float[] appTimeTable =
    {
        1800,1680,1560,1440,1320,1200,1050,900,750,600,450
    };

    // Use this for initialization

    void Start()
    {
        Debug.Assert(block != null);
        music = GetComponent<AudioSource>();
        bm = new BeatMap();

        // for debug
        bm.readFromFile("test.txt");
        songName = "STYX HELIX";
        InitialSquare();
        InitialNote();
        StartCoroutine(LoadMusic());
        blockRotate = block.transform.rotation;
        appTime = getAppTime(bm.ar);
        judgeRange = getJudgeRange(bm.od);
        recordBlockPosition();
        setDropTime(appTime/1000);
        sliderAnimation = Instantiate(layer, Vector3.zero, Quaternion.identity) as GameObject;
        sliderAnimation.GetComponentInChildren<RowAnime>().Initiate();
        sliderAnimation.SetActive(false);
    }

    // Use WWW to asynchronously load a music resource
    IEnumerator LoadMusic()
    {
        
        string songPath = "Songs/" + songName + "/" + songName + ".mp3";
        string beatmapPath = "Songs/" + songName + "/" + songName + ".mcb";
        songFullPathAndName = "file:///" + System.IO.Path.Combine(Application.streamingAssetsPath, songPath);
        beatmapFullPathAndName = System.IO.Path.Combine(Application.streamingAssetsPath, beatmapPath);
       
        www = new WWW(songFullPathAndName);
        yield return www;

        music.clip = AudioLoader.FromMp3Data(www.bytes, "music");
        //music.clip= www.GetAudioClip(true, true);
        musicLength = music.clip.length;
        Debug.Log("music: " + songName + "load success\n" + "Length: " + musicLength.ToString());

        if (state == GameState.Play)
            music.Play();
        isDone = true;
    }

    public int getRandomInt(int min, int max)
    {
        float num = UnityEngine.Random.Range((float)min, (float)max + 1.0f);
        int ans = (int)num;
        if (ans > max)
        {
            ans = max;
        }
        return ans;
    }

    // Update is called once per frame
    void Update()
    {
        if (www == null || !isDone)
           return;
        switch(state)
        {
            // 游戏模式
            case GameState.Play:
                {

                    // update time
                    clearBlockAnimation();
                    resetBlockPosition();
                    if (timeStamp.Count == 0 || noteCount >timeStamp.Count)
                        break;
                    int timeMs = (int)(timeCount * 1000);
                   // if (timeMs > timeStamp[timeStamp.Count - 1] + 10000f)
                   //     break;

                    List<int> notesIndex = getTimeRangeNoteIndexDelay(timeMs, (int)appTime, 4*(int)judgeRange);
                    List<Slider> slidersRender = getTimeRangeSlider(timeMs, (int)appTime);
                    foreach (int i in notesIndex)
                    {
                        foreach (Note nt in notes[timeStamp[i]])
                        {
                            renderSingleNoteStaticly(nt, timeStamp[i], timeMs);
                        }
                    }

                    foreach (Slider sld in slidersRender)
                    {
                        renderSingleSliderStaticly(sld.nt, sld.start, sld.end, timeMs);
                    }

                    if (timeMs - judgeRange > timeStamp[noteCount])
                    {
                        if (noteCount < timeStamp.Count - 1)
                        {
                            noteCount++;
                            currentNotes = nextNotes;
                            nextNotes = notes[timeStamp[noteCount]];
                        }
                    }

                    timeCount += Time.deltaTime > 0.1f ? 0 : Time.deltaTime;
                    break;
                }
            // 编辑模式，画面静止
            case GameState.Edit:
                {
                    if (timeStamp.Count == 0)
                        break;
                    clearBlockAnimation();
                    resetBlockPosition();
                    int timeMs = (int)(timeCount * 1000);
                    if (timeMs > timeStamp[timeStamp.Count - 1])
                        break;
                    List<int> notesIndex = getTimeRangeNoteIndex(timeMs, (int)appTime);
                    List<Slider> slidersRender = getTimeRangeSlider(timeMs, (int)appTime);
                    foreach(int i in notesIndex)
                    {
                        foreach(Note nt in notes[timeStamp[i]])
                        {
                            renderSingleNoteStaticly(nt, timeStamp[i], timeMs);
                        }
                    }
                    
                    foreach(Slider sld in slidersRender)
                    {
                        renderSingleSliderStaticly(sld.nt, sld.start, sld.end, timeMs);
                    }

                    if(!isPaused)
                    {
                        // timeCount += Time.deltaTime;
                        
                    }
                    
                    /*
                     * [TODO]
                     */
                    break;
                }
        }
    }

    private List<Slider> getTimeRangeSlider(int timeMs, int appTime)
    {
        List<Slider> ret = new List<Slider>();
        foreach(Slider sld in sliders)
        {
            if (sld.start - appTime > timeMs || sld.end < timeMs)
                continue;
            if (sld.start - appTime < timeMs && sld.end > timeMs)
                ret.Add(sld);
        }
        return ret;
    }

    public void SetTime(float t)
    {
        timeCount = t;
        int realTime = (int)System.Math.Ceiling((double)(t * 1000));
        if ((int)realTime % timeSlice != 0)
            realTime -= 1;
    }
    public void UpdateBeatMap()
    {

    }
    public float GetTime()
    {
        return timeCount;
    }

    void InitialSquare()
    {
        Transform blockTrans = block.transform;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    squareBlock[i, j, k] = new BlockStatus();
                    squareBlock[i, j, k].originalLocation = transform.position + (blockTrans.right * (k - 1) * blockSize) + (blockTrans.forward * (j - 1) * blockSize) + (blockTrans.up * (i - 1) * blockSize);
                    squareBlock[i, j, k].block = (GameObject)Instantiate(block, squareBlock[i, j, k].originalLocation, blockTrans.rotation);
                    squareBlock[i, j, k].block.transform.parent = transform;
                    squareBlock[i, j, k].block.name = (i * 9 + j * 3 + k).ToString();
                    squareBlock[i, j, k].activeMt = activeMt;
                    squareBlock[i, j, k].inActiveMt = inActiveMt;
                    squareBlock[i, j, k].blockSize = blockSize;
                    Block blockScript = squareBlock[i, j, k].block.GetComponent<Block>();
                    if (blockScript)
                        blockScript.BindParent(k, i, j);
                }
            }
        }
    }

    void InitialNote()
    {
        notes = bm.getNotes();
        int offset = bm.GetOffset();
        foreach(int key in notes.Keys)
        {
            timeStamp.Add(key);
        }
        beatMapLength = timeStamp[timeStamp.Count - 1];
        if (notes.Count == 0) // 没有notes
            isOver = true;
        else if(notes.Count == 1)
        {
            currentNotes = nextNotes = notes[timeStamp[0]];
        }
        else
        {
            currentNotes = notes[timeStamp[0]];
            nextNotes = notes[timeStamp[1]];
        }
        // add sliders
        foreach(int key in notes.Keys)
        {
            foreach(Note nt in notes[key])
            {
                if(nt.type == NoteType.Slider)
                {
                    Slider sld = new Slider();
                    sld.nt = nt;
                    sld.start = key;
                    sld.end = key + nt.duration;
                    sliders.Add(sld);
                }
            }
        }
    }

    private void recordBlockPosition()
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++)
                    initialBlockPosition[i, j, k] = squareBlock[i, j, k].block.transform.position;

    }

    private int[] getSliderIndex(Direction dir, int layer)
    {
        int[] ret = new int[9];
        switch(dir)
        {
            case Direction.xminus:
            case Direction.xplus:
                {
                    for(int i = 0; i < 3; i++)
                    {
                        ret[3 * i] = 9 * i + 3 * layer;
                        ret[3 * i + 1] = 9 * i + 3 * layer + 1;
                        ret[3 * i + 2] = 9 * i + 3 * layer + 2;
                    }
                    break;
                }
            case Direction.yminus:
            case Direction.yplus:
                {
                    for(int i = 0; i < 3; i++)
                    {
                        ret[3 * i] = 3 * i + 9 * layer;
                        ret[3 * i + 1] = 3 * i + 9 * layer + 1;
                        ret[3 * i + 2] = 3 * i + 9 * layer + 2;
                    }
                    break;
                }
            case Direction.zminus:
            case Direction.zplus:
                {
                    for(int i = 0; i < 3; i++)
                    {
                        ret[3 * i] = 9 * i + layer;
                        ret[3 * i + 1] = 9 * i + layer + 3;
                        ret[3 * i + 2] = 9 * i + layer + 6;
                    }
                    break;
                }
            default: break;
        }
        return ret;
    }
    private int[] getSliderReplaceIndex(Direction dir, int layer)
    {
        int[] ret = new int[9];
        switch(dir)
        {
            case Direction.xplus:
                {
                    for(int i = 0; i < 3; i++)
                    {
                        ret[3 * i] =  2 - i + 3 * layer;
                        ret[3 * i + 1] = 2 - i + 3 * layer + 9;
                        ret[3 * i + 1] = 2 - i + 3 * layer + 18;
                    }
                    break;
                }
            case Direction.xminus:
                {
                    for(int i = 0; i < 3; i++)
                    {
                        ret[3 * i] = i + 3 * layer + 18;
                        ret[3 * i + 1] = i + 3 * layer + 9;
                        ret[3 * i + 2] = i + 3 * layer;
                    }
                    break;
                }
            case Direction.yplus:
                {
                    for(int i = 0; i < 3; i++)
                    {
                        ret[3 * i] = (2 - i) + 9 * layer;
                        ret[3 * i + 1] = (2 - i) + 9 * layer + 3;
                        ret[3 * i + 2] = (2 - i) + 9 * layer + 6;
                    }
                    break;
                }
            case Direction.yminus:
                {
                    for(int i = 0; i < 3; i++)
                    {
                        ret[3 * i] = i + 9 * layer + 6;
                        ret[3 * i + 1] = i + 9 * layer + 3;
                        ret[3 * i + 2] = i * layer;
                    }
                    break;
                }
            case Direction.zplus:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        ret[3 * i] = 6 + 9 * layer + i; ;
                        ret[3 * i + 1] = 3 + 9 * layer + i;
                        ret[3 * i + 1] = 9 * layer + i;
                    }
                    break;
                }
            case Direction.zminus:
                {
                    for(int i = 0; i < 3; i++)
                    {
                        ret[3 * i] = (2 - i) + 9 * layer;
                        ret[3 * i + 1] = (2 - i) + 9 * layer + 3;
                        ret[3 * i + 2] = (2 - i) * 9 * layer + 6;
                    }
                    break;
                }
        }
        return ret;
    }

    public void renderNote(Note nt)
    {
        switch (nt.type)
        {
            case NoteType.Note:
                {
                    renderSingleNote(nt);
                    break;
                }
            case NoteType.Slider:
                {
                    renderSingleSlider(nt);
                    break;
                }
            case NoteType.Plane:
                {
                    break;
                }
        }
    }

    private void renderSingleNote(Note nt)
    {
        if (nt.type != NoteType.Note)
            return;
        GameObject target = GameObject.Find(blockPath + nt.id.ToString());
        target.GetComponent<SelectAnime>().playPlaneRaise(nt.dir, appTime);
    }
    
    private void renderSingleSlider(Note nt)
    {
        if (nt.type != NoteType.Slider)
            return;
        int layerID = nt.id;
        Direction dir = nt.dir;
        float duration = nt.duration / 1000f;
        int[] blocksBefore = getSliderIndex(dir, layerID);
        BlockIndex centerIndex = getBlockIndex(blocksBefore[4]);
        Vector3 rotateDir = getSliderRotateDirection(dir);
        Vector3 centerPoint = squareBlock[centerIndex.x,centerIndex.y,centerIndex.z].block.transform.position;

        //GameObject sliderAnimation = Instantiate(layer, centerPoint, Quaternion.identity) as GameObject;
        sliderAnimation.GetComponent<RowAnime>().Initiate();
        sliderAnimation.GetComponent<RowAnime>().raiseTime = appTime / 1000;
        sliderAnimation.GetComponent<RowAnime>().autoPlay(RowState.raise);
        for(int i = 0; i < 9; i++)
        {
            int beforeIndex = blocksBefore[i];
            GameObject blk = getBlockById(beforeIndex);
            // 总是旋转90度
            blk.GetComponent<SelectAnime>().autoPlayRotate(centerPoint, rotateDir, 90f, duration, appTime);
        }
        // Update blockPos
        resetBlockPosition();
        /*
        for (int i = 0; i < 9; i++)
        {
            int beforeIndex = blocksBefore[i];
            BlockIndex bi = getBlockIndex(beforeIndex);
            squareBlock[bi.x, bi.y, bi.z].block.name = BIList[i].ToString();
        }
        */
    }

    private Vector3 getSliderRotateDirection(Direction dir)
    {
        Vector3 ret;
        switch(dir)
        {
            case Direction.xplus:
                ret = Vector3.forward;
                ret.Normalize();
                break;
            case Direction.xminus:
                ret = Vector3.back;
                ret.Normalize();
                break;
            case Direction.yplus:
                ret = Vector3.up;
                break;
            case Direction.yminus:
                ret = Vector3.down;
                break;
            case Direction.zplus:
                ret = Vector3.right;
                ret.Normalize();
                break;
            case Direction.zminus:
                ret = Vector3.left;
                ret.Normalize();
                break;
            default:
                ret = Vector3.zero;
                break;
        }
        return ret;
    }

    private void renderSinglePlane(BlockStatus target, Note nt)
    {
        if (nt.type != NoteType.Plane)
            return;
        // [TODO]
    }

    private void renderNoteStaticly(Note nt, int noteTime, int endTime, int t)
    {
        switch(nt.type)
        {
            case NoteType.Note:
                renderSingleNoteStaticly(nt, noteTime, t);
                break;
            case NoteType.Slider:
                renderSingleSliderStaticly(nt, noteTime, endTime, t);
                break;
            case NoteType.Plane:
                break;
            default:
                break;
        }
    }

    private void renderSingleNoteStaticly(Note nt, int noteTime, int t)
    {
        if (nt.type != NoteType.Note)
            return;
        float animationTime = 1f -((float)(noteTime - t)) / appTime;
        if (animationTime < 0)
            return;
        GameObject target = GameObject.Find(blockPath + nt.id.ToString());
        target.GetComponent<SelectAnime>().playPlaneRaiseStaticly(nt.dir, animationTime);
    }
    private void renderSingleSliderStaticly(Note nt, int noteTime, int endTime, int t)
    {
        if (nt.type != NoteType.Slider)
            return;
        if (t <= noteTime - appTime || t >= endTime)
        {
            return;
        }
        // 缩圈
        else
        {
            int layerID = nt.id;
            Direction dir = nt.dir;
            float animationPercent = (float)(t - noteTime) / (float)(nt.duration);
            int[] blocksBefore = getSliderIndex(dir, layerID);
            int[] blocksAfter = getSliderReplaceIndex(dir, layerID);
            BlockIndex centerIndex = getBlockIndex(blocksBefore[4]);
            Vector3 rotateDir = getSliderRotateDirection(dir);
            Vector3 centerPoint = squareBlock[centerIndex.x, centerIndex.y, centerIndex.z].block.transform.position;
            sliderAnimation.SetActive(true);
            sliderAnimation.transform.position = centerPoint;
            //sliderAnimation.GetComponent<SliderRotate>().Rotate(dir);
            int startTime = (int)(noteTime - appTime);
            //GameObject sliderAnimation = Instantiate(layer, centerPoint, new Quaternion(rotateDir))
            if (t > noteTime - appTime && t <= noteTime)
            {
                sliderAnimation.GetComponentInChildren<RowAnime>().playRaise((float)(t - startTime) / appTime);
            }
            else if (t > noteTime && t < endTime)
            {
                float pullingPercent = (t - (float)noteTime) / ((float)endTime - noteTime);
                print(pullingPercent);
                sliderAnimation.GetComponentInChildren<RowAnime>().playPulling(pullingPercent, pullingPercent, pullingPercent);
                sliderAnimation.transform.rotation = Quaternion.identity;
                sliderAnimation.transform.rotation = Quaternion.Euler(pullingPercent * rotateDir * 90f);
                List<int> BIList = new List<int>();
                //GameObject sliderAnimation = Instantiate();
                //resetBlockPosition();
                for (int i = 0; i < 9; i++)
                {
                    int beforeIndex = blocksBefore[i];
                    int afterIndex = blocksAfter[i];
                    GameObject blk = GameObject.Find(blockPath + beforeIndex.ToString());
                    // 总是旋转90度
                    blk.GetComponent<SelectAnime>().playRotate(centerPoint, rotateDir, 90f * animationPercent);
                    BIList.Add(afterIndex);
                }
            }
        }
    }

    private void clearBlockAnimation()
    {
        foreach(BlockStatus blk in squareBlock)
        {
            blk.block.GetComponent<SelectAnime>().playPlaneClearStaticly();
        }
    }

    public float GetSongLength()
    {
        return musicLength;
    }
    public void PlayBlockSelectedAnimation(int id)
    {
        squareBlock[id / 9, (id % 9) / 3, id % 3].block.GetComponent<SelectAnime>().autoPlay(CubeState.select);
    }
    public void PlayBlockIdleAnimation(int id)
    {
        squareBlock[id / 9, (id % 9) / 3, id % 3].block.GetComponent<SelectAnime>().autoPlay(CubeState.unSelect);
    }

    private List<int> getTimeRangeNoteIndexDelay (int t, int range, int delay)
    {
        List<int> ret = new List<int>();
        int lt = t - delay;
        int rt = t + range;
        if (timeStamp[0] > rt)
            return ret;
        int lowPoint = BinarySearch(timeStamp, lt);
        int highPoint = BinarySearch(timeStamp, rt);
        lowPoint += timeStamp[lowPoint] < lt ? 1 : 0;
        highPoint -= timeStamp[highPoint] > rt ? 1 : 0;
        if (lowPoint < 0)
            lowPoint = 0;
        for (int i = lowPoint; i <= highPoint; i++)
            ret.Add(i);

        return ret;
    }

    // 编辑函数
    private List<int> getTimeRangeNoteIndex(int t, int range)
    {
        List<int> ret = new List<int>();
        int rt = t + range;
        if (timeStamp[0] > rt)
            return ret;
        int lowPoint = BinarySearch(timeStamp, t);
        int highPoint = BinarySearch(timeStamp, rt);
        lowPoint += timeStamp[lowPoint] < t ? 1 : 0;
        highPoint -= timeStamp[highPoint] > rt ? 1 : 0;
        if (lowPoint < 0)
            lowPoint = 0;
        for(int i = lowPoint; i <= highPoint; i++)
            ret.Add(i);

        return ret;
    }

    private int BinarySearch(List<int> data, int a)
    {
        int low = 0;
        int high = data.Count - 1;
        int mid = -1;

        while(low <= high)
        {
            mid = (low + high) / 2;

            if (data[mid] == a)
                return mid;
            else if (data[mid] > a)
                high = mid - 1;
            else if (data[mid] < a)
                low = mid + 1;
        }
        return mid;
    }
    public int getSliderLayer(Direction dir, int blockID)
    {
        int layer = 0;
        switch(dir)
        {
            case Direction.xplus:
            case Direction.xminus:
                layer = (blockID % 9) / 3;
                break;
            case Direction.yplus:
            case Direction.yminus:
                layer = blockID /9;
                break;
            case Direction.zplus:
            case Direction.zminus:
                layer = blockID % 3;
                break;
            default:
                break;
        }
        return layer;
    }

    public void addNote(int t,int blockID, Axis a)
    {
        Note nt = new Note();
        nt.type = NoteType.Note;
        nt.id = blockID;
        Direction dirs = DirectionMap.instance.dirMap[blockID];
        Direction dir = Direction.illegal;
        switch (a)
        {
            case Axis.x:
                dir = dirs & (Direction.xplus | Direction.xminus);
                break;
            case Axis.y:
                dir = dirs & (Direction.yplus | Direction.yminus);
                break;
            case Axis.z:
                dir = dirs & (Direction.zplus | Direction.zminus);
                break;
            default:
                break;
        }
        if (dir == Direction.illegal)
            return;
        nt.dir = dir;
        add2Notes(t, nt);
    }
    public void addSlider(int t, int blockID, Direction dir, int duration)
    {
        Note nt = new Note();
        nt.type = NoteType.Slider;
        nt.id = getSliderLayer(dir,blockID);
        nt.dir = dir;
        nt.duration = duration;
        add2Notes(t, nt);
        Slider sld = new Slider();
        sld.nt = nt;
        sld.start = t;
        sld.end = t + duration;
        add2Sliders(sld);
    }
    private void add2Notes(int t, Note nt)
    {
        int index = BinarySearch(timeStamp, t);
        if (!timeStamp.Contains(t))
        {
            timeStamp.Add(t);
            timeStamp.Sort();
            List<Note> temp = new List<Note>();
            temp.Add(nt);
            notes.Add(t, temp);
        }
        else
        {
            notes[t].Add(nt);
        }
    }
    private void add2Sliders(Slider sld)
    {
        sliders.Add(sld);
        sliders.Sort();
        printSliders();
    }
    private void deleteNote(int t, Note nt)
    {
        if (!timeStamp.Contains(t))
            return;
        int index = BinarySearch(timeStamp, t);
        notes[t].Remove(nt);
        if (notes[t].Count == 0)
        {
            notes.Remove(t);
            timeStamp.Remove(t);
        } 
    }
    public void deleteNote(int t, int blockID)
    {
        if (!timeStamp.Contains(t))
            return;
        Note deleteNote = new Note();
        bool isNote = false;
        foreach (Note nt in notes[t])
        {
            if (nt.id == blockID && nt.type == NoteType.Note)
            {
                deleteNote = nt;
                isNote = true;
                break;
            }
        }
        if (isNote != true)
        {
            Slider deleteSlider = new Slider();
            foreach (Slider sld in sliders)
            {
                if (getSliderLayer(sld.nt.dir, sld.nt.id) == sld.nt.id)
                {
                    deleteNote = sld.nt;
                    deleteSlider = sld;
                    break;
                }
            }
            sliders.Remove(deleteSlider);
            sliderAnimation.SetActive(false);
        }
        notes[t].Remove(deleteNote);
        if (notes[t].Count == 0)
        {
            notes.Remove(t);
            timeStamp.Remove(t);
        }
    }
    // for debug
    private void printSliders()
    {
        foreach(Slider sld in sliders)
        {
            Debug.Log("start: " + sld.start + "/ end: " + sld.end);
        }
    }
    private void SyncNote() // 强制同步beatMap中的notes和MagiCube中保存notes的数据结构
    {
        bm.notes = notes;
    }

    public void saveBeatMap()
    {
        if (songFullPathAndName != null)
            bm.writeToFile(beatmapFullPathAndName);
    }
    public float getProgress()
    {
        return timeCount * 1000 / (float)beatMapLength;
    }
    public GameObject getBlockById(int id)
    {
        GameObject blk = GameObject.Find(blockPath + id.ToString()) as GameObject;
        return blk;
    }

    public void setDropTime(float t)
    {
        foreach(BlockStatus blk in squareBlock)
        {
            blk.block.GetComponent<SelectAnime>().setDropTime(t);
        }
    }

    public struct BlockIndex
    {
        public BlockIndex(int xx, int yy, int zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }
        public int x;
        public int y;
        public int z;
    }

    private BlockIndex getBlockIndex(int id)
    {
        BlockIndex bi = new BlockIndex(id / 9, (id % 9) / 3, id % 3);
        return bi;
    } 

    private void resetBlockPosition()
    {
        if (squareBlock == null)
            return;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++)
                {
                    squareBlock[i, j, k].block.transform.position = initialBlockPosition[i, j, k];
                    squareBlock[i, j, k].block.transform.rotation = Quaternion.identity;
                }
    }

    private float getAppTime(float ar)
    {
        int up = (int)Math.Ceiling(ar);
        int down = (int)(ar);
        return Mathf.Lerp(appTimeTable[down], appTimeTable[up], ar);
    }
    private float getJudgeRange(float od)
    {
        return 80 - 6 * od;
    }
    
    class BlockStatus
    {
        public GameObject block;
        public state status = state.inAcive;
        public Vector3 forward;
        public Vector3 originalLocation;
        public float blockSize;
        public Material activeMt;
        public Material inActiveMt;
        public enum state
        {
            inAcive,
            movingUp,
            movingDown,
            active
        }
        public void moveBack()
        {
            block.GetComponent<Renderer>().material = inActiveMt;
            block.transform.Translate(forward * Time.deltaTime * blockSize);
            status = state.inAcive;
        }
        public IEnumerator moveUp()
        {
            for (int i = 0; i < 10; i++)
            {
                block.transform.position = originalLocation + forward * i * 0.05f * blockSize;
                yield return null;
            }
            status = state.active;
            block.GetComponent<Renderer>().material = activeMt;
        }
    }

    [System.Serializable]
    public class Face
    {
        public faceType first;
        public faceType second;
        public faceType third;
        public faceType getRandomFace()
        {
            float face = UnityEngine.Random.Range(0, 3);
            if (face >= 0 && face < 1)
            {
                return first;
            }
            if (face >= 1 && face < 2)
            {
                return second;
            }
            if (face >= 2 && face < 3)
            {
                return third;
            }
            return first;
        }
        public enum faceType
        {
            yMinus,
            yPlus,
            xMinus,
            xPlus,
            zMinus,
            zPlus
        }
    }
}