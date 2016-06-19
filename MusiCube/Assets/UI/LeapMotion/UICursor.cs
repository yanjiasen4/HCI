using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UICursor : MonoBehaviour {

    public 

    // Use this for initialization
    void Start() {

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
                case "Select Song":
                    break;
                case "Edit Song":
                    break;
                case "Exit":
                    break;
                case "Menu":
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
