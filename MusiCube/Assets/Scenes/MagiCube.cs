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
    public float blockSize = 0.05f;
    public Material activeMt;
    public Material inActiveMt;
    public float timeSlice = 0.5f;
    public Face threeFace;
    BlockStatus[,,] squareBlock = new BlockStatus[3, 3, 3];
    public float timeCount = 0;
    int noteCount = 0;
    SortedDictionary<int, List<Note>> notes = new SortedDictionary<int, List<Note>>();
    List<int> timeStamp = new List<int>();
    List<Note> currentNotes = new List<Note>();
    List<Note> nextNotes = new List<Note>();
    bool isOver = false;
    public GameState state;
    public bool isPaused = false;
    // 音乐控件
    public AudioSource music;
    float musicLength; // ms

    public Quaternion blockRotate;
    // 帮助加载音乐
    WWW www;
    public bool isDone = false;

    string songName;
    string songFullPathAndName;
    string beatmapFullPathAndName;
    string blockPath = "MagiCube/";
    public BeatMap bm;

    public float appTime; // 缩圈时间/s
    public float judgeRange; // 判定范围/s

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
        appTime = getAppTime(bm.ar)/1000;
        judgeRange = getJudgeRange(bm.od)/1000;
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

        music.clip = AudioLoader.FromMp3Data(www.bytes);
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
        switch(state)
        {
            // 正常播放，可以从任意时间戳开始
            case GameState.Play:
                {
                    // update time
                    timeCount += Time.deltaTime;

                    // Game over
                    if (noteCount >= timeStamp.Count)
                    {
                        return;
                    }
                    // render notes
                    else
                    {
                        int timeMs = (int)(timeCount * 1000);
                        if (timeMs >= timeStamp[noteCount]+bm.GetOffset() - 400)
                        {
                            foreach (Note nt in currentNotes)
                            {
                                renderNote(nt);
                            }
                            // update notes
                            currentNotes = nextNotes;
                            if (++noteCount >= timeStamp.Count)
                                return;
                            nextNotes = notes[timeStamp[noteCount]];
                        }
                    }
                    break;
                }
            // 编辑模式，画面静止
            case GameState.Edit:
                {
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

        /*
        timeCount += Time.deltaTime;
        if (timeCount > timeSlice)
        {
            Face.faceType face = threeFace.getRandomFace();
            int i = getRandomInt(0, 2);
            int j = getRandomInt(0, 2);
            BlockStatus target;
            switch (face)
            {
                case Face.faceType.yPlus:
                    target = squareBlock[2, i, j];
                    if (target.status == BlockStatus.state.inAcive)
                    {
                        target.forward = target.block.transform.up;
                        StartCoroutine(target.moveUp());
                    }
                    break;
                case Face.faceType.xPlus:
                    target = squareBlock[i, j, 2];
                    if (target.status == BlockStatus.state.inAcive)
                    {
                        target.forward = target.block.transform.right;
                        StartCoroutine(target.moveUp());
                    }
                    break;
                case Face.faceType.yMinus:
                    target = squareBlock[0, i, j];
                    if (target.status == BlockStatus.state.inAcive)
                    {
                        target.forward = -target.block.transform.up;
                        StartCoroutine(target.moveUp());
                    }
                    break;
                case Face.faceType.xMinus:
                    target = squareBlock[i, j, 0];
                    if (target.status == BlockStatus.state.inAcive)
                    {
                        target.forward = -target.block.transform.right;
                        StartCoroutine(target.moveUp());
                    }
                    break;
                case Face.faceType.zMinus:
                    target = squareBlock[i, 0, j];
                    if (target.status == BlockStatus.state.inAcive)
                    {
                        target.forward = -target.block.transform.forward;
                        StartCoroutine(target.moveUp());
                    }
                    break;
            }

            timeCount = 0;
        }
        */
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
    }

    public void ClickSquare(int x, int y, int z)
    {
       // PlayerPrefs.SetInt("index", 1);
        /*
        BlockStatus blockSt = squareBlock[y, z, x];
        if (blockSt.status == BlockStatus.state.active)
        {
            blockSt.status = BlockStatus.state.movingDown;
            StartCoroutine(blockSt.moveBack());
        }
        */
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
                    //BlockStatus target;
                   // BlockIndex bi = getBlockIndex(nt.id);
                   // target = squareBlock[bi.x, bi.y, bi.z];
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
        int[] blocksAfter = getSliderReplaceIndex(dir, layerID);
        BlockIndex centerIndex = getBlockIndex(blocksBefore[4]);
        Vector3 rotateDir = getSliderRotateDirection(dir);
        Vector3 centerPoint = squareBlock[centerIndex.x,centerIndex.y,centerIndex.z].block.transform.position;
        List<int> BIList = new List<int>();
        //GameObject sliderAnimation = Instantiate();
        for(int i = 0; i < 9; i++)
        {
            int beforeIndex = blocksBefore[i];
            int afterIndex = blocksAfter[i];
            BlockIndex bi = getBlockIndex(beforeIndex);
            GameObject blk = GameObject.Find(blockPath + beforeIndex.ToString());
            // 总是旋转90度
            blk.GetComponent<SelectAnime>().autoPlayRotate(centerPoint, rotateDir, 90f, duration, appTime);
           // squareBlock[bi.x, bi.y, bi.z].block.GetComponent<SelectAnime>().autoPlayRotate(centerPoint,rotateDir , 90f, 1f);
            BIList.Add(afterIndex);
        }
        // Update blockPos
        
        
        for(int i = 0; i < 9; i++)
        {
            int beforeIndex = blocksBefore[i];
            BlockIndex bi = getBlockIndex(beforeIndex);
            squareBlock[bi.x, bi.y, bi.z].block.name = BIList[i].ToString();
        }
    }

    private Vector3 getSliderRotateDirection(Direction dir)
    {
        Vector3 ret;
        switch(dir)
        {
            case Direction.xplus:
                ret = Vector3.forward + Vector3.left;
                ret.Normalize();
                break;
            case Direction.xminus:
                ret = Vector3.back + Vector3.right;
                ret.Normalize();
                break;
            case Direction.yplus:
                ret = Vector3.up;
                break;
            case Direction.yminus:
                ret = Vector3.down;
                break;
            case Direction.zplus:
                ret = Vector3.forward + Vector3.right;
                ret.Normalize();
                break;
            case Direction.zminus:
                ret = Vector3.back + Vector3.left;
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

    public void saveBeatMap()
    {
        if (songFullPathAndName != null)
            bm.writeToFile(beatmapFullPathAndName);
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