using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace RockRainLive
{
    public class AudioLibrary
    {
        private SoundEffect explosion;
        private SoundEffect newMeteor;
        private SoundEffect menuBack;
        private SoundEffect menuSelect;
        private SoundEffect menuScroll;
        private SoundEffect powerGet;
        private SoundEffect powerShow;
        private Song backMusic;
        private Song startMusic;

        public SoundEffect Explosion
        {
            get { return explosion; }
        }

        public SoundEffect NewMeteor
        {
            get { return newMeteor; }
        }

        public SoundEffect MenuBack
        {
            get { return menuBack; }
        }

        public SoundEffect MenuSelect
        {
            get { return menuSelect; }
        }

        public SoundEffect MenuScroll
        {
            get { return menuScroll; }
        }

        public SoundEffect PowerGet
        {
            get { return powerGet; }
        }

        public SoundEffect PowerShow
        {
            get { return powerShow; }
        }

        public Song BackMusic
        {
            get { return backMusic; }
        }

        public Song StartMusic
        {
            get { return startMusic; }
        }

        public void LoadContent(ContentManager Content)
        {
            explosion = Content.Load<SoundEffect>("explosion");
            newMeteor = Content.Load<SoundEffect>("newmeteor");
            backMusic = Content.Load<Song>("backMusic");
            startMusic = Content.Load<Song>("startMusic");
            menuBack = Content.Load<SoundEffect>("menu_back");
            menuSelect = Content.Load<SoundEffect>("menu_select3");
            menuScroll = Content.Load<SoundEffect>("menu_scroll");
            powerShow = Content.Load<SoundEffect>("powershow");
            powerGet = Content.Load<SoundEffect>("powerget");
        }
    }
}