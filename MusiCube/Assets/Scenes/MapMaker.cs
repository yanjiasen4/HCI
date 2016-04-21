using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using MusiCube;

public class MapMaker : MonoBehaviour {

    public Slider tl;
    public Text time;
    public MagiCube mc;

    //bool isPlaying = false;

	// Use this for initialization
	void Start () {
       // CreateSong();
        mc = GetComponentInChildren<MagiCube>();
        mc.state = MagiCube.GameState.Edit;
        mc.music.Pause();
        tl.minValue = 0;
        tl.maxValue = mc.GetSongLength();
	}
	
	// Update is called once per frame
	void Update () {
        SetUITime();
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

    private void SetUITime()
    {
        float t = mc.GetTime();
        tl.value = t;
        int minute = (int)(t) / 60;
        int second = (int)(t) % 60;
        int millis = (int)(1000 * (t - (int)t));
        time.text = minute.ToString() + ":" + second.ToString() + ":" + millis.ToString();
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
        mc.SetTime(t);
        // 如果滑条被拖动，设置音乐播放的进度
        if (t > currTime + Time.deltaTime || t < currTime - Time.deltaTime)
            mc.music.time = t;
    }
}
