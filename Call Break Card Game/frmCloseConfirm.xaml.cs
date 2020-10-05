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
    public partial class frmCloseConfirm : Window
    {
        public frmCloseConfirm()
        {
            InitializeComponent();

            Loaded += frmCloseConfirm_Loaded;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            PlaySound("tick.wav");
            System.Threading.Thread.Sleep(800);
            this.Close();
            Application.Current.Shutdown();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            PlaySound("tap.wav");
            this.Close();
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

        private void frmCloseConfirm_Loaded(object sender, RoutedEventArgs e)
        {
            PlaySound("error.wav");
        }
    }
}
