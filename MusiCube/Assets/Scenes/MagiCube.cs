using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusiCube;

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
    float timeCount = 0;
    int noteCount = 0;
    SortedDictionary<double, List<Note>> notes = new SortedDictionary<double, List<Note>>();
    List<float> timeStamp = new List<float>();
    List<Note> currentNotes = new List<Note>();
    List<Note> nextNotes = new List<Note>();
    bool isOver = false;
    GameState state;

    public BeatMap bm;
    // Use this for initialization
    void Start()
    {
        Debug.Assert(block != null);
        bm = new BeatMap();
        bm.readFromFile("test.txt");
        InitialSquare();
        InitialNote();
        Debug.Log(notes[timeStamp[9]]);
        Debug.Log(timeStamp[9]);
    }

    public int getRandomInt(int min, int max)
    {
        float num = Random.Range((float)min, (float)max + 1.0f);
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
                            Debug.Log(noteCount);
                            nextNotes = notes[timeStamp[noteCount]];
                        }
                    }
                    break;
                }
            case GameState.Edit:
                {
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
        foreach(float key in notes.Keys)
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

    public struct BlockIndex
    {
        public int x;
        public int y;
        public int z;
    }

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
            float face = Random.Range(0, 3);
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