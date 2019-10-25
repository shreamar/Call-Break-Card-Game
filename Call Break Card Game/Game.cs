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
        /// <returns></returns>
        public static Card.CardNumber CardIDtoNumber(int cardID)
        {
            return (Card.CardNumber)(cardID % 13);
        }

        /// <summary>
        /// Converts cardID to card suit
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static Card.CardSuit CardIDtoSuit(int cardID)
        {
            return (Card.CardSuit)((cardID - (int)(CardIDtoNumber(cardID))) / 13);
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
        /// Process Lead Card and Power Card, and validity checks and make changes if required
        /// </summary>
        /// <param name="leadCardID"></param>
        /// <param name="powerCardID"></param>
        /// <returns>Corrected power and lead card ID values</returns>
        public static int[] LeadVsPowerCard(int leadCardID, int powerCardID)
        {
            int[] index = new int[2];

            //if the power card is higher ranking but not same suit and is not spade then power and lead card are same
            //or if power card has less rank than lead card
            if (((Game.CardIDtoSuit(leadCardID) != Game.CardIDtoSuit(powerCardID) &&
                (Game.CardIDtoValue(leadCardID) > Game.CardIDtoValue(powerCardID) && Game.CardIDtoSuit(leadCardID) != Card.CardSuit.Spade))) ||
                Game.CardIDtoValue(powerCardID) <= Game.CardIDtoValue(leadCardID))
            {
                powerCardID = leadCardID;
            }

            index[0] = leadCardID;
            index[1] = powerCardID;

            return index;
        }
    }
}
