using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static TechTestWPF.Player;
using Image = System.Windows.Controls.Image;
using Label = System.Windows.Controls.Label;

namespace TechTestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static String appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public MainWindow()
        {
            InitializeComponent();

        }

        async void isStraightBlackjack(Player player)
        {
            try
            {
                if (player.hand.Count == 2)
                {
                    if (player.hand.Any(x => x.isTenCard && player.hand.Any(y => y.cardValue == Cards.CardValue.Ace)))
                    {

                        Label label = new Label();
                        label.Name = "lblBust_" + player.PlayerId;

                        label.Content = "Blackjack";
                        label.Foreground = System.Windows.Media.Brushes.Red;
                        label.FontWeight = FontWeights.Bold;
                        label.Background = System.Windows.Media.Brushes.Transparent;

                        label.FontSize = 40;
                        label.VerticalAlignment = VerticalAlignment.Center;
                        label.HorizontalAlignment = HorizontalAlignment.Center;

                        Grid.SetColumn(label, player.PlayerId);
                        Grid.SetRow(label, 3);

                        var btnStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "btnStackPanel_" + player.PlayerId).FirstOrDefault();
                        var btns = btnStackPanel.Children.OfType<Button>().Where(x => x.DataContext == player).ToList();

                        foreach (var button in btns)
                            button.IsEnabled = false;

                        var betStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "betStackPanel_" + player.PlayerId).FirstOrDefault();
                        var betRow1SP = betStackPanel.Children.OfType<StackPanel>().Where(x => x.Name == "betRow1SP_" + player.PlayerId).FirstOrDefault();
                        var betRow2SP = betStackPanel.Children.OfType<StackPanel>().Where(x => x.Name == "betRow2SP_" + player.PlayerId).FirstOrDefault();

                        var buttons = betRow1SP.Children.OfType<Button>().Where(x => x.DataContext == player).ToList();
                        buttons.AddRange(betRow2SP.Children.OfType<Button>().Where(x => x.DataContext == player).ToList());

                        await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                        {
                            gameGrid.Children.Add(label);
                            foreach (var button in buttons)
                                button.IsEnabled = false;
                        }));

                        player.playerState = Player.PlayerState.Blackjack;

                        //return true;
                    }


                }
                //return false;
            }
            catch (Exception exc)
            {
                //return false;
            }
        }

        int numberOfPlayers = 0;
        int numberOfDecks = 0;


        List<Player> playerList = new List<Player>();
        CardDeck cardDeck;
        private async void btnConfirmSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnDeal.IsEnabled = false;
                //for (int i = 1; i <= numberOfPlayers; i++)
                //{
                //foreach (var tmp in playerNamesGrid.Children.OfType<StackPanel>())
                //{
                int id = 0;
                foreach (var tb in playerNamesGrid.Children.OfType<TextBox>())
                {
                    Player player = new Player();
                    player.Name = tb.Text;
                    player.PlayerId = id;
                    player.playerState = Player.PlayerState.None;
                    player.HandScore = 0;
                    player.AceScore = 0;
                    playerList.Add(player);

                    CreateChipButtons(id, player);

                    id++;
                }

                for (int i = id; i < 7; i++)
                {
                    AddNewPlayerButton(i);
                    
                }
                //}

                gameGrid.Visibility = Visibility.Visible;

                if (numberOfDecks == 0 || tbNoOfDecks.Text == string.Empty)
                    numberOfDecks = 1;

                numberOfDecks = int.Parse(tbNoOfDecks.Text);
                cardDeck = new CardDeck(numberOfDecks);
                StartGrid.Visibility = Visibility.Collapsed;

            }
            catch (Exception exc)
            {

            }
        }

        private void AddNewPlayerButton(int id)
        {
            try
            {
                Button addPlayerBtn = new Button();

                addPlayerBtn.Content = new System.Windows.Controls.Image
                {
                    Source = new BitmapImage(new Uri(appStartPath + @"\Images\AddPlayer.png")),
                };

                addPlayerBtn.Tag = id;
                addPlayerBtn.Name = "AddPlayer_" + id;
                addPlayerBtn.Click += AddPlayerBtn_Click;
                addPlayerBtn.Background = System.Windows.Media.Brushes.Transparent;

                Grid.SetRow(addPlayerBtn, 3);
                Grid.SetColumn(addPlayerBtn, id);
                gameGrid.Children.Add(addPlayerBtn);
            }
            catch (Exception exc)
            {

            }
        }

        private void AddPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;

                int id = int.Parse(btn.Tag.ToString());
                gameGrid.Children.Remove(btn);

                StackPanel spNewPlayer = new StackPanel();
                spNewPlayer.Name = "spNewPlayer_" + id;

                TextBox tbNewPlayerName = new TextBox();
                tbNewPlayerName.Name = "tbAddPlayer_" + id;
                tbNewPlayerName.FontSize = 25;
                tbNewPlayerName.Height = 35;
                Grid.SetColumn(tbNewPlayerName, id);
                Grid.SetRow(tbNewPlayerName, 1);

                gameGrid.Children.Add(tbNewPlayerName);

                Button addPlayer = new Button();
                addPlayer.Name = "btnAddPlayer";
                addPlayer.Content = "Add Player";
                //addPlayer.Height = 40;
                addPlayer.FontSize = 25;
                addPlayer.DataContext = tbNewPlayerName.Text + "_" + id;
                addPlayer.Click += AddPlayer_Click;
                //Grid.SetColumn(addPlayer, id);
                //Grid.SetRow(addPlayer, 2);

                spNewPlayer.Children.Add(addPlayer);


                Button Backbtn = new Button();
                Backbtn.Name = "btnBack";
                Backbtn.Content = "Back";
                Backbtn.Height = 40;
                Backbtn.FontSize = 25;
                Backbtn.DataContext = id;
                Backbtn.Click += Backbtn_Click;
                //Grid.SetColumn(Backbtn, id);
                //Grid.SetRow(Backbtn, 2);

                spNewPlayer.Children.Add(Backbtn);

                Grid.SetColumn(spNewPlayer, id);
                Grid.SetRow(spNewPlayer, 2);
                gameGrid.Children.Add(spNewPlayer);


            }
            catch (Exception exc)
            {

            }
        }

        private async void Backbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;

                if(btn.DataContext != null)
                {
                    int id = int.Parse(btn.DataContext.ToString());

                    var tbNewPlayer = gameGrid.Children.OfType<TextBox>().Where(x => x.Name == "tbAddPlayer_" + id).FirstOrDefault();

                    var spNewPlayer = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "spNewPlayer_" + id).FirstOrDefault();

                    await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                    {
                        gameGrid.Children.Remove(spNewPlayer);
                        gameGrid.Children.Remove(tbNewPlayer);
                        
                    }));

                    AddNewPlayerButton(id);
                }
            }
            catch(Exception exc)
            {

            }
        }

        private async void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;

                var dataContext = button.DataContext.ToString().Split('_');
                string playerName = dataContext[0];
                int id = int.Parse(dataContext[1]);

                var tbNewPlayer = gameGrid.Children.OfType<TextBox>().Where(x => x.Name == "tbAddPlayer_" + id).FirstOrDefault();

                Player player = new Player();
                player.Name = tbNewPlayer.Text;
                player.PlayerId = id;
                player.playerState = Player.PlayerState.None;
                player.AceScore = 0;
                player.HandScore = 0;
                playerList.Add(player);

                var spNewPlayer = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "spNewPlayer_" + id).FirstOrDefault();

                await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                {
                    gameGrid.Children.Remove(spNewPlayer);
                    gameGrid.Children.Remove(tbNewPlayer);
                    CreateChipButtons(id, player);
                }));



            }
            catch (Exception exc)
            {

            }
        }

        private void ChipBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Button btn = sender as Button;

                if (btn != null)
                {
                    Player player = btn.DataContext as Player;
                    string amount = btn.Name;

                    int chipAmt = 0;

                    switch (amount)
                    {
                        case "OnePound":
                            chipAmt = 100;
                            break;

                        case "FivePound":
                            chipAmt = 500;
                            break;

                        case "TenPound":
                            chipAmt = 1000;
                            break;

                        case "FiftyPound":
                            chipAmt = 5000;
                            break;

                        case "OneHundredPound":
                            chipAmt = 10000;
                            break;

                        case "OneThousandPound":
                            chipAmt = 100000;
                            break;
                    }

                    player.PlayerBet += chipAmt;
                    var label = gameGrid.Children.OfType<Label>().Where(x => x.Name == "betLbl_" + player.PlayerId).FirstOrDefault();
                    label.Content = "Bet placed : " + ((player.PlayerBet / 100).ToString("C"));


                    player.PlayerBalance -= chipAmt;
                    var balanceLabel = gameGrid.Children.OfType<Label>().Where(x => x.Name == "balanceLbl_" + player.PlayerId).FirstOrDefault();
                    balanceLabel.Content = "Current Balance : " + ((player.PlayerBalance / 100).ToString("C"));

                    player.playerState = Player.PlayerState.InPlay;

                    if (!btnDeal.IsEnabled)
                        btnDeal.IsEnabled = true;
                }
            }
            catch (Exception exc)
            {

            }
        }

        List<String> betChips = new List<string>() { "OnePound", "FivePound", "TenPound", "FiftyPound", "OneHundredPound", "OneThousandPound" };

        private void CreateChipButtons(int id, Player player)
        {
            int column = id;
            try
            {
                //foreach (Player player in playerList)
                //{
                Label lblName = new Label();
                lblName.Content = player.Name;
                Grid.SetRow(lblName, 1);
                Grid.SetColumn(lblName, column);
                lblName.FontSize = 25;
                lblName.VerticalAlignment = VerticalAlignment.Center;
                lblName.HorizontalAlignment = HorizontalAlignment.Center;
                gameGrid.Children.Add(lblName);

                Button btnHit = new Button();
                Button btnStick = new Button();

                btnHit.Content = "Hit";
                btnStick.Content = "Stick";

                btnHit.Tag = player.Name + "_Hit";
                btnStick.Tag = player.Name + "_Stick";

                btnHit.DataContext = player;
                btnHit.Click += BtnHit_Click;

                btnStick.DataContext = player;
                btnStick.Click += BtnStick_Click;

                btnStick.IsEnabled = false;
                btnHit.IsEnabled = false;

                Label lblScore = new Label();
                lblScore.Name = "lblScore_" + player.PlayerId;
                lblScore.Content = "";
                lblScore.FontSize = 22;


                StackPanel btnStackPanel = new StackPanel();
                btnStackPanel.Name = "btnStackPanel_" + player.PlayerId;
                btnStackPanel.Children.Add(btnHit);
                btnStackPanel.Children.Add(btnStick);

                StackPanel spScore = new StackPanel();
                spScore.Name = "spScore_" + player.PlayerId;
                spScore.Children.Add(lblScore);
                btnStackPanel.Children.Add(spScore);


                Grid.SetRow(btnStackPanel, 3);
                Grid.SetColumn(btnStackPanel, column);

                gameGrid.Children.Add(btnStackPanel);

                StackPanel betStackPanel = new StackPanel();
                betStackPanel.Name = "betStackPanel_" + player.PlayerId;
                betStackPanel.Orientation = Orientation.Vertical;
                StackPanel betRow1SP = new StackPanel();
                betRow1SP.Name = "betRow1SP_" + player.PlayerId;
                StackPanel betRow2SP = new StackPanel();
                betRow2SP.Name = "betRow2SP_" + player.PlayerId;
                betRow1SP.Orientation = Orientation.Horizontal;
                betRow2SP.Orientation = Orientation.Horizontal;
                int noOfBtns = 1;

                foreach (string amount in betChips)
                {
                    Button chipBtn = new Button();
                    chipBtn.Style = (Style)FindResource("chipBtn");// as Style;

                    chipBtn.Content = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri(appStartPath + @"\Images\chip" + amount + ".png")),
                    };

                    //chipBtn.Tag = amount + "_" + player.PlayerId;
                    chipBtn.DataContext = player;
                    chipBtn.Name = amount;
                    chipBtn.Click += ChipBtn_Click;

                    if (noOfBtns > 3)
                    {
                        betRow2SP.Children.Add(chipBtn);
                    }
                    else
                        betRow1SP.Children.Add(chipBtn);



                    noOfBtns++;
                }
                betStackPanel.Children.Add(betRow1SP);
                betStackPanel.Children.Add(betRow2SP);

                betStackPanel.VerticalAlignment = VerticalAlignment.Center;

                Grid.SetRow(betStackPanel, 4);
                Grid.SetColumn(betStackPanel, column);
                gameGrid.Children.Add(betStackPanel);

                //Binding b = new Binding((player.PlayerBet / 100).ToString("C"));

                Label betLbl = new Label();
                betLbl.Name = "betLbl_" + player.PlayerId;
                //betLbl.Content = b.Path;
                betLbl.FontSize = 18;
                betLbl.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetColumn(betLbl, column);
                Grid.SetRow(betLbl, 5);
                gameGrid.Children.Add(betLbl);

                Label balanceLbl = new Label();
                balanceLbl.FontSize = 18;
                balanceLbl.Name = "balanceLbl_" + player.PlayerId;
                balanceLbl.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetColumn(balanceLbl, column);
                Grid.SetRow(balanceLbl, 6);
                gameGrid.Children.Add(balanceLbl);

                //d.Add(betLbl + "_" + player.PlayerId, 0);

                //column++;

                //}
            }
            catch (Exception exc)
            {

            }
        }

        private async void BtnStick_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;

                if (btn != null)
                {
                    //if aces, calculate highest score


                    Player player = btn.DataContext as Player;
                    player.playerState = Player.PlayerState.Stuck;

                    UpdateScore(player);


                    DisableButtons(player);

                    //if no players still in play, reveal dealers cards
                    if (playerList.All(x => x.playerState != Player.PlayerState.InPlay))
                    {

                        RevealDealerCard();
                    }
                }
            }
            catch (Exception exc)
            {

            }

        }

        private async void UpdateScore(Player player)
        {
            try
            {
                var btnStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "btnStackPanel_" + player.PlayerId).FirstOrDefault();

                var scoreStackPanel = btnStackPanel.Children.OfType<StackPanel>().Where(x => x.Name == "spScore_" + player.PlayerId).FirstOrDefault();
                var Scorelabel = scoreStackPanel.Children.OfType<Label>().Where(x => x.Name == "lblScore_" + player.PlayerId).FirstOrDefault();

                

                if (Scorelabel.Content.ToString().Contains("/"))
                {
                    
                    var playerScores = Scorelabel.Content.ToString().Split(':')[1].Split('/');

                    List<int> scores = new List<int>();
                    foreach (var score in playerScores)
                    {
                        scores.Add(int.Parse(score));
                    }

                    var scoreList = scores.OrderBy(x => x);
                    player.HandScore = int.Parse(playerScores[0]);
                }
                else
                    Scorelabel.Content = "Current Score : " + player.HandScore;


            }
            catch (Exception exc)
            {

            }
        }

        public async void DisableButtons(Player player)
        {
            try
            {
                var btnStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "btnStackPanel_" + player.PlayerId).FirstOrDefault();

                var btns = btnStackPanel.Children.OfType<Button>().Where(x => x.DataContext == player).ToList();

                var betStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "betStackPanel_" + player.PlayerId).FirstOrDefault();
                var betRow1SP = betStackPanel.Children.OfType<StackPanel>().Where(x => x.Name == "betRow1SP_" + player.PlayerId).FirstOrDefault();
                var betRow2SP = betStackPanel.Children.OfType<StackPanel>().Where(x => x.Name == "betRow2SP_" + player.PlayerId).FirstOrDefault();

                var buttons = betRow1SP.Children.OfType<Button>().Where(x => x.DataContext == player).ToList();
                buttons.AddRange(betRow2SP.Children.OfType<Button>().Where(x => x.DataContext == player).ToList());

                await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                {
                    foreach (var button in buttons)
                        button.IsEnabled = false;

                    foreach (var btn in btns)
                        btn.IsEnabled = false;
                }));
            }
            catch (Exception exc)
            {

            }
        }

        public async void RevealDealerCard()
        {
            try
            {
                await Task.Delay(500);
                var image = gameGrid.Children.OfType<Image>().Where(x => x.Name == "DealerBackCard").FirstOrDefault();
                Cards hiddenCard = dealer.hand.Find(x => x.hiddenCard);// dealer.hiddenCard;

                await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                {
                    image.Source = new BitmapImage(new Uri(appStartPath + @"\Images\" + hiddenCard.cardImage));
                }));

                await Task.Delay(500);

                hiddenCard.hiddenCard = false;

                dealer.HandScore = dealer.CalculateScore(dealer.hand, false);

                var lblDealerScore = gameGrid.Children.OfType<Label>().Where(x => x.Name == "lblDealerScore").FirstOrDefault();
                lblDealerScore.Content = "Dealer Score : " + Environment.NewLine + dealer.HandScore;

                //dealer score??

                while (dealer.HandScore < 17)
                {
                    Cards card = cardDeck.DrawCard();

                    int noOfCards = dealer.hand.Count;
                    Image cardImage = new Image();
                    cardImage.Name = "cardImage_dealer";
                    cardImage.Source = new BitmapImage(new Uri(appStartPath + @"\Images\" + card.cardImage));
                    cardImage.HorizontalAlignment = HorizontalAlignment.Left;
                    cardImage.VerticalAlignment = VerticalAlignment.Top;
                    int leftMargin = (noOfCards) * 35;
                    cardImage.Margin = new Thickness(leftMargin + 15, 30, 0, 30);
                    Grid.SetColumn(cardImage, 3);
                    Grid.SetRow(cardImage, 0);
                    Grid.SetColumnSpan(cardImage, 3);

                    dealer.hand.Add(card);

                    dealer.HandScore = dealer.CalculateScore(dealer.hand, false);
                    await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                    {
                        gameGrid.Children.Add(cardImage);
                        lblDealerScore.Content = "Dealer Score : " + Environment.NewLine + dealer.HandScore;
                    }));
                    await Task.Delay(500);

                    if (dealer.HandScore > 21)
                    {
                        if (dealer.hand.Any(x => x.cardValue == Cards.CardValue.Ace))
                        {
                            dealer.HandScore = dealer.CalculateScore(dealer.hand, true);
                            lblDealerScore.Content = "Dealer Score : " + Environment.NewLine + dealer.HandScore;
                        }

                    }
                }

                if (dealer.HandScore > 21)
                {

                    Label label = new Label();
                    label.Name = "lblBust_dealer";

                    label.Content = "BUST";
                    label.Foreground = System.Windows.Media.Brushes.Red;
                    label.FontWeight = FontWeights.Bold;
                    label.Background = System.Windows.Media.Brushes.Transparent;

                    label.FontSize = 100;
                    label.VerticalAlignment = VerticalAlignment.Center;
                    label.HorizontalAlignment = HorizontalAlignment.Center;

                    Grid.SetColumn(label, 2);
                    Grid.SetColumnSpan(label, 3);
                    Grid.SetRow(label, 0);
                    gameGrid.Children.Add(label);

                    dealer.HandScore = 0;

                }


                if (dealer.hand.Count == 2 && dealer.hand.Any(x => x.isTenCard && dealer.hand.Any(y => y.cardValue == Cards.CardValue.Ace)))
                {
                    dealer.isBlackjack = true;

                }


                CompareAgainstPlayerScores();

            }
            catch (Exception exc)
            {

            }
        }

        private async void CompareAgainstPlayerScores()
        {
            try
            {
                foreach (Player player in playerList.Where(x => x.playerState != Player.PlayerState.Bust && x.playerState != Player.PlayerState.OptedOut).OrderBy(x => x.PlayerId))
                {
                    if (dealer.isBlackjack && player.isBlackjack)
                    {
                        decimal winAmount = player.PlayerBet;
                        player.PlayerBalance += winAmount;

                        Label label = new Label();
                        label.Name = "lblBust_" + player.PlayerId;

                        label.Content = "PUSH";
                        label.Foreground = System.Windows.Media.Brushes.Red;
                        label.FontWeight = FontWeights.Bold;
                        label.Background = System.Windows.Media.Brushes.Transparent;

                        label.FontSize = 50;
                        label.VerticalAlignment = VerticalAlignment.Center;
                        label.HorizontalAlignment = HorizontalAlignment.Center;

                        Grid.SetColumn(label, player.PlayerId);
                        Grid.SetRow(label, 3);

                        await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                        {
                            var balanceLabel = gameGrid.Children.OfType<Label>().Where(x => x.Name == "balanceLbl_" + player.PlayerId).FirstOrDefault();
                            balanceLabel.Content = "Current Balance : " + ((player.PlayerBalance / 100).ToString("C"));
                            gameGrid.Children.Add(label);

                            //Push (no winner)
                        }));
                    }
                    else if (player.isBlackjack)
                    {
                        decimal winAmount = player.PlayerBet * 2.5m;
                        player.PlayerBalance += winAmount;
                        //blackjack pays 3 to 2

                        Label label = new Label();
                        label.Name = "lblBust_" + player.PlayerId;

                        label.Content = "BLACKJACK" + Environment.NewLine + "YOU WIN " + (winAmount / 100).ToString("C");
                        label.Foreground = System.Windows.Media.Brushes.Red;
                        label.FontWeight = FontWeights.Bold;
                        label.Background = System.Windows.Media.Brushes.Transparent;

                        label.FontSize = 25;
                        label.VerticalAlignment = VerticalAlignment.Center;
                        label.HorizontalAlignment = HorizontalAlignment.Center;
                        label.HorizontalContentAlignment = HorizontalAlignment.Center;

                        Grid.SetColumn(label, player.PlayerId);
                        Grid.SetRow(label, 3);

                        await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                        {
                            gameGrid.Children.Add(label);
                            var balanceLabel = gameGrid.Children.OfType<Label>().Where(x => x.Name == "balanceLbl_" + player.PlayerId).FirstOrDefault();
                            balanceLabel.Content = "Current Balance : " + ((player.PlayerBalance / 100).ToString("C"));

                            //Blackjack() do animation or something??
                        }));
                    }
                    else if (player.HandScore == dealer.HandScore)
                    {
                        //no one wins, player gets money back
                        decimal winAmount = player.PlayerBet;
                        player.PlayerBalance += winAmount;

                        Label label = new Label();
                        label.Name = "lblBust_" + player.PlayerId;

                        label.Content = "PUSH";
                        label.Foreground = System.Windows.Media.Brushes.Red;
                        label.FontWeight = FontWeights.Bold;
                        label.Background = System.Windows.Media.Brushes.Transparent;

                        label.FontSize = 50;
                        label.VerticalAlignment = VerticalAlignment.Center;
                        label.HorizontalAlignment = HorizontalAlignment.Center;

                        Grid.SetColumn(label, player.PlayerId);
                        Grid.SetRow(label, 3);

                        await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                        {
                            var balanceLabel = gameGrid.Children.OfType<Label>().Where(x => x.Name == "balanceLbl_" + player.PlayerId).FirstOrDefault();
                            balanceLabel.Content = "Current Balance : " + ((player.PlayerBalance / 100).ToString("C"));
                            gameGrid.Children.Add(label);

                            //Push (no winner)
                        }));
                    }
                    else if (player.HandScore > dealer.HandScore)
                    {
                        //player wins
                        decimal winAmount = player.PlayerBet * 2;
                        player.PlayerBalance += winAmount;

                        Label label = new Label();
                        label.Name = "lblBust_" + player.PlayerId;

                        label.Content = "YOU WIN" + Environment.NewLine + (winAmount / 100).ToString("C");
                        label.Foreground = System.Windows.Media.Brushes.Red;
                        label.FontWeight = FontWeights.Bold;
                        label.Background = System.Windows.Media.Brushes.Transparent;

                        label.FontSize = 30;
                        label.VerticalAlignment = VerticalAlignment.Center;
                        label.HorizontalAlignment = HorizontalAlignment.Center;

                        Grid.SetColumn(label, player.PlayerId);
                        Grid.SetRow(label, 3);

                        await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                        {
                            var balanceLabel = gameGrid.Children.OfType<Label>().Where(x => x.Name == "balanceLbl_" + player.PlayerId).FirstOrDefault();
                            balanceLabel.Content = "Current Balance : " + ((player.PlayerBalance / 100).ToString("C"));
                            gameGrid.Children.Add(label);
                            //Lose
                        }));
                    }
                    else if (dealer.HandScore > player.HandScore)
                    {
                        Label label = new Label();
                        label.Name = "lblBust_" + player.PlayerId;

                        label.Content = "LOSE";
                        label.Foreground = System.Windows.Media.Brushes.Red;
                        label.FontWeight = FontWeights.Bold;
                        label.Background = System.Windows.Media.Brushes.Transparent;

                        label.FontSize = 50;
                        label.VerticalAlignment = VerticalAlignment.Center;
                        label.HorizontalAlignment = HorizontalAlignment.Center;

                        Grid.SetColumn(label, player.PlayerId);
                        Grid.SetRow(label, 3);

                        DisableButtons(player);

                        await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                        {
                            gameGrid.Children.Add(label);

                        }));

                    }

                    await Task.Delay(500);
                }
                await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                {
                    spNewGame.Visibility = Visibility.Visible;
                }));
            }
            catch (Exception exc)
            {

            }
        }

        private async void BtnHit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;

                if (btn != null)
                {
                    Player player = btn.DataContext as Player;

                    Cards card = cardDeck.DrawCard();

                    player.hand.Add(card);

                    int noOfCards = player.hand.Count;

                    int leftMargin = (noOfCards - 1) * 25;

                    Image cardImage = new Image();
                    cardImage.Name = "cardImage_" + player.PlayerId;
                    cardImage.Source = new BitmapImage(new Uri(appStartPath + @"\Images\" + card.cardImage));
                    cardImage.HorizontalAlignment = HorizontalAlignment.Left;
                    cardImage.Margin = new Thickness(leftMargin, 0, 0, 0);
                    Grid.SetColumn(cardImage, player.PlayerId);
                    Grid.SetRow(cardImage, 2);
                    gameGrid.Children.Add(cardImage);

                    var btnStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "btnStackPanel_" + player.PlayerId).FirstOrDefault();
                    var scoreStackPanel = btnStackPanel.Children.OfType<StackPanel>().Where(x => x.Name == "spScore_" + player.PlayerId).FirstOrDefault();
                    var Scorelabel = scoreStackPanel.Children.OfType<Label>().Where(x => x.Name == "lblScore_" + player.PlayerId).FirstOrDefault();
                    Scorelabel.Content = "";

                    player.HandScore = player.CalculateScore(player.hand, false);

                    UpdateScore(player);


                    if (player.HandScore > 21)
                    {
                        if (player.hand.Any(x => x.cardValue == Cards.CardValue.Ace))
                        {
                            player.HandScore = player.CalculateScore(player.hand, true);
                            UpdateScore(player);
                            //Scorelabel.Content = "Current Score : " + player.HandScore;
                        }

                        player.isBust = true;
                        player.playerState = Player.PlayerState.Bust;

                        Label label = new Label();
                        label.Name = "lblBust_" + player.PlayerId;

                        label.Content = "BUST";
                        label.Foreground = System.Windows.Media.Brushes.Red;
                        label.FontWeight = FontWeights.Bold;
                        label.Background = System.Windows.Media.Brushes.Transparent;

                        label.FontSize = 50;
                        label.VerticalAlignment = VerticalAlignment.Center;
                        label.HorizontalAlignment = HorizontalAlignment.Center;

                        Grid.SetColumn(label, player.PlayerId);
                        Grid.SetRow(label, 3);
                        gameGrid.Children.Add(label);

                        DisableButtons(player);
                    }
                    else
                    {
                        if (player.hand.Any(x => x.cardValue == Cards.CardValue.Ace))
                        {
                            //player.HandScore = player.CalculateScore(player.hand, true);
                            player.AceScore = player.CalculateScore(player.hand, true);
                            UpdateScore(player);
                            if (player.AceScore <= 21 && player.AceScore != player.HandScore)
                                Scorelabel.Content += " / " + player.AceScore;
                        }
                    }
                    if (player.HandScore == 21)
                    {
                        DisableButtons(player);
                        //var btnStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "btnStackPanel_" + player.PlayerId).FirstOrDefault();
                        var stickHitBtns = btnStackPanel.Children.OfType<Button>().Where(x => x.DataContext == player).ToList();

                        await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                        {
                            foreach (var button in stickHitBtns)
                                btn.IsEnabled = false;
                        }));

                        player.playerState = Player.PlayerState.Stuck;
                    }
                    if (playerList.All(x => x.playerState != Player.PlayerState.InPlay))
                    {

                        RevealDealerCard();
                    }

                }
            }
            catch (Exception exc)
            {

            }
        }


        private void tbNoOfDecks_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                var key = e.Text;
                if (int.TryParse(key, out int noOfDecks))
                {
                    //numberOfDecks = noOfDecks;
                    //playerNamesGrid.Children.Clear();

                    //for (int i = 1; i <= noOfPlayers; i++)
                    //{
                    //    StackPanel stackPanel = new StackPanel();
                    //    playerNamesGrid
                    //}
                }

                else
                    e.Handled = true;
                //else if (e.Key == Key.Back || e.Key == Key.Delete)
                //{

                //}
                //numberOfDecks = 2;
            }
            catch (Exception exc)
            {



            }
        }


        private void tbNoOfPlayers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var key = e.Text;
            if (int.TryParse(key, out int noOfPlayers))
            {

                if (noOfPlayers > 7)
                {
                    MessageBox.Show("Max number of 7 players allowed", "Players exceeded", MessageBoxButton.OK, MessageBoxImage.Information);
                    tbNoOfPlayers.Text = String.Empty;
                    playerList = new List<Player>();
                    e.Handled = true;
                    return;
                }

                numberOfPlayers = noOfPlayers;
                //tbNoOfPlayers.Text = noOfPlayers.ToString();

                for (int i = 1; i <= numberOfPlayers; i++)
                {
                    playerNamesGrid.Visibility = Visibility.Visible;

                    //playerNamesGrid.RowDefinitions.Add(new RowDefinition());

                    //StackPanel stackPanel = new StackPanel();
                    //stackPanel.Height = 150;
                    //stackPanel.Orientation = Orientation.Horizontal;

                    Label nameLabel = new Label();
                    nameLabel.Name = "lblName_" + i;
                    nameLabel.Content = "Player " + i + " Name";
                    nameLabel.FontSize = 25;
                    nameLabel.Margin = new Thickness(0, 10, 0, 10);
                    Grid.SetRow(nameLabel, i - 1);


                    TextBox tbPlayerName = new TextBox();
                    tbPlayerName.Name = "tbPlayerName_" + i;
                    tbPlayerName.Text = String.Empty;
                    tbPlayerName.Width = 300;
                    tbPlayerName.HorizontalAlignment = HorizontalAlignment.Left;
                    tbPlayerName.Margin = new Thickness(0, 10, 0, 10);
                    tbPlayerName.FontSize = 25;
                    tbPlayerName.VerticalContentAlignment = VerticalAlignment.Center;
                    //Width="150" HorizontalAlignment="Left"
                    Grid.SetRow(tbPlayerName, i - 1);
                    Grid.SetColumn(tbPlayerName, 1);

                    //stackPanel.Children.Add(nameLabel);
                    //stackPanel.Children.Add(tbPlayerName);

                    //Grid.SetRow(stackPanel, i - 1);


                    //playerNamesGrid.Children.Add(stackPanel);
                    //yaxis += (int)stackPanel.Height;
                    playerNamesGrid.Children.Add(nameLabel);
                    playerNamesGrid.Children.Add(tbPlayerName);
                }
            }
            else
                e.Handled = true;
            //else if (e..Key == Key.Back || e.Key == Key.Delete)
            //{
            //    tbNoOfPlayers.Text = tbNoOfPlayers.Text.Remove(tbNoOfPlayers.Text.Length - 1);

            //}
        }

        Dealer dealer;
        private async void btnDeal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (playerList.All(x => x.PlayerBet <= 0))
                {
                    MessageBox.Show("At least one player must place a bet", "Please place bet", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                btnDeal.Visibility = Visibility.Collapsed;
                Cards cards = new Cards();
                dealer = new Dealer();
                int leftMargin = 0;
                for (int cardDealt = 1; cardDealt <= 2; cardDealt++)
                {
                    Cards card;

                    foreach (Player player in playerList) //.Where(x => x.PlayerBet > 0))
                    {
                        if (player.PlayerBet == 0)
                        {
                            player.playerState = Player.PlayerState.OptedOut;

                            DisableButtons(player);
                        }
                        else
                        {

                            if (player.hand == null)
                                player.hand = new List<Cards>();


                            card = cardDeck.DrawCard();
                            player.hand.Add(card);

                            Image cardImage = new Image();
                            cardImage.Name = "cardImage_" + player.PlayerId;
                            cardImage.Source = new BitmapImage(new Uri(appStartPath + @"\Images\" + card.cardImage));
                            cardImage.HorizontalAlignment = HorizontalAlignment.Left;
                            cardImage.Margin = new Thickness(leftMargin, 0, 0, 0);
                            Grid.SetColumn(cardImage, player.PlayerId);
                            Grid.SetRow(cardImage, 2);
                            gameGrid.Children.Add(cardImage);

                            //display card drawn

                            if (cardDealt == 2)
                            {
                                DisableButtons(player);
                                //btnStackPanel --> scorestackpanel --> label
                                var btnStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "btnStackPanel_" + player.PlayerId).FirstOrDefault();
                                var scoreStackPanel = btnStackPanel.Children.OfType<StackPanel>().Where(x => x.Name == "spScore_" + player.PlayerId).FirstOrDefault();
                                var Scorelabel = scoreStackPanel.Children.OfType<Label>().Where(x => x.Name == "lblScore_" + player.PlayerId).FirstOrDefault();

                                //if score is over 21 and contains an ace, count ace as 1
                                player.HandScore = player.CalculateScore(player.hand, false);
                                UpdateScore(player);

                                //var btnStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "btnStackPanel_" + player.PlayerId).FirstOrDefault();
                                var stickHitBtns = btnStackPanel.Children.OfType<Button>().Where(x => x.DataContext == player).ToList();

                                await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                                {
                                    foreach (var btn in stickHitBtns)
                                        btn.IsEnabled = true;
                                }));


                                //Scorelabel.Content = "Current Score : " + player.HandScore;
                                if (player.HandScore == 21)
                                {
                                    //bool blackjack = isStraightBlackjack(player).Result;
                                    //is it blackjack?

                                    player.playerState = Player.PlayerState.Blackjack;

                                    player.isBlackjack = true;
                                    //DisableButtons(player);

                                    await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                                    {
                                        foreach (var btn in stickHitBtns)
                                            btn.IsEnabled = false;
                                    }));

                                    if (playerList.All(x => x.playerState == Player.PlayerState.Blackjack))
                                        RevealDealerCard();

                                }

                                else if (player.hand.Any(x => x.cardValue == Cards.CardValue.Ace))
                                {
                                    //player.HandScore = player.CalculateScore(player.hand, true);
                                    player.AceScore = player.CalculateScore(player.hand, true);
                                    UpdateScore(player);
                                    if (player.AceScore <= 21 && player.AceScore != player.HandScore)
                                        Scorelabel.Content += " / " + player.AceScore;

                                }
                                if (player.HandScore > 21)
                                {


                                    player.isBust = true;
                                }


                            }
                        }
                    }


                    card = cardDeck.DrawCard();
                    dealer.hand.Add(card);
                    if (cardDealt == 1)
                    {
                        //dealer.hiddenCard = card;

                        //dealer.hideCard = true;
                        card.hiddenCard = true;
                        Image cardImage = new Image();
                        cardImage.Name = "DealerBackCard";
                        cardImage.Source = new BitmapImage(new Uri(appStartPath + @"\Images\CardBack.png"));
                        cardImage.HorizontalAlignment = HorizontalAlignment.Left;
                        cardImage.VerticalAlignment = VerticalAlignment.Top;
                        cardImage.Margin = new Thickness(leftMargin, 30, 0, 30);
                        Grid.SetColumn(cardImage, 3);
                        Grid.SetRow(cardImage, 0);
                        Grid.SetColumnSpan(cardImage, 3);
                        gameGrid.Children.Add(cardImage);
                    }
                    else
                    {

                        Image cardImage = new Image();
                        cardImage.Name = "cardImage_dealer";
                        cardImage.Source = new BitmapImage(new Uri(appStartPath + @"\Images\" + card.cardImage));
                        cardImage.HorizontalAlignment = HorizontalAlignment.Left;
                        cardImage.VerticalAlignment = VerticalAlignment.Top;
                        cardImage.Margin = new Thickness(leftMargin + 15, 30, 0, 30);
                        Grid.SetColumn(cardImage, 3);
                        Grid.SetRow(cardImage, 0);
                        Grid.SetColumnSpan(cardImage, 3);
                        gameGrid.Children.Add(cardImage);

                        int dealerScore = dealer.CalculateScore(dealer.hand, false);


                        Label lblDealerScore = new Label();
                        lblDealerScore.Name = "lblDealerScore";
                        Grid.SetColumn(lblDealerScore, 1);
                        Grid.SetRow(lblDealerScore, 0);
                        lblDealerScore.FontSize = 25;
                        lblDealerScore.VerticalAlignment = VerticalAlignment.Bottom;
                        lblDealerScore.Content = "Current Score : " + dealerScore;
                        gameGrid.Children.Add(lblDealerScore);


                        Label lblDealerName = new Label();
                        lblDealerName.Name = "lblDealerName";
                        Grid.SetColumn(lblDealerName, 1);
                        Grid.SetRow(lblDealerName, 0);
                        lblDealerName.FontSize = 25;
                        lblDealerName.VerticalAlignment = VerticalAlignment.Top;
                        lblDealerName.Content = "Dealer Name :" + Environment.NewLine + "Alan";
                        gameGrid.Children.Add(lblDealerName);

                        //cardImage.Source = imagePath;
                        //display card drawn

                        if (dealer.hand.Any(x => x.isTenCard && dealer.hand.Any(y => y.cardValue == Cards.CardValue.Ace)))
                        {
                            dealer.isBlackjack = true;
                            dealer.playerState = PlayerState.Blackjack;

                            foreach (Player player in playerList)
                            {
                                player.playerState = PlayerState.Stuck;
                                DisableButtons(player);
                            }
                            RevealDealerCard();
                        }
                    }

                    leftMargin += 25;
                }

            }
            catch (Exception ex)
            {

            }
        }

        private async void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            foreach (Player player in playerList)
            {
                player.hand = new List<Cards>();
                player.isBlackjack = false;
                player.PlayerBet = 0;
                player.isBust = false;
                player.playerState = Player.PlayerState.None;
                player.AceScore = 0;
                player.HandScore = 0;

                var btnStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "btnStackPanel_" + player.PlayerId).FirstOrDefault();
                var scoreStackPanel = btnStackPanel.Children.OfType<StackPanel>().Where(x => x.Name == "spScore_" + player.PlayerId).FirstOrDefault();
                var Scorelabel = scoreStackPanel.Children.OfType<Label>().Where(x => x.Name == "lblScore_" + player.PlayerId).FirstOrDefault();

                var cardImage = gameGrid.Children.OfType<Image>().Where(x => x.Name == "cardImage_" + player.PlayerId).ToList();

                var dealerCards = gameGrid.Children.OfType<Image>().Where(x => x.Name == "cardImage_dealer").ToList();
                dealerCards.AddRange(gameGrid.Children.OfType<Image>().Where(x => x.Name == "DealerBackCard").ToList());
                //var betStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "betStackPanel").FirstOrDefault();

                var btns = btnStackPanel.Children.OfType<Button>().Where(x => x.DataContext == player).ToList();

                var betStackPanel = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "betStackPanel_" + player.PlayerId).FirstOrDefault();
                var betRow1SP = betStackPanel.Children.OfType<StackPanel>().Where(x => x.Name == "betRow1SP_" + player.PlayerId).FirstOrDefault();
                var betRow2SP = betStackPanel.Children.OfType<StackPanel>().Where(x => x.Name == "betRow2SP_" + player.PlayerId).FirstOrDefault();

                var bustlbl = gameGrid.Children.OfType<Label>().Where(x => x.Name == "lblBust_" + player.PlayerId).FirstOrDefault();

                var buttons = betRow1SP.Children.OfType<Button>().Where(x => x.DataContext == player).ToList();
                buttons.AddRange(betRow2SP.Children.OfType<Button>().Where(x => x.DataContext == player).ToList());

                var lblBetPlaced = gameGrid.Children.OfType<Label>().Where(x => x.Name == "betLbl_" + player.PlayerId).FirstOrDefault();

                var bustlbldealer = gameGrid.Children.OfType<Label>().Where(x => x.Name == "lblBust_dealer").FirstOrDefault();
                var stickHitBtns = btnStackPanel.Children.OfType<Button>().Where(x => x.DataContext == player).ToList();

                await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
                {
                    foreach (var btn in btns)
                    {
                        btn.IsEnabled = true;
                    }

                    foreach (var betBtn in buttons)
                    {
                        betBtn.IsEnabled = true;
                    }

                    gameGrid.Children.Remove(scoreStackPanel);

                    foreach (var image in cardImage)
                        gameGrid.Children.Remove(image);

                    foreach (var card in dealerCards)
                        gameGrid.Children.Remove(card);

                    gameGrid.Children.Remove(bustlbl);
                    Scorelabel.Content = "";
                    lblBetPlaced.Content = "";
                    gameGrid.Children.Remove(bustlbldealer);

                    foreach (var btn in stickHitBtns)
                        btn.IsEnabled = false;
                }));


                //current score
                //bet placed

                //remove all bust textboxes, etc
                //update all UI/textboxes, etc
            }

            dealer.hand = new List<Cards>();
            dealer.HandScore = 0;

            var lblDealerScore = gameGrid.Children.OfType<Label>().Where(x => x.Name == "lblDealerScore").FirstOrDefault();
            //lblDealerScore.Content = "Dealer Score : " + Environment.NewLine + dealer.HandScore;

            var dealerBustlbl = gameGrid.Children.OfType<StackPanel>().Where(x => x.Name == "lblBust_dealer").FirstOrDefault();

            var lblDealerName = gameGrid.Children.OfType<Label>().Where(x => x.Name == "lblDealerName").FirstOrDefault();

            await Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
            {
                gameGrid.Children.Remove(lblDealerScore);
                gameGrid.Children.Remove(lblDealerScore);
                spNewGame.Visibility = Visibility.Collapsed;
                btnDeal.Visibility = Visibility.Visible;

                gameGrid.Children.Remove(lblDealerName);

                //disable hit/stick buttons
            }));

            cardDeck = new CardDeck(numberOfDecks);
        }

        private void tbNoOfPlayers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    tbNoOfPlayers.Text = string.Empty;
                    playerList = new List<Player>();

                    var nameLbl = playerNamesGrid.Children.OfType<Label>().Where(x => x.Name.Contains("lblName")).ToList();
                    var tbName = playerNamesGrid.Children.OfType<TextBox>().Where(x => x.Name.Contains("tbPlayerName")).ToList();

                    foreach (var name in nameLbl)
                        playerNamesGrid.Children.Remove(name);

                    foreach (var tb in tbName)
                        playerNamesGrid.Children.Remove(tb);
                }
                if ((e.Key >= Key.D0 && e.Key <= Key.D7) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad7))
                {
                    tbNoOfPlayers.Text = string.Empty;
                }
            }
            catch (Exception exc)
            {

            }
        }
    }
}
