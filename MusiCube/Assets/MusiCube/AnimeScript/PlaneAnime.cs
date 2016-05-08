using UnityEngine;
using System.Collections;
namespace MusiCube
{
    public enum PlAnimeType
    {
        none,
        raise,
        silentRaise,
        fail,
        perfect,
        good,
        normal
    }
    public class PlaneAnime : MonoBehaviour
    {
        private Transform plane;
        public PlAnimeType anime = PlAnimeType.none;
        public Color themeColor;
        public Color perfectColor;
        public Color goodColor;
        public Color normalColor;
        public float keyHeight = 0.25f;
        public float lightScaleX = 0.95f;
        public float lightScaleZ = 0.95f;
        public AnimationCurve[] dropHeightCurves;
        public AnimationCurve[] dropScalesCurves;
        public AnimationCurve[] dropTransparentCurves;
        public AnimationCurve raiseCurve;
        public AnimationCurve lightHeightCurve;
        public AnimationCurve perfectLightScaleCurve;
        public AnimationCurve perfectLightTransparentCurve;
        public AnimationCurve goodLightScaleCurve;
        public AnimationCurve goodLightTransparentCurve;
        public AnimationCurve normalLightScaleCurve;
        public AnimationCurve normalLightTransparentCurve;
        private GameObject[] dropPlanes;
        private GameObject pressLight;


        //for test
        private float curTime = 0;
        public float testTime = 10; 
        // Use this for initialization
        void Start()
        {
            plane = GetComponentsInChildren<Transform>()[1];
            //plane = transform.GetComponentInChildren<Transform>();
            Texture[] tex = AnimeResource.instance.planeRaiseSequence;
            GameObject dropOpl = AnimeResource.instance.dropPlane as GameObject;
            int nums = dropHeightCurves.Length;
            //Debug.Log(nums);
            dropPlanes = new GameObject[nums];
            for(int i = 0; i < nums; i++)
            {
                dropPlanes[i] = Instantiate(dropOpl, transform.position, transform.rotation) as GameObject;
                dropPlanes[i].transform.parent = transform;
                dropPlanes[i].SetActive(false);
            }
            pressLight = Instantiate(AnimeResource.instance.pressLight, transform.position, transform.rotation) as GameObject;
            pressLight.transform.parent = transform;
            pressLight.SetActive(false); 
        }

        //失败动画
        public void playFail(float time)
        {
            if(anime != PlAnimeType.fail)
            {
                StateSwitch(PlAnimeType.fail);
            }
            float height = Mathf.Lerp(keyHeight, 0, time);
            plane.transform.localPosition = new Vector3(0, height, 0);

            //Debug.Log("play fail");
        }

        //缩圈动画
        public void playRaise(float time)
        {
            //Debug.Log("play");
            if(anime!= PlAnimeType.raise)
            {
                StateSwitch(PlAnimeType.raise);
            }
            plane.transform.localPosition = new Vector3(0, raiseCurve.Evaluate(time));
            Texture[] raiseSequence = AnimeResource.instance.planeRaiseSequence;
            int texIdx = (int)(time * raiseSequence.Length);
            if(texIdx >= raiseSequence.Length)
            {
                texIdx = raiseSequence.Length - 1;
            }
            plane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", raiseSequence[texIdx]);
            for(int i = 0; i < dropPlanes.Length; i++)
            {
                dropPlanes[i].transform.localPosition = new Vector3(0, dropHeightCurves[i].Evaluate(time));
                float scale = dropScalesCurves[i].Evaluate(time);
                dropPlanes[i].transform.localScale = new Vector3(scale,1, scale);
                Color old = dropPlanes[i].GetComponent<MeshRenderer>().material.color;
                old.a = dropTransparentCurves[i].Evaluate(time);
                dropPlanes[i].GetComponent<MeshRenderer>().material.color = old;
            }
        }

        public void playSilentRaise(float time)
        {
            if (anime != PlAnimeType.silentRaise)
            {
                StateSwitch(PlAnimeType.silentRaise);
            }
            float height = Mathf.Lerp(0, keyHeight, time);
            plane.transform.localPosition = new Vector3(0, height, 0);
        }

        //perfect光束
        public void playPerfect(float time)
        {
            if(anime != PlAnimeType.perfect)
            {
                StateSwitch(PlAnimeType.perfect);
            }
            float height = Mathf.Lerp(keyHeight, 0, time);
            plane.transform.localPosition = new Vector3(0, height, 0);
            pressLight.transform.localPosition = new Vector3(0, lightHeightCurve.Evaluate(time));
            pressLight.transform.localScale = new Vector3(lightScaleX, perfectLightScaleCurve.Evaluate(time), lightScaleZ);
            MeshRenderer lightMesh = pressLight.GetComponent<MeshRenderer>();
            lightMesh.material.SetFloat("_OutAlpha",perfectLightTransparentCurve.Evaluate(time));
            

        }

