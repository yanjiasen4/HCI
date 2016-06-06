using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using MusiCube;
using UnityEngine.SceneManagement;

namespace ChooseSongUI
{
    public class scrolltest : MonoBehaviour
    {
        // the intrinsic things on UI canvas
        public RectTransform content;
        public Button b;
        public RawImage ri;
        public Canvas c;
        public Button difficulty;
        public Text songName;

        // using for the songs UI
        // the song on the front which is chosen
        private Song now;
        // the difficulty for the current song
        private int nowDiff = 0;
        // song List
        private List<Song> sl;
        // the song and the songimage pair
        private Dictionary<Song, Button> songButton;

        // read the background picture of the song
        IEnumerator readSongTexture()
        {
            foreach (KeyValuePair<Song, Button> song in songButton)
            {
                RawImage songri = song.Value.GetComponentInChildren<RawImage>();
                // the filePath of the background picture
                string filePath = "file://" + song.Key.songPrefixPath + "/" + song.Key.backgroundFileName;
                //Debug.Log(filePath);
                WWW www = new WWW(filePath);
                yield return www;
                songri.texture = www.texture;
            }
        }

        // creat the button for each song
        void addSongButton()
        {
            // init and get the list of the song
            int i = 0;
            foreach (Song song in sl)
            {
                // new and set a button and image for the song
                Button tmp = Instantiate(b);
                // set the location and parent
                tmp.GetComponent<RectTransform>().localPosition = new Vector3(20 * i + c.GetComponent<RectTransform>().sizeDelta.x / 2, c.GetComponent<RectTransform>().sizeDelta.y / 2, 0);
                tmp.transform.SetParent(content.transform);
                //tmp.transform.parent = content.transform;
                RawImage tmpri = tmp.GetComponentInChildren<RawImage>();
                tmpri.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, -1);
                songButton.Add(song, tmp);

                i += 1;
                //Debug.Log(song.diffCount);
                //Debug.Log(song.diffs.Count);
                //Debug.Log(song.songName);
                //Debug.Log(song.audioFileName);
                //Debug.Log(song.backgroundFileName);
            }
            Destroy(b);
            Destroy(ri);
        }

        // Use this for initialization
        void Start()
        {
            // init the songlist and get it
            sl = SongList.instance.songList;
            //Debug.Log(sl.Count);
            songButton = new Dictionary<Song, Button>();
            content.sizeDelta = new Vector2(20 * sl.Count, 0);
            b.GetComponent<RectTransform>().position = new Vector2(0, 0);

            addSongButton();

            StartCoroutine(readSongTexture());
        }

        // Update is called once per frame
        void Update()
        {
            DisplayChange();
            DisplaySongName();
            DisplayDifficulty();

            if (Input.GetKey(KeyCode.Return))
            {
                SceneManager.LoadScene(1);
            }
        }

        void DisplayDifficulty()
        {
            //Debug.Log(now.diffCount);
            //Debug.Log(now.diffs.Count);
            if (now.diffs.Count == 0) return;
            difficulty.GetComponentInChildren<Text>().text = now.diffs[nowDiff].difficultyName;
        }

        void DisplaySongName()
        {
            songName.text = now.songName;
        }

        void DisplayChange()
        {
            float right = c.GetComponent<RectTransform>().sizeDelta.x;

            float lastlength = -1;
            Song last = now;

            foreach (KeyValuePair<Song, Button> song in songButton)
            {
                Button songb = song.Value;
                RawImage songri = songb.GetComponentInChildren<RawImage>();
                float length = (c.GetComponent<RectTransform>().sizeDelta.x/2 - Mathf.Abs(songb.GetComponent<RectTransform>().position.x - right / 2));
                float height = length / 16 * 9;

                if (length >= lastlength)
                {
                    now = song.Key;
                    songb.transform.SetAsLastSibling();
                }
                else
                {
                    songb.transform.SetAsFirstSibling();
                }
                lastlength = length;
                songri.GetComponent<RectTransform>().sizeDelta = new Vector2(length, height);
                songb.GetComponent<RectTransform>().sizeDelta = new Vector2(length, height);
            }

            if (last != now)
            {
                nowDiff = 0;
                PlayerPrefs.SetInt("nowDiff", nowDiff);
            }
            else
            {
                nowDiff = PlayerPrefs.GetInt("nowDiff");
                //Debug.Log(now.diffCount);
                if (nowDiff>=now.diffCount)
                {
                    nowDiff = 0;
                    PlayerPrefs.SetInt("nowDiff", nowDiff);
                }
            }
        }
    }
}
