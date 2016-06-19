using UnityEngine;
using System.Collections;
using MusiCube;

public class SliderRotate : MonoBehaviour {

    public GameObject slider;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Rotate(Direction dir)
    {
        Transform t = slider.transform;
        Quaternion r = t.localRotation; 
        switch(dir)
        {
            case Direction.xplus:
                r = Quaternion.Euler(0, 90, 0);
                t.localRotation = r;
                break;
            case Direction.xminus:
                r = Quaternion.Euler(0, 270, 0);
                t.localRotation = r;
                break;
            case Direction.yplus:
                r = Quaternion.Euler(0, 0, 90);
                t.localRotation = r;
                break;
            case Direction.yminus:
                r = Quaternion.Euler(0, 0, 270);
                t.localRotation = r;
                break;
            case Direction.zplus:
                r = Quaternion.Euler(90, 0, 0);
                t.localRotation = r;
                break;
            case Direction.zminus:
                r = Quaternion.Euler(270, 0, 0);
                t.localRotation = r;
                break;
            default:
                break;
        }
    }
}
