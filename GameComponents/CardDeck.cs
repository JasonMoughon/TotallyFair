using System;
using System.Collections.Generic;

namespace TotallyFair.GameComponents
{
    internal class CardDeck
    {
        /****************************************************/
        /* PUBLIC CARD DECK INFO AVAILABLE TO THE MAIN GAME */
        /****************************************************/
        public const int SIZE = 52; //Total Number of cards in a deck
        public const int SUIT_NUMBER = 4; //Total Number of Suits
        public const int FACEVALUE_NUMBER = 13; //Total Number of FaceValues

        public List<Card> AvailableCards = new List<Card>();
        private Random Rand = new Random();


        public CardDeck()
        {
            GenerateDeck();
        }

        public Card DealCard()
        {
            Card Result;
            int Index = (int)Math.Round(Rand.NextDouble() * AvailableCards.Count);
            if (Index >= AvailableCards.Count) Index = AvailableCards.Count - 1;
            if (Index <= 0) Index = 0;
            Result = AvailableCards[Index];
            AvailableCards.RemoveAt(Index);
            return Result;
        }

        public void ClearDeck()
        {
            AvailableCards = new List<Card>();
        }

        public void GenerateDeck()
        {
            for (int i = 0; i < SUIT_NUMBER; i++)
            {
                for (int j = 0; j < FACEVALUE_NUMBER; j++)
                {
                    AvailableCards.Add(new Card(i * 100 + j, i, j)); //Initializes all 52 cards in a regular card deck
                }
            }
        }
    }
    public class Card
    {
        public int ID;
        public Suits Suit;
        public FaceValues FaceValue;
        public bool CardPlayed = false;

        public enum Suits { SPADE, CLUB, HEART, DIAMOND };
        public enum FaceValues { TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING, ACE };


        public Card(int ID, int SuitInt, int FaceValueInt)
        {
            this.ID = ID;
            Suit = (Suits)SuitInt;
            FaceValue = (FaceValues)FaceValueInt;
        }
    }

}
