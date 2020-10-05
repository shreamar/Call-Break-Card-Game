using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Windows;

namespace Call_Break_Card_Game
{
    /// <summary>
    /// Interaction logic for frmPlaceBid.xaml
    /// </summary>
    public partial class frmPlaceBid : Window
    {
        public frmPlaceBid()
        {
            PlaySound("untap.wav");
            InitializeComponent();
        }

        private void btnPlaceBid_Click(object sender, RoutedEventArgs e)
        {
            PlaySound("switch5.wav");
            Game.PlaceBid(Game.HumanPlayerID, (int)sliderPlaceBid.Value);
            this.Close();
        }

        private void sliderPlaceBid_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PlaySound("chipLay1.wav");
            lblBid.Content = sliderPlaceBid.Value.ToString("n0");
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
