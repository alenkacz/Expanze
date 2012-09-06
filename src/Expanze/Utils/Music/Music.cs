using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Expanze.Utils.Music
{
    public enum SongEnum
    {
        menu
    }

    public class MusicManager
    {
        private static MusicManager inst;
        private Dictionary<string, Song> songs;
        Song playedSong;
        ContentManager content;

        private MusicManager()
        {
            playedSong = null;
            songs = new Dictionary<string, Song>();
            MediaPlayer.IsMuted = false;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.IsShuffled = true;
        }

        public static MusicManager Inst()
        {
            if (inst == null)
            {
                inst = new MusicManager();
            }
            return inst;
        }

        public void PlaySong(SongEnum songEnum)
        {
            string name = songEnum.ToString();
            StopSong();

            if (!songs.ContainsKey(name))
            {
                Thread loading = new Thread(X => LoadAndPlaySong(name));
                loading.Start();
            }
            else
            {
                playedSong = songs[name];
                MediaPlayer.Play(playedSong);
                //MediaPlayer.ActiveSongChanged += new EventHandler<EventArgs>(MediaPlayer_ActiveSongChanged);
            }
        }

        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            
        }

        public void StopSong()
        {
            if (playedSong != null)
            {
                MediaPlayer.Stop();
                playedSong = null;
            }
        }

        private void LoadAndPlaySong(string name)
        {
            string src = "Music/" + name;
            Song song = content.Load<Song>(src);
            songs[name] = song;
            playedSong = song;
            MediaPlayer.Play(playedSong);
        }

        public ContentManager Content {
            
            set
            {
                content = value;
            }
        }
    }
}
