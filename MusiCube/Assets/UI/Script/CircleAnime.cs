using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace MusiCube
{
    [RequireComponent(typeof(Image))]
    public class CircleAnime : MonoBehaviour
    {
        public float totalTime = 1;
        private Coroutine curRoutine;
        Image img;
        // Use this for initialization
        void Start()
        {
            img = GetComponent<Image>();
            autoPlay();
        }

        public void playCircle(float time)
        {
            Sprite[] circleSeq = AnimeResource.instance.circleSequence;
            int idx = (int)(time * circleSeq.Length) % circleSeq.Length;
            img.sprite = circleSeq[idx];
        }

        public void autoPlay()
        {
            if (curRoutine != null)
            {
                StopCoroutine(curRoutine);
            }
            curRoutine = StartCoroutine(playCoroutine());
        }
        IEnumerator playCoroutine()
        {
            
            float curTime = 0;
            while (curTime < totalTime)
            {
                playCircle(curTime/totalTime);
                curTime += Time.deltaTime;
                yield return null;
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}