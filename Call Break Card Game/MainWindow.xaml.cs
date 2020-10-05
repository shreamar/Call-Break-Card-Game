using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Call_Break_Card_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //used to flag wether human player has played their turn via UI
        TaskCompletionSource<bool> hasHumanPlayed_Flag;

        public MainWindow()
        {
            //PlaybackMusic();
            
            InitializeComponent();

            //Dynamically set canvas width and height based on the window size/screen resolution
            canvasGame.Height = System.Windows.SystemParameters.PrimaryScreenHeight / 1.127937336814621; //(864/766)
            canvasGame.Width = System.Windows.SystemParameters.PrimaryScreenWidth / 1.003921568627451; //(1536/1530)

            //Dynamically set canvas margin based on window size
            canvasGame.Margin = new Thickness(0, (System.Windows.SystemParameters.PrimaryScreenHeight / 14.89655172413793), 0, 0);
            //Margin="0,58,0,0"

            //Dynamically set font size of big info label based on screen resolution/window size
            lblBigInfo_Center.FontSize = System.Windows.SystemParameters.PrimaryScreenHeight / 8.64; //FontSize="100" based on height

            //Dynamically change top bar properties based on screen resolution/window size
            lblTopBar.Height = System.Windows.SystemParameters.PrimaryScreenHeight / 16.30188679245283;//Height="53"
            //lblTopBar.Width = System.Windows.SystemParameters.PrimaryScreenWidth / 1.219047619047619;//Width="1260" 
            lblTopBar.FontSize = System.Windows.SystemParameters.PrimaryScreenHeight / 34.56;//FontSize="25" based on height

            //Initializes game
            Game.InitializeGame("You", Game.MaxHandsToPlay);
            /// MessageBox.Show(System.Windows.SystemParameters.PrimaryScreenHeight + " " + System.Windows.SystemParameters.PrimaryScreenWidth);
            _ = GamePlayAsync(this, new EventArgs());
        }

        /// <summary>
        /// Performs whole game play 
        /// </summary>
        private async Task GamePlayAsync(object sender, EventArgs e)
        {
            PlayBackGroundMusic();

            for (int currentHand = 0; currentHand < Game.MaxHandsToPlay; currentHand++)//Iteration of hands played
            {
                //updates current hand data in Game class
                //Game.CurrentHand = currentHand;

                Refresh_Canvas(Game.CurrentDealer, false, false, false);

                PointCurrentPlayer_Canvas(Game.CurrentPlayer);

                //Reinitialize components to restart hand
                Game.ReinitializeHand();
                //TestWindow();

                //Shows deck of card in the dealers side
                ShowDealReady_Deck();

                lblBigInfo_Center.Content = "Dealing Cards...";
                lblBigInfo_Center.Visibility = Visibility.Visible;
                //TestWindow();

                //Show players names and icons
                Show_PlayersName_Icon();

                //shows top bar
                Show_Info_TopBar();
                ///TestWindow();
                lblTopBar.IsEnabled = false;

                //creates pause effect
                await Task.Delay(TimeSpan.FromMilliseconds(1200));

                //Deals cards to all players
                DealCards();
                //TestWindow();

                //create pause effect
                await Task.Delay(1500);

                //changes label to placing bids
                lblBigInfo_Center.Content = "Placing Bids...";

                /// <summary>
                /// Place biddings before playing the hand
                /// </summary>
                for (int i = (Game.CurrentDealer + 1) % 4, counter = 0; counter < 4; counter++, i++)
                {
                    //update current player
                    Game.CurrentPlayer = Game.CurrentDealer;

                    int currentBidder = i % 4;
                    TestWindow();
                    if (currentBidder == Game.HumanPlayerID)//human player
                    {
                        //creates pause effect
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));

                        frmPlaceBid frmPlaceBid = new frmPlaceBid();
                        frmPlaceBid.ShowDialog();
                    }
                    else
                    {
                        //PlaceBids_Auto(currentBidder);
                        Game.PlaceBid(currentBidder, Game.Players[currentBidder].Bid_AI());

                        //creates pause effect
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));
                    }
                    //TestWindow();

                    Show_PlayersBids_WinCounts(true, true, currentBidder);
                }
                //creates pause effect of 2000ms
                await Task.Delay(TimeSpan.FromMilliseconds(500));

                //hides the placing bids label
                lblBigInfo_Center.Visibility = Visibility.Hidden;

                //Refresh canvas
                Refresh_Canvas(Game.CurrentDealer, true);
                TestWindow();
                for (int currentTrick = 0; currentTrick < 13; currentTrick++)//iteration of tricks played in a given hand
                {
                    for (int i = Game.CurrentDealer, j = 0; j < 4; i++, j++)//iteration of players
                    {
                        int currentPlayer = i % 4;

                        PointCurrentPlayer_Canvas(currentPlayer);

                        //Update Current Player
                        Game.CurrentPlayer = currentPlayer;

                        //creates pause effect
                        if (currentPlayer != Game.HumanPlayerID)
                        {
                            //await Task.Delay(TimeSpan.FromMilliseconds(300));
                        }

                        //Point Current Player
                        PointCurrentPlayer_Canvas(currentPlayer);

                        // for testing purpose only
                        string playables = Game.Players[currentPlayer].Name;
                        foreach (Card card in Game.Players[currentPlayer].Cards)
                        {
                            foreach (int id in Game.Players[currentPlayer].PlayableIDs)
                            {
                                if (id == card.ID)
                                {
                                    playables += " " + card.Name;
                                    break;
                                }
                            }
                        }
                        playables += "\r\nLead: " + (Game.CardIDtoCard(Game.LeadCardID) != null ? Game.CardIDtoCard(Game.LeadCardID).Name : "--") +
                            "\r\nPower: " + (Game.CardIDtoCard(Game.PowerCardID) != null ? Game.CardIDtoCard(Game.PowerCardID).Name : "--");
                        //MessageBox.Show(playables);

                        //creates pause effect of 2000ms
                        if (currentPlayer != Game.HumanPlayerID || Game.CurrentTrickWinner == Game.HumanPlayerID)
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(1000));
                        }

                        if (currentPlayer == Game.HumanPlayerID)
                        {
                            Game.Players[Game.HumanPlayerID].HasPlayed = false;

                            //humanPlayersTurn should be set to true bc the canvas is refreshed before the turn is played
                            //otherwise, the thrown cards in the middle of table will have logical problem (cards thrown will be misaligned 
                            //bc of miscalculation)
                            Refresh_Canvas(currentPlayer, true, false, true, true);

                            PointCurrentPlayer_Canvas(currentPlayer);

                            //await for human player to play his turn
                            hasHumanPlayed_Flag = new TaskCompletionSource<bool>();
                            await hasHumanPlayed_Flag.Task;

                        }
                        else
                        {
                            PlayCard_Bots(currentPlayer);

                            //creates pause effect
                            await Task.Delay(TimeSpan.FromMilliseconds(400));

                            //Refresh Canvas
                            Refresh_Canvas(currentPlayer, true);
                        }

                        ///testing
                        playables = "";
                        playables = "No. of cards in table: " + Game.CardsInTable.Count;
                        playables += "\r\nCards in table:";
                        for (int x = 0; x < Game.CardsInTable.Count; x++)
                        {
                            playables += " " + Game.CardsInTable[x].Name;
                        }
                        //playables += "\rLead: " + Game.CardIDtoCard(Game.LeadCardID) != null ? Game.CardIDtoCard(Game.LeadCardID).Name : "--" +
                        //   "\rPower: " + Game.CardIDtoCard(Game.PowerCardID) != null ? Game.CardIDtoCard(Game.PowerCardID).Name : "--";
                        //MessageBox.Show(playables);

                        TestWindow();
                    }
                    //creates pause effect
                    await Task.Delay(TimeSpan.FromMilliseconds(700));

                    //Process the results of the current trick
                    Game.ProcessTrickWinner();

                    //Update current player
                    Game.CurrentPlayer = Game.CurrentTrickWinner;

                    //Refresh Canvas and show trick winner animation
                    Refresh_Canvas(Game.CurrentTrickWinner, true, true, true, false, true);

                    //Clear cards from table in code
                    Game.CardsInTable.Clear();
                }
                //creates pause effect
                await Task.Delay(TimeSpan.FromMilliseconds(1000));

                //Updates Scoreboard at the end of the hand
                Game.UpdateScoreBoard();

                //if (currentHand+1 == Game.MaxHandsToPlay)
                {
                    //Shows score board
                    frmScoreBoard scoreBoard = new frmScoreBoard();
                    scoreBoard.ShowDialog();
                }
            }
        }


        /// <summary>
        /// Deals card to all players and show relevant information on canvas
        /// </summary>
        private void DealCards()
        {
            //Deals cards
            Game.DealCards();

            //clears canvas
            canvasGame.Children.Clear();

            //show player icons, names, bids and win counts
            Show_PlayersName_Icon();

            //play dealing sound
            PlaySound("cardFan2.wav");

            //Show bots cards with dealing animation
            ShowCardsOnCanvas_Bots(true);

            //Show human's cards with dealing animation
            ShowCardsOnCanvas_Human(true, false);

            //point current player
            PointCurrentPlayer_Canvas(Game.CurrentDealer);

            //Show top bar
            Show_Info_TopBar();
            //TestWindow();
        }

        /// <summary>
        /// Places random bid for selected player ID
        /// [Needs better algorithm to perform this task]
        /// </summary>
        private void PlaceBids_Auto(int PlayerID)
        {
            Random rnd = new Random();

            int counter = 0;
            foreach (Player player in Game.Players)
            {
                if (counter == PlayerID)
                {
                    //Random rand = new Random();
                    Game.PlaceBid(PlayerID, rnd.Next(1, 7));
                    break;
                }
                counter++;
            }
        }

        private void Refresh_Canvas(int currentPlayer, bool showCardsOnTable = true, bool showTrickAnimation = false, bool showBids = true,
            bool humanPlayersTurn = false, bool disableHumanPlayerCard = false)
        {
            //First clear the canvas
            canvasGame.Children.Clear();

            //Add names and Icons
            Show_PlayersName_Icon();

            //Add the current player pointer
            //PointCurrentPlayer_Canvas(currentPlayer);

            if (showBids)
            {
                //Add bids and win counts for players
                Show_PlayersBids_WinCounts();
            }

            //Add bot players cards
            if (showCardsOnTable)
            {
                ShowCardsOnCanvas_Bots(true, true);
            }
            else
            {
                ShowCardsOnCanvas_Bots();
            }

            //Add human player's cards
            if (currentPlayer == Game.HumanPlayerID && !Game.Players[Game.HumanPlayerID].HasPlayed && !disableHumanPlayerCard)
            {
                //if current player is human and they haven't played yet then enable card
                ShowCardsOnCanvas_Human(true, true, true, true);
            }
            else
            {
                ShowCardsOnCanvas_Human(false, false);                 //if not disable cards control
            }

            if (showCardsOnTable)//show cards on table while the trick is being played
            {
                int i = humanPlayersTurn ? (4 + currentPlayer - Game.CardsInTable.Count) % 4
                    : (4 + currentPlayer - Game.CardsInTable.Count + 1) % 4;
                for (int j = 0; j < Game.CardsInTable.Count; j++, i++)
                {
                    int id = i % 4;

                    if (showTrickAnimation)
                    {
                        //shows card translation animation at the end of the trick
                        Show_PlayedCards_Table(id, Game.CardsInTable[j], false, true, Game.CurrentTrickWinner);
                    }
                    else
                    {
                        Show_PlayedCards_Table(id, Game.CardsInTable[j], true);
                    }
                }
            }

            //Refresh top bar
            Show_Info_TopBar();
            ///TestWindow();
        }

        /// <summary>
        /// Plays card for bots
        /// </summary>
        /// <param name="currentPlayer"></param>
        private void PlayCard_Bots(int currentPlayer)
        {
            //Randomly selects a card from playables
            Random rnd = new Random();
            int cardID = Game.Players[currentPlayer].SelectPlayingCard_AI();

            //Play the randomly selected card
            Game.Players[currentPlayer].PlayCard(cardID);

            Refresh_Canvas(currentPlayer, true);
            //Show_PlayedCards_Table(currentPlayer, cardID);
        }

        private void btnDealCards_Click(object sender, RoutedEventArgs e)
        {
        }

        private void markPlayables()
        {
            /////////////////////////////////////
        }

        private void playCardsNow()
        {
            ListBox lbCards = (ListBox)this.FindName("lbCards" + Game.CurrentDealer);

            foreach (int a in Game.Players[Game.CurrentDealer].PlayableIDs)
            {
                if (1 == 1)//lbCards.Items[lbCards.SelectedIndex].ToString == Game.CardIDtoCard(a).Name)
                {
                    lbCards.Items.Clear();
                    Game.Players[Game.CurrentDealer].PlayCard(a);
                    break;
                }
            }
            showPlayables();
            showPlayersCards(Game.CurrentDealer);
        }

        private void showPlayables()
        {
            //lbPlayable.Items.Clear();

            foreach (Card card in Game.CardsInTable)
            {
                // lbPlayable.Items.Add(card.Name);
            }
        }

        private void showPlayersCards(int id)
        {
            ListBox lbCards = (ListBox)this.FindName("lbCards" + id);

            lbCards.Items.Clear();

            foreach (Card card in Game.Players[id].Cards)
            {
                lbCards.Items.Add(card.Name);
            }
        }


        /// <summary>
        /// Maps cards to the corresponding .png files
        /// </summary>
        /// <param name="card"></param>
        /// <returns>filename of the corresponding card</returns>
        private string MapCardToFile(Card card)
        {
            string cardNumber = "", cardSuit = "", number = "";
            if ((int)card.Number >= 0 && (int)card.Number < 9)
            {
                cardNumber = ((int)card.Number + 2).ToString();
            }
            else
            {
                cardNumber = card.Number.ToString().ToLower();
            }

            //since the card with pictures of jack, queen, king are in the same card name ending in 2
            //e.g. King of Hearts has no picture in "king_of_hearts.png", but has image of kind in "king_of_hearts2.png"
            if ((int)card.Number >= 9 && (int)card.Number < 12)
            {
                number = "2";
            }

            cardSuit = card.Suit.ToString().ToLower() + "s";

            return "/cards/" + cardNumber + "_of_" + cardSuit + number + ".png";
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            PlaySound("playcard.wav");
            Image image = sender as Image;
            //image.Source = (ImageSource)img.FindResource("ImgBtnLightbulbOn");
            Canvas.SetTop(image, -30);
            Canvas.SetLeft(image, 0);
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            //image.Source = (ImageSource)img.FindResource("ImgBtnLightbulbOn");         
            Canvas.SetTop(image, 0);
            Canvas.SetLeft(image, 0);
        }

        /// <summary>
        /// Drop shadow effect on card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_DropShadowEffect(object sender, byte a = 0, byte r = 0, byte g = 0, byte b = 0,
            double direction = 320, double shadowdepth = 0, double opacity = 50, double blurradius = 15)
        {
            Image image = sender as Image;
            UIElement uie = image;

            //card edge glowing effect
            uie.Effect = new DropShadowEffect
            {
                Color = new Color { A = a, R = r, G = g, B = b },
                Direction = direction,
                ShadowDepth = shadowdepth,
                Opacity = opacity,
                BlurRadius = blurradius
            };

            //clears effect
            //image.ClearValue(EffectProperty);
        }

        /// <summary>
        /// Blur effect on card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_BitmapBlurEffect(object sender)
        {
            Image image = sender as Image;
            UIElement uie = image;

            uie.Effect = new BlurEffect
            {
                KernelType = KernelType.Gaussian,
                Radius = 5,
                RenderingBias = RenderingBias.Performance
            };

            //disable image when blurred
            image.IsEnabled = false;
        }

        private void Image_EmbossBitmapEffect(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
        }

        /// <summary>
        /// Shows interactive human cards on canvas
        /// </summary>
        private void ShowCardsOnCanvas_Human(bool showAnimation = false, bool enableCards = true, bool inGameAnimation = false, bool activePlayer = false)
        {
            //canvasGame.Children.Clear();

            int counter = 0;
            foreach (Card card in Game.Players[Game.HumanPlayerID].Cards)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MapCardToFile(card), UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = System.Windows.SystemParameters.PrimaryScreenWidth / 10.24; //150
                image.Height = System.Windows.SystemParameters.PrimaryScreenHeight / 4.547368421052632; //190

                image.HorizontalAlignment = HorizontalAlignment.Center;
                image.VerticalAlignment = VerticalAlignment.Bottom;

                //set animation speed to either show it or not
                double animationSpeed = showAnimation ? 1 : 0;

                if (inGameAnimation)//animation of reducing card when a card is played
                {
                    AnimateCardTranslation(image, card.XPos, card.YPos, ((canvasGame.Width - (image.Width +
                   (Game.Players[Game.HumanPlayerID].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenWidth / 13.96363636363636))) / 2)
                   + (System.Windows.SystemParameters.PrimaryScreenWidth / 13.96363636363636) * counter, (canvasGame.Height - image.Height) + (System.Windows.SystemParameters.PrimaryScreenHeight / 66.46153846153846), 0.25); //110  // 110 //13
                }
                else
                {
                    //Animates card dealing to human player
                    AnimateCardTranslation(image, canvasGame.Width / 2, (canvasGame.Height - image.Height) + (System.Windows.SystemParameters.PrimaryScreenHeight / 66.46153846153846), ((canvasGame.Width - (image.Width + //13
                        (Game.Players[Game.HumanPlayerID].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenWidth / 13.96363636363636))) / 2) //110
                        + (System.Windows.SystemParameters.PrimaryScreenWidth / 13.96363636363636) * counter, (canvasGame.Height - image.Height) + (System.Windows.SystemParameters.PrimaryScreenHeight / 66.46153846153846), animationSpeed); //110 //13
                }

                //record the current postion of the card
                card.XPos = (((canvasGame.Width - (image.Width +
                   (Game.Players[Game.HumanPlayerID].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenWidth / 13.96363636363636))) / 2)
                   + (System.Windows.SystemParameters.PrimaryScreenWidth / 13.96363636363636) * counter); //110 //110
                card.YPos = ((canvasGame.Height - image.Height) + (System.Windows.SystemParameters.PrimaryScreenHeight / 66.46153846153846)); //13

                //enables or disables human players cards
                image.IsEnabled = enableCards;

                if (activePlayer)//shows blurr effect only when human player is the active player
                {
                    //Finds out if the given card is playable card or not
                    int match = 0;
                    foreach (int ndx in Game.Players[Game.HumanPlayerID].PlayableIDs)
                    {
                        if (card.ID == ndx)
                        {
                            match++;
                            break;
                        }
                    }

                    //blurrs and disables the card if there is no match with playabel list
                    if (match == 0)
                    {
                        Image_BitmapBlurEffect(image);
                    }
                }

                //places human player's cards in the center bottom of the canvas
                //image.Margin = new Thickness(((canvasGame.Width - (image.Width +
                //(Game.Players[Game.HumanPlayerID].CardCount - 1) * 110)) / 2) +
                //  110 * counter, (canvasGame.Height - image.Height) + 13, 0, 0);

                //image.Margin = new Thickness(0, 0, 0, 0);

                //changes cursor to hand cursor when pointed at cards
                image.Cursor = Cursors.Hand;

                //slightly pulls up the given card when mouse enters
                image.MouseEnter += new MouseEventHandler(Image_MouseEnter);
                //puts the card back down when mouse leaves
                image.MouseLeave += new MouseEventHandler(Image_MouseLeave);

                image.MouseDown += new MouseButtonEventHandler(Show_PlayedCards_Human);

                //adds the card to the canvas
                canvasGame.Children.Add(image);

                counter++;
            }
        }

        /// <summary>
        /// Show cards(unrevealed) of bots on canvas
        /// </summary>
        private void ShowCardsOnCanvas_Bots(bool showAnimation = false, bool inGameAnimation = false)
        {
            //canvasGame.Children.Clear();

            string faceDownCard = "/cards/extras/Back.png";

            //set animation speed to either show it or not
            double animationSpeed = showAnimation ? 1 : 0;

            //start point is after the human player's id
            for (int i = (Game.HumanPlayerID + 1) % 4, j = 0; j < 3; i++, j++)
            {
                //bot players id
                int id = i % 4;

                if (j == 0)//player on right handside
                {
                    for (int counter = 0; counter < Game.Players[id].CardCount; counter++)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(faceDownCard, UriKind.Relative);
                        bitmap.EndInit();
                        Image image = new Image();
                        image.Source = bitmap;
                        image.Width = (System.Windows.SystemParameters.PrimaryScreenWidth / 21.94285714285714);//70
                        image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 5.76);//150

                        image.HorizontalAlignment = HorizontalAlignment.Left;

                        if (inGameAnimation)//animation of reducing card when a card is played
                        {
                            AnimateCardTranslation(image, Game.Players[id].Cards[counter].XPos, Game.Players[id].Cards[counter].YPos,
                                canvasGame.Width - image.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 307.2), ((canvasGame.Height -                                                  //5
                                (((Game.Players[i].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) + image.Height)) / 2) + (counter * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) - (System.Windows.SystemParameters.PrimaryScreenHeight / 17.28), 0.25); //28 //28 //50
                        }
                        else
                        {
                            //show animation of cards being dealt
                            AnimateCardTranslation(image, canvasGame.Width - image.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 307.2), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 8.64), canvasGame.Width - image.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 307.2), ((canvasGame.Height -                                                    //5 //100 //5
                            (((Game.Players[i].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) + image.Height)) / 2) + (counter * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) - (System.Windows.SystemParameters.PrimaryScreenHeight / 17.28), animationSpeed); //28 //28 //50
                        }

                        //record the current position of the card in the canvas
                        Game.Players[id].Cards[counter].XPos = (canvasGame.Width - image.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 307.2));    //5
                        Game.Players[id].Cards[counter].YPos = (((canvasGame.Height -
                            (((Game.Players[i].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) + image.Height)) / 2) + (counter * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) - (System.Windows.SystemParameters.PrimaryScreenHeight / 17.28));      //28 //28 //50

                        image.Margin = new Thickness(0, 0, 0, 0);

                        canvasGame.Children.Add(image);
                    }
                }
                else if (j == 1)//player on the over the top
                {
                    for (int counter = 0; counter < Game.Players[id].CardCount; counter++)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(faceDownCard, UriKind.Relative);
                        bitmap.EndInit();
                        Image image = new Image();
                        image.Source = bitmap;
                        image.Width = (System.Windows.SystemParameters.PrimaryScreenWidth / 17.06666666666667);//90
                        image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 8.64);//100

                        image.HorizontalAlignment = HorizontalAlignment.Center;
                        image.VerticalAlignment = VerticalAlignment.Top;

                        if (inGameAnimation)////animation of reducing card when a card is played
                        {
                            AnimateCardTranslation(image, Game.Players[id].Cards[counter].XPos, Game.Players[id].Cards[counter].YPos,
                                ((canvasGame.Width - (image.Width + (Game.Players[id].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenWidth / 43.88571428571429))) / 2) + ((System.Windows.SystemParameters.PrimaryScreenWidth / 43.88571428571429) * counter), 0, 0.25); //35 //35
                        }
                        else
                        {
                            //places bot2 player's cards in the center top of the canvas
                            AnimateCardTranslation(image, canvasGame.Width / 2, 0, ((canvasGame.Width - (image.Width +
                                (Game.Players[id].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenWidth / 43.88571428571429))) / 2) + ((System.Windows.SystemParameters.PrimaryScreenWidth / 43.88571428571429) * counter), 0, animationSpeed);
                            //35 //35
                        }

                        //record the current position of the card in the canvas
                        Game.Players[id].Cards[counter].XPos = (int)(((canvasGame.Width - (image.Width + (Game.Players[id].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenWidth / 43.88571428571429))) / 2) + ((System.Windows.SystemParameters.PrimaryScreenWidth / 43.88571428571429) * counter)); //35 //35
                        Game.Players[id].Cards[counter].YPos = 0;

                        image.Margin = new Thickness(0, 0, 0, 0);

                        canvasGame.Children.Add(image);
                    }
                }
                else if (j == 2)//player on left handside
                {
                    for (int counter = 0; counter < Game.Players[id].CardCount; counter++)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(faceDownCard, UriKind.Relative);
                        bitmap.EndInit();
                        Image image = new Image();
                        image.Source = bitmap;
                        image.Width = (System.Windows.SystemParameters.PrimaryScreenWidth / 21.94285714285714);//70
                        image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 5.76);//150

                        image.HorizontalAlignment = HorizontalAlignment.Left;

                        if (inGameAnimation)//animation of reducing card when a card is played
                        {
                            AnimateCardTranslation(image, Game.Players[id].Cards[counter].XPos, Game.Players[id].Cards[counter].YPos, (System.Windows.SystemParameters.PrimaryScreenWidth / 153.6), ((canvasGame.Height -           //10
                           (((Game.Players[id].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) + image.Height)) / 2) + (counter * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) - (System.Windows.SystemParameters.PrimaryScreenHeight / 17.28), 0.25);            //28 //28 //50
                        }
                        else
                        {
                            //places bot1 player's cards in the right of the canvas
                            AnimateCardTranslation(image, (System.Windows.SystemParameters.PrimaryScreenWidth / 153.6), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 8.64), (System.Windows.SystemParameters.PrimaryScreenWidth / 153.6), ((canvasGame.Height -              //10 //100 //10
                            (((Game.Players[id].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) + image.Height)) / 2) + (counter * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) - (System.Windows.SystemParameters.PrimaryScreenHeight / 17.28), animationSpeed);         //28 //28 //50
                        }

                        //record the current position the card on the canvas
                        Game.Players[id].Cards[counter].XPos = (System.Windows.SystemParameters.PrimaryScreenWidth / 153.6); //10
                        Game.Players[id].Cards[counter].YPos = (((canvasGame.Height -
                         (((Game.Players[id].CardCount - 1) * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) + image.Height)) / 2) + (counter * (System.Windows.SystemParameters.PrimaryScreenHeight / 30.85714285714286)) - (System.Windows.SystemParameters.PrimaryScreenHeight / 17.28));                            //28 //28 //50

                        image.Margin = new Thickness(0, 0, 0, 0);

                        canvasGame.Children.Add(image);
                    }
                }
            }
        }

        /// <summary>
        /// Shows player's name and icon on the canvas
        /// </summary>
        private void Show_PlayersName_Icon()
        {
            for (int i = (Game.HumanPlayerID) % 4, j = 0; j < 4; i++, j++)
            {
                //player ids
                int id = i % 4;
                string personIcon = "/cards/personicons/personicon" + Game.Players[id].IconNumber + ".png";

                if (j == 0)//human player
                {
                    //label for name
                    Label name = new Label();
                    name.Content = "[You]";
                    name.HorizontalAlignment = HorizontalAlignment.Center;
                    name.VerticalAlignment = VerticalAlignment.Bottom;
                    name.Margin = new Thickness((canvasGame.Width / 2) - (System.Windows.SystemParameters.PrimaryScreenWidth / 8.533333333333333), canvasGame.Height - (System.Windows.SystemParameters.PrimaryScreenHeight / 3.6), 0, 0); //180 //240
                    name.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 19.2); //45 based on height
                    name.FontWeight = FontWeights.Bold;
                    name.FontFamily = new FontFamily("Georgia");
                    name.Foreground = Brushes.DarkGreen;

                    //icon
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("/cards/personicons/personicon.png", UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = (System.Windows.SystemParameters.PrimaryScreenHeight / 12.34285714285714);//70 based on height
                    image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 12.34285714285714);//70 based on height
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.Margin = new Thickness(-(System.Windows.SystemParameters.PrimaryScreenWidth / 30.72) + canvasGame.Width / 2, canvasGame.Height - (System.Windows.SystemParameters.PrimaryScreenHeight / 3.456), 0, 0); //50 //250

                    canvasGame.Children.Add(name);
                    canvasGame.Children.Add(image);
                }
                else if (j == 1)//player on the right side
                {
                    //label for name
                    Label name = new Label();
                    name.Content = String.Format("[{0}]", Game.Players[id].Name);//Game.Players[id].Name);
                    name.HorizontalAlignment = HorizontalAlignment.Right;
                    name.VerticalAlignment = VerticalAlignment.Bottom;
                    name.Margin = new Thickness(canvasGame.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 7.492682926829268), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 4.214634146341463), 0, 0); //205 //205
                    name.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 24.68571428571429); //35 based on height
                    name.FontWeight = FontWeights.Bold;
                    name.FontFamily = new FontFamily("Georgia");
                    name.Foreground = Brushes.SeaGreen;

                    //icon
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(personIcon, UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = (System.Windows.SystemParameters.PrimaryScreenHeight / 12.34285714285714);//70 based on height
                    image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 12.34285714285714);//70 based on height
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.Margin = new Thickness(canvasGame.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 9.6), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 5.574193548387097), 0, 0); //160 //155

                    canvasGame.Children.Add(name);
                    canvasGame.Children.Add(image);
                }
                else if (j == 2)//player over the top
                {
                    //label for name
                    Label name = new Label();
                    name.Content = "[" + Game.Players[id].Name + "]";
                    name.HorizontalAlignment = HorizontalAlignment.Center;
                    name.VerticalAlignment = VerticalAlignment.Bottom;
                    name.Margin = new Thickness((canvasGame.Width / 2) - (System.Windows.SystemParameters.PrimaryScreenWidth / 8.777142857142857), (System.Windows.SystemParameters.PrimaryScreenHeight / 7.854545454545455), 0, 0);                  //175 //110
                    name.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 24.68571428571429); //35 based on height
                    name.FontWeight = FontWeights.Bold;
                    name.FontFamily = new FontFamily("Georgia");
                    name.Foreground = Brushes.DarkGreen;

                    //icon
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(personIcon, UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = (System.Windows.SystemParameters.PrimaryScreenHeight / 12.34285714285714);//70 based on height
                    image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 12.34285714285714);//70 based on height
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.Margin = new Thickness(canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 30.72), (System.Windows.SystemParameters.PrimaryScreenHeight / 8.228571428571429), 0, 0);               //50 //105

                    canvasGame.Children.Add(name);
                    canvasGame.Children.Add(image);
                }
                else if (j == 3)//player on the left hand side
                {
                    //label for name
                    Label name = new Label();
                    name.Content = String.Format("[{0}]", Game.Players[id].Name);//Game.Players[id].Name);
                    name.HorizontalAlignment = HorizontalAlignment.Right;
                    name.VerticalAlignment = VerticalAlignment.Bottom;
                    name.Margin = new Thickness((System.Windows.SystemParameters.PrimaryScreenWidth / 19.2), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 4.214634146341463), 0, 0);          //80 //205
                    name.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 24.68571428571429); //35 based on height
                    name.FontWeight = FontWeights.Bold;
                    name.FontFamily = new FontFamily("Georgia");
                    name.Foreground = Brushes.SeaGreen;

                    //icon
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(personIcon, UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = (System.Windows.SystemParameters.PrimaryScreenHeight / 12.34285714285714);//70 based on height
                    image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 12.34285714285714);//70 based on height
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.Margin = new Thickness((System.Windows.SystemParameters.PrimaryScreenWidth / 17.06666666666667), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 5.574193548387097), 0, 0);            //90 //155

                    canvasGame.Children.Add(name);
                    canvasGame.Children.Add(image);
                }
            }
        }

        /// <summary>
        /// Shows bids and win counts of all players on the canvas
        /// </summary>
        private void Show_PlayersBids_WinCounts(bool animate = false, bool showOnlyOne = false, int playerID = 0)
        {
            if (animate)
            {
                PlaySound("chipsHandle2.wav");
            }

            for (int i = (Game.HumanPlayerID) % 4, j = -1; j < 3; i++, j++)
            {
                //players id
                int id = i % 4;

                //jumps interation to the required player when only one players info have to be shown
                if (showOnlyOne)
                {
                    i += (4 - id + playerID) % 4;
                    j += (4 - id + playerID) % 4;
                }

                //updated player id
                id = i % 4;

                string personIcon = "/cards/personicons/personicon" + Game.Players[id].IconNumber + ".png";

                if (j == -1)//human player
                {
                    Label bid = new Label();
                    bid.Content = "Win: " + Game.TricksWon[Game.HumanPlayerID];
                    bid.HorizontalAlignment = HorizontalAlignment.Center;
                    bid.VerticalAlignment = VerticalAlignment.Bottom;
                    bid.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 57.6);//15 based on height
                    bid.FontWeight = FontWeights.Bold;
                    bid.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    bid.Background = Brushes.DeepSkyBlue;

                    Label score = new Label();
                    score.Content = "Bid: " + Game.Bidding[Game.HumanPlayerID];
                    score.HorizontalAlignment = HorizontalAlignment.Center;
                    score.VerticalAlignment = VerticalAlignment.Bottom;

                    score.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 57.6);//15 based on height
                    score.FontWeight = FontWeights.Bold;
                    score.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    score.Background = Brushes.IndianRed;

                    if (animate)
                    {
                        AnimateCardTranslation(bid, canvasGame.Width, canvasGame.Height - (System.Windows.SystemParameters.PrimaryScreenHeight / 3.6), (canvasGame.Width / 2) + (System.Windows.SystemParameters.PrimaryScreenWidth / 76.8), canvasGame.Height - (System.Windows.SystemParameters.PrimaryScreenHeight / 3.6), 0.5);              //240 //20 //240 
                        AnimateCardTranslation(score, canvasGame.Width, canvasGame.Height - (System.Windows.SystemParameters.PrimaryScreenHeight / 4.114285714285714), (canvasGame.Width / 2) + (System.Windows.SystemParameters.PrimaryScreenWidth / 76.8), canvasGame.Height - (System.Windows.SystemParameters.PrimaryScreenHeight / 4.114285714285714), 0.5);                    //210 //20 //210
                    }
                    else
                    {
                        bid.Margin = new Thickness((canvasGame.Width / 2) + (System.Windows.SystemParameters.PrimaryScreenWidth / 76.8), canvasGame.Height - (System.Windows.SystemParameters.PrimaryScreenHeight / 3.6), 0, 0); //20 //240
                        score.Margin = new Thickness((canvasGame.Width / 2) + (System.Windows.SystemParameters.PrimaryScreenWidth / 76.8), canvasGame.Height - (System.Windows.SystemParameters.PrimaryScreenHeight / 4.114285714285714), 0, 0); //20 //210
                    }

                    canvasGame.Children.Add(bid);
                    canvasGame.Children.Add(score);
                }
                else if (j == 0)//player on right handside
                {
                    Label bid = new Label();
                    bid.Content = "Win: " + Game.TricksWon[id];
                    bid.HorizontalAlignment = HorizontalAlignment.Center;
                    bid.VerticalAlignment = VerticalAlignment.Bottom;
                    bid.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 57.6);//15 based on height
                    bid.FontWeight = FontWeights.Bold;
                    bid.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    bid.Background = Brushes.DeepSkyBlue;

                    Label score = new Label();
                    score.Content = "Bid: " + Game.Bidding[id];
                    score.HorizontalAlignment = HorizontalAlignment.Center;
                    score.VerticalAlignment = VerticalAlignment.Bottom;
                    score.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 57.6);//15 based on height
                    score.FontWeight = FontWeights.Bold;
                    score.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    score.Background = Brushes.IndianRed;

                    if (animate)
                    {
                        AnimateCardTranslation(bid, canvasGame.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 10.24), 0, canvasGame.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 10.24), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 10.8), 0.4); //150 //150 //80
                        AnimateCardTranslation(score, canvasGame.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 10.24), 0, canvasGame.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 10.24), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 17.28), 0.4); //150 //150 //50
                    }
                    else
                    {
                        bid.Margin = new Thickness(canvasGame.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 10.24), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 10.8), 0, 0); //150 //80
                        score.Margin = new Thickness(canvasGame.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 10.24), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 17.28), 0, 0); //150 //50
                    }

                    canvasGame.Children.Add(bid);
                    canvasGame.Children.Add(score);
                }
                else if (j == 1)//player over the top
                {
                    Label bid = new Label();
                    bid.Content = "Win: " + Game.TricksWon[id];
                    bid.HorizontalAlignment = HorizontalAlignment.Center;
                    bid.VerticalAlignment = VerticalAlignment.Bottom;
                    bid.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 57.6);//15 based on height
                    bid.FontWeight = FontWeights.Bold;
                    bid.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    bid.Background = Brushes.DeepSkyBlue;

                    Label score = new Label();
                    score.Content = "Bid: " + Game.Bidding[id];
                    score.HorizontalAlignment = HorizontalAlignment.Center;
                    score.VerticalAlignment = VerticalAlignment.Bottom;
                    score.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 57.6);//15 based on height
                    score.FontWeight = FontWeights.Bold;
                    score.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    score.Background = Brushes.IndianRed;

                    if (animate)
                    {
                        AnimateCardTranslation(bid, 0, (System.Windows.SystemParameters.PrimaryScreenHeight / 7.854545454545455), (canvasGame.Width / 2) + (System.Windows.SystemParameters.PrimaryScreenWidth / 61.44), (System.Windows.SystemParameters.PrimaryScreenHeight / 7.854545454545455), 0.5); //110 //25 //110
                        AnimateCardTranslation(score, 0, (System.Windows.SystemParameters.PrimaryScreenHeight / 6.171428571428571), (canvasGame.Width / 2) + (System.Windows.SystemParameters.PrimaryScreenWidth / 61.44), (System.Windows.SystemParameters.PrimaryScreenHeight / 6.171428571428571), 0.5); //140 //25 //140
                    }
                    else
                    {
                        bid.Margin = new Thickness((canvasGame.Width / 2) + (System.Windows.SystemParameters.PrimaryScreenWidth / 61.44), (System.Windows.SystemParameters.PrimaryScreenHeight / 7.854545454545455), 0, 0); //25 //110
                        score.Margin = new Thickness((canvasGame.Width / 2) + (System.Windows.SystemParameters.PrimaryScreenWidth / 61.44), (System.Windows.SystemParameters.PrimaryScreenHeight / 6.171428571428571), 0, 0); //25 //140
                    }

                    canvasGame.Children.Add(bid);
                    canvasGame.Children.Add(score);
                }
                else if (j == 2)//player on the left side
                {
                    Label bid = new Label();
                    bid.Content = "Win: " + Game.TricksWon[id];
                    bid.HorizontalAlignment = HorizontalAlignment.Center;
                    bid.VerticalAlignment = VerticalAlignment.Bottom;
                    bid.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 57.6);//15 based on height
                    bid.FontWeight = FontWeights.Bold;
                    bid.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    bid.Background = Brushes.DeepSkyBlue;

                    Label score = new Label();
                    score.Content = "Bid: " + Game.Bidding[id];
                    score.HorizontalAlignment = HorizontalAlignment.Center;
                    score.VerticalAlignment = VerticalAlignment.Bottom;
                    score.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 57.6);//15 based on height
                    score.FontWeight = FontWeights.Bold;
                    score.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    score.Background = Brushes.IndianRed;

                    if (animate)
                    {
                        AnimateCardTranslation(bid, (System.Windows.SystemParameters.PrimaryScreenWidth / 17.06666666666667), canvasGame.Height, (System.Windows.SystemParameters.PrimaryScreenWidth / 17.06666666666667), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 10.8), 0.4); //90 //90 //80
                        AnimateCardTranslation(score, (System.Windows.SystemParameters.PrimaryScreenWidth / 17.06666666666667), canvasGame.Height, (System.Windows.SystemParameters.PrimaryScreenWidth / 17.06666666666667), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 17.28), 0.4); //90 //90 //50
                    }
                    else
                    {
                        bid.Margin = new Thickness((System.Windows.SystemParameters.PrimaryScreenWidth / 17.06666666666667), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 10.8), 0, 0); //90 //80
                        score.Margin = new Thickness((System.Windows.SystemParameters.PrimaryScreenWidth / 17.06666666666667), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 17.28), 0, 0); //90 //50
                    }

                    canvasGame.Children.Add(bid);
                    canvasGame.Children.Add(score);
                }

                //breaks loop if the required player is found and job is done
                if (showOnlyOne)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Shows played cards on the table given the playerID and cardID
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="cardID"></param>
        private void Show_PlayedCards_Table(int playerID, Card playedCard, bool showThrowAnimation = false, bool animateTrickWin = false, int winnerID = 0)
        {
            if (showThrowAnimation && !playedCard.IsPlayed)
            {
                PlaySound("cardSlide8.wav");
            }

            if (animateTrickWin)
            {
                PlaySound("cardSlide7.wav");
            }
            //playerID = Game.HumanPlayerID;//only for testing purpose

            Random rnd = new Random();
            int rndm = rnd.Next(99999);

            double directionX = 0, directionY = 0;
            //decides the direction cards go when trick is won
            if (winnerID == Game.HumanPlayerID) // when winner is human player
            {
                directionX = canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 34.13333333333333); //45
                directionY = canvasGame.Height;
            }
            else if (winnerID == (Game.HumanPlayerID + 1) % 4) //winner is player on the right
            {
                directionX = canvasGame.Width + (System.Windows.SystemParameters.PrimaryScreenWidth / 15.36); //100
                directionY = canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 5.4); //160
            }
            else if (winnerID == (Game.HumanPlayerID + 2) % 4) //winner is player over the top
            {
                directionX = canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 27.92727272727273); //55
                directionY = -(System.Windows.SystemParameters.PrimaryScreenHeight / 2.88); //300
            }
            else if (winnerID == (Game.HumanPlayerID + 3) % 4) //winner is on the left
            {
                directionX = -(System.Windows.SystemParameters.PrimaryScreenWidth / 7.68); //200
                directionY = canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 5.4); //160
            }

            double animateSpeed = 0.5;
            double throwSpeed = 0.4;

            if (playerID == Game.HumanPlayerID)//human players card
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MapCardToFile(playedCard), UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = (System.Windows.SystemParameters.PrimaryScreenWidth / 13.35652173913043); //115
                image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 4.32); //200
                image.HorizontalAlignment = HorizontalAlignment.Left;

                //Random rand = new Random(rndm);
                //int angle = rand.Next(41) - 20; //rest positoin is 0 degree, with -20 to 20 range

                if (animateTrickWin)
                {
                    double angle = playedCard.Angle;
                    RotateTransform rotateTransform = new RotateTransform(angle);
                    image.RenderTransform = rotateTransform;
                    AnimateCardTranslation(image, canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 34.13333333333333),
                    canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 8.64), directionX, directionY, animateSpeed); //45 //100
                }
                else
                {
                    if (showThrowAnimation)
                    {
                        if (!playedCard.IsPlayed)
                        {
                            AnimateCardTranslation(image, canvasGame.Width / 2, (canvasGame.Height - image.Height) + (System.Windows.SystemParameters.PrimaryScreenHeight / 66.46153846153846),
                                canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 34.13333333333333), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 8.64), throwSpeed);    //13, //45 //100

                            playedCard.IsPlayed = true;
                        }
                        else
                        {
                            double angle = playedCard.Angle;
                            RotateTransform rotateTransform = new RotateTransform(angle);
                            image.RenderTransform = rotateTransform;

                            //image.Margin = new Thickness(canvasGame.Width / 2 - 45, canvasGame.Height / 2 - 100, 0, 0);
                            Canvas.SetLeft(image, canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 34.13333333333333)); //45
                            Canvas.SetTop(image, canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 8.64));  //100
                        }
                    }

                    //Vector offset = VisualTreeHelper.GetOffset(image);//position of the image in the container                    
                }

                //image.IsEnabled = false;//disable card control after it's played
                canvasGame.Children.Add(image);
            }
            else if ((Game.HumanPlayerID + 1) % 4 == playerID)//player on the right side
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MapCardToFile(playedCard), UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = (System.Windows.SystemParameters.PrimaryScreenWidth / 13.35652173913043); //115
                image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 4.32); //200
                image.HorizontalAlignment = HorizontalAlignment.Left;

                //Random rand = new Random(rndm * 2);
                //int angle = rand.Next(70, 111) * (-1);//rest positoin is 90 degree, with 70 to 110 range

                if (animateTrickWin)
                {
                    AnimateCardTranslation(image, canvasGame.Width / 2,
                        canvasGame.Height / 2, directionX, directionY, animateSpeed);
                }
                else
                {
                    if (showThrowAnimation)
                    {
                        if (!playedCard.IsPlayed)
                        {
                            AnimateCardTranslation(image, canvasGame.Width - image.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 307.2), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 8.64),
                                canvasGame.Width / 2, canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 8.64), throwSpeed);     //5 //100 //100

                            playedCard.IsPlayed = true;
                        }
                        else
                        {
                            double angle = playedCard.Angle;
                            RotateTransform rotateTransform = new RotateTransform(angle);
                            image.RenderTransform = rotateTransform;

                            //image.Margin = new Thickness(canvasGame.Width / 2 + 180, canvasGame.Height / 2 - 100, 0, 0);
                            Canvas.SetLeft(image, canvasGame.Width / 2);
                            Canvas.SetTop(image, canvasGame.Height / 2);
                        }
                    }

                }

                canvasGame.Children.Add(image);
            }
            else if ((Game.HumanPlayerID + 2) % 4 == playerID)//player over the top
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MapCardToFile(playedCard), UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = (System.Windows.SystemParameters.PrimaryScreenWidth / 13.35652173913043); //115
                image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 4.32); //200
                image.HorizontalAlignment = HorizontalAlignment.Left;

                //Random rand = new Random(rndm * 3);
                //int angle = rand.Next(41) - 20;//rest positoin is 0 degree, with 20 to -20 range



                if (animateTrickWin)
                {
                    AnimateCardTranslation(image, canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 61.44),
                        canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 5.082352941176471), directionX, directionY, animateSpeed);     //25 //170
                }
                else
                {
                    if (!playedCard.IsPlayed)
                    {
                        AnimateCardTranslation(image, canvasGame.Width / 2, 0, canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 61.44), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 5.082352941176471), throwSpeed);         //25 //170

                        playedCard.IsPlayed = true;
                    }
                    else
                    {
                        double angle = playedCard.Angle;
                        RotateTransform rotateTransform = new RotateTransform(angle);
                        image.RenderTransform = rotateTransform;
                        //image.Margin = new Thickness(canvasGame.Width / 2 - 25, canvasGame.Height / 2 - 180, 0, 0);
                        Canvas.SetLeft(image, canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 61.44));   //25
                        Canvas.SetTop(image, canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 5.082352941176471));  //170
                    }

                }
                canvasGame.Children.Add(image);
            }
            else if ((Game.HumanPlayerID + 3) % 4 == playerID)//player over the top
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MapCardToFile(playedCard), UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = (System.Windows.SystemParameters.PrimaryScreenWidth / 13.35652173913043); //115
                image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 4.32); //200
                image.HorizontalAlignment = HorizontalAlignment.Left;

                //Random rand = new Random(rndm * 3);
                //int angle = rand.Next(70, 111) * (-1);//rest positoin is -90 degree, with -70 to -110 range

                if (animateTrickWin)
                {
                    AnimateCardTranslation(image, canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 10.24),
                        canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 43.2), directionX, directionY, animateSpeed);      //150 //20
                }
                else
                {
                    if (!playedCard.IsPlayed)
                    {
                        AnimateCardTranslation(image, (System.Windows.SystemParameters.PrimaryScreenWidth / 153.6), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 8.64), canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 10.24), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 7.2), throwSpeed); //10 //100 //150 //120

                        playedCard.IsPlayed = true;
                    }
                    else
                    {
                        double angle = playedCard.Angle;
                        RotateTransform rotateTransform = new RotateTransform(angle);
                        image.RenderTransform = rotateTransform;

                        //image.Margin = new Thickness(canvasGame.Width / 2 - 150, canvasGame.Height / 2 - 20, 0, 0);
                        Canvas.SetLeft(image, canvasGame.Width / 2 - (System.Windows.SystemParameters.PrimaryScreenWidth / 10.24));  //150
                        Canvas.SetTop(image, canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 43.2));   //20
                    }
                }
                canvasGame.Children.Add(image);
            }

        }

        /// <summary>
        /// Shows played cards for human
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Show_PlayedCards_Human(Object sender, MouseEventArgs e)
        {
            PlaySound("cardSlide1.wav");

            Image image = sender as Image;

            Game.Players[Game.HumanPlayerID].PlayCard(ImageToCardID(image));

            Game.Players[Game.HumanPlayerID].HasPlayed = true;

            Refresh_Canvas(Game.HumanPlayerID, true);

            //flag that the player has played their turn to finish awaiting
            hasHumanPlayed_Flag.SetResult(true);

            //Refresh canvas for next player
            //Refresh_Canvas((Game.HumanPlayerID + 1) % 4);
        }

        /// <summary>
        /// Animates movement of Image from one point to another
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="timeSpan"></param>
        public void AnimateCardTranslation(Image target, double startX, double startY,
            double endX, double endY, double timeSpan = 1)
        {
            TranslateTransform trans = new TranslateTransform();
            target.RenderTransform = trans;
            //RotateTransform rotateTransform = new RotateTransform();
            //target.RenderTransform = rotateTransform;
            DoubleAnimation animX = new DoubleAnimation(startX, endX, TimeSpan.FromSeconds(timeSpan));
            DoubleAnimation animY = new DoubleAnimation(startY, endY, TimeSpan.FromSeconds(timeSpan));
            //DoubleAnimation animAngle = new DoubleAnimation(startAngle, endAngle, TimeSpan.FromSeconds(timeSpan));
            trans.BeginAnimation(TranslateTransform.XProperty, animX, HandoffBehavior.Compose);
            trans.BeginAnimation(TranslateTransform.YProperty, animY, HandoffBehavior.Compose);
            //rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animAngle);
        }

        /// <summary>
        /// Animates movement of labels from one point to another
        /// Overloaded method
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="timeSpan"></param>
        public void AnimateCardTranslation(Label target, double startX, double startY,
            double endX, double endY, double timeSpan = 1)
        {
            TranslateTransform trans = new TranslateTransform();
            target.RenderTransform = trans;
            //RotateTransform rotateTransform = new RotateTransform();
            //target.RenderTransform = rotateTransform;
            DoubleAnimation animX = new DoubleAnimation(startX, endX, TimeSpan.FromSeconds(timeSpan));
            DoubleAnimation animY = new DoubleAnimation(startY, endY, TimeSpan.FromSeconds(timeSpan));
            //DoubleAnimation animAngle = new DoubleAnimation(startAngle, endAngle, TimeSpan.FromSeconds(timeSpan));
            trans.BeginAnimation(TranslateTransform.XProperty, animX, HandoffBehavior.Compose);
            trans.BeginAnimation(TranslateTransform.YProperty, animY, HandoffBehavior.Compose);
            //rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animAngle);
        }

        /// <summary>
        /// Animates rotation of card from one angle to other
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="timeSpan"></param>
        private void AnimateCardRotation(Image target, double startAngle, double endAngle, double timeSpan)
        {
            RotateTransform rotate = new RotateTransform();
            target.RenderTransform = rotate;
            DoubleAnimation animAngle = new DoubleAnimation(startAngle, endAngle, TimeSpan.FromSeconds(timeSpan));
            rotate.BeginAnimation(RotateTransform.AngleProperty, animAngle, HandoffBehavior.Compose);
        }

        private void ShowCardDeal_Animation()
        {
            ShowDealReady_Deck(true);
        }

        /// <summary>
        /// Shows deck of card on the given dealers side
        /// </summary>
        /// <param name="deal"></param>
        private void ShowDealReady_Deck(bool deal = false)
        {
            string faceDownCard = "/cards/extras/Back.png";

            for (int i = 0; i < 52; i++)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(faceDownCard, UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = (System.Windows.SystemParameters.PrimaryScreenWidth / 17.06666666666667);//90
                image.Height = (System.Windows.SystemParameters.PrimaryScreenHeight / 8.64);//100;

                image.HorizontalAlignment = HorizontalAlignment.Center;
                image.VerticalAlignment = VerticalAlignment.Top;

                //places the deck of card based on current dealer
                int a = Game.CurrentDealer;
                double x = 0, y = 0;
                if (a == (Game.HumanPlayerID + 3) % 4)
                {
                    x = (System.Windows.SystemParameters.PrimaryScreenWidth / 19.2) + i / 4;     //80
                    y = (System.Windows.SystemParameters.PrimaryScreenHeight / 2.88) + i / 4;    //300
                }
                else if (a == (Game.HumanPlayerID + 1) % 4)
                {
                    x = ((canvasGame.Width) - (System.Windows.SystemParameters.PrimaryScreenWidth / 9.035294117647059)) - i / 4;     //170
                    y = (System.Windows.SystemParameters.PrimaryScreenHeight / 2.88) + i / 4;        //300
                }
                else if (a == (Game.HumanPlayerID + 2) % 4)
                {
                    x = (canvasGame.Width / 2) + (System.Windows.SystemParameters.PrimaryScreenWidth / 102.4) + i / 4;        //15
                    y = (System.Windows.SystemParameters.PrimaryScreenHeight / 7.854545454545455) + i / 4;            //110
                }
                else if (a == Game.HumanPlayerID)
                {
                    x = (canvasGame.Width / 2) + (System.Windows.SystemParameters.PrimaryScreenWidth / 153.6) + i / 4;        //10
                    y = canvasGame.Height - (System.Windows.SystemParameters.PrimaryScreenHeight / 3.6) + i / 4;        //240
                }

                image.Margin = new Thickness(x, y, 0, 0);

                canvasGame.Children.Add(image);
            }
        }

        /// <summary>
        /// Points arrow on the canvas to the current player
        /// </summary>
        /// <param name="playerID"></param>
        private void PointCurrentPlayer_Canvas(int playerID)
        {
            Label arrow = new Label();
            arrow.Content = "⮚";
            arrow.Foreground = Brushes.Yellow;
            arrow.FontSize = (System.Windows.SystemParameters.PrimaryScreenHeight / 19.2);//45 based on height
            if (playerID == (Game.HumanPlayerID + 1) % 4)//player on the right side
            {
                arrow.Margin = new Thickness(canvasGame.Width - (System.Windows.SystemParameters.PrimaryScreenWidth / 6.4), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 4.018604651162791), 0, 0);    //240 //215
            }
            else if (playerID == (Game.HumanPlayerID + 3) % 4)//player on the left side
            {
                arrow.Content = "⮘";
                arrow.Margin = new Thickness((System.Windows.SystemParameters.PrimaryScreenWidth / 7.876923076923077), canvasGame.Height / 2 - (System.Windows.SystemParameters.PrimaryScreenHeight / 4.018604651162791), 0, 0);   //195 //215
            }
            else if (playerID == (Game.HumanPlayerID + 2) % 4)//player over the top
            {
                arrow.Margin = new Thickness((canvasGame.Width / 2) - (System.Windows.SystemParameters.PrimaryScreenWidth / 7.314285714285714), (System.Windows.SystemParameters.PrimaryScreenHeight / 8.64), 0, 0);      //210 //100
            }
            else if (playerID == Game.HumanPlayerID)//human player
            {
                arrow.Margin = new Thickness((canvasGame.Width / 2) - (System.Windows.SystemParameters.PrimaryScreenWidth / 7.144186046511628), canvasGame.Height - (System.Windows.SystemParameters.PrimaryScreenHeight / 3.526530612244898), 0, 0);      //215 //245
            }
            canvasGame.Children.Add(arrow);
        }

        /// <summary>
        /// Returns corresponding card ID of the associated image control
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private int ImageToCardID(Image image)
        {
            string imagePath = (image.Source as BitmapImage).UriSource.ToString();
            string[] splitPath = imagePath.Split('/');
            string fileName = splitPath[splitPath.Length - 1];

            int i;
            for (i = 0; i < 52; i++)
            {
                if ("/cards/" + fileName == MapCardToFile(Game.CardIDtoCard(i)))
                {
                    break;
                }
            }
            return i;
        }

        /// <summary>
        /// Shows information about game on top bar
        /// </summary>
        private void Show_Info_TopBar()
        {
            lblTopBar.Content = String.Format("Current Player: [{0}]            Current Hand: [{1}/{2}]            Tricks Won: [{3}]            Score: [{4}]",
                Game.Players[Game.CurrentPlayer].Name, Game.CurrentHand + 1, Game.MaxHandsToPlay,
                Game.TricksWon[Game.HumanPlayerID], Game.CumulativeScore[Game.HumanPlayerID].ToString("0.0"));
        }

        private void TestWindow()
        {
            //lblTopBar.FontSize = 12;
            //lblTopBar.Content = "";

            //foreach (Player player in Game.Players)
            //{
            //    lblTopBar.Content += player.Name + ": ";
            //    foreach (Card card in player.Cards)
            //    {
            //        lblTopBar.Content += card.Name + " ";
            //    }
            //    lblTopBar.Content += " | ";
            //}
        }

        /// <summary>
        /// Changes contents according to screen size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvasGame.Width = e.NewSize.Width;
            canvasGame.Height = e.NewSize.Height;

            double xChange = 1, yChange = 1;

            if (e.PreviousSize.Width != 0)
                xChange = (e.NewSize.Width / e.PreviousSize.Width);

            if (e.PreviousSize.Height != 0)
                yChange = (e.NewSize.Height / e.PreviousSize.Height);

            foreach (FrameworkElement fe in canvasGame.Children)
            {
                /*because I didn't want to resize the grid I'm having inside the canvas in this particular instance. (doing that from xaml) */
                if (fe is Grid == false)
                {
                    fe.Height = fe.ActualHeight * yChange;
                    fe.Width = fe.ActualWidth * xChange;

                    Canvas.SetTop(fe, Canvas.GetTop(fe) * yChange);
                    Canvas.SetLeft(fe, Canvas.GetLeft(fe) * xChange);

                }
            }
        }

        /// <summary>
        /// Override close button
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //Display warning dialogue
            frmCloseConfirm frmClose = new frmCloseConfirm();
            frmClose.ShowDialog();

            //cancel close
            e.Cancel = true;
            base.OnClosing(e);
        }

        //Define music player and plath
        public MediaPlayer backgroundMusicPlayer = new MediaPlayer();
        Uri backgroundMusicFilePath = new Uri("../../sfx/DeerPortal-GamePlay.mp3", UriKind.Relative);

        public void PlayBackGroundMusic()
        {
            if (backgroundMusicFilePath != null)
            {
                backgroundMusicPlayer.Open(backgroundMusicFilePath);
                backgroundMusicPlayer.Position = TimeSpan.FromMilliseconds(1);
                backgroundMusicPlayer.Volume = 0.07;
                backgroundMusicPlayer.MediaEnded += new EventHandler(Media_Ended);//this line makes it play on loop
                backgroundMusicPlayer.Play();
            }
        }

        //loops the music when the music ends
        private void Media_Ended(object sender, EventArgs e)
        {
            backgroundMusicPlayer.Position = TimeSpan.FromMilliseconds(1);
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



