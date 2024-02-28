using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TechTestWPF.Cards;

namespace TechTestWPF
{
    public class CardDeck
    {
        public List<Cards> cardDeck;

        public CardDeck(int noOfDecks = 1)
        {
            //if (cardDeck == null)
            //{

                cardDeck = new List<Cards>();
                ShuffleDeck(noOfDecks);
            //}
        }

        public void ShuffleDeck(int noOfDecks)
        {
            List<Cards> cards = new List<Cards>();

            for (int decks = 1; decks <= noOfDecks; decks++)
            {
                foreach (var cardSuit in (Suit[])Enum.GetValues(typeof(Suit)))
                {
                    foreach (var value in (CardValue[])Enum.GetValues(typeof(CardValue)))
                    {
                        Cards card = new Cards()
                        {
                            suit = cardSuit,
                            cardValue = value,
                            cardImage = Enum.GetName(typeof(Suit), cardSuit) + value + ".png"
                        };

                        cards.Add(card);
                    }
                }
            }
                //Had to look at Fisher-Yates Shuffling Algorithm to do this section (https://exceptionnotfound.net/understanding-the-fisher-yates-card-shuffling-algorithm/)
                Random random = new Random();

                List<Cards> shuffledCards = cards.ToList();

                for (int i = shuffledCards.Count - 1; i > 0; --i)
                {
                    int j = random.Next(i + 1);

                    Cards tmp = shuffledCards[i];
                    shuffledCards[i] = shuffledCards[j];
                    shuffledCards[j] = tmp;
                }

                foreach (var card in shuffledCards)
                {
                    cardDeck.Add(card);
                }
            
        }

        /// <summary>
        /// Draws a random card from the deck left
        /// </summary>
        /// <returns>The card drawn from the deck</returns>
        public Cards DrawCard()
        {
            //Random random = new Random();
            //int card = random.Next(0, cardDeck.Count - 1);
            Cards cardDrawn = cardDeck[0];
            cardDeck.Remove(cardDrawn);
            return cardDrawn;
        }
    }

   
}
