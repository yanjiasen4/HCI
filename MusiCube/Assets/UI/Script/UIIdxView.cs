using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace MusiCube
{
   
    public class UIIdxView : Graphic
    {
        //节拍间隔
        public int beatWidth = 100;
        protected List<Note> _notes;
        protected int _beatTime;
        //分拍
        public int divide = 4;
        //目前的时间(毫秒)
        public int curTime;
        //线宽1px
        public int lineWidth = 1;


        private Dictionary<int, List<Note>> noteSet;
        private List<Note> sliderSet;

        protected void fillSets()
        {
            
            if(_notes == null)
            {
                return;
            }
            noteSet = new Dictionary<int, List<Note>>();
            sliderSet = new List<Note>();
            int length = _notes.Count;
            for(int i = 0; i < length; i++)
            {
                if(_notes[i].type == NoteType.Slider)
                {
                    sliderSet.Add(_notes[i]);
                }
                if(_notes[i].type == NoteType.Note)
                {

                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

        }
        void Update()
        {
            
            SetVerticesDirty();
        }
        
        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            Vector2 corner1 = Vector2.zero;
            Vector2 corner2 = Vector2.zero;

            corner1.x = 0f;
            corner1.y = 0f;
            corner2.x = 1f;
            corner2.y = 1f;

            corner1.x -= rectTransform.pivot.x;
            corner1.y -= rectTransform.pivot.y;
            corner2.x -= rectTransform.pivot.x;
            corner2.y -= rectTransform.pivot.y;

            corner1.x *= rectTransform.rect.width;
            corner1.y *= rectTransform.rect.height;
            corner2.x *= rectTransform.rect.width;
            corner2.y *= rectTransform.rect.height;

            vbo.Clear();

            UIVertex vert = UIVertex.simpleVert;

            vert.position = new Vector2(corner1.x, corner1.y);
            vert.color = color;
            vbo.Add(vert);

            vert.position = new Vector2(corner1.x, corner2.y);
            vert.color = color;
            vbo.Add(vert);

            vert.position = new Vector2(corner2.x, corner2.y);
            vert.color = color;
            vbo.Add(vert);

            vert.position = new Vector2(corner2.x, corner1.y);
            vert.color = color;
            vbo.Add(vert);
        }
    }
}