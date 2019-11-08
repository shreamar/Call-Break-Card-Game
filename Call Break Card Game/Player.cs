using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Call_Break_Card_Game
{
    public class Player : ISwapCards<Card>
    {
        private List<Card> _Cards = new List<Card>();
        private string _Name;
        private int _Score;
        private int _ID;
        private PlayerType _Type;
        private Card _LastPlayedCard;
        private List<int> _PlayableIDs = new List<int>();
        private int _IconNumber;
        private bool _HasPlayed;

        public Player()
        {
            Name = "Random Player";
            Score = 0;
            Type = PlayerType.Bot;
            ID = 0;
            _LastPlayedCard = null;
        }

        public Player(string name, PlayerType type)
        {
            Name = name;
            Score = 0;
            Type = type;
            ID = 0;
            _LastPlayedCard = null;
        }

        /// <summary>
        /// Keeps track of how many cards player has 
        /// </summary>
        public int CardCount
        {
            get { return _Cards.Count; }
        }

        public List<Card> Cards
        {
            get { return _Cards; }
            set { _Cards = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public int Score
        {
            get { return _Score; }
            set { _Score = value; }
        }

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public PlayerType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public Card LastPlayedCard
        {
            get { return _LastPlayedCard; }
            set { _LastPlayedCard = value; }
        }

        public int IconNumber
        {
            get { return _IconNumber; }
            set { _IconNumber = value; }
        }

        public bool HasPlayed
        {
            get { return _HasPlayed; }
            set { _HasPlayed = value; }
        }

        public List<int> PlayableIDs
        {
            get 
            {
                _PlayableIDs = ListPlayableCards(Game.LeadCardID, Game.PowerCardID);
                return _PlayableIDs; 
            }
        }

        public void swapCards(Card a, Card b)
        {
            Card temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Sorts players cards in descending order based on the cards ID
        /// </summary>
        public void sortCards()
        {
            Cards = Cards.OrderByDescending(card => card.ID).ToList();
        }

        /// <summary>
        /// Plays the card for the given trick
        /// Removes the given card from the hand
        /// </summary>
        /// <param name="cardIndex">Index of the card to be played</param>
        /// <returns>True if the said card was successfully played</returns>
        public bool PlayCard(int cardIndex)
        {
            if (cardIndex <= 51 && cardIndex >= 0)
            {
                LastPlayedCard = Game.CardIDtoCard(cardIndex);

                //Assign angles to played cards. This angle is used to rotate transform card when thrown on the table
                Random rand = new Random();
                int angle = 0;
                if (Game.CardBelongsTo(cardIndex) == Game.HumanPlayerID)//human player
                {
                    angle = rand.Next(41) - 20;
                }
                else if (Game.CardBelongsTo(cardIndex) == (Game.HumanPlayerID + 1) % 4)//player on the right side
                {
                    angle = rand.Next(70, 111) * (-1);
                }
                else if (Game.CardBelongsTo(cardIndex) == (Game.HumanPlayerID + 2) % 4)//player over the top
                {
                    angle = rand.Next(41) - 20;
                }
                else if (Game.CardBelongsTo(cardIndex) == (Game.HumanPlayerID + 3) % 4)//player on the left side
                {
                    angle = rand.Next(70, 111) * (-1);
                }

                LastPlayedCard.Angle = angle;

                //Removes card from players stacks and adds to the cards in the table
                Game.CardsInTable.Add(LastPlayedCard);

                var itemToRemove = Cards.Where(card => card.ID == cardIndex).ToList();
                Cards.Remove(itemToRemove[0]);               

                HasPlayed = true;
                return true;
            }
            else
            {
                return false;
            }
        }

       /// <summary>
       /// Counts and returns the number of each suits in player's cards as an array
       /// </summary>
        public int[] SuitCount
        {
            get
            {
                int[] suitCount = new int[4];// counts each suits in the cards the player has. suitCount[0] is diamond,..., suitCount[3] is spade

                foreach (Card card in Cards)//counts the number of each suits in the player's cards
                {
                    if (card.Suit == Card.CardSuit.Diamond)
                    {
                        suitCount[0]++;
                    }
                    else if (card.Suit == Card.CardSuit.Club)
                    {
                        suitCount[1]++;
                    }
                    else if (card.Suit == Card.CardSuit.Heart)
                    {
                        suitCount[2]++;
                    }
                    else if (card.Suit == Card.CardSuit.Spade)
                    {
                        suitCount[3]++;
                    }
                }

                return suitCount;
            }
        }

        /// <summary>
        /// Strategically gives a bidding number for the given player
        /// </summary>
        /// <returns></returns>
        public int Bid_AI()
        {
            Random rnd = new Random();
            int r = rnd.Next(99999);

            int bid = 0;

            if (SuitCount[3] != 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Random rand = new Random(r + i + 100);

                    //adds 1 to bid if any other suits have 1 or 2, or less cards
                    if (SuitCount[i] <= rand.Next(1,3))
                    {
                        bid++;
                    }
                }

                //if number of spades is less than the bid then bid is set to be the count of spades
                if (SuitCount[3] < bid)
                {
                    bid = SuitCount[3];
                }
            }

            int countFaceCards = 0;
            foreach(Card card in Cards)
            {
                if((int)card.Number >= 9) //facecards
                {
                    countFaceCards++;
                }
            }

            //add half the number of face cards in bid
            bid += countFaceCards % 4 == 0 ? countFaceCards / 2 : (countFaceCards + 1) / 2;

            bid += SuitCount[3] / (rnd.Next(2, 4));//divides spade count by either 2 or 3, adds to bids and adds either 1 or 0

            
            if (bid > 5)
            {
                if (countFaceCards >= 4)
                {
                    int count = 0;
                    foreach (Card card in Cards)
                    {
                        if (card.Suit == Card.CardSuit.Spade && (int)card.Number > 9)//counts number of face cards in spades
                        {
                            count++;
                        }
                    }

                    if (count < 2)
                    {
                        bid--;
                    }
                }
                else
                {
                    bid -= 2;
                }
            }

            if (bid > 6)
            {
                if (!(SuitCount[3] >= 5 && countFaceCards>=5))
                {
                    bid--;
                }
            }

            bid = bid > 8 ? 8 : bid;//if bid is greater than 8, sets it to 8

            bid = bid < 1 ? 1 : bid;//if bid is less than 1, sets it to 1

            return bid;
        }

        /// <summary>
        /// Lists the IDs of playable cards for the given lead card and power card
        /// Lead card: The card played at start of th given trick
        /// Power card: other played card which is higher ranking than lead card
        /// </summary>
        /// <param name="leadCardID">Invalid id means start of trick</param>
        /// <param name="powerCardID">Invalid id means start of trick</param>
        /// <returns>List returns null if power card is less ranking than lead card</returns>
        private List<int> ListPlayableCards(int leadCardID, int powerCardID)
        {
            List<int> list = new List<int>();

                //validity of card ID
                if (leadCardID >= 0 && leadCardID < 52 && powerCardID >= 0 && powerCardID < 52)
                {
                    //when leading card is the highest ranking card on the table
                    if (leadCardID == powerCardID)
                    {
                        foreach (Card card in Cards)
                        {
                            //add all the cards with matching suit
                            if (card.Suit == Game.CardIDtoSuit(leadCardID))
                            {
                                list.Add(card.ID);
                            }
                        }
                        //if list is emtpy due to no matching suits
                        if (list.Count == 0)
                        {
                            int spadeMatches = 0;
                            foreach (Card card in Cards)
                            {
                                //Add all spade cards in the hand
                                if (card.Suit == Card.CardSuit.Spade)
                                {
                                    list.Add(card.ID);
                                    spadeMatches++;
                                }
                            }

                            //when there are no spade cards in the hand at all
                            if (spadeMatches == 0)
                            {
                                //Add all cards at hand
                                foreach (Card card in Cards)
                                {
                                    list.Add(card.ID);
                                }
                            }
                        }
                        //when matching suit cards are available
                        else if (list.Count > 0)
                        {
                            //check if there are any greater cards amont matching suit cards
                            int numberOfGreaterSuits = 0;
                            foreach (int cardIndex in list)
                            {
                                if (Game.CardIDtoNumber(cardIndex) > Game.CardIDtoNumber(leadCardID))
                                {
                                    numberOfGreaterSuits++;
                                }
                            }

                            //when there are higher ranked suit matching cards than leading card
                            if (numberOfGreaterSuits > 0)
                            {
                                //clear all cards from list
                                list.Clear();

                                //add cards which has both matching suits and greater ranking then the lead card
                                foreach (Card card in Cards)
                                {
                                    if ((card.Suit == Game.CardIDtoSuit(leadCardID) && Game.CardIDtoNumber(card.ID) > Game.CardIDtoNumber(leadCardID)))
                                    {
                                        list.Add(card.ID);
                                    }
                                }
                            }
                        }
                    }
                    //when there is higher ranking card than lead card in the table
                    else if (Game.CardIDtoValue(powerCardID) > Game.CardIDtoValue(leadCardID))
                    {
                        foreach (Card card in Cards)
                        {
                            //add all the cards with matching suit
                            if (card.Suit == Game.CardIDtoSuit(leadCardID))
                            {
                                list.Add(card.ID);
                            }
                        }

                        //when power card has same suit as the lead card
                        if (Game.CardIDtoSuit(powerCardID) == Game.CardIDtoSuit(leadCardID))
                        {
                            foreach (Card card in Cards)
                            {
                                //add all the cards with matching suit
                                if (card.Suit == Game.CardIDtoSuit(powerCardID))
                                {
                                    list.Add(card.ID);
                                }
                            }
                            //if list is emtpy due to no matching suits
                            if (list.Count == 0)
                            {
                                int spadeMatches = 0;
                                foreach (Card card in Cards)
                                {
                                    if (card.Suit == Card.CardSuit.Spade)
                                    {
                                        spadeMatches++;
                                    }
                                }

                                //when there are no spade cards in the hand at all
                                if (spadeMatches == 0)
                                {
                                    //Add all cards at hand
                                    foreach (Card card in Cards)
                                    {
                                        list.Add(card.ID);
                                    }
                                }
                                else if (spadeMatches > 0)
                                {
                                    //add just the spade cards when there are spade cards
                                    foreach (Card card in Cards)
                                    {
                                        if (card.Suit == Card.CardSuit.Spade)
                                        {
                                            list.Add(card.ID);
                                        }
                                    }
                                }
                            }
                            //when matching suit cards are available
                            else if (list.Count > 0)
                            {
                                //check if there are any greater cards amont matching suit cards
                                int numberOfGreaterSuits = 0;
                                foreach (int cardIndex in list)
                                {
                                    if (Game.CardIDtoNumber(cardIndex) > Game.CardIDtoNumber(powerCardID))
                                    {
                                        numberOfGreaterSuits++;
                                    }
                                }

                                //when there are higher ranked suit matching cards than leading card
                                if (numberOfGreaterSuits > 0)
                                {
                                    //clear all cards from list
                                    list.Clear();

                                    //add cards which has both matching suits and greater ranking then the lead card
                                    foreach (Card card in Cards)
                                    {
                                        if ((card.Suit == Game.CardIDtoSuit(powerCardID) && Game.CardIDtoNumber(card.ID) > Game.CardIDtoNumber(powerCardID)))
                                        {
                                            list.Add(card.ID);
                                        }
                                    }
                                }
                            }
                        }
                        //when the power card has spade suit
                        else if (Game.CardIDtoSuit(powerCardID) > Game.CardIDtoSuit(leadCardID))
                        {
                            if (list.Count == 0)
                            {
                                int spadeMatches = 0;
                                foreach (Card card in Cards)
                                {
                                    if (card.Suit == Card.CardSuit.Spade)
                                    {
                                        spadeMatches++;
                                    }
                                }

                                //when there are no spade cards in the hand at all
                                if (spadeMatches == 0)
                                {
                                    //Add all cards at hand
                                    foreach (Card card in Cards)
                                    {
                                        list.Add(card.ID);
                                    }
                                }
                                else if (spadeMatches > 0)
                                {
                                    //add just the spade cards when there are spade cards
                                    foreach (Card card in Cards)
                                    {
                                        if (card.Suit == Card.CardSuit.Spade)
                                        {
                                            list.Add(card.ID);
                                        }
                                    }
                                }
                            }

                        }

                    }


                }
                //invalid card ID means start of trick so all cards will be available
                else
                {
                    foreach (Card card in Cards)
                    {
                        list.Add(card.ID);
                    }
                }
            
            return list;
        }


        /// <summary>
        /// Player type human or computer
        /// </summary>
        public enum PlayerType { Human, Bot };

        //public enum Playability {FirstTrick, LeadTrick, }
    }
}
