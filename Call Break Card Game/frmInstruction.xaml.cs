using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class frmInstruction : Window
    {
        public frmInstruction()
        {
            InitializeComponent();
            Instructions();

            PlaySound("draw.wav");
        }

        private void Instructions()
        {
            TextBlock textBlock = new TextBlock();
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            scrollViewer.Content = textBlock;      
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.FontFamily = new FontFamily("Verdana");
            textBlock.FontSize = 17;
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.Foreground = Brushes.LightCyan;

            textBlock.Text = "Call Break (also known as Call Bridge) is a game of tricks, trumps, and bidding which is popular in South Asia (especially Nepal, India, and Bangladesh). It seems to be very similar to the card game 'Contract Bridge'. The rules vary from place to place but this game uses the standard rules.\n\n " +
        "Players and Cards:\n " +
        "This game is normally played by 4 people using a standard 52 - card pack.\n " +
        "The cards of each suit rank from high to low A - K - Q - J - 10 - 9 - 8 - 7 - 6 - 5 - 4 - 3 - 2. Spades are permanent trumps: any card of the Spade suit beats any card of any other suit.\n Deal and play are counter-clockwise.\n\n " +
        "Deal:\n Any player may deal first: subsequently the turn to deal passes to the right.\n " +
        "The dealer deals out all the cards, one at a time, face down, so that each player has 13 cards. The players pick up their cards and look at them.\n\n " +
        "Bidding:\n " +
        "Starting with the player to dealer's right, and continuing counter-clockwise around the table, ending with the dealer, each player calls a number, which must be at least 1. The maximum sensible call is 8, as any call greater or equals to 8 counts for bonus call (see 'Scoring' section for information about bonus call). This call represents the number of tricks that the player undertakes to win. In this game, the tricks bid are known as \"calls\" (Hence, the name 'Call Break').\n\n " +
        "Play:\n The player to dealer's right leads to the first trick, and subsequently the winner of each trick leads to the next.\n Any card may be led, and the other three players must follow the suit if they can. A player who cannot follow the suit must trump with a spade, provided that this spade is high enough to beat any spades already in the trick. A player who has no cards of the suit led and no spades high enough to head the trick may play any card.\n The trick is won by the player of the highest spade in it, or if it contains no spade, by the player of the highest card of the suit that was led.\n Note that:\n A player who is able to play a card of the suit that was led is not obliged to head the trick. This also applies when spades are led: players may play higher or lower spades as they wish.\n " +
        "A player who has no cards of a suit is said to be \"off\" that suit. If it is off the suit that was led, and there are no spades in the trick yet, the player must play a spade if possible. If there is already a spade in the trick, the player who is \"off\" the lead suit must play a higher spade if possible. If the player only has lower spades, s/he can \"waste\" one of these spades to avoid taking an unwanted trick later or can throw a card of another suit.\n\n " +
        "Scoring:\n To succeed, a player must win the number of tricks called or more tricks than the call. If a player succeeds, the number called is added to his/her cumulative score. Otherwise, the number of the bid is subtracted.\n For example, a player who calls 4 must win 4 or more tricks to succeed, and in this case gains 4 points and 0.1 points for each extra tricks won. Say, the player wins 5 tricks for a bid of 4 then s/he scores 4.1. Winning 3 or fewer tricks for a bid of 4 counts as a loss, and the player loses 4 points.\n Bonus call: Calls of 8 tricks or more are bonus calls. A player who calls 8 or more scores 13 points(instead of the amount of the call) if successful. If unsuccessful, the amount of the call is subtracted as usual. So for example a player who calls 8 scores +13 for 8 or more tricks, but -8 for 7 or fewer.\n\n There is no fixed end to the game. Players continue for as long as they wish, and when the game ends the player with the highest cumulative score is the winner." +
        "\nReference: https://www.pagat.com/auctionwhist/call_bridge.html" +
        "\n\nDeveloped by: Amar Shrestha"
        + "\nSpecial thanks to Binod Sujakhu for game insights and advice.";

            grdInstruction.Children.Add(scrollViewer);
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
