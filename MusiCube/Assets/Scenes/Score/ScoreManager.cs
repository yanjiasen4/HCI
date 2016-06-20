using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    public Text scoreText;
    public Text perfectText;
    public Text goodText;
    public Text normalText;
    public Text missText;
    public Text comboText;
    public Text accText;
    public Image rankImage;

    #region
    public Sprite rankSS;
    public Sprite rankS;
    public Sprite rankA;
    public Sprite rankB;
    public Sprite rankC;
    public Sprite rankD;
    #endregion

    private float acc = 0f;

    // Use this for initialization
    void Start () {
        scoreText.text = "Score:" + ((int)PlayerPrefs.GetFloat("score")).ToString();
        perfectText.text = PlayerPrefs.GetInt("perfectCount").ToString();
        goodText.text = PlayerPrefs.GetInt("goodCount").ToString();
        normalText.text = PlayerPrefs.GetInt("normalCount").ToString();
        missText.text = PlayerPrefs.GetInt("missCount").ToString();
        comboText.text = PlayerPrefs.GetInt("maxCombo").ToString();
        acc = 100 * PlayerPrefs.GetFloat("acc");
        accText.text = acc.ToString("f2") + "%";

        if(acc < 60)
        {
            rankImage.sprite = rankD;
        }
        else if(acc >= 60 && acc < 75)
        {
            rankImage.sprite = rankC;
        }
        else if(acc >= 75 && acc < 90)
        {
            rankImage.sprite = rankB;
        }
        else if(acc >= 90 && acc < 95)
        {
            rankImage.sprite = rankA;
        }
        else if(acc >= 95 && acc < 100)
        {
            rankImage.sprite = rankS;
        }
        else if(acc == 100f)
        {
            rankImage.sprite = rankSS;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
