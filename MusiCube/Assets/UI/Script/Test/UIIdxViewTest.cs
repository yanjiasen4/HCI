using UnityEngine;
using System.Collections.Generic;
namespace MusiCube
{
    namespace Test
    {
        [RequireComponent(typeof(UIIdxView))]
        public class UIIdxViewTest : MonoBehaviour
        {

            void Start()
            {
                test(GetComponent<UIIdxView>());
            }
            public void test(UIIdxView view)
            {
                view.timestamps = new List<int>();
                view.timestamps.Add(200);
                view.notes = new SortedDictionary<int, List<Note>>();
                view.notes[200] = new List<Note>();
                view.notes[200].Add(new Note());
                view.notes[200].Add(new Note());
            }
        }
    }
}