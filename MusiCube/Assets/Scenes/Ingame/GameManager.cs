using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using MusiCube;
using UnityEngine.UI;
using ProgressBar;

[RequireComponent(typeof(GameManager))]
public class GameManager : MonoBehaviour {

    public MagiCube mc;

    /* Hit: 每一个note都在经过（或没有）一次击打后播放相应反馈动画然后被消除
     * 生命周期
     * TimeLine:  -----------> 1 ------------> 2/3  -------------------------> 4 ------------>
     * hitStatus:   incoming   |    hitable    |  hited/miss   |  feedbacking  |     dead
     * 1:   noteTime - judgeRange
     * 2/3: noteTime + judgeRange
     * 4:   hitTime + feedbackAnimaLength
     */
    public enum hitStatus
    {
        incoming, hitable, hited, missed, feedbacking, dead
    }

    public class Hit
    {
        public int noteTime; // note应该被击打的时间
        public int hitTime; // 玩家击打的时间
        public Note nt;
        public hitStatus status;
        public hitGrade grade;
        public PlAnimeType anim;
    }

    public enum hitGrade
    {
        none, miss, normal, good, perfect
    }
    private int gradeNum = 4;

    public List<Hit> currentHits;
    public int currentNoteTime;
    public int hitTimeCount = 0;

    private float feedbackAnimaLength = 0.275f;

    #region
    private int perfectCount = 0;
    private int goodCount = 0;
    private int normalCount = 0;
    private int missCount = 0;

    private float score = 0f;    // 得分数
    private int combo = 0;       // 连击数
    private int maxCombo = 0;    // 最大连击数
    private float acc = 0f;      // 准确度
    private float progress = 0f; // 游玩的进度
    #endregion

    #region
    private float perfectScore = 300f;
    private float goodScore = 150f;
    private float normalScore = 50f;
    #endregion

    #region  
    public Text scoreText;
    public Text comboText;
    public GameObject progressBar;
    #endregion

    private GameObject Hands;

    // Use this for initialization
    void Start() {
        mc = GetComponentInChildren<MagiCube>();
        mc.state = MagiCube.GameState.Play;
        currentHits = new List<Hit>();
    }

    // Update is called once per frame
    void Update() {
        
        setScoreText();
        setComboText();
        setProgress();
        // end of beatmap
        //if (hitTimeCount >= mc.timeStamp.Count)
        //    return;
        // 更新下一次Hit
        if (hitTimeCount == mc.noteCount)
        {
            updateCurrentHits();
            hitTimeCount++;
            //  printCurrentHits();
        }

        int currTime = (int)(mc.GetTime() * 1000);
        if(currTime >= mc.beatMapLength + 1000f)
        {
            switchToScore();
        }

        // 在判定范围内
        if (currTime >= currentNoteTime - gradeNum * mc.judgeRange && currTime <= currentNoteTime + gradeNum * mc.judgeRange)
        {
            foreach (Hit ht in currentHits)
            {
                if (ht.status == hitStatus.incoming)
                {
                    //print(ht.nt.id);
                    ht.status = hitStatus.hitable;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.X))
            {
                int dt = Mathf.Abs(currTime - currentNoteTime);
                print(currentNoteTime + "/" + currTime);
                hitGrade hg = hitGrade.miss;
                if (dt <= mc.judgeRange)
                    hg = hitGrade.perfect;
                else if (dt > mc.judgeRange && dt < 2 * mc.judgeRange)
                    hg = hitGrade.good;
                else if (dt > 2 * mc.judgeRange && dt <= 3 * mc.judgeRange)
                    hg = hitGrade.normal;
                else
                    hg = hitGrade.miss;

                print(hg);

                foreach (Hit ht in currentHits)
                {
                    if (ht.status == hitStatus.hitable)
                    {
                        ht.status = hitStatus.hited;
                        ht.grade = hg;
                        ht.hitTime = currTime;
                    }
                }
            }
            
        }

        // 按照每个Hit的状态执行
        foreach (Hit ht in currentHits)
        {
            if (ht.status == hitStatus.dead)
            {
                if(currTime > ht.noteTime + 10 * mc.judgeRange)
                {
                    GameObject blk = mc.getBlockById(ht.nt.id) as GameObject;
                    blk.GetComponent<SelectAnime>().feedback = false;
                }
            }
            // 检测是否有未击打而漏过的note
            if (ht.status == hitStatus.hitable)
            {
                //print(ht.nt.id);
                if (currTime > ht.noteTime + 3 * mc.judgeRange)
                {
                    ht.status = hitStatus.missed;
                    ht.grade = hitGrade.miss;
                    ht.hitTime = currTime;
                    // print(ht.nt.id);
                }
            }
            // 更新播放完feedback动画的note
            if (ht.status == hitStatus.feedbacking)
            {
                if (currTime > ht.hitTime + (int)(1000 * feedbackAnimaLength))
                {
                    ht.status = hitStatus.dead;
                }
            }
            // 为击打过或漏过的note设定feedback动画
            if (ht.status == hitStatus.hited || ht.status == hitStatus.missed)
            {
                PlAnimeType pt = PlAnimeType.none;
                switch (ht.grade)
                {
                    case hitGrade.perfect:
                        pt = PlAnimeType.perfect;
                        break;
                    case hitGrade.good:
                        pt = PlAnimeType.good;
                        break;
                    case hitGrade.normal:
                        pt = PlAnimeType.normal;
                        break;
                    case hitGrade.miss:
                        pt = PlAnimeType.fail;
                        break;
                    default:
                        break;
                }
                ht.status = hitStatus.feedbacking;
                ht.anim = pt;
            }
            // 为在feedback阶段的note更新播放动画
            if (ht.status == hitStatus.feedbacking)
            {
                GameObject blk = mc.getBlockById(ht.nt.id) as GameObject;
                float animationPercent = ((float)(currTime - ht.hitTime)) / (1000f * feedbackAnimaLength);
                blk.GetComponent<SelectAnime>().playPlaneFeedback(ht.nt.dir, ht.anim, animationPercent);
            }

        }
        
        //printCurrentHits();
    }

