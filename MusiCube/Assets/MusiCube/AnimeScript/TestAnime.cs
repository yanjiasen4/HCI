using UnityEngine;
using System.Collections;
namespace MusiCube
{
    namespace Test
    {
        public class TestAnime : MonoBehaviour
        {
            public SelectAnime anime;
            // Use this for initialization
            void Start()
            {
                StartCoroutine(wait());
                
            }
            IEnumerator wait()
            {
                yield return null;
                for(int i = 0; i < 20; i++)
                {
                    anime.playPlaneClearStaticly();
                    anime.playPlaneFeedback(Direction.xminus,PlAnimeType.perfect,i/(float)20);
                    yield return null;
                }
                for (int i = 0; i < 100; i++)
                {
                    //anime.playFail(i / (float)100);
                    yield return null;
                }
            }
            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}