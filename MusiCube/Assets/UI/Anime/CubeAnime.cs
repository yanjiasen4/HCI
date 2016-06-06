using UnityEngine;
using System.Collections;
namespace MusiCube
{
    public class CubeAnime : MonoBehaviour
    {
        public GameObject cubeFx;
        
        // Use this for initialization
        void Start()
        {
            //Duplicate();
            //cubeFx.GetComponent<Animator>().speed = 2;
            StartCoroutine(Duplicate());
        }
        IEnumerator Duplicate()
        {
            yield return new WaitForSeconds(0.5f);
            GameObject newObj = Instantiate<GameObject>(cubeFx);
            RuntimeAnimatorController newContoller = Instantiate<RuntimeAnimatorController>(cubeFx.GetComponent<Animator>().runtimeAnimatorController);
            
            newObj.GetComponent<Animator>().runtimeAnimatorController = newContoller;
            newObj.transform.parent = transform;
            newObj.transform.localPosition = new Vector3(0, 0, 0);
            //newObj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            newObj.transform.localRotation = Quaternion.identity;
            newObj.GetComponent<MeshRenderer>().material.renderQueue = cubeFx.GetComponent<MeshRenderer>().material.renderQueue + 1;
            newObj.GetComponent<Animator>().speed = 1f;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}