    void updateCurrentHits()
    {
        int time = mc.timeStamp[mc.noteCount];
        currentNoteTime = time;
        // 删除没用的Hit
        foreach (Hit ht in currentHits)
        {
            //if (ht.status == hitStatus.dead)
            // currentHits.Remove(ht);
        }
        // 添加新的Hit
        foreach (Note nt in mc.notes[time])
        {
            Hit ht = new Hit();
            ht.nt = nt;
            ht.noteTime = time;
            ht.hitTime = 0;
            ht.status = hitStatus.incoming;
            ht.grade = hitGrade.none;
            ht.anim = PlAnimeType.none;
            if (!currentHits.Contains(ht))
                currentHits.Add(ht);
        }
    }

    // for Debug
    void printCurrentHits()
    {
        foreach (Hit ht in currentHits)
        {
            Debug.Log(ht.noteTime + " " + ht.nt.type.ToString() + " " + ht.nt.id.ToString() + " " + ht.status);
        }
    }

    internal void ClickSquare(int xIdx, int yIdx, int zIdx)
    {
        foreach (Hit ht in currentHits)
        {
            if (ht.status == hitStatus.hitable)
            {
                if (ht.nt.id == xIdx + zIdx * 3 + yIdx * 9)
                {
                    GameObject blk = mc.getBlockById(ht.nt.id) as GameObject;
                    blk.GetComponent<SelectAnime>().feedback = true;
                    int currTime = (int)(mc.GetTime() * 1000);
                    int dt = Mathf.Abs(currTime - currentNoteTime);
                    //print(currentNoteTime + "/" + currTime);
                    hitGrade hg = hitGrade.miss;
                    float bouns = getComboBonusPara(combo);
                    if (dt <= mc.judgeRange)
                    {
                        hg = hitGrade.perfect;
                        perfectCount++;
                        score += perfectScore * bouns;
                        combo++;
                    }
                    else if (dt > mc.judgeRange && dt < 2 * mc.judgeRange)
                    {
                        hg = hitGrade.good;
                        goodCount++;
                        score += goodScore * bouns;
                        combo++;
                    }
                    else if (dt > 2 * mc.judgeRange && dt <= 3 * mc.judgeRange)
                    {
                        hg = hitGrade.normal;
                        normalCount++;
                        score += normalScore * bouns;
                        combo++;
                    }
                    else
                    {
                        hg = hitGrade.miss;
                        missCount++;
                        combo = 0;
                    }
                    acc = (3 * perfectCount + 2 * goodCount + normalCount) / (3 * ((float)perfectCount + goodCount + normalCount + missCount));
                    maxCombo = combo > maxCombo ? combo : maxCombo; // 更新最大连击数
                    ht.status = hitStatus.hited;
                    ht.hitTime = currTime;
                    ht.grade = hg;
                }
            }
        }
    }

    private float getComboBonusPara(int cb)
    {
        return 1f + 0.1f * ((float)cb / 100f);
    }

    void setScoreText()
    {
        scoreText.text = "Score: " + ((int)score).ToString();
    } 
    void setComboText()
    {
        comboText.text = "Combo: " + combo.ToString();
    }
    void setProgress()
    {
        progressBar.GetComponent<ProgressBarBehaviour>().Value = 100 * mc.getProgress();
    }

    void switchToScore()
    {
        PlayerPrefs.SetFloat("score", score);
        PlayerPrefs.SetInt("perfectCount", perfectCount);
        PlayerPrefs.SetInt("goodCount", goodCount);
        PlayerPrefs.SetInt("normalCount", normalCount);
        PlayerPrefs.SetInt("missCount", missCount);
        PlayerPrefs.SetInt("maxCombo", maxCombo);
        PlayerPrefs.SetFloat("acc", acc);
        SceneManager.LoadScene(4);
    }
}
