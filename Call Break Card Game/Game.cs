﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Call_Break_Card_Game
{
    public static class Game
    {
        private static Player[] _Players = new Player[4];
        private static int _MaxTricksToPlay;
        private static Deck _DeckOfCards;
        private static int _CurrentTrick;
        private static double[,] _ScoreBoard;
        private static int[,] _Bidding;
        //private static int _LeadCardID;
        //private static int _PowerCardID;
        private static List<Card> _CardsInTable = new List<Card>();
        private static int _TurnCounter;

        public static Player[] Players
        {
            get
            {
                return _Players;
            }
            set
            {
                _Players = value;
            }
        }

        public static int MaXTrickToPlay
        {
            get { return _MaxTricksToPlay; }
            set { _MaxTricksToPlay = value; }
        }

        public static Deck DeckOfCards
        {
            get { return _DeckOfCards; }
            //set { _DeckOfCards = value; }
        }

        public static int CurrentTrick
        {
            get { return _CurrentTrick; }
        }

        public static double[,] ScoreBoard
        {
            get { return _ScoreBoard; }
        }

        public static int[,] Bidding
        {
            get { return _Bidding; }
            set { _Bidding = value; }
        }

        /// <summary>
        /// First card played in the table is lead card
        /// </summary>
        public static int LeadCardID
        {
            get 
            {
                return CardsInTable[0].ID;
            }
            //set { _LeadCardID = value; }
        }

        /// <summary>
        /// Finds the power card among the cards played in the table
        /// </summary>
        public static int PowerCardID
        {
            get 
            {
                int index = 0;
                foreach(Card card in CardsInTable)
                {
                    if(card.Suit == CardIDtoSuit(LeadCardID) && card.Value > CardIDtoValue(LeadCardID))
                    {
                        index = card.ID;
                    }
                    else if(card.Suit!=CardIDtoSuit(LeadCardID) && card.Suit == Card.CardSuit.Spade)
                    {
                        index = card.ID;
                    }
                }

                return index;
            }
            //set { _PowerCardID = value; }
        }

        public static List<Card> CardsInTable
        {
            get { return _CardsInTable; }
        }

        public static int TurnCounter
        {
            get { return _TurnCounter; }
        }

        /// <summary>
        /// Initializes all players and puts them in random order
        /// </summary>
        /// <param name="playerName"></param>
        public static void InitializePLayers(string playerName)
        {
            _Players[0] = new Player(playerName, Player.PlayerType.Human);
            for (int i = 1; i <= 3; i++)
            {
                _Players[i] = new Player(i==1?"Reiner":(i==2?"Bertholdt":"Annie"), Player.PlayerType.Bot);
            }

            Random random = new Random();
            int rand = random.Next(999999999);

            for (int i = 0; i < 1000; i++)
            {
                Random rnd = new Random(rand * i);
                int index = rnd.Next(4);

                Player temp = _Players[0];
                _Players[0] = _Players[index];
                _Players[index] = temp;
            }

            //assign IDs to players
            for (int i = 0; i < 4; i++)
            {
                _Players[i].ID = i;
            }
        }

        /// <summary>
        /// Deals all cards from the deck to all players,
        /// As the card is being dealt to players, the given cards are being removed from deck pile at the same time.
        /// </summary>
        public static void DealCards()
        {
            _DeckOfCards = new Deck();

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    //Adds the card from top of the deck to players pile
                    Players[j].Cards.Add(DeckOfCards.Cards[0]);
                    //Removes the card from top of the deck since now its moved to player's pile
                    DeckOfCards.Cards.Remove(DeckOfCards.Cards[0]);
                }
            }
        }

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
