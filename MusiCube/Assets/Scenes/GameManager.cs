using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusiCube;

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

    private float feedbackAnimaLength = 0.4f;

	// Use this for initialization
	void Start () {
        mc = GetComponentInChildren<MagiCube>();
        mc.state = MagiCube.GameState.Play;
        currentHits = new List<Hit>();
	}
	
	// Update is called once per frame
	void Update () {
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

        // test
        if(Input.GetKey(KeyCode.Z))
        {
            //print(currTime);
        }

        // 在判定范围内
        if (currTime >= currentNoteTime - gradeNum * mc.judgeRange && currTime <= currentNoteTime + gradeNum * mc.judgeRange)
        {
            foreach(Hit ht in currentHits)
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
                print(currentNoteTime +"/" +currTime);
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
        foreach(Hit ht in currentHits)
        {
            if (ht.status == hitStatus.dead)
                continue;
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
            if(ht.status == hitStatus.feedbacking)
            {
                if (currTime > ht.hitTime + (int)(1000 * feedbackAnimaLength))
                {
                    ht.status = hitStatus.dead;
                    print(ht.nt.id);
                    print(ht.status);
                    print(ht.grade);
                    print(ht.noteTime + "/" + ht.hitTime);
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
                GameObject blk = mc.getBlockById(ht.nt.id) as GameObject;
                blk.GetComponent<SelectAnime>().feedback = true;
                ht.anim = pt;
            }
            // 为在feedback阶段的note更新播放动画
            if(ht.status == hitStatus.feedbacking)
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
        foreach(Hit ht in currentHits)
        {
            //if (ht.status == hitStatus.dead)
               // currentHits.Remove(ht);
        }
        // 添加新的Hit
        foreach(Note nt in mc.notes[time])
        {
            Hit ht = new Hit();
            ht.nt = nt;
            ht.noteTime = time;
            ht.hitTime = 0;
            ht.status = hitStatus.incoming;
            ht.grade = hitGrade.none;
            ht.anim = PlAnimeType.none;
            if(!currentHits.Contains(ht))
                currentHits.Add(ht);
        }
    }

    // for Debug
    void printCurrentHits()
    {
        foreach(Hit ht in currentHits)
        {
            Debug.Log(ht.noteTime + " " + ht.nt.type.ToString() + " " + ht.nt.id.ToString() + " " + ht.status);
        }
    }
}
