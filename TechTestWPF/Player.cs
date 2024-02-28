using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Net.Mime.MediaTypeNames;

namespace TechTestWPF
{
    public class Player// : INotifyPropertyChanged
    {
        private decimal _playerBet;

        public String Name { get; set; }
        public int PlayerId { get; set; } = -1;
        public List<Cards> hand { get; set; } = new List<Cards>();
        public int HandScore { get; set; }
        public decimal PlayerBalance { get; set; }
        public decimal PlayerBet { get; set; }
        public int AceScore { get; set; }

        public PlayerState playerState { get; set; } = PlayerState.None;
        //{
        //    get { return _playerBet; }
        //    set { _UpdateField(ref _playerBet, value); } 
        //}
        public bool isBust { get; set; } = false;
        public bool isBlackjack { get; set; } = false;

        public enum PlayerState
        {
            None,
            InPlay,
            Stuck,
            Bust,
            OptedOut,
            Blackjack
        }

        public int CalculateScore(List<Cards> playerHand, bool countAceAsOne)
        {
            try
            {
                int playerScore = 0;
                bool isAce = false;

                //calculate other cards first as their value cannot be changed, whereas an ace can be 1 or 11, so can change depending on the current score
                foreach (Cards card in playerHand.Where(x => x.cardValue != Cards.CardValue.Ace))
                {
                    if (!card.hiddenCard)
                    {
                        if (card.cardValue == Cards.CardValue.Ace)
                        {
                            isAce = true;
                        }

                        if (card.isTenCard)
                            playerScore += 10;

                        else
                            playerScore += (int)card.cardValue;
                    }
                    else
                    {

                    }
                }

                foreach(Cards card in playerHand.Where(x => x.cardValue == Cards.CardValue.Ace))
                {
                    if (!countAceAsOne)
                    {
                        playerScore += 11;

                        if (playerScore > 21)
                            playerScore -= 10;
                    }
                        
                    else
                    {
playerScore += 1;
                        //playerScore += 11;

                    }




                    //if(playerScore > 21)
                    //{
                    //    playerScore -= 10;
                    //}
                }


                return playerScore;
            }
            catch (Exception exc)
            {
                return 0;
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //private void _UpdateField<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        //{
        //    if (!EqualityComparer<T>.Default.Equals(field, newValue))
        //    {
        //        field = newValue;
        //        _OnPropertyChanged(propertyName);
        //    }
        //}

        //private void _OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //    switch (propertyName)
        //    {
        //        case nameof(PlayerBet):
        //            PlayerBet = _playerBet;
        //            break;
        //    }
        //}

    }
}
