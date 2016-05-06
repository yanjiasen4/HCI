using UnityEngine;
using System.Collections;
namespace MusiCube
{
    public class AnimeResource : MonoBehaviour
    {
        private Texture[] _planeRaiseSequence;
        private Object _dropPlane;
        private Object _pressLight;
        private static readonly AnimeResource _instance = new AnimeResource();
        public static AnimeResource instance { get { return _instance; } }
        public Texture[] planeRaiseSequence
        {
            get
            {
                if(_planeRaiseSequence == null)
                {
                    //load Sequence texture
                    //Debug.Log("load");
                    _planeRaiseSequence = Resources.LoadAll<Texture>("Texture/PlaneRaiseSequence");
                    //Debug.Log(_planeSequence);
                }
               
                return _planeRaiseSequence;
            }
        }
        public Object pressLight
        {
            get
            {
                if(_pressLight == null)
                {
                    _pressLight = Resources.Load<Object>("Prefab/PressLight");
                }
                return _pressLight;
            }
        }
        public Object dropPlane
        {
            get
            {
                if(_dropPlane == null)
                {
                    _dropPlane = Resources.Load<Object>("Prefab/DropPlane");
                }
                return _dropPlane;
            }
        }
    }
}