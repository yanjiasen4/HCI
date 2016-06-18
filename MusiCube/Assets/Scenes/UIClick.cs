using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIClick : MonoBehaviour {

    public Camera c;
    public Image img;
    float totalTime = 2;

	// Use this for initialization
	void Start () {
        PlayerPrefs.SetInt("Clicked", 0);
        PlayerPrefs.SetFloat("ClickedTime", 0);
    }
	
	// Update is called once per frame
	void Update () {

        Vector3  posThumb = GameObject.Find("thumb/bone3").GetComponent<Transform>().position;

        Vector3  posIndex = GameObject.Find("index/bone3").GetComponent<Transform>().position;

        double distance = System.Math.Pow(
                System.Math.Pow((posIndex.x - posThumb.x),2)+
                System.Math.Pow((posIndex.y - posThumb.y), 2) +
                System.Math.Pow((posIndex.z - posThumb.z), 2)
                ,0.5);
        //Debug.Log(distance);
        if (distance < 0.6)
        {
            //Debug.Log("distance");
            //Debug.Log(PlayerPrefs.GetFloat("ClickedTime"));
            PlayerPrefs.SetInt("Clicked", 1);
            PlayerPrefs.SetFloat("ClickedTime", PlayerPrefs.GetFloat("ClickedTime") + Time.deltaTime);
            playCircle(PlayerPrefs.GetFloat("ClickedTime") / totalTime);
        }
        else
        {
            PlayerPrefs.SetInt("Clicked", 0);
            PlayerPrefs.SetFloat("ClickedTime", 0);
            Debug.Log(CircleSource.instance.circleSequence.Length);
            img.sprite = CircleSource.instance.circleSequence[0];
        }

        //Vector3 GUIPos = new Vector3(ScreenPos.x, ScreenPos.y, 0);
        //t2.position = GUIPos;
    }

    /*IEnumerator playCoroutine()
    {
        float curTime = 0;
        while (curTime < totalTime)
        {
            playCircle(curTime / totalTime);
            curTime += Time.deltaTime;
            yield return null;
        }
    }*/
    public void playCircle(float time)
    {
        Sprite[] circleSeq = CircleSource.instance.circleSequence;
        int idx = (int)(time * circleSeq.Length) % circleSeq.Length;
        img.sprite = circleSeq[idx];
    }

}
