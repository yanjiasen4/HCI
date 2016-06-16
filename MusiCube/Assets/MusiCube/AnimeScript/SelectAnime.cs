using UnityEngine;
using System.Collections;
namespace MusiCube
{
    public enum CubeState
    {
        select,
        unSelect
    }
    public class SelectAnime : MonoBehaviour
    {
        public GameObject corner;
        public Color newColor;
        public AnimationCurve scaleCurve;
        public AnimationCurve alphaCurve;
        //for auto play anime
        public float totalTime;
        private Coroutine curRoutine;

        private Color originColor;
        private CubeState state = CubeState.unSelect;

        public GameObject xplusPlane;
        public GameObject xminusPlane;
        public GameObject yplusPlane;
        public GameObject yminusPlane;
        public GameObject zplusPlane;
        public GameObject zminusPlane;

        public bool feedback = false;

        // Use this for initialization
        void Start()
        {
            originColor = corner.GetComponent<MeshRenderer>().material.color;
        }

        public void setDropTime(float t)
        {
            xplusPlane.GetComponent<PlaneAnime>().raiseTime = t;
            xminusPlane.GetComponent<PlaneAnime>().raiseTime = t;
            yplusPlane.GetComponent<PlaneAnime>().raiseTime = t;
            yminusPlane.GetComponent<PlaneAnime>().raiseTime = t;
            zplusPlane.GetComponent<PlaneAnime>().raiseTime = t;
            zminusPlane.GetComponent<PlaneAnime>().raiseTime = t;
        }

        //播放选中动画
        public void playSelect(float time)
        {
            if(state != CubeState.select)
            {
                stateSwitch(CubeState.select);
            }
            float scale = scaleCurve.Evaluate(time);
            corner.transform.localScale = new Vector3(scale, scale, scale);
            float alpha = alphaCurve.Evaluate(time);
            Color curColor = originColor * (1 - time) + newColor * time;
            curColor.a = alpha;
            corner.GetComponent<MeshRenderer>().material.color = curColor;
        }

        //未选中动画
        public void playUnSelect(float time)
        {
            if(state != CubeState.unSelect)
            {
                stateSwitch(CubeState.unSelect);
            }
            float scale = scaleCurve.Evaluate(1 - time);
            corner.transform.localScale = new Vector3(scale, scale, scale);
            float alpha = alphaCurve.Evaluate(1 - time);
            Color curColor = originColor * (time) + newColor * (1 - time);
            curColor.a = alpha;
            corner.GetComponent<MeshRenderer>().material.color = curColor;
        }

        //自动播放，使用coroutine
        public void autoPlay(CubeState nState)
        {
            if(curRoutine != null)
            {
                StopCoroutine(curRoutine);
            }
            curRoutine = StartCoroutine(playCoroutine(nState));
        }

        IEnumerator playCoroutine(CubeState nState)
        {
            float curTime = 0;
            while (curTime < totalTime)
            {
                switch (nState)
                {
                    case CubeState.select:
                        playSelect(curTime / totalTime);
                        break;
                    case CubeState.unSelect:
                        playUnSelect(curTime / totalTime);
                        break;
                }
                curTime += Time.deltaTime;
                yield return null;
            }
        }

        public void playPlaneRaise(MusiCube.Direction dir, float time)
        {
            if (feedback)
                return;
            switch(dir)
            {
                case Direction.xplus:
                    xplusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.raise);

                   // xplusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.perfect);
                    break;
                case Direction.xminus:
                    xminusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.raise);

                 //   xminusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.perfect);
                    break;
                case Direction.yplus:
                    yplusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.raise);

