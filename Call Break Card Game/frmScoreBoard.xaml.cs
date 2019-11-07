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
    /// Interaction logic for frmScoreBoard.xaml
    /// </summary>
    public partial class frmScoreBoard : Window
    {
        public frmScoreBoard()
        {
            InitializeComponent();
            Game.InitializeGame("you");
            Game.CurrentHand = 15;

            if (Game.CurrentHand == Game.MaxHandsToPlay)//change button label at the end of the game
            {
                btnOK.FontSize = 17;
                btnOK.Content = "Restart Game";
                btnOK.Margin = new Thickness(409, 587, 14.4, 4.6);

                //Make exit button visible
                btnExitGame.Visibility = Visibility.Visible;
                btnExitGame.Margin = new Thickness(409, 553, 14.4, 38.6);
            }

            lblBottomBar.Content = String.Format("Hands Played: [{0}/{1}]         Winner: [{2}]",
                Game.CurrentHand, Game.MaxHandsToPlay, Game.Players[Game.CumulativeWinner].Name);

            //Scoreboard
            ScoreBoard();
        }

        private void btnPlaceBid_Click(object sender, RoutedEventArgs e)
        {
            if (Game.CurrentHand == Game.MaxHandsToPlay)//if its the end of the game the button restarts the game
            {
                frmStartGame startGame = new frmStartGame();
                startGame.Show();
            }
            this.Close();

        }

        private void ScoreBoard()
        {
            grdScoreboard.RowDefinitions.Clear();
            grdScoreboard.ColumnDefinitions.Clear();

            //creates grid row
            for (int i = 0; i <= Game.MaxHandsToPlay + 1; i++)
            {
                grdScoreboard.RowDefinitions.Add(new RowDefinition());
            }

            //creates grid columns
            for (int i = 0; i < 4; i++)
            {
                grdScoreboard.ColumnDefinitions.Add(new ColumnDefinition());
            }

            //Fill Scoreboard
            for (int row = 0; row <= Game.MaxHandsToPlay + 1; row++)//First row is table head, last row is total
            {
                for (int col = 0; col < 4; col++)
                {
                    Label label = new Label();
                    label.HorizontalAlignment = HorizontalAlignment.Stretch;
                    label.VerticalAlignment = VerticalAlignment.Stretch;
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    label.VerticalContentAlignment = VerticalAlignment.Center;

                    if (row == 0)//Table head
                    {
                        label.Foreground = new SolidColorBrush(Colors.White);
                        label.Background = new SolidColorBrush(Colors.DarkGreen);
                        label.FontFamily = new FontFamily("Segoe UI");
                        label.FontWeight = FontWeights.Bold;
                        //label.FontStyle = FontStyles.Italic;
                        label.FontSize = 25 - Game.MaxHandsToPlay;

                        label.BorderThickness = new Thickness(0, 2, 0, 2);
                        label.BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue);

                        label.Content = Game.Players[col].Name;
                    }
                    else if (row < Game.MaxHandsToPlay + 1)//rows excluding the first and last
                    {
                        label.Foreground = Game.ScoreBoard[row - 1, col] < 0 ? new SolidColorBrush(Colors.OrangeRed) : new SolidColorBrush(Colors.White);
                        label.Background = (row % 2 == 0) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.ForestGreen);//alternating row colors
                        label.FontFamily = new FontFamily("Time");
                        label.FontSize = 15;

                        label.Content = (row - 1 < Game.CurrentHand) ? Game.ScoreBoard[row - 1, col].ToString("F1") : " ";//shows scores only until the current hand
                    }
                    else
                    {
                        label.Foreground = Game.CumulativeScore[col] < 0 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.LawnGreen);
                        label.Background = new SolidColorBrush(Colors.DarkCyan);
                        label.FontFamily = new FontFamily("Courier New");
                        label.FontSize = 15;
                        label.FontWeight = FontWeights.UltraBlack;

                        label.BorderThickness = new Thickness(0, 2, 0, 0);
                        label.BorderBrush = new SolidColorBrush(Colors.AliceBlue);

                        label.Content = Game.CumulativeScore[col].ToString("F1");
                    }

                    //add the required and appropriate labels to the current cell in grid
                    Grid.SetColumn(label, col);
                    Grid.SetRow(label, row);
                    grdScoreboard.Children.Add(label);
                }
            }
        }

        private void btnExitGame_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
