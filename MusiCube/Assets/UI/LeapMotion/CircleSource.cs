using UnityEngine;
using System.Collections;

public class CircleSource : MonoBehaviour {

    private Sprite[] _circleSequence;
    private static readonly CircleSource _instance = new CircleSource();
    public static CircleSource instance { get { return _instance; } }
    public Sprite[] circleSequence
    {
        get
        {
            if (_circleSequence == null)
                _circleSequence = Resources.LoadAll<Sprite>("CircleSequence2");
            return _circleSequence;
        }
    }
    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