                 //   yplusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.perfect);
                    break;
                case Direction.yminus:
                    yminusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.raise);

                 //   yminusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.perfect);
                    break;
                case Direction.zplus:
                    zplusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.raise);

                //    zplusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.perfect);
                    break;
                case Direction.zminus:
                    zminusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.raise);

                //    zminusPlane.GetComponent<PlaneAnime>().autoPlay(PlAnimeType.perfect);
                    break;
                default:
                    break;
            }
        }
        public void playPlaneRaiseStaticly(MusiCube.Direction dir, float time)
        {
            switch (dir)
            {
                case Direction.xplus:
                    xplusPlane.GetComponent<PlaneAnime>().playRaise(time);
                    break;
                case Direction.xminus:
                    xminusPlane.GetComponent<PlaneAnime>().playRaise(time);
                    break;
                case Direction.yplus:
                    yplusPlane.GetComponent<PlaneAnime>().playRaise(time);
                    break;
                case Direction.yminus:
                    yminusPlane.GetComponent<PlaneAnime>().playRaise(time);
                    break;
                case Direction.zplus:
                    zplusPlane.GetComponent<PlaneAnime>().playRaise(time);
                    break;
                case Direction.zminus:
                    zminusPlane.GetComponent<PlaneAnime>().playRaise(time);
                    break;
                default:
                    break;
            }
        }

        public void playPlaneFeedback(Direction dir, PlAnimeType pt, float time)
        {
            switch(pt)
            {
                case PlAnimeType.perfect:
                    playPlanePerfect(dir, time);
                    break;
                case PlAnimeType.good:
                    playPlaneGood(dir, time);
                    break;
                case PlAnimeType.normal:
                    playPlaneNormal(dir, time);
                    break;
                case PlAnimeType.fail:
                    playPlaneFailed(dir, time);
                    break;
                default:
                    break;
            }
        }

        public void playPlanePerfect(Direction dir, float time)
        {
            switch (dir)
            {
                case Direction.xplus:
                    xplusPlane.GetComponent<PlaneAnime>().playPerfect(time);
                    break;
                case Direction.xminus:
                    xminusPlane.GetComponent<PlaneAnime>().playPerfect(time);
                    break;
                case Direction.yplus:
                    yplusPlane.GetComponent<PlaneAnime>().playPerfect(time);
                    break;
                case Direction.yminus:
                    yminusPlane.GetComponent<PlaneAnime>().playPerfect(time);
                    break;
                case Direction.zplus:
                    zplusPlane.GetComponent<PlaneAnime>().playPerfect(time);
                    break;
                case Direction.zminus:
                    zminusPlane.GetComponent<PlaneAnime>().playPerfect(time);
                    break;
                default:
                    break;
            }
        }
        
        public void playPlaneGood(Direction dir, float time)
        {
            switch (dir)
            {
                case Direction.xplus:
                    xplusPlane.GetComponent<PlaneAnime>().playGood(time);
                    break;
                case Direction.xminus:
                    xminusPlane.GetComponent<PlaneAnime>().playGood(time);
                    break;
                case Direction.yplus:
                    yplusPlane.GetComponent<PlaneAnime>().playGood(time);
                    break;
                case Direction.yminus:
                    yminusPlane.GetComponent<PlaneAnime>().playGood(time);
                    break;
                case Direction.zplus:
                    zplusPlane.GetComponent<PlaneAnime>().playGood(time);
                    break;
                case Direction.zminus:
                    zminusPlane.GetComponent<PlaneAnime>().playGood(time);
                    break;
                default:
                    break;
            }
        }

        public void playPlaneNormal(Direction dir, float time)
        {
            switch (dir)
            {
                case Direction.xplus:
                    xplusPlane.GetComponent<PlaneAnime>().playNormal(time);
                    break;
                case Direction.xminus:
                    xminusPlane.GetComponent<PlaneAnime>().playNormal(time);
                    break;
                case Direction.yplus:
                    yplusPlane.GetComponent<PlaneAnime>().playNormal(time);
                    break;
                case Direction.yminus:
                    yminusPlane.GetComponent<PlaneAnime>().playNormal(time);
                    break;
                case Direction.zplus:
                    zplusPlane.GetComponent<PlaneAnime>().playNormal(time);
                    break;
                case Direction.zminus:
                    zminusPlane.GetComponent<PlaneAnime>().playNormal(time);
                    break;
                default:
                    break;
            }
        }

        public void playPlaneFailed(Direction dir, float time)
        {
            switch (dir)
            {
                case Direction.xplus:
                    xplusPlane.GetComponent<PlaneAnime>().playFail(time);
                    break;
                case Direction.xminus:
                    xminusPlane.GetComponent<PlaneAnime>().playFail(time);
                    break;
                case Direction.yplus:
                    yplusPlane.GetComponent<PlaneAnime>().playFail(time);
                    break;
                case Direction.yminus:
                    yminusPlane.GetComponent<PlaneAnime>().playFail(time);
                    break;
                case Direction.zplus:
                    zplusPlane.GetComponent<PlaneAnime>().playFail(time);
                    break;
                case Direction.zminus:
                    zminusPlane.GetComponent<PlaneAnime>().playFail(time);
                    break;
                default:
                    break;
            }
        }

        public void playPlaneClearStaticly()
        {
            xplusPlane.GetComponent<PlaneAnime>().StateSwitch(PlAnimeType.none);
            xminusPlane.GetComponent<PlaneAnime>().StateSwitch(PlAnimeType.none);
            yplusPlane.GetComponent<PlaneAnime>().StateSwitch(PlAnimeType.none);
            yminusPlane.GetComponent<PlaneAnime>().StateSwitch(PlAnimeType.none);
            zplusPlane.GetComponent<PlaneAnime>().StateSwitch(PlAnimeType.none);
            zminusPlane.GetComponent<PlaneAnime>().StateSwitch(PlAnimeType.none);
        }

        public void autoPlayRotate(Vector3 point, Vector3 axis, float angle, float t, float delay)
        {
            if (curRoutine != null)
            {
                StopCoroutine(curRoutine);
            }
            curRoutine = StartCoroutine(playCoroutineRotate(point, axis, angle, t, delay));
        }

        IEnumerator playCoroutineRotate(Vector3 point, Vector3 axis, float angle, float t, float delay)
        {
            yield return new WaitForSeconds(delay);
            float currTime = 0;
            while(currTime < t)
            {
                print(currTime);
                float dt = Time.deltaTime;
                playRotate(point, axis, angle * dt/t);
                currTime += dt;
                yield return null;
            }
        }

        public void playRotate(Vector3 point, Vector3 axis, float angle)
        {
            transform.RotateAround(point, axis, angle);
        }

        void stateSwitch(CubeState nState)
        {
            float scale = 0;
            switch (nState)
            {
                case CubeState.select:
                    scale = scaleCurve.Evaluate(0);
                    corner.transform.localScale = new Vector3(scale, scale, scale);
                    corner.GetComponent<MeshRenderer>().material.color = originColor;
                    break;
                case CubeState.unSelect:
                    scale = scaleCurve.Evaluate(1);
                    corner.transform.localScale = new Vector3(scale, scale, scale);
                    corner.GetComponent<MeshRenderer>().material.color = originColor;
                    break; 
            }
            state = nState;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}