using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using MusiCube;

namespace ChooseSongUI
{
    public class scrolltest : MonoBehaviour
    {
        // the intrinsic things on UI canvas
        public Camera camera;
        public RectTransform content;
        public Button b;
        public RawImage ri;
        public Canvas c;
        //public RectTransform d;
        public Button difficulty;
        public Text songName;
        public Transform scroll;

        public bool fixFlag = false;
        public double fixStartPosition;
        public AnimationCurve positionCurve;

        private int deltaWidth = 100;

        // using for the songs UI
        // the song on the front which is chosen
        private Song now;
        private int nowi;
        // the difficulty for the current song
        private int nowDiff = 0;
        // song List
        private List<Song> sl;
        // the song and the songimage pair
        private List<KeyValuePair<Song, Button>> songButton;

        // read the background picture of the song
        IEnumerator readSongTexture()
        {
            foreach (KeyValuePair<Song, Button> song in songButton)
            {
                Debug.Log(song.Key.backgroundFileName);
                RawImage songri = song.Value.GetComponentInChildren<RawImage>();
                // the filePath of the background picture
                string filePath = "file://" + song.Key.songPrefixPath + "/" + song.Key.backgroundFileName;
                Debug.Log(filePath);
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
                tmp.GetComponent<RectTransform>().localPosition = new Vector3(deltaWidth * i + c.GetComponent<RectTransform>().sizeDelta.x / 2, 0, 0);
                //tmp.GetComponent<RectTransform>().localPosition = new Vector3(20 * i,, 0);
                tmp.transform.SetParent(content.transform);
                //tmp.transform.parent = content.transform;
                RawImage tmpri = tmp.GetComponentInChildren<RawImage>();
                tmpri.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, -1);
                songButton.Add(new KeyValuePair<Song,Button>(song, tmp));

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
            songButton = new List<KeyValuePair<Song, Button>>();
            content.sizeDelta = new Vector2(20 * sl.Count, 0);
            content.position = new Vector2(0,0);
            b.GetComponent<RectTransform>().position = new Vector2(0, 0);

            addSongButton();

            StartCoroutine(readSongTexture());

            Debug.Log(scroll.position);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //Debug.Log(fixFlag);
            if (fixFlag)
                Constrain();
            DisplayChange();
            DisplaySongName();
            DisplayDifficulty();
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
            Vector3 ScreenPos = camera.WorldToScreenPoint(scroll.position);
            Vector3 GUIPos = new Vector3(ScreenPos.x, Screen.height - ScreenPos.y, 0);

            content.localPosition = new Vector3(GUIPos.x- Screen.width , 0, 0);
            float right = c.GetComponent<RectTransform>().sizeDelta.x;

            float lastlength = -1;
            Song last = now;
            int i = 0;

            foreach (KeyValuePair<Song, Button> song in songButton)
            {
                Button songb = song.Value;
                RawImage songri = songb.GetComponentInChildren<RawImage>();
                float length = (c.GetComponent<RectTransform>().sizeDelta.x/2 - Mathf.Abs(songb.GetComponent<RectTransform>().position.x - right / 2));
                float height = length / 16 * 9;

                if (length >= lastlength)
                {
                    if (!fixFlag)
                    {
                        now = song.Key;
                        nowi = i;
                    }
                    songb.transform.SetAsLastSibling();
                }
                else
                {
                    songb.transform.SetAsFirstSibling();
                }
                lastlength = length;
                songri.GetComponent<RectTransform>().sizeDelta = new Vector2(length, height);
                songb.GetComponent<RectTransform>().sizeDelta = new Vector2(length, height);
                i++;
            }

            //Debug.Log(last == now);

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

        private void Constrain()
        {
            if((fixStartPosition>Screen.width/2 && songButton[nowi].Value.transform.position.x<Screen.width/2)
                || (fixStartPosition < Screen.width / 2 && songButton[nowi].Value.transform.position.x > Screen.width / 2))
            {
                fixFlag = false;
                return;
            }

            float scaleV = (float)(System.Math.Abs(fixStartPosition - Screen.width / 2)/(Screen.width/2));

            double scalePos = System.Math.Abs((songButton[nowi].Value.transform.position.x-Screen.width/2) / (fixStartPosition-Screen.width/2));

            if (songButton[nowi].Value.transform.position.x > Screen.width / 2 )
                scroll.position = new Vector3(scroll.position.x - positionCurve.Evaluate((float)scalePos)*scaleV, scroll.position.y, scroll.position.z);
            else if(songButton[nowi].Value.transform.position.x < Screen.width / 2)
                scroll.position = new Vector3(scroll.position.x + positionCurve.Evaluate((float)scalePos)*scaleV, scroll.position.y, scroll.position.z);
            else fixFlag = false;

            Vector3 ScreenPos = camera.WorldToScreenPoint(scroll.position);
            Vector3 GUIPos = new Vector3(ScreenPos.x, Screen.height - ScreenPos.y, 0);

            content.localPosition = new Vector3(GUIPos.x - Screen.width, 0, 0);
        }

        public void SliderRelasedTrigger()
        {
            fixFlag = true;
            fixStartPosition = (songButton[nowi].Value).transform.position.x;
        }

        public void SliderPressedTrigger()
        {
            fixFlag = false;
        }

        public void LeftButtonTrigger()
        {
            //Debug.Log(nowi);
            nowi--;
            if (nowi == -1) nowi++;
            now = songButton[nowi].Key;
            SliderRelasedTrigger();
        }

        public void RightButtonTrigger()
        {
            //Debug.Log(nowi);
            nowi++;
            if (nowi == songButton.Count) nowi--;
            now = songButton[nowi].Key;
            SliderRelasedTrigger();
        }

        public int getSongDiffNumber()
        {
            return now.diffCount; 
        }
    }
}
