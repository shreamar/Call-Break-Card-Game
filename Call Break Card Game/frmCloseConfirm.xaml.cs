using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
