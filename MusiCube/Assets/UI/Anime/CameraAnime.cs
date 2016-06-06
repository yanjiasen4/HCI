using UnityEngine;
using System.Collections;
namespace MusiCube
{
    [RequireComponent(typeof(Animator))]
    public class CameraAnime : MonoBehaviour
    {
        Animator anim;
        // Use this for initialization
        void Start()
        {
            anim = GetComponent<Animator>();
            StartCoroutine(WaitForPlay(2));

        }
        IEnumerator WaitForPlay(float time)
        {
            yield return new WaitForSeconds(time);
            anim.SetBool("MoveToCube", true);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}