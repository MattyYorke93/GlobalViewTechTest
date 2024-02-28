using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TechTestWPF
{
    public class Cards
    {
        public String cardImage { get; set; }
        public CardValue cardValue { get; set; }
        public Suit suit { get; set; }
        public bool hiddenCard { get; set; } = false;

        public bool isTenCard
        {
            get
            {
                if (cardValue == CardValue.Jack || cardValue == CardValue.Queen || cardValue == CardValue.King)
                    return true;
                
                    
                else
                {
                    return false;
                }
            }
        }

        public Cards()
        {
            cardValue = 0;
            suit = 0;
            cardImage = String.Empty;
        }

        public enum CardValue
        {
            Ace = 1,
            Two = 2,
            Three = 3 ,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10,
            Jack = 11,
            Queen = 12,
            King = 13
        }

        public enum Suit
        {
            Clubs,
            Diamonds,
            Hearts,
            Spades
        }
    }
}
