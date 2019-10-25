using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Call_Break_Card_Game
{
    public static class Game
    {


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
            if (cardID >= 0 && cardID < 52)
            {
                return CardIDtoCard(cardID).Value;
            }
            else
            {
                return -1;
            }
        }
    }
}
