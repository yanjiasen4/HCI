using UnityEngine;
using System.Collections;
namespace MusiCube
{
    public class AnimeResource : MonoBehaviour
    {
        private Texture[] _planeRaiseSequence;
        private Object _dropPlane;
        private Object _pressLight;
        private Object _rowSubPlane;
        private Object _rowMovePlane;
        private Object _rowHintPlane;
        private Texture _rowSubPlaneArrow;
        private Texture _rowSubPlaneFill;
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
        public Object rowSubPlane
        {
            get
            {
                if(_rowSubPlane == null)
                {
                    _rowSubPlane = Resources.Load<Object>("Prefab/RowSubPlane");
                }
                return _rowSubPlane;
            }
        }
        public Object rowMovePlane
        {
            get
            {
                if(_rowMovePlane == null)
                {
                    _rowMovePlane = Resources.Load<Object>("Prefab/RowMovePlane");
                }
                return _rowMovePlane;
            }
        }

        public Object rowHintPlane
        {
            get
            {
                if(_rowHintPlane == null)
                {
                    _rowHintPlane = Resources.Load<Object>("Prefab/RowHintPlane");
                }
                return _rowHintPlane;
            }
        }
        public Texture rowSubPlaneArrow
        {
            get
            {
                if(_rowSubPlaneArrow == null)
                {
                    _rowSubPlaneArrow = Resources.Load<Texture>("Texture/RowSubPlaneArrow");
                }
                return _rowSubPlaneArrow;
            }
        }

        public Texture rowSubPlaneFill
        {
            get
            {
                if(_rowSubPlaneFill == null)
                {
                    _rowSubPlaneFill = Resources.Load<Texture>("Texture/RowSubPlaneFill");
                }
                return _rowSubPlaneFill;
            }
        }

        
    }
}