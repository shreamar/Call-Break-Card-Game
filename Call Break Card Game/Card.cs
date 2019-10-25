using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Call_Break_Card_Game
{
    public class Card
    {
        private CardNumber _Number;
        private CardSuit _Suit;
        private bool _IsPlayed = false;

        public Card()
        {
            _Number = CardNumber.Ace;
            _Suit = CardSuit.Club;
        }

        public Card(CardNumber number, CardSuit suit)
        {
            _Number = number;
            _Suit = suit;
        }

        public CardNumber Number
        {
            get { return _Number; }
            set { _Number = value; }
        }

        public CardSuit Suit
        {
            get { return _Suit; }
            set { _Suit = value; }
        }

        public int ID
        {
            get { return ((int)Suit * 13) + (int)Number; }        //A-Club is 0, K-Spade is 51
        }

        /// <summary>
        /// Hierarchical value a card given its suit and number
        /// </summary>
        public int Value
        {
            get
            {
                //Ace is highest among numbers and all other numbers have values given their order
                //Space has highest value while all other suits have same value
                int suitValue = Suit == CardSuit.Spade ? 13 : 0;
                int numberValue = Number == CardNumber.Ace ? 12 : (int)Number - 1;

                //examples: 5-H = 5-D = 5-C =4, A-H = A-D = A-C =12, 2-S = 13, K-S = 24, A-S =25
                return suitValue + numberValue;
            }
        }

        public bool IsPlayed
        {
            get { return _IsPlayed; }
            set { _IsPlayed = value; }
        }

        /// <summary>
        /// Displays the name of the given card
        /// For example, 9-Heart
        /// </summary>
        public string Name
        {
            get
            {
                string name = "";
                
                    //since Two = 0, 9 = Jack
                    if ((int)Number < 10 && (int)Number > 0)
                    {
                        name = ((int)Number + 1).ToString();
                    }
                    else if ((int)Number >= 10 || (int)Number == 0)
                    {
                        //picks only the first letter of face/ace cards
                        name = Number.ToString()[0].ToString();
                    }
                    name += "-" + Suit.ToString();

                return name;

            }
        }

        /// <summary>
        /// Converts cardID to card number
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns>Returns -1 if invalid ID</returns>
        public int CardIDtoNumber(int cardID)
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
        public int CardIDtoSuit(int cardID)
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
        public Card CardIDtoCard(int cardID)
        {
            if (cardID >= 0 && cardID < 52)
            {                
                return (new Card((CardNumber)CardIDtoNumber(cardID), (CardSuit)CardIDtoSuit(cardID)));
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
        public int CardIDtoValue(int cardID)
        {
            if(cardID>=0 && cardID < 52)
            {
                return CardIDtoCard(cardID).Value;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Creates deep copy of card
        /// </summary>
        /// <returns></returns>
        public Card CreateDeepCopy()
        {
            return new Card(this.Number, this.Suit);
        }

        public enum CardSuit { Club, Diamond, Heart, Spade };

        public enum CardNumber { Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King };

    }
}
