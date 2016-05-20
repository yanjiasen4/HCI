using UnityEngine;
using System.Collections;
namespace MusiCube
{
    public enum RowState
    {
        none,
        raise,
        pulling,
        perfect,
        good,
        normal,
        fail
    }
    public class RowAnime : MonoBehaviour
    {
        private RowState state = RowState.none;
        public float curTime;
        public float arrowSpeed;
        public float gap;
        public AnimationCurve raiseHeightCurve;
        public AnimationCurve raiseTransparentCurve;
        public AnimationCurve raiseUVCurve;
        public PlaneAnime[] planes = new PlaneAnime[12];
        private GameObject[] subPlanes = new GameObject[12];
        private GameObject[] movePlanes = new GameObject[4];
        private GameObject[] hintPlanes = new GameObject[4];
        private Coroutine curRoutine;
        private float heightOffset;
        private float hintHeight = 0.000001f;
        public float cubeSize = 1;
        public float raiseTime = 1;
        public float pullingTime = 1;
        public float perfectTime = 1;
        public float goodTime = 1;
        public float normalTime = 1;
        public float failTime = 1;
        void Start()
        {
            heightOffset = 1.5f * cubeSize + gap + hintHeight;
            for(int i = 0; i < 3; i++)
            {
                subPlanes[i] = Instantiate(AnimeResource.instance.rowSubPlane) as GameObject;
                subPlanes[i].transform.parent = transform;
                subPlanes[i].transform.localPosition = new Vector3((1 + gap) * (i - 1), heightOffset);
                subPlanes[i].transform.localRotation = Quaternion.identity;
                subPlanes[i].SetActive(false);
            }

            for(int i = 0; i < 3; i++)
            {
                subPlanes[i + 3] = Instantiate(AnimeResource.instance.rowSubPlane) as GameObject;
                subPlanes[i + 3].transform.parent = transform;
                subPlanes[i + 3].transform.localPosition = new Vector3(heightOffset, (1 + gap) * (1 - i));
                Quaternion tempR = Quaternion.identity;
                tempR.eulerAngles = new Vector3(0, 0, -90);
                subPlanes[i + 3].transform.localRotation = tempR;
                subPlanes[i + 3].SetActive(false);
            }
            for(int i = 0; i < 3; i++)
            {
                subPlanes[i + 6] = Instantiate(AnimeResource.instance.rowSubPlane) as GameObject;
                subPlanes[i + 6].transform.parent = transform;
                subPlanes[i + 6].transform.localPosition = new Vector3((1 + gap) * (i - 1), -heightOffset);
                Quaternion tempR = Quaternion.identity;
                tempR.eulerAngles = new Vector3(0, 0, -180);
                subPlanes[i + 6].transform.localRotation = tempR;
                subPlanes[i + 6].SetActive(false);
            }
            for(int i = 0; i < 3; i++)
            {
                subPlanes[i + 9] = Instantiate(AnimeResource.instance.rowSubPlane) as GameObject;
                subPlanes[i + 9].transform.parent = transform;
                subPlanes[i + 9].transform.localPosition = new Vector3(-heightOffset, (1 + gap) * (1 - i));
                Quaternion tempR = Quaternion.identity;
                tempR.eulerAngles = new Vector3(0, 0, -270);
                subPlanes[i + 9].transform.localRotation = tempR;
                subPlanes[i + 9].SetActive(false);
            }
            for(int i = 0; i < 4; i++)
            {
                hintPlanes[i] = Instantiate(AnimeResource.instance.rowHintPlane) as GameObject;
                movePlanes[i] = Instantiate(AnimeResource.instance.rowMovePlane) as GameObject;
                hintPlanes[i].transform.parent = transform;
                movePlanes[i].transform.parent = transform;
                hintPlanes[i].SetActive(false);
                movePlanes[i].SetActive(false);
            }
            Quaternion rotate = Quaternion.identity;
            float raiseHeight = raiseHeightCurve.Evaluate(1);
            movePlanes[0].transform.localPosition = new Vector3(-(1.5f * cubeSize + gap), heightOffset + raiseHeight);
            movePlanes[1].transform.localPosition = new Vector3(heightOffset + raiseHeight, -(1.5f * cubeSize + gap));
            hintPlanes[0].transform.localPosition = new Vector3(-(1.5f * cubeSize + gap), heightOffset + raiseHeight + hintHeight);
            hintPlanes[1].transform.localPosition = new Vector3(heightOffset + raiseHeight + hintHeight, -(1.5f * cubeSize + gap));
            rotate.eulerAngles = new Vector3(0, 0, -90);
            movePlanes[1].transform.localRotation = rotate;
            hintPlanes[1].transform.localRotation = rotate;
            movePlanes[2].transform.localPosition = new Vector3((1.5f * cubeSize + gap), -(heightOffset + raiseHeight));
            hintPlanes[2].transform.localPosition = new Vector3((1.5f * cubeSize + gap), -(heightOffset + raiseHeight + hintHeight));
            rotate.eulerAngles = new Vector3(0, 0, -180);
            movePlanes[2].transform.localRotation = rotate;
            hintPlanes[2].transform.localRotation = rotate;
            movePlanes[3].transform.localPosition = new Vector3(-(heightOffset + raiseHeight), (1.5f * cubeSize + gap));
            hintPlanes[3].transform.localPosition = new Vector3(-(heightOffset + raiseHeight + hintHeight), (1.5f * cubeSize + gap));
            rotate.eulerAngles = new Vector3(0, 0, -270);
            movePlanes[3].transform.localRotation = rotate;
            hintPlanes[3].transform.localRotation = rotate;
            autoPlay(RowState.raise);
        }

