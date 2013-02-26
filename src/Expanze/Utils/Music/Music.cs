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
        menu,
        ingame1,
    }

    public enum SoundEnum
    {
        building,
        button1,
        button2,
        coins,
        fort,
        market,
        mine,
        monastery,
        quarry,
        road,
        sawmill,
        stepherd,
        town,
        windmill,
        disaster,
        miracle, 
        fortcapture,
        fortdestroyhex,
        forttraining,
        upgrade,
        assembly
    }

    public class MusicManager
    {
        private static MusicManager inst;
        private Dictionary<string, Song> songs;
        private Dictionary<string, SoundEffect> sounds;
        Song playedSong;
        SoundEffect lastPlayedSound;
        ContentManager content;

        private MusicManager()
        {
            playedSong = null;
            songs = new Dictionary<string, Song>();
            lastPlayedSound = null;
            sounds = new Dictionary<string, SoundEffect>();
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

        public void PlaySound(SoundEnum soundEnum)
        {
            string name = soundEnum.ToString();

            if (!sounds.ContainsKey(name))
            {
                Thread loading = new Thread(X => LoadAndPlaySound(name));
                loading.Start();
            }
            else
            {
                lastPlayedSound = sounds[name];
                lastPlayedSound.Play();
            }
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
            MediaPlayer.Volume = 0.025f;
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

        private void LoadSound(string name)
        {
            string src = "Sounds/" + name;
            SoundEffect sound = content.Load<SoundEffect>(src);
            sounds[name] = sound;
        }
        private void LoadAndPlaySound(string name)
        {
            LoadSound(name);
            sounds[name].Play();
        }

        public ContentManager Content {
            
            set
            {
                content = value;
            }
        }
    }
}
