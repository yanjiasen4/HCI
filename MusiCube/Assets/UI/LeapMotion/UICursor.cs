using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UICursor : MonoBehaviour {

    public MainManager mm;

    // Use this for initialization
    void Start() {
        PlayerPrefs.SetInt("Buttoned", 0);
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Button"))
        {
            PlayerPrefs.SetFloat("ClickedTime", 0);
            PlayerPrefs.SetInt("Buttoned", 1);
            other.GetComponent<Button>().Select();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Button") && PlayerPrefs.GetFloat("ClickedTime")>2)
        {
            switch (other.gameObject.name)
            {
                case "SelectSong":
                    //PlayerPrefs.SetInt("Buttoned", 0);
                    mm.switchToScene(MainManager.game);
                    break;
                case "EditSong":
                    //PlayerPrefs.SetInt("Buttoned", 0);
                    mm.switchToScene(MainManager.edit);
                    break;
                case "Exit":
                    mm.Quit();
                    break;
                case "Menu":
                    //PlayerPrefs.SetInt("Buttoned", 0);
                    SceneManager.LoadScene(0);
                    break;
                case "BackToSelectEdit":
                    //PlayerPrefs.SetInt("Buttoned", 0);
                    SceneManager.LoadScene(5);
                    break;
                default:
                    break;

            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Button"))
        {
            PlayerPrefs.SetFloat("ClickedTime", 0);
            PlayerPrefs.SetInt("Buttoned", 0);
            EventSystem e = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            e.SetSelectedGameObject(null);
        }
    }
}