        public void autoPlay(RowState type)
        {
            if (curRoutine != null)
            {
                StopCoroutine(curRoutine);
            }
            curRoutine = StartCoroutine(playCoroutine(type));
        }

        IEnumerator playCoroutine(RowState type)
        {
            curTime = 0;
            switch (type)
            {
                case RowState.raise:
                    while (curTime < raiseTime)
                    {
                        playRaise(curTime / raiseTime);
                        curTime += Time.deltaTime;
                        yield return null;
                    }
                    break;
                case RowState.fail:
                    while (curTime < failTime)
                    {
                        playFail(curTime / failTime);
                        curTime += Time.deltaTime;
                        yield return null;
                    }
                    break;
                case RowState.perfect:
                    while (curTime < perfectTime)
                    {
                        playPerfect(curTime / perfectTime);
                        curTime += Time.deltaTime;
                        yield return null;
                    }
                    break;
                case RowState.good:
                    while (curTime < goodTime)
                    {
                        playGood(curTime / goodTime);
                        curTime += Time.deltaTime;
                        yield return null;
                    }
                    break;
                case RowState.normal:
                    while (curTime < normalTime)
                    {
                        playNormal(curTime / normalTime);
                        curTime += Time.deltaTime;
                        yield return null;
                    }
                    break;
                case RowState.pulling:
                    while (curTime < pullingTime) {
                        playPulling(curTime / pullingTime, curTime / pullingTime, curTime / pullingTime);
                        curTime += Time.deltaTime;
                        yield return null;
                    }
                    break;
            }
        }

        void StateSwitch(RowState nState) {
            if(state == nState)
            {
                return;
            }

            float raiseHeight = raiseHeightCurve.Evaluate(1);
            switch (state)
            {
                case RowState.raise:
                    for(int i = 0; i < 12; i++) {
                        subPlanes[i].SetActive(false);

                        if (planes[i] != null)
                        {
                            planes[i].playSilentHeight(0);
                        }
                    }
                    break;
                case RowState.pulling:
                    for(int i =0; i< 12; i++)
                    {
                        subPlanes[i].SetActive(false);
                        if(planes[i] != null)
                        {
                            planes[i].playNone();
                        }
                    }
                    for(int i = 0; i < 4; i++)
                    {
                        movePlanes[i].SetActive(false);
                        hintPlanes[i].SetActive(false);
                    }
                    break;
                case RowState.perfect:
                case RowState.good:
                case RowState.normal:
                case RowState.fail:
                    for(int i = 0; i < 12; i++)
                    {
                        if (planes[i] != null)
                        {
                            planes[i].playNone();
                        }
                    }
                    break;
            }
            switch (nState)
            {
                case RowState.raise:
                    for(int i = 0; i < 12; i++)
                    {
                        //重置平面的纹理为箭头
                        subPlanes[i].SetActive(true);
                        subPlanes[i].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", AnimeResource.instance.rowSubPlaneArrow);
                        subPlanes[i].GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(0, 0));
                        subPlanes[i].GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(1, 1));
                        Vector3 pos = subPlanes[i].transform.localPosition;
                  
                        if(i - 6 < 0)
                        {
                            if (i - 3 < 0)
                            {
                                pos.y = heightOffset;
                            }else
                            {
                                pos.x = heightOffset;
                            }
                        }else
                        {
                            if (i - 9 < 0)
                            {
                                pos.y = -heightOffset;
                            }
                            else
                            {
                                pos.x = -heightOffset;
                            }
                        }
                        subPlanes[i].transform.localPosition = pos;
                        if (planes[i] != null)
                        {
                            planes[i].playSilentHeight(0);
                        }
                    }
                    break;
                case RowState.pulling:
                    for(int i= 0; i < 12; i++)
                    {
                        //重置平面的纹理为填充
                        subPlanes[i].SetActive(true);
                        subPlanes[i].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", AnimeResource.instance.rowSubPlaneFill);
                        subPlanes[i].GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(0, 0));
                        subPlanes[i].GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(0.5f, 1));

