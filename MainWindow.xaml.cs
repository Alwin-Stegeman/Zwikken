using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Zwikken
{
    /// <summary>
    /// MainWindow class contains event handlers for the buttons and images on the main window.
    /// Depending on the game phase the button have different uses.
    /// The card images can or cannot be clicked, also depending on the game phase.
    /// For each game phase the event handlers take the relevant action, which makes the game event-driven.
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameState GS;


        public MainWindow()
        {
            InitializeComponent();
            GS = new GameState();
            DataContext = GS;

        }

        

        private void Button_Top_Click(object sender, RoutedEventArgs e)
        {
            if (GS.GamePhase == "NewGame")
            {
                GS.NewGame();
            }
            if (GS.GamePhase == "NewRound")
            {
                GS.NewRound(false);
            }
            if (GS.GamePhase == "SpeelPasUser")
            {
                GS.PlayerPlay[3] = true;
                GS.PlayerCard[3].Border = GS.blauw;
                for (int card = 0; card < 3; card++)
                {
                    if (GS.UserHand[card].Kaart.Soort == GS.Troef[GS.TroefNum].Kaart.Soort && 
                        GS.UserHand[card].Kaart.Punten == 0)
                    {
                        GS.PlayerTroef10[GS.TroefNum] = 3;
                        GS.PlayerCard[3].Kaart.Change(1070);
                    }
                }
                if (GS.PlayerTroef10[GS.TroefNum] != 3)
                {
                    GS.PlayerCard[3].Kaart.Change(70);
                }                
                GS.PlayerFiches[3]--;
                GS.PotFiches++;
                GS.PlayerCard[3].TextFiches = GS.PlayerFiches[3].ToString() + " fiches";
                GS.Pot.TextFiches = GS.PotFiches.ToString() + " fiches";
                GS.ButtonTopTxt = " ";
                GS.ButtonBottomTxt = " ";
                GS.MsgTxt = " ";
                GS.SpeelPasPhase(GS.UserOrder + 1);
            }
            if (GS.GamePhase == "NewTroefUser")
            {
                GS.ClearTable();
                GS.TroefNum++;
                GS.Troef[GS.TroefNum].Border = GS.zwart;
                GS.Troef[GS.TroefNum].TextColor = GS.blauw;
                if (GS.TroefNum == 1)
                {
                    GS.GamePhase = "Contribute";
                    GS.ButtonTopTxt = " ";
                    GS.ButtonBottomTxt = "INZETTEN";
                    GS.MsgTxt = "Zet fiches in";
                }
                else
                {
                    GS.GamePhase = "SpeelPasComp";
                    GS.ButtonTopTxt = " ";
                    GS.ButtonBottomTxt = " ";
                    GS.MsgTxt = " ";
                    GS.ShowNewTroef();
                }
            }
            if (GS.GamePhase == "Troef10User")
            {
                GS.ClearTable();
                GS.ButtonTopTxt = " ";
                GS.ButtonBottomTxt = " ";
                GS.MsgTxt = " ";
                GS.ZwikCheck();
            }

        }

        private void Button_Bottom_Click(object sender, RoutedEventArgs e)
        {
            if (GS.GamePhase == "Contribute")
            {
                GS.Contribute();
            }
            if (GS.GamePhase == "SpeelPasUser")
            {
                GS.PlayerPlay[3] = false;
                for (int card = 0; card < 3; card++)
                {
                    if (GS.UserHand[card].Kaart.Soort == GS.Troef[GS.TroefNum].Kaart.Soort &&
                        GS.UserHand[card].Kaart.Punten == 0)
                    {
                        GS.PlayerTroef10[GS.TroefNum] = 3;
                        GS.PlayerCard[3].Kaart.Change(1069);
                    }
                }
                if (GS.PlayerTroef10[GS.TroefNum] != 3)
                {
                    GS.PlayerCard[3].Kaart.Change(69);
                }                
                GS.ButtonTopTxt = " ";
                GS.ButtonBottomTxt = " ";
                GS.MsgTxt = " ";
                GS.GamePhase = "SpeelPasComp";
                GS.SpeelPasPhase(GS.UserOrder + 1);
            }
            if (GS.GamePhase == "NewTroefUser")
            {
                GS.ClearTable();
                GS.NewRound(false);
            }
        }

        private void UserCard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (GS.GamePhase == "UserTurn")
            {
                Image img = (Image)sender;
                int num = Convert.ToInt32(img.Name.Substring(4));
                int temp = GS.UserHand[num].Kaart.ID;
                if (temp > 0)
                {
                    GS.UserHand[num].Border = GS.rood;
                }                
            }
        }

        private void UserCard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (GS.GamePhase == "UserTurn")
            {
                Image img = (Image)sender;
                int num = Convert.ToInt32(img.Name.Substring(4));
                int temp = GS.UserHand[num].Kaart.ID;
                if (temp > 0)
                {
                    GS.UserHand[num].Border = GS.zwart;
                }                
            }
        }

        private void UserCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GS.GamePhase == "UserTurn")
            {
                if (GS.UserOrder == 0)
                {
                    GS.ClearTable();
                }
                Image img = (Image)sender;
                int num = Convert.ToInt32(img.Name.Substring(4));
                int kaartID = GS.UserHand[num].Kaart.ID;
                if (kaartID > 0)
                {
                    GS.CheckUserCard(num);
                }                
            }
        }


        private void Troef_MouseEnter(object sender, MouseEventArgs e)
        {
            if (GS.GamePhase == "Troef10User")
            {
                Image img = (Image)sender;
                int num = Convert.ToInt32(img.Name.Substring(4));
                Card.Suit troef = GS.Troef[GS.TroefNum].Kaart.Soort;
                if (GS.TroefNum >= num && GS.Troef[num].Kaart.Soort == troef)
                {
                    GS.Troef[num].Border = GS.rood;
                }
            }
        }

        private void Troef_MouseLeave(object sender, MouseEventArgs e)
        {
            if (GS.GamePhase == "Troef10User")
            {
                Image img = (Image)sender;
                int num = Convert.ToInt32(img.Name.Substring(4));
                Card.Suit troef = GS.Troef[GS.TroefNum].Kaart.Soort;
                if (GS.TroefNum >= num && GS.Troef[num].Kaart.Soort == troef)
                {
                    GS.Troef[num].Border = GS.zwart;
                }
            }
        }

        private void TroefLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GS.GamePhase == "Troef10User")
            {
                Image img = (Image)sender;
                int num = Convert.ToInt32(img.Name.Substring(4));
                Card.Suit troef = GS.Troef[GS.TroefNum].Kaart.Soort;
                if (GS.TroefNum >= num && GS.Troef[num].Kaart.Soort == troef)
                {
                    int troefID = GS.Troef[num].Kaart.ID;
                    int troef10num = -1;
                    int troef10ID = 0;
                    for (int count = 0; count < 3; count++)
                    {
                        if (GS.UserHand[count].Kaart.Soort == troef && GS.UserHand[count].Kaart.Punten == 0)
                        {
                            troef10num = count;
                            troef10ID = GS.UserHand[count].Kaart.ID;
                        }
                    }
                    GS.Troef[num].Kaart.Change(troef10ID);
                    GS.UserHand[troef10num].Kaart.Change(troefID);
                    GS.PlayerHand[3].RemoveAt(troef10num);
                    GS.PlayerHand[3].Add(new Card(troefID));
                    GS.Troef[num].Border = GS.zwart;
                    GS.ClearTable();
                    GS.ButtonTopTxt = " ";
                    GS.ButtonBottomTxt = " ";
                    GS.MsgTxt = " ";
                    GS.ZwikCheck();
                }
            }
        }
    }
}


