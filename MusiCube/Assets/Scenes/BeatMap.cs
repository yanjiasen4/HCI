using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;

namespace MusiCube
{
    // 方向
    public enum Direction
    {
        xplus, xminus,
        yplus, yminus,
        zplus, zminus
    }
    // 基础的三种玩法
    public enum NoteType
    {
        Note,
        Slider,
        Plane
    }
    public struct Note
    {
        public NoteType type;
        public int id;
        public Direction dir;
        public double duration; // only Slider's != 0
    }

    public class TimeLine
    {
        public TimeLine()
        {
            length = bpm = offset = 0.0;
            notes = new SortedDictionary<double, List<Note>>();
        }
        public double length;
        public double bpm;
        public double offset;

        public SortedDictionary< double, List<Note> > notes;

        public bool addNote(double t, Note nt)
        {
            if(notes.ContainsKey(t))
            {
                if (notes[t].Contains(nt))
                    return false;
                else
                    notes[t].Add(nt);
            }
            else
            {
                List<Note> list = new List<Note>();
                list.Add(nt);
                notes.Add(t, list);
            }
            return true;
        }
        public bool deleteNote(double t, Note nt)
        {
            if(notes.ContainsKey(t))
            {
                if(notes[t].Contains(nt))
                {
                    notes[t].Remove(nt);
                    return true;
                }
            }
            return false;
        }
    }

    /*
     * Class: BeatMap
     *        a single difficulty of a song
     */
    public class BeatMap
    {
        public string difficultyName = "";
        public float ar = 0f;
        public float od = 0f;

        public TimeLine tl = new TimeLine();

        public SortedDictionary<double, List<Note>> getNotes()
        {
            return tl.notes;
        }

        public void readFromFile(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            parseMapHeaders(reader);
            char[] delim = { ','};
            string buffer = "";
            while (buffer != null)
            {
                buffer = reader.ReadLine();
                if (buffer == null)
                    break;
                string[] str = buffer.Split(delim);
                parseNote(str);
            }
            reader.Close();
        }
        public void writeToFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writeMapHeaders(writer);
            foreach (var item in tl.notes)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    writeNote(writer, item.Key, item.Value[i]);
                }
            }      
            writer.Close();
        }

        public void addNote(double t, Note nt)
        {
            bool r = tl.addNote(t, nt);
        }
        public void deleteNote(double t, Note nt)
        {
            bool r = tl.deleteNote(t, nt);
        }

        // read timeline from file
        private void readTimeLine(StreamReader sr)
        {
            tl = new TimeLine();
        }

        // write beatmap header info
        private void writeMapHeaders(StreamWriter sw)
        {
            sw.WriteLine("diffname:" + difficultyName);
            sw.WriteLine("ar:" + ar.ToString());
            sw.WriteLine("od:" + od.ToString());
        }
        // write a single note to file
        private void writeNote(StreamWriter sw, double t, Note nt)
        {
            string str = "";
            if(nt.type == NoteType.Note)
            {
                str = "0," + t.ToString() + "," + nt.id.ToString() + "," + ((int)nt.dir).ToString();
            }
            if(nt.type == NoteType.Slider)
            {
                str = "1," + t.ToString() + "," + nt.duration.ToString() + "," + nt.id.ToString() + "," + ((int)nt.dir).ToString();
            }
            if(nt.type == NoteType.Plane)
            {
                str = "2," + t.ToString() + "," + nt.id.ToString() + "," + ((int)nt.dir).ToString();
            }
            sw.WriteLine(str);
        }
        private void parseMapHeaders(StreamReader sr)
        {
            difficultyName = parseMapHeadersItem(sr, "diffname");
            ar = float.Parse(parseMapHeadersItem(sr, "ar"));
            od = float.Parse(parseMapHeadersItem(sr, "od"));
        }
        private string parseMapHeadersItem(StreamReader sr, string name)
        {
            string str = sr.ReadLine();
            
            if (str != null && str.Length > name.Length)
            {                
                char[] delim = { ':' };
                string[] res;
                res = str.Split(delim);
                return res[1];
            }            
            else
                return "";
        }
        private void parseNote(string[] str)
        {
            Note nt = new Note();
            float t = float.Parse(str[1]);
            if (str[0] == "0") // Note
            {
                nt.type = NoteType.Note;
                nt.id = int.Parse(str[2]);
                nt.dir = (Direction)(int.Parse(str[3]));
                nt.duration = 0f;
            }
            if(str[0] == "1") // Slider
            {
                nt.type = NoteType.Slider;
                nt.id = int.Parse(str[1]);
                nt.dir = (Direction)(int.Parse(str[3]));
                nt.duration = float.Parse(str[4]);
            }
            if(str[0] == "2") // Plane
            {
                nt.type = NoteType.Plane;
                nt.id = int.Parse(str[1]);
                nt.dir = (Direction)(int.Parse(str[3]));
                nt.duration = 0f;
            }
            addNote(t, nt);
        }
    }

    public class Song
    {
        public string songName;
        public BeatMap[] diffs;

        public void readSong()
        {

        }
    }
}