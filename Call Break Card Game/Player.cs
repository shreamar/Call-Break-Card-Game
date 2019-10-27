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
        private List<int> _Playables = new List<int>();

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

        public List<int> Playables
        {
            get 
            {
                _Playables = ListPlayableCards(Game.LeadCardID, Game.PowerCardID);
                return _Playables; 
            }
        }

        public void swapCards(Card a, Card b)
        {
            Card temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Sorts players cards based on the cards ID
        /// </summary>
        public void sortCards()
        {
            Cards.Sort((a, b) => (a.ID.CompareTo(b.ID)));
        }

        /// <summary>
        /// Plays the card for the given trick
        /// Removes the given card from the hand
        /// </summary>
        /// <param name="cardIndex">Index of the card to be played</param>
        /// <returns>True if the said card was successfully played</returns>
        public bool playCard(int cardIndex)
        {
            if (CardCount - 1 >= cardIndex && cardIndex >= 0)
            {
                LastPlayedCard = Cards[cardIndex];
                
                //Removes card from players stacks and adds to the cards in the table
                Game.CardsInTable.Add(Cards[cardIndex]);
                Cards.Remove(Cards[cardIndex]);
                return true;
            }
            else
            {
                return false;
            }
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
