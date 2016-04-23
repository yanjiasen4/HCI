﻿using UnityEngine;
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
        public int duration; // only Slider's != 0
    }

    public class TimeLine
    {
        public TimeLine()
        {
            length = bpm = 0f;
            offset = 0;
            notes = new SortedDictionary<int, List<Note>>();
        }
        public float length;
        public float bpm;
        public int offset; // ms

        // key:   offset of notes(ms)
        // value: list of notes
        public SortedDictionary< int, List<Note> > notes;

        public bool addNote(int t, Note nt)
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
        public bool deleteNote(int t, Note nt)
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
        // Check if nt is a legal note
        private bool checkNote(Note nt)
        {
            switch(nt.dir)
            {
                case Direction.xplus:
                    {
                        if (nt.id >= 0 && nt.id <= 8)
                            return true;
                        break;
                    }
                case Direction.zplus:
                    {
                        if (nt.id == 3)
                            break;
                        break;
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
        public float fullLength;
        public float ar = 0f;
        public float od = 0f;

        public TimeLine tl = new TimeLine();

        public SortedDictionary<int, List<Note>> getNotes()
        {
            return tl.notes;
        }
        public float GetBpm()
        {
            return tl.bpm;
        }
        public void SetBpm(float bpm)
        {
            tl.bpm = bpm;
        }
        public int GetOffset()
        {
            return tl.offset;
        }
        public void SetOffset(int offset)
        {
            tl.offset = offset;
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

        public void addNote(int t, int blockID, Direction dir)
        {
            Note nt = new Note();
            nt.type = NoteType.Note;
            nt.id = blockID;
            nt.dir = dir;
            nt.duration = 0;
            bool r = tl.addNote(t, nt);
        }
        public void addNote(int t, Note nt)
        {
            bool r = tl.addNote(t, nt);
        }
        public void deleteNote(int t, Note nt)
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
            sw.WriteLine("bpm:" + tl.bpm.ToString());
            sw.WriteLine("offset:" + tl.offset.ToString());
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
            tl.bpm = float.Parse(parseMapHeadersItem(sr, "bpm"));
            tl.offset = int.Parse(parseMapHeadersItem(sr, "offset"));      
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
            int t = int.Parse(str[1]);
            if (str[0] == "0") // Note
            {
                nt.type = NoteType.Note;
                nt.id = int.Parse(str[2]);
                nt.dir = (Direction)(int.Parse(str[3]));
                nt.duration = 0;
            }
            if(str[0] == "1") // Slider
            {
                nt.type = NoteType.Slider;
                nt.id = int.Parse(str[1]);
                nt.dir = (Direction)(int.Parse(str[3]));
                nt.duration = int.Parse(str[4]);
            }
            if(str[0] == "2") // Plane
            {
                nt.type = NoteType.Plane;
                nt.id = int.Parse(str[1]);
                nt.dir = (Direction)(int.Parse(str[3]));
                nt.duration = 0;
            }
            addNote(t, nt);
        }
    }

    public class Song
    {
        public string songName;
        public float songLength;
        public BeatMap[] diffs;

        public void readSong()
        {
            DirectoryInfo TheFolder = new DirectoryInfo(songName);
            int diffcount = 0;
            foreach(FileInfo file in TheFolder.GetFiles())
            {
                diffs[diffcount] = new BeatMap();
                diffs[diffcount].readFromFile(file.FullName);
            }
        }
        public void writeSong()
        {
            if (!Directory.Exists(songName))
            {
                Directory.CreateDirectory(songName);
            }
            foreach(BeatMap bm in diffs)
            {
                bm.writeToFile(songName + '/' + bm.difficultyName);
            }
        }
    }
}