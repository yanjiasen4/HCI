using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;

//using DirectionArray = short;


namespace MusiCube
{

    public enum Axis
    {
        x, y, z
    }
    // 方向
    public enum Direction
    {
        xplus = 0x01, xminus = 0x02,
        yplus = 0x04, yminus = 0x08,
        zplus = 0x10, zminus = 0x20,
        illegal = 0x40
    }

    /* Singleton class: DirectionMap
     * 帮助查找每一个ID的方块的可弹出方向
     */
    public class DirectionMap
    {
        public Direction[] dirMap;
        private DirectionMap()
        {
            dirMap = new Direction[26];
            for (int i = 0; i < 26; i++)
            {
                dirMap[i] = 0x0;
            }
            int[] xplusBlock  = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            int[] xminusBlock = { 18, 19, 20, 21, 24 };
            int[] yplusBlock  = { 0, 1, 2, 9, 10, 11, 18, 19, 20 };
            int[] yminusBlock = { 6, 7, 8, 15, 24 };
            int[] zplusBlock  = { 0, 3, 6, 9, 12, 15, 18, 21, 24 };
            int[] zminusBlock = { 2, 5, 8, 11, 20 };
            List<int[]> dirArray = new List<int[]>();
            dirArray.Add(xplusBlock);
            dirArray.Add(xminusBlock);
            dirArray.Add(yplusBlock);
            dirArray.Add(yminusBlock);
            dirArray.Add(zplusBlock);
            dirArray.Add(zminusBlock);
            Direction currDir = Direction.xplus;
            for (int i = 0; i < 6; i++)
            {
                int[] dirBlock = dirArray[i];
                foreach(int k in dirBlock)
                {
                    dirMap[k] |= currDir;
                }
                currDir = (Direction)((int)currDir << 1);
            }
        }
        public static readonly DirectionMap instance = new DirectionMap(); 
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
        public static int var = 1;
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

        /* 
         * public API 
         */
        /* Get & Set function */
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

        /* Read and Write file function */
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
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
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

        /* edit beatmap function */
        public bool addNote(int t, int blockID, Axis a)
        {
            Note nt = new Note();
            nt.type = NoteType.Note;
            nt.id = blockID;
            Direction dirs = DirectionMap.instance.dirMap[blockID];
            Direction dir = Direction.illegal;
            switch (a)
            {
                case Axis.x:
                    dir = dirs & (Direction.xplus | Direction.xminus);  
                    break;
                case Axis.y:
                    dir = dirs & (Direction.yplus | Direction.yminus);
                    break;
                case Axis.z:
                    dir = dirs & (Direction.zplus | Direction.zminus);
                    break;
                default:
                    break;
            }
            if (dir == Direction.illegal)
                return false;
            nt.dir = dir;
            nt.duration = 0;
            PrintNote(nt);
            bool r = tl.addNote(t, nt);
            return r;
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

        // Get legal direction for block which ID is 'id'
        Direction GetNoteDir(int id)
        {
            return DirectionMap.instance.dirMap[id];
        }

        // For debug
        void PrintNote(Note nt)
        {
            Debug.Log(nt.dir);
        }
    }

    public class Song
    {
        public string songName;
       // public float songLength;
        public string backgroundFileName;
        public string audioFileName;
        public int diffCount = 0;
        public List<BeatMap> diffs;

        public string songPrefixPath;
        private string songFullPath;

        public Song(string name)
        {
            songName = name;
            diffs = new List<BeatMap>();

            string songSuffixPath = songName + ".mci";
            songPrefixPath = Application.streamingAssetsPath + "/Songs/" + songName;
            songFullPath = System.IO.Path.Combine(songPrefixPath, songSuffixPath);
        }

        public void readSong()
        {
            DirectoryInfo TheFolder = new DirectoryInfo(songPrefixPath);
            readSongInfo();
            foreach(FileInfo file in TheFolder.GetFiles())
            {
                if(file.Extension.Equals(".mcb"))
                {
                    diffs.Add(new BeatMap());
                    diffs[diffCount].readFromFile(file.FullName);
                    diffCount++;
                }    
            }
        }
        public void writeSong()
        {
            Debug.Log(songFullPath);
            if (!Directory.Exists(songPrefixPath))
            {
                Directory.CreateDirectory(songPrefixPath);
            }
            writeSongInfo();
            foreach(BeatMap bm in diffs)
            {
                bm.writeToFile(songFullPath + '/' + bm.difficultyName + ".mcb");
            }
        }
        void readSongInfo()
        {
            StreamReader reader = new StreamReader(songFullPath);
            char[] delim = { ':' };
            songName = reader.ReadLine().Split(delim)[1];
         //   songLength = float.Parse(reader.ReadLine().Split(delim)[1]);
            backgroundFileName = reader.ReadLine().Split(delim)[1];
            audioFileName = reader.ReadLine().Split(delim)[1];
            reader.Close();
        }
        void writeSongInfo()
        {
            FileStream fs = new FileStream(songFullPath, FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.WriteLine("SongName:" + songName);
        //  writer.WriteLine("SongLength:" + songLength.ToString());
            writer.WriteLine("BackgroundFileName:" + backgroundFileName);
            writer.WriteLine("AudioFileName:" + audioFileName);
            writer.Close();
        }
    }

    public class SongList
    {
        public List<Song> songList;

        private SongList()
        {
            songList = new List<Song>();
            string songPath = "Songs/";
            string songFullPath = System.IO.Path.Combine(Application.streamingAssetsPath, songPath);
            if(!Directory.Exists(songFullPath))
            {
                Directory.CreateDirectory(songFullPath);
            }
            DirectoryInfo TheFolder = new DirectoryInfo(songFullPath);
            DirectoryInfo[] songsDir = TheFolder.GetDirectories();
            foreach(DirectoryInfo song in songsDir)
            {
                Song readSong = new Song(song.Name);
                readSong.readSong();
                songList.Add(readSong);
            }
        }

        public void printAllSongs()
        {
            if(songList.Count != 0)
            {
                foreach(Song song in songList)
                {
                    Debug.Log(song.songName);
                    Debug.Log(song.audioFileName);
                    Debug.Log(song.backgroundFileName);
                }
            }
        }

        public static readonly SongList instance = new SongList();
    }
    
}