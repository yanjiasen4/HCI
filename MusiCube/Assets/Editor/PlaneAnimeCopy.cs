using UnityEngine;
using UnityEditor;
using System.Collections;
namespace MusiCube
{
    public class PlaneAnimeCopy : EditorWindow
    {
        public GameObject src = null;
        public GameObject[] targets = new GameObject[0];
        [MenuItem("Window/MusiCube/Plane Animation Copy")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            PlaneAnimeCopy window = (PlaneAnimeCopy)EditorWindow.GetWindow(typeof(PlaneAnimeCopy));
            window.Show();
        }

        void OnGUI()
        {
            // The actual window code goes here
            src = (GameObject)EditorGUILayout.ObjectField("src", src, typeof(GameObject),true);
            EditorGUILayout.LabelField("targets");
            EditorGUI.indentLevel++;
            int ptLength = targets.Length;
            ptLength = EditorGUILayout.IntField("Size", ptLength);
            //长度不同拷贝一份
            if (ptLength != targets.Length)
            {

                GameObject[] newObjs = new GameObject[ptLength];
                for (int i = 0; i < ptLength; i++)
                {
                    if (i < targets.Length)
                    {
                        newObjs[i] = targets[i];
                    }
                }
                targets = newObjs;
            }
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = (GameObject)EditorGUILayout.ObjectField(targets[i], typeof(GameObject), true);
            }
            EditorGUI.indentLevel--;
            if (GUILayout.Button("copy"))
            {
                PlaneAnime srcAnime = src.GetComponent<PlaneAnime>();
                for(int i = 0; i < targets.Length; i++)
                {
                    PlaneAnime dest = targets[i].GetComponent<PlaneAnime>();
                    EditorUtility.CopySerialized(srcAnime, dest);
                }
            }
        }
    }
}