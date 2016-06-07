using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace MusiCube
{
    

    [ExecuteInEditMode]
    public class UIIdxView : Graphic
    {
        
        //节拍间隔
        public float beatWidth = 100;
        public SortedDictionary<int, List<Note>> notes;
        [HideInInspector]
        public List<int> timestamps;
        public float bpm = 120;
        //分拍
        public int divide = 4;
        //目前的时间(毫秒)
        public int curTime;
        //线宽1px
        public float lineWidth = 1;
        public float lineHeight = 100;
        public float blockHeight = 20;
        public float centerHeight = 70;
        public float centerWidth = 4;

        public Color bigLineColor = new Color(1, 1, 1, 1);
        public Color smallLineColor = new Color(1, 0, 1, 1);
        public Color curLineColor = new Color(1, 0, 0, 1);
        public Color noteColor = new Color(0, 1, 0, 1);
        public AnimationCurve alphaCurve;
        private float blockPadding = 2;
        //返回timestamps中的idx
        int findIdx(int time)
        {
            return divideFind(time, 0, timestamps.Count - 1);
        }
        
        int divideFind(int time,int begin,int end)
        {
            if(begin >= end)
            {
                return begin;
            }
            int mid = begin + end / 2;
            if (timestamps[mid] < time)
            {
                return divideFind(time, mid + 1, end);
            }else
            {
                return divideFind(time, begin, mid);
            }
        }

        void Update()
        {
           
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(Mesh mesh)
        {
            VertexHelper vh = new VertexHelper();
            /*Vector2 corner1 = Vector2.zero;
            Vector2 corner2 = Vector2.zero;

            corner1.x = 0f;
            corner1.y = 0f;
            corner2.x = 1f;
            corner2.y = 0.5f;

            corner1.x -= rectTransform.pivot.x;
            corner1.y -= rectTransform.pivot.y;
            corner2.x -= rectTransform.pivot.x;
            corner2.y -= rectTransform.pivot.y;

            corner1.x *= rectTransform.rect.width;
            corner1.y *= rectTransform.rect.height;
            corner2.x *= rectTransform.rect.width;
            corner2.y *= rectTransform.rect.height;
            
            UIVertex[] quad = new UIVertex[4];
            UIVertex vert = UIVertex.simpleVert;
            vert.position = new Vector2(corner1.x, corner1.y);
            vert.color = color;
            quad[0] = vert;

            vert.position = new Vector2(corner1.x, corner2.y);
            vert.color = color;
            quad[1] = vert;

            vert.position = new Vector2(corner2.x, corner2.y);
            vert.color = color;
            quad[2] = vert;

            vert.position = new Vector2(corner2.x, corner1.y);
            vert.color = color;
            quad[3] = vert;

            vh.AddUIVertexQuad(quad);*/
            
            if(divide == 0||bpm == 0)
            {
                return;
            }
            float timeGap = 60000 / bpm;

            int idx = (int)(curTime / timeGap);

            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            float offsetW = ((curTime - idx * timeGap) / timeGap) * beatWidth;

            int idxBegin = (int)(idx - ((width / 2 - offsetW) / beatWidth + 1));
            int idxEnd = (int)(idx + ((width / 2 + offsetW) / beatWidth + 1));
            int timestampIdxBegin = findIdx((int)(idxBegin * timeGap));
            int timestampIdxEnd = findIdx((int)(idxEnd * timeGap));
            for(int i = idxBegin; i <= idxEnd; i++)
            {
                float posX = width / 2 - offsetW - (idx - i) * beatWidth;
                DrawBigLine(posX, vh);
                for(int j = 0; j < divide; j++)
                {
                    float subPosX = posX + (j+1)* beatWidth / divide;
                    DrawSmallLine(subPosX, vh);
                }
                
            }
            
            if (timestamps != null && notes != null)
            {
                for (int i = timestampIdxBegin; i <= timestampIdxEnd; i++)
                {
                    int time = timestamps[i];
                    List<Note> subNotes = notes[time];
                    if (subNotes != null)
                    {
                        int subIdx = (int)(time / timeGap);
                        float subTime = time - subIdx * timeGap;
                        int subSubIdx = (int)(subTime / (timeGap / divide));
                        float posX = width / 2 - offsetW - (idx - subIdx) * beatWidth + (subSubIdx) * beatWidth / divide;
                        DrawBlocks(posX, subNotes.Count, vh);
                    }
                }
            }

            DrawCenterLine(vh);
            vh.FillMesh(mesh);
        }
        void DrawCenterLine(VertexHelper vh)
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            UIVertex[] quad = new UIVertex[4];
            UIVertex vert = UIVertex.simpleVert;
            Color curColor;
            vert.position = new Vector3(width/2- centerWidth/2, -(height - centerHeight));
            curColor = curLineColor;
            curColor.a *= alphaCurve.Evaluate((width / 2 - centerWidth/2) / width);
            vert.color = curColor;
            quad[0] = vert;

            vert.position = new Vector3(width / 2 - centerWidth/2, -height);
            curColor = curLineColor;
            curColor.a *= alphaCurve.Evaluate((width / 2 - centerWidth/2) / width);
            vert.color = curColor;
            quad[1] = vert;

            vert.position = new Vector3(width / 2 + centerWidth/2, -height);
            curColor = curLineColor;
            curColor.a *= alphaCurve.Evaluate((width / 2 + centerWidth / 2) / width);
            vert.color = curColor;
            quad[2] = vert;

            vert.position = new Vector3(width / 2 + centerWidth / 2, -(height - centerHeight));
            curColor = curLineColor;
            curColor.a *= alphaCurve.Evaluate((width / 2 + centerWidth / 2) / width);
            vert.color = curColor;
            quad[3] = vert;

            vh.AddUIVertexQuad(quad);
        }
        void DrawBigLine(float pos, VertexHelper vh)
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            UIVertex[] quad = new UIVertex[4];
            UIVertex vert = UIVertex.simpleVert;
            Color curColor;
            vert.position = new Vector3(pos - lineWidth, -(height - lineHeight));
            curColor = bigLineColor;
            curColor.a *= alphaCurve.Evaluate((pos-lineWidth)/width);
            vert.color = curColor;
            quad[0] = vert;

            vert.position = new Vector3(pos - lineWidth, -height);
            curColor = bigLineColor;
            curColor.a *= alphaCurve.Evaluate((pos - lineWidth) / width);
            vert.color = curColor;
            quad[1] = vert;

            vert.position = new Vector3(pos + lineWidth, -height);
            curColor = bigLineColor;
            curColor.a *= alphaCurve.Evaluate((pos + lineWidth) / width);
            vert.color = curColor;
            quad[2] = vert;

            vert.position = new Vector3(pos + lineWidth, -(height - lineHeight));
            curColor = bigLineColor;
            curColor.a *= alphaCurve.Evaluate((pos + lineWidth) / width);
            vert.color = curColor;
            quad[3] = vert;

            vh.AddUIVertexQuad(quad);
        }

        void DrawSmallLine(float pos, VertexHelper vh)
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            UIVertex[] quad = new UIVertex[4];
            UIVertex vert = UIVertex.simpleVert;
            Color curColor;
            vert.position = new Vector3(pos - lineWidth/2, -(height - lineHeight/3));
            curColor = smallLineColor;
            curColor.a *= alphaCurve.Evaluate((pos - lineWidth/2) / width);
            vert.color = curColor;
            quad[0] = vert;

            vert.position = new Vector3(pos - lineWidth/2, -height);
            curColor = smallLineColor;
            curColor.a *= alphaCurve.Evaluate((pos - lineWidth / 2) / width);
            vert.color = curColor;
            quad[1] = vert;

            vert.position = new Vector3(pos + lineWidth/2, -height);
            curColor = smallLineColor;
            curColor.a *= alphaCurve.Evaluate((pos + lineWidth / 2) / width);
            vert.color = curColor;
            quad[2] = vert;

            vert.position = new Vector3(pos + lineWidth/2, -(height - lineHeight/3));
            curColor = smallLineColor;
            curColor.a *= alphaCurve.Evaluate((pos + lineWidth / 2) / width);
            vert.color = curColor;
            quad[3] = vert;
            vh.AddUIVertexQuad(quad);
        }

        void DrawBlocks(float smallLinePos, int blockNum, VertexHelper vh)
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            float sWidth = beatWidth / divide;
            UIVertex vert = UIVertex.simpleVert;
            Color curColor;
            for (int i = 0; i < blockNum; i++)
            {
                UIVertex[] quad = new UIVertex[4];
                vert.position = new Vector3(smallLinePos + lineWidth + blockPadding, -(height - (i + 1) * blockHeight + blockPadding));
                curColor = noteColor;
                curColor.a *= alphaCurve.Evaluate((smallLinePos + lineWidth + blockPadding) / width);
                vert.color = curColor;
                quad[0] = vert;

                vert.position = new Vector3(smallLinePos + lineWidth + blockPadding, -(height - i * blockHeight));
                curColor = noteColor;
                curColor.a *= alphaCurve.Evaluate((smallLinePos + lineWidth + blockPadding) / width);
                vert.color = curColor;
                quad[1] = vert;

                vert.position = new Vector3(smallLinePos + sWidth - lineWidth - blockPadding, -(height - i * blockHeight));
                curColor = noteColor;
                curColor.a *= alphaCurve.Evaluate((smallLinePos + sWidth - lineWidth - blockPadding) / width);
                vert.color = curColor;
                quad[2] = vert;

                vert.position = new Vector3(smallLinePos + sWidth - lineWidth - blockPadding, -(height - (i + 1) * blockHeight + blockPadding));
                curColor = noteColor;
                curColor.a *= alphaCurve.Evaluate((smallLinePos + sWidth - lineWidth - blockPadding) / width);
                vert.color = curColor;
                quad[3] = vert;

                vh.AddUIVertexQuad(quad);
            }
        }
    }
}