        //good光束
        public void playGood(float time)
        {
            if (anime != PlAnimeType.good)
            {
                StateSwitch(PlAnimeType.good);
            }
            float height = Mathf.Lerp(keyHeight, 0, time);
            plane.transform.localPosition = new Vector3(0, height, 0);
            pressLight.transform.localPosition = new Vector3(0, lightHeightCurve.Evaluate(time));
            pressLight.transform.localScale = new Vector3(lightScaleX, goodLightScaleCurve.Evaluate(time), lightScaleZ);
            MeshRenderer lightMesh = pressLight.GetComponent<MeshRenderer>();
            lightMesh.material.SetFloat("_OutAlpha", goodLightTransparentCurve.Evaluate(time));
        }

        //normal光束
        public void playNormal(float time)
        {
            if (anime != PlAnimeType.normal)
            {
                StateSwitch(PlAnimeType.normal);
            }
            float height = Mathf.Lerp(keyHeight, 0, time);
            plane.transform.localPosition = new Vector3(0, height, 0);
            pressLight.transform.localPosition = new Vector3(0, lightHeightCurve.Evaluate(time));
            pressLight.transform.localScale = new Vector3(lightScaleX, normalLightScaleCurve.Evaluate(time), lightScaleZ);
            MeshRenderer lightMesh = pressLight.GetComponent<MeshRenderer>();
            lightMesh.material.SetFloat("_OutAlpha", normalLightTransparentCurve.Evaluate(time));
        }

        void StateSwitch(PlAnimeType nType)
        {
            if(anime == nType)
            {
                return;
            }   
            switch (anime)
            {
                case PlAnimeType.raise:
                    for(int i = 0; i < dropPlanes.Length; i++)
                    {
                        dropPlanes[i].SetActive(false);
                    }
                    break;
                case PlAnimeType.perfect:
                case PlAnimeType.good:
                case PlAnimeType.normal:
                    pressLight.SetActive(false);
                    break;
            }
            switch (nType)
            {
                case PlAnimeType.raise:
                    for(int i = 0; i < dropPlanes.Length; i++)
                    {
                        dropPlanes[i].SetActive(true);
                        dropPlanes[i].GetComponent<MeshRenderer>().material.color = themeColor;
                    }
                    break;
                case PlAnimeType.fail:
                    plane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", null);
                    break;
                case PlAnimeType.perfect:
                    pressLight.SetActive(true);
                    pressLight.GetComponent<MeshRenderer>().material.color = perfectColor;
                    plane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", null);
                    break;
                case PlAnimeType.good:
                    pressLight.SetActive(true);
                    pressLight.GetComponent<MeshRenderer>().material.color = goodColor;
                    plane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", null);
                    break;
                case PlAnimeType.normal:
                    pressLight.SetActive(true);
                    pressLight.GetComponent<MeshRenderer>().material.color = normalColor;
                    plane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", null);
                    break;
                case PlAnimeType.silentRaise:
                    plane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", null);
                    break;
            }
            anime = nType;
        }
        // Update is called once per frame
        void Update()
        {
            /*
            if (anime == PlAnimeType.none)
            {
                playRaise(curTime / testTime);
            }
            if(anime == PlAnimeType.fail)
            {
                playFail(curTime / testTime);
            }
            if(anime == PlAnimeType.raise)
            {
                playRaise(curTime / testTime);
            }
            if(anime == PlAnimeType.perfect)
            {
                playPerfect(curTime / testTime);
            }
            if(anime == PlAnimeType.good)
            {
                playGood(curTime / testTime);
            }
            if(anime == PlAnimeType.normal)
            {
                playNormal(curTime / testTime);
            }
            if(anime == PlAnimeType.silentRaise)
            {
                playSilentRaise(curTime / testTime);
            }
            curTime += Time.deltaTime;
            if(curTime > testTime)
            {
                curTime = 0;
                if(anime == PlAnimeType.raise)
                {
                    //playFail(curTime / testTime);
                    playPerfect(curTime / testTime);
                    //playGood(curTime / testTime);
                    //playNormal(curTime / testTime);
                    return;
                }
                if(anime == PlAnimeType.fail)
                {
                    playRaise(curTime / testTime);
                    return;
                }
                if(anime == PlAnimeType.perfect || anime == PlAnimeType.good || anime == PlAnimeType.normal)
                {
                    playRaise(curTime / testTime);
                    return;
                }
            }*/  
        }

    }
}