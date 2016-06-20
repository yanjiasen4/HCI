using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainManager : MonoBehaviour
{

    public GameObject cube;
    public GameObject btn0;
    public GameObject btn1;
    public GameObject btn2;

    private float progress = 0f;
    private float cubeScale = 0.01f;

    public const int game = 1;
    public const int edit = 5;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cube.transform.position += progress * cubeScale * Vector3.left;
        btn0.transform.position += progress * Vector3.right;
        btn1.transform.position += progress * Vector3.right;
        btn2.transform.position += progress * Vector3.right;
        if (Input.GetKeyUp(KeyCode.Alpha1))
            switchToScene(game);
        if (Input.GetKeyUp(KeyCode.Alpha2))
            switchToScene(edit);
    }

    public void switchToScene(int sceneID)
    {
        StartCoroutine(loadScene(sceneID));
    }

    IEnumerator loadScene(int sceneID)
    {
        AsyncOperation asO = SceneManager.LoadSceneAsync(sceneID, LoadSceneMode.Single);
        asO.allowSceneActivation = false;
        while (progress < 100)
        {
            progress++;
            yield return new WaitForEndOfFrame();
        }
        asO.allowSceneActivation = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}