                        if(planes[i] != null)
                        {
                            planes[i].playSilentHeight(raiseHeight-0.0001f);
                        }
                    }
                    for(int i = 0; i < 4; i++)
                    {
                        movePlanes[i].SetActive(true);
                        hintPlanes[i].SetActive(true);
                    }
                    //调整平面高度
                    for(int i = 0; i < 12; i++)
                    {
                        Vector3 pos = subPlanes[i].transform.localPosition;
                        if (i - 6 < 0)
                        {
                            if (i - 3 < 0)
                            {
                                pos.y = heightOffset + raiseHeight;
                            }
                            else
                            {
                                pos.x = heightOffset + raiseHeight;
                            }
                        }
                        else
                        {
                            if (i - 9 < 0)
                            {
                                pos.y = -(heightOffset + raiseHeight);
                            }
                            else
                            {
                                pos.x = -(heightOffset + raiseHeight);
                            }
                        }
                        subPlanes[i].transform.localPosition = pos;
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        subPlanes[i].SetActive(true);
                    }
                    movePlanes[0].transform.localPosition = new Vector3(-(1.5f * cubeSize + gap), heightOffset + raiseHeight);
                    movePlanes[1].transform.localPosition = new Vector3(heightOffset + raiseHeight, -(1.5f * cubeSize + gap));
                    movePlanes[2].transform.localPosition = new Vector3((1.5f * cubeSize + gap), -(heightOffset + raiseHeight));
                    movePlanes[3].transform.localPosition = new Vector3(-(heightOffset + raiseHeight), (1.5f * cubeSize + gap));
                    hintPlanes[0].transform.localPosition = new Vector3(-(1.5f * cubeSize + gap), heightOffset + raiseHeight + hintHeight);
                    hintPlanes[1].transform.localPosition = new Vector3(heightOffset + raiseHeight + hintHeight, -(1.5f * cubeSize + gap));
                    hintPlanes[2].transform.localPosition = new Vector3((1.5f * cubeSize + gap), -(heightOffset + raiseHeight + hintHeight));
                    hintPlanes[3].transform.localPosition = new Vector3(-(heightOffset + raiseHeight + hintHeight), (1.5f * cubeSize + gap));
                    break;
                    
            }
            state = nState;
        }

        public void playRaise(float time)
        {
            if(state != RowState.raise)
            {
                StateSwitch(RowState.raise);
            }
            float timeGap = 1f / 6;
            Color color = subPlanes[0].GetComponent<MeshRenderer>().material.color;
            for (int i = 0; i < 3; i++)
            {
                //curve的时间尺度为1/6s
                float subTime = time - timeGap * i;
                float opacity = raiseTransparentCurve.Evaluate(subTime);
                color.a = opacity;
                float subHeight = raiseHeightCurve.Evaluate(subTime);
                Vector3 pos = subPlanes[i].transform.localPosition;
                pos.y = heightOffset + subHeight;
                subPlanes[i].transform.localPosition = pos;
                subPlanes[i].GetComponent<MeshRenderer>().material.color = color;
                pos.y = -pos.y;
                subPlanes[i + 6].transform.localPosition = pos;
                subPlanes[i + 6].GetComponent<MeshRenderer>().material.color = color;
                if (planes[i] != null)
                    planes[i].playSilentHeight(subHeight);
                if(planes[i+6] != null)
                    planes[i + 6].playSilentHeight(subHeight);
            }
            
            for(int i = 0; i < 3; i++)
            {
                float subTime = time - timeGap * (i+3);
                float opacity = raiseTransparentCurve.Evaluate(subTime);
                color.a = opacity;
                float subHeight = raiseHeightCurve.Evaluate(subTime);
                Vector3 pos = subPlanes[i + 3].transform.localPosition;
                pos.x = heightOffset + subHeight;
                subPlanes[i + 3].transform.localPosition = pos;
                subPlanes[i + 3].GetComponent<MeshRenderer>().material.color = color;
                pos.x = -pos.x;
                subPlanes[i + 9].transform.localPosition = pos;
                subPlanes[i + 9].GetComponent<MeshRenderer>().material.color = color;
                if (planes[i + 3] != null)
                    planes[i + 3].playSilentHeight(subHeight);
                if(planes[i + 9] != null)
                    planes[i + 9].playSilentHeight(subHeight);
            }
            float offsetX = raiseUVCurve.Evaluate(time);
            for(int i = 0; i < 12; i++)
            {
                subPlanes[i].GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(offsetX, 0));
            }

        }

        public void playPulling(float time,float extent,float hintExtent)
        {
            if(state != RowState.pulling)
            {
                StateSwitch(RowState.pulling);
            }
            float raiseHeight = raiseHeightCurve.Evaluate(1);
            float pos = Mathf.Lerp(-(1.5f*cubeSize + gap), 1.5f*cubeSize + gap,extent);
            movePlanes[0].transform.localPosition = new Vector3(pos, heightOffset + raiseHeight);
            movePlanes[2].transform.localPosition = new Vector3(-pos, -(heightOffset + raiseHeight));
            //y轴朝上，因此pos要反一下
            movePlanes[1].transform.localPosition = new Vector3(heightOffset + raiseHeight, -pos);
            movePlanes[3].transform.localPosition = new Vector3(-(heightOffset + raiseHeight), pos);

            //修改hint plane 位置
            pos = Mathf.Lerp(-(1.5f * cubeSize + gap), 1.5f * cubeSize + gap, hintExtent);
            hintPlanes[0].transform.localPosition = new Vector3(pos, heightOffset + raiseHeight + hintHeight);
            hintPlanes[2].transform.localPosition = new Vector3(-pos, -(heightOffset + raiseHeight + hintHeight));
            //y轴朝上，因此pos要反一下
            hintPlanes[1].transform.localPosition = new Vector3(heightOffset + raiseHeight + hintHeight, -pos);
            hintPlanes[3].transform.localPosition = new Vector3(-(heightOffset + raiseHeight + hintHeight), pos);
            for (int i = 0; i < 3; i++)
            {
                float fill = Mathf.Lerp(0, 0.5f, extent * (3 + 2 * gap) - i * (1 + gap));
                subPlanes[i].GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(fill, 0));
                subPlanes[i + 3].GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(fill, 0));
                subPlanes[i + 6].GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(fill, 0));
                subPlanes[i + 9].GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(fill, 0));
            }
        }

        public void playPerfect(float time)
        {
            if(state != RowState.perfect)
            {
                StateSwitch(RowState.perfect);
            }
            for(int i = 0; i < 12; i++) {
                if(planes[i] != null)
                {
                    planes[i].playPerfect(time);
                }
            }
        }

        public void playGood(float time)
        {
            if(state != RowState.good)
            {
                StateSwitch(RowState.good);
            }
            for(int i = 0; i < 12; i++)
            {
                if(planes[i] != null)
                {
                    planes[i].playGood(time);
                }
            }
        }

        public void playNormal(float time)
        {
            if(state != RowState.normal)
            {
                StateSwitch(RowState.normal);
            }
            for(int i = 0; i < 12; i++)
            {
                if(planes[i] != null)
                {
                    planes[i].playNormal(time);
                }
            }
        }

        public void playFail(float time)
        {
            if(state != RowState.fail)
            {
                StateSwitch(RowState.fail);
            }
            for(int i = 0; i < 12; i++)
            {
                if(planes[i] != null)
                {
                    planes[i].playFail(time);
                }
            }
        }

        void Update()
        {
            //playRaise(0.7f);
            //playPulling(0.1f, 0.6f);
        }
    }
}