using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Call_Break_Card_Game
{
    /// <summary>
    /// Interaction logic for frmStartGame.xaml
    /// </summary>
    public partial class frmStartGame : Window
    {
        //Define music player and plath
        public MediaPlayer backgroundMusicPlayer = new MediaPlayer();       
        Uri backgroundMusicFilePath = new Uri("resources/sfx/DeerPortal-GamePlay.mp3", UriKind.Relative);

        public frmStartGame()
        {
            //PlaybackMusic();

            InitializeComponent();
        }

        private void sliderMaxHandsToPlay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblMaxHandsToPlay.Content = sliderMaxHandsToPlay.Value.ToString("n0");
            PlaySound("chipLay1.wav");
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            PlaySound("Digital Accept Button.wav");
            System.Threading.Thread.Sleep(200);
            Game.MaxHandsToPlay = int.Parse(lblMaxHandsToPlay.Content.ToString());

            MainWindow main = new MainWindow();
            main.Show();

            this.Close();
        }

        public void PlaybackMusic()
        {
            if (backgroundMusicFilePath != null)
            {
                backgroundMusicPlayer.Open(backgroundMusicFilePath);
                backgroundMusicPlayer.Position = TimeSpan.FromMilliseconds(100);
                backgroundMusicPlayer.Volume = 1;
                backgroundMusicPlayer.MediaEnded += new EventHandler(Media_Ended);//this line makes it play on loop
                backgroundMusicPlayer.Play();
            }
        }

        private void Media_Ended(object sender, EventArgs e)
        {
            backgroundMusicPlayer.Position = TimeSpan.FromMilliseconds(100);
            backgroundMusicPlayer.Play();
        }

        private void PlaySound(string fileName)
        {
            Uri uri = new Uri("/sfx/" + fileName, UriKind.Relative);
            SoundPlayer player = new SoundPlayer(GetResourceStream(uri.ToString()));
          
            //player.LoadAsync();
            player.Play();
        }

        //gets the path stream for files flagged as embedded resources
        protected static Stream GetResourceStream(string resourcePath)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<string> resourceNames = new List<string>(assembly.GetManifestResourceNames());

            resourcePath = resourcePath.Replace(@"/", ".");
            resourcePath = resourceNames.FirstOrDefault(r => r.Contains(resourcePath));

            if (resourcePath == null)
                throw new FileNotFoundException("Resource not found");

            return assembly.GetManifestResourceStream(resourcePath);
        }
    }
}
