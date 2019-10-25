using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Call_Break_Card_Game
{
    class Player : ISwapCards<Card>
    {
        private List<Card> _Cards = new List<Card>();
        private string _Name;
        private int _Score;
        private int _ID;
        private PlayerType _Type;
        private Card _LastPlayedCard;

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
        /// <param name="leadCardID"></param>
        /// <param name="powerCardID"></param>
        /// <returns>List of intergers indicating card Ids of playable cards</returns>
        public List<int> ListPlayableCards(int leadCardID, int powerCardID)
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
                        if ((int)card.Suit == CardIDtoSuit(leadCardID))
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
                            if (CardIDtoNumber(cardIndex) > CardIDtoNumber(leadCardID))
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
                                if (((int)card.Suit == CardIDtoSuit(leadCardID) && CardIDtoNumber(card.ID) > CardIDtoNumber(leadCardID)))
                                {
                                    list.Add(card.ID);
                                }
                            }
                        }
                    }
                }
                //when there is higher ranking card than lead card in the table
                else if (CardIDtoValue(powerCardID) > CardIDtoValue(leadCardID))
                {
                    foreach (Card card in Cards)
                    {
                        //add all the cards with matching suit
                        if ((int)card.Suit == CardIDtoSuit(leadCardID))
                        {
                            list.Add(card.ID);
                        }
                    }

                    //when power card has same suit as the lead card
                    if (CardIDtoSuit(powerCardID) == CardIDtoSuit(leadCardID))
                    {
                        foreach (Card card in Cards)
                        {
                            //add all the cards with matching suit
                            if ((int)card.Suit == CardIDtoSuit(powerCardID))
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
                                if (CardIDtoNumber(cardIndex) > CardIDtoNumber(powerCardID))
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
                                    if (((int)card.Suit == CardIDtoSuit(powerCardID) && CardIDtoNumber(card.ID) > CardIDtoNumber(powerCardID)))
                                    {
                                        list.Add(card.ID);
                                    }
                                }
                            }
                        }
                    }
                    //when the power card has spade suit
                    else if (CardIDtoSuit(powerCardID) > CardIDtoSuit(leadCardID))
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
        /// Converts cardID to card number
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns>Returns -1 if invalid ID</returns>
        public static int CardIDtoNumber(int cardID)
        {
            if (cardID >= 0 && cardID < 52)
            {
                return (cardID % 13);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Converts cardID to card suit
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns>Return -1 if invalid ID</returns>
        public static int CardIDtoSuit(int cardID)
        {
            if (cardID >= 0 && cardID < 52)
            {
                return (cardID - (int)(CardIDtoNumber(cardID))) / 13;
            }
            else
            {
                return -1;
            }
        }


        /// <summary>
        /// Converts cardID number into card
        /// </summary>
        /// <param name="cardID">Unique ID of the card ranging 0-51</param>
        /// <returns>Corresponding card of given ID, returns null if invalid ID</returns>
        public static Card CardIDtoCard(int cardID)
        {
            if (cardID >= 0 && cardID < 52)
            {
                return (new Card((Card.CardNumber)CardIDtoNumber(cardID), (Card.CardSuit)CardIDtoSuit(cardID)));
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Converts CardID to card value
        /// </summary>
        /// <param name="cardID">CardID of the said card</param>
        /// <returns>Return number ranging 0-25, returns -1 if invalid cardID</returns>
        public static int CardIDtoValue(int cardID)
        {
            if (cardID >= 0 && cardID < 52)
            {
                return CardIDtoCard(cardID).Value;
            }
            else
            {
                return -1;
            }
        }



        /// <summary>
        /// Player type human or computer
        /// </summary>
        public enum PlayerType { Human, Bot };

        //public enum Playability {FirstTrick, LeadTrick, }
    }
}
