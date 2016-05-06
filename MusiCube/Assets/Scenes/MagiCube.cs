using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusiCube;
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
    int[,,] blockPos = new int[3,3,3]; // 记录魔方各个位置的当前方块ID
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

    // 帮助加载音乐
    WWW www;
    public bool isDone = false;

    string songName;
    string songFullPathAndName;
    string beatmapFullPathAndName;
    public BeatMap bm;

    // Use this for initialization

    void Start()
    {
        Debug.Assert(block != null);
        music = GetComponent<AudioSource>();
        bm = new BeatMap();
        bm.readFromFile("test.txt");
        songName = "STYX HELIX";
        InitialSquare();
        InitialNote();
        StartCoroutine(LoadMusic());
    }

    // Use WWW to asynchronously load a music resource
    IEnumerator LoadMusic()
    {
        string songPath = "Songs/" + songName + "/" + songName + ".ogg";
        string beatmapPath = "Songs/" + songName + "/" + songName + ".mcb";
        songFullPathAndName = "file:///" + System.IO.Path.Combine(Application.streamingAssetsPath, songPath);
        beatmapFullPathAndName = System.IO.Path.Combine(Application.streamingAssetsPath, beatmapPath);

        www = new WWW(songFullPathAndName);
        yield return www;

        music.clip= www.GetAudioClip(true, true);
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
                        if (timeCount >= timeStamp[noteCount])
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
                    blockPos[i, j, k] = i * 9 + j * 3 + k;
                }
            }
        }
    }

    void InitialNote()
    {
        notes = bm.getNotes();
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
        PlayerPrefs.SetInt("index", 1);
        /*
        BlockStatus blockSt = squareBlock[y, z, x];
        if (blockSt.status == BlockStatus.state.active)
        {
            blockSt.status = BlockStatus.state.movingDown;
            StartCoroutine(blockSt.moveBack());
        }
        */
    }

    public void renderNote(Note nt)
    {
        switch (nt.type)
        {
            case NoteType.Note:
                {
                    BlockStatus target;
                    BlockIndex bi = ID2Index(nt.id);
                    target = squareBlock[bi.x, bi.y, bi.z];
                    renderSingleNote(target, nt);
                    break;
                }
            case NoteType.Slider:
                {
                    break;
                }
            case NoteType.Plane:
                {
                    break;
                }
        }
    }

    private void renderSingleNote(BlockStatus target, Note nt)
    {
        if (nt.type != NoteType.Note)
            return;
        switch (nt.dir)
        {
            case Direction.xminus:
                {
                    if (target.status == BlockStatus.state.inAcive)
                    {
                        target.forward = -target.block.transform.right; ;
                        StartCoroutine(target.moveUp());
                    }
                    break;
                }
            case Direction.xplus:
                {
                    if (target.status == BlockStatus.state.inAcive)
                    {
                        target.forward = -block.transform.right; ;
                        StartCoroutine(target.moveUp());
                    }
                    break;
                }
            case Direction.yminus:
                {
                    if (target.status == BlockStatus.state.inAcive)
                    {
                        target.forward = -block.transform.up;
                        StartCoroutine(target.moveUp());
                    }
                    break;
                }
            case Direction.yplus:
                {
                    if (target.status == BlockStatus.state.inAcive)
                    {
                        target.forward = block.transform.up;
                        StartCoroutine(target.moveUp());
                    }
                    break;
                }
            case Direction.zminus:
                {
                    if (target.status == BlockStatus.state.inAcive)
                    {
                        target.forward = -block.transform.forward;
                        StartCoroutine(target.moveUp());
                    }
                    break;
                }
            case Direction.zplus:
                {
                    if (target.status == BlockStatus.state.inAcive)
                    {
                        target.forward = block.transform.forward;
                        StartCoroutine(target.moveUp());
                    }
                    break;
                }
            default:
                break;
        }
    }
    
    private void renderSingleSlider(BlockStatus target, Note nt)
    {
        if (nt.type != NoteType.Slider)
            return;
    }
    private void renderSinglePlane(BlockStatus target, Note nt)
    {
        if (nt.type != NoteType.Plane)
            return;
    }

    public float GetSongLength()
    {
        return musicLength;
    }
    public void PlayBlockSelectedAnimation(int id)
    {
        squareBlock[id / 9, (id % 9) / 3, id % 3].block.GetComponent<Animator>().Play("Selected", -1, 0f);
    }
    public void PlayBlockIdleAnimation(int id)
    {
        squareBlock[id / 9, (id % 9) / 3, id % 3].block.GetComponent<Animator>().Play("Cancel", -1, 0f);
    }

    public void saveBeatMap()
    {
        if (songFullPathAndName != null)
            bm.writeToFile(beatmapFullPathAndName);
    }

    public struct BlockIndex
    {
        public int x;
        public int y;
        public int z;
    }


    //太影响效率了吧，用3进制把每一位取出来就好了，还有blockpos这个数组有必要吗
    private BlockIndex ID2Index(int id)
    {
        BlockIndex ret = new BlockIndex();
        for (int i = 0; i < 3; i++)
        { 
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (blockPos[i, j, k] == id)
                    {
                        ret.x = i;
                        ret.y = j;
                        ret.z = k;
                        return ret;
                    }
                }
            }
        }
        return ret;
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