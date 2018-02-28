using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading.Tasks;



namespace Zwikken
{
    /// <summary>
    /// Contains GameState class in which all gameplay events and phases are dealt with.
    /// Class CardBox has as objects boxes in the main window in which cards are displayed.
    /// Class Card has as objects the cards with their properties.
    /// </summary>
    public partial class App : Application
    {
    }


    public class GameState : INotifyPropertyChanged
    {
        public string[] Names { get; set; } 
        public CardBox Pot { get; set; }
        public CardBox[] Troef { get; set; }
        public CardBox[] PlayerCard { get; set; }
        public CardBox[] UserHand { get; set; }
        public int PotFiches { get; set; }
        public int[] PlayerFiches { get; set; }
        public List<int> MainDeck { get; set; }
        public List<Card>[] PlayerHand { get; set; }
        public bool[] PlayerPlay { get; set; }
        public List<int>[] PlayerPoints { get; set; }
        public bool[] PlayerInGame { get; set; }
        public bool[] PlayerJustOut { get; set; }
        public string GamePhase { get; set; }
        public int Dealer { get; set; }
        public List<int> PlayerOrder { get; set; }
        public int UserOrder { get; set; }
        public int SlagNum { get; set; }
        public List<Card> SlagCard { get; set; }
        public int TroefNum { get; set; }
        public int[] PlayerTroef10 { get; set; }       

        private string _msgtxt;
        public string MsgTxt
        {
            get { return _msgtxt; }
            set
            {
                if (this._msgtxt != value)
                {
                    this._msgtxt = value;
                    this.NotifyPropertyChanged("MsgTxt");
                }
            }
        }
        private string _buttontoptxt;
        public string ButtonTopTxt
        {
            get { return _buttontoptxt; }
            set
            {
                if (this._buttontoptxt != value)
                {
                    this._buttontoptxt = value;
                    this.NotifyPropertyChanged("ButtonTopTxt");
                }
            }
        }
        private string _buttonbottomtxt;
        public string ButtonBottomTxt
        {
            get { return _buttonbottomtxt; }
            set
            {
                if (this._buttonbottomtxt != value)
                {
                    this._buttonbottomtxt = value;
                    this.NotifyPropertyChanged("ButtonBottomTxt");
                }
            }
        }

        public SolidColorBrush rood { get; set; }
        public SolidColorBrush goud { get; set; }
        public SolidColorBrush zwart { get; set; }
        public SolidColorBrush blauw { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        public GameState()
        {
            Names = new string[4];
            Names[0] = "Maikel";
            Names[1] = "Dirkje";
            Names[2] = "Frenk";
            Names[3] = "Uzelf";

            rood = (SolidColorBrush)Application.Current.FindResource("ColorRed");
            goud = (SolidColorBrush)Application.Current.FindResource("ColorGold");
            blauw = (SolidColorBrush)Application.Current.FindResource("ColorBlue");
            zwart = (SolidColorBrush)Application.Current.FindResource("ColorBlack");

            MainDeck = new List<int>();

            PlayerHand = new List<Card>[4];
            for (int player = 0; player < 4; player++)
            {
                PlayerHand[player] = new List<Card>();
            }

            PlayerPoints = new List<int>[4];
            for (int player = 0; player < 4; player++)
            {
                PlayerPoints[player] = new List<int>();
                for (int slag = 0; slag < 3; slag++)
                {
                    PlayerPoints[player].Add(0);
                }
            }

            PotFiches = 0;
            Pot = new CardBox(-1, " ", PotFiches.ToString() + " fiches");

            Troef = new CardBox[3];
            Troef[0] = new CardBox(0, "Troef");
            Troef[1] = new CardBox();
            Troef[1].TextName = "Troef 2";
            Troef[2] = new CardBox();
            Troef[2].TextName = "Troef 3";

            PlayerFiches = new int[4];

            PlayerCard = new CardBox[4];
            for (int player = 0; player < 4; player++)
            {
                PlayerCard[player] = new CardBox(0, Names[player]);
            }

            UserHand = new CardBox[3];
            UserHand[0] = new CardBox(0, "Kaart 1");
            UserHand[1] = new CardBox(0, "Kaart 2");
            UserHand[2] = new CardBox(0, "Kaart 3");

            PlayerPlay = new bool[4];

            PlayerInGame = new bool[4];
            PlayerJustOut = new bool[4];
            PlayerOrder = new List<int>();
            SlagCard = new List<Card>();

            PlayerTroef10 = new int[3];

            GamePhase = "NewGame";
            ButtonTopTxt = "START SPEL";
            ButtonBottomTxt = " ";
            MsgTxt = "Welkom bij\n Zwikken !";
        }


        public void NewGame()
        {// Reset game state variables for a new game
            PlayerOrder.Clear();
            PotFiches = 0;
            Pot.TextFiches = PotFiches.ToString() + " fiches";            

            for (int player = 0; player < 4; player++)
            {
                PlayerFiches[player] = 20;
                PlayerCard[player].TextColor = blauw;
                PlayerCard[player].TextFiches = PlayerFiches[player].ToString() + " fiches";
                PlayerCard[player].Border = zwart;
                PlayerOrder.Add(player);
            }

            for (int player = 0; player < 4; player++)
            {
                PlayerInGame[player] = true;
                PlayerJustOut[player] = false;
            }

            NewRound(true);
        }


        public async void NewRound(bool newgame)
        {// Deal new cards and reset several game state variables for a new round
            ClearTable();
            MainDeck.Clear();
            for (int suit = 1; suit <= 4; suit++)
            {
                for (int card = 0; card <= 4; card++)
                {
                    MainDeck.Add(10 * suit + card);
                }
            }
            MainDeck.Shuffle();            

            for (int player = 0; player < 4; player++)
            {
                PlayerHand[player].Clear();
            }

            for (int card = 0; card < 3; card++)
            {
                for (int player = 0; player < 4; player++)
                {
                    int kaartid = MainDeck[MainDeck.Count - 1];
                    PlayerHand[player].Add(new Card(kaartid));
                    MainDeck.RemoveAt(MainDeck.Count - 1);
                }
            }

            TroefNum = 0;
            Troef[0].Border = zwart;
            Troef[0].TextColor = blauw;
            Troef[0].Kaart.Change(0);
            Troef[1].EmptyBox();
            Troef[2].EmptyBox();

            foreach (int player in PlayerOrder)
            {
                PlayerCard[player].Border = zwart;
            }

            for (int player = 0; player < 4; player++)
            {
                PlayerPlay[player] = false;
            }

            for (int player = 0; player < 4; player++)
            {
                for (int slag = 0; slag < 3; slag++)
                {
                    PlayerPoints[player][slag] = 0;
                }
            }

            for (int count = 0; count < 3; count++)
            {
                PlayerTroef10[count] = -1;
            }

            if (newgame)
            {
                PlayerCard[Dealer].TextName = Names[Dealer];
                Dealer = 0;
                PlayerCard[Dealer].TextName = "(Z) " + Names[Dealer];
                SetPlayerOrder(Dealer + 1);
            }
            else
            {
                NewDealer();
            }

            SlagCard.Clear();
            SlagNum = 1;

            for (int card = 0; card < 3; card++)
            {
                UserHand[card].TextColor = blauw;
                UserHand[card].Border = zwart;
                UserHand[card].Kaart.Change(PlayerHand[3][card].ID);
            }

            ButtonTopTxt = " ";
            ButtonBottomTxt = " ";
            MsgTxt = " ";

            for (int card = 0; card < 3; card++)
            {
                UserHand[card].Border = rood;
            }
            await MySleep(400);
            for (int card = 0; card < 3; card++)
            {
                UserHand[card].Border = zwart;
            }
            await MySleep(400);
            for (int card = 0; card < 3; card++)
            {
                UserHand[card].Border = rood;
            }
            await MySleep(400);
            for (int card = 0; card < 3; card++)
            {
                UserHand[card].Border = zwart;
            }

            GamePhase = "Contribute";
            ButtonTopTxt = " ";
            ButtonBottomTxt = "INZETTEN";
            MsgTxt = "Zet fiches in";
        }


        public void SetPlayerOrder(int beginner)
        {// PlayerOrder contains the players in the game in the order they play their cards
            PlayerOrder.Clear();
            int player = beginner;
            int count = 0;
            for (int loop = 0; loop < 4; loop++)
            {
                if (player > 3)
                {
                    player = player - 4;
                }
                if (player == 3)
                {
                    UserOrder = count;
                }
                if (PlayerInGame[player] == true)
                {
                    PlayerOrder.Add(player);
                    count++;
                }                
                player++;
            }
        }


        public void Contribute()
        {// Let players in the game pay up before a new troef is turned
            foreach (int player in PlayerOrder)
            {
                if (PlayerFiches[player] > 0)
                {
                    PlayerFiches[player]--;
                    PotFiches++;
                    PlayerCard[player].TextFiches = PlayerFiches[player].ToString() + " fiches";
                }
                if (PlayerFiches[player] == 0)
                {
                    PlayerInGame[player] = false;
                    PlayerJustOut[player] = true;
                }
            }
            Pot.TextFiches = PotFiches.ToString() + " fiches";
            OutOfGameCheck();          
        }
              

        public void NewDealer()
        {// Card dealer moves to next player in PlayerOrder
            SetPlayerOrder(Dealer + 1);
            PlayerCard[Dealer].TextName = Names[Dealer];
            Dealer = PlayerOrder[0];
            PlayerCard[Dealer].TextName = "(Z) " + Names[Dealer];
            SetPlayerOrder(Dealer + 1);
        }
            

        public async void SpeelPasPhase(int count)
        {// Main function of the game phase in which players Play or Pass based on their cards
            GamePhase = "SpeelPasComp";
            if (count > PlayerOrder.Count - 1)
            {
                bool AllPlayersPas = true;
                foreach (int player in PlayerOrder)
                {
                    if (PlayerPlay[player] == true)
                    {
                        AllPlayersPas = false;
                    }
                }
                if (AllPlayersPas == true && TroefNum == 2)
                {
                    MsgTxt = "Volgende ronde";
                    GamePhase = "NewRound";
                    ButtonTopTxt = "START RONDE";
                    ButtonBottomTxt = " ";
                }
                else if (AllPlayersPas == true && TroefNum < 2)
                {
                    if (Dealer == 3)
                    {
                        await MySleep(10);
                        GamePhase = "NewTroefUser";
                        ButtonTopTxt = "TROEF++";
                        ButtonBottomTxt = "GEEN TROEF";
                        MsgTxt = "Nieuwe troef \n  draaien of \n   volgende \n    ronde ?";
                    }
                    else
                    {
                        GamePhase = "NewTroefComp";
                        ButtonTopTxt = " ";
                        ButtonBottomTxt = " ";
                        MsgTxt = " ";
                        await MySleep(800);
                        NewTroef(Dealer);
                    }                    
                }
                else
                {
                    GamePhase = "CompTurn";
                    ButtonTopTxt = " ";
                    ButtonBottomTxt = " ";
                    MsgTxt = " ";
                    Troef10Phase(PlayerTroef10[TroefNum]);
                }                
            }
            else
            {
                int player = PlayerOrder[count];
                if (player == 3)
                {
                    GamePhase = "SpeelPasUser";
                    ButtonTopTxt = "SPEEL";
                    ButtonBottomTxt = "PAS";
                    MsgTxt = "Speel of Pas ?";
                }
                else
                {
                    if (count == 0)
                    {
                        await MySleep(600);
                    }
                    else
                    {
                        await MySleep(800);
                    }                    
                    Card.Suit troef = Troef[TroefNum].Kaart.Soort;
                    bool playerspeel = SpeelPas(player, troef, true);
                    if (playerspeel == true)
                    {
                        if (PlayerTroef10[TroefNum] == player)
                        {
                            PlayerCard[player].Kaart.Change(1070);
                        }
                        else
                        {
                            PlayerCard[player].Kaart.Change(70);
                        }
                        PlayerPlay[player] = true;
                        PlayerCard[player].Border = blauw;                        
                        PlayerFiches[player]--;
                        PotFiches++;
                        PlayerCard[player].TextFiches = PlayerFiches[player].ToString() + " fiches";
                        Pot.TextFiches = PotFiches.ToString() + " fiches";
                    }
                    else
                    {
                        if (PlayerTroef10[TroefNum] == player)
                        {
                            PlayerCard[player].Kaart.Change(1069);
                        }
                        else
                        {
                            PlayerCard[player].Kaart.Change(69);
                        }
                        PlayerPlay[player] = false;                      
                    }
                    SpeelPasPhase(count + 1);
                }                
            }            
        }


        public bool SpeelPas(int player, Card.Suit troef, bool actualcall)
        {// Determines whether individual player decides to Plat or Pass
         // NB: actualcall = false is used when Dealer decides to turn another troef
            bool playerspeel = false;  
            // First check if player has Zwik or can make Zwik by swapping troef 10 (only for actual call).
            // NB: player with Zwik will always play, so not needed to check when actualcall = false
            if (actualcall == true)
            {
                // Player has Zwik and Plays
                if (PlayerHand[player][0].Punten == PlayerHand[player][1].Punten &&
                PlayerHand[player][1].Punten == PlayerHand[player][2].Punten)
                {
                    playerspeel = true;
                    return playerspeel;
                }
                // Can player make Zwik by swapping troef 10 ?
                int[] cardpoints = new int[3];
                int count = 0;
                foreach (Card card in PlayerHand[player])
                {
                    if (card.Soort == troef && card.Punten == 0)
                    {
                        PlayerTroef10[TroefNum] = player;
                        cardpoints[count] = Troef[TroefNum].Kaart.Punten;
                    }
                    else
                    {
                        cardpoints[count] = PlayerHand[player][count].Punten;
                    }
                    count++;
                }
                if (PlayerTroef10[TroefNum] == player && cardpoints[0] == cardpoints[1] &&
                    cardpoints[1] == cardpoints[2])
                {
                    playerspeel = true;
                    return playerspeel;
                }
            }
            // Decide whether player Plays or Passes by adding up points of the cards in hand, 
            // troef is counted double
            int puntentotaal = 0;
            foreach (Card card in PlayerHand[player])
            {
                if (card.Soort == troef)
                {// If player has troef 10, then count points of troef card (swapping assumed)
                 // NB: case of troef 10 swapped with different troef card is dealt with in ShowNewTroef()
                    if (card.Punten == 0)
                    {                 
                        puntentotaal += 2 * Troef[TroefNum].Kaart.Punten;
                    }
                    else
                    {
                        puntentotaal += 2 * card.Punten;
                    }                    
                }
                else
                {
                    puntentotaal += card.Punten;
                }                
            }
            int threshold = 12;
            if (PlayerOrder[0] == player)
            {
                threshold = 10;
            }
            if (puntentotaal > threshold)
            {
                playerspeel = true;
            }
            else if (puntentotaal > threshold - 3)
            {
                Random rng = new Random();
                if (rng.Next(100) < 50)
                {
                    playerspeel = true;
                }
            }
            return playerspeel;            
        }        


        public void NewTroef(int player)
        {// Decided whether Dealer (Zwikkier, player) will turn another troef card
            bool newtroef = false;
            Card.Suit troef0 = Troef[0].Kaart.Soort;
            List<Card.Suit> kleuren = new List<Card.Suit>();
            kleuren.Add(Card.Suit.harten);
            kleuren.Add(Card.Suit.ruiten);
            kleuren.Add(Card.Suit.klaver);
            kleuren.Add(Card.Suit.schoppen);
            if (TroefNum == 0)
            {
                int score = 0;
                foreach (Card.Suit kleur in kleuren)
                {
                    if (kleur != troef0 && SpeelPas(player, kleur, false) == true)
                    {
                        score++;
                    }
                }
                if (score > 0)
                {
                    newtroef = true;
                }
            }
            if (TroefNum == 1)
            {// When Dealer has already turned a new troef, then turn another (free of charge anyway)
                newtroef = true;
            }
            if (newtroef == true)
            {
                TroefNum++;
                Troef[TroefNum].Border = zwart;
                Troef[TroefNum].TextColor = blauw;
                if (TroefNum == 1)
                {// Players need to pay only for first new turned troef
                    GamePhase = "Contribute";
                    ButtonTopTxt = " ";
                    ButtonBottomTxt = "INZETTEN";
                    MsgTxt += "Zwikkier draait \n   nieuwe troef \n\n  Zet fiches in";
                }
                else
                {
                    GamePhase = "SpeelPasComp";
                    ButtonTopTxt = " ";
                    ButtonBottomTxt = " ";
                    ShowNewTroef();
                }
            }
            else
            {
                MsgTxt = "Zwikkier draait \n    geen troef \n\n Volgende ronde";
                GamePhase = "NewRound";
                ButtonTopTxt = "START RONDE";
                ButtonBottomTxt = " ";
            }
        }


        public async void ShowNewTroef()
        {// Show new turned troef card and handle all possibilities
            int kaartID = MainDeck[MainDeck.Count - 1];
            Troef[TroefNum].Kaart.Change(kaartID);
            MainDeck.RemoveAt(MainDeck.Count - 1);
            Troef[TroefNum].Border = rood;
            await MySleep(400);
            Troef[TroefNum].Border = zwart;
            await MySleep(400);
            Troef[TroefNum].Border = rood;
            await MySleep(400);
            Troef[TroefNum].Border = zwart;
            ClearTable();
            Card.Suit troef0 = Troef[0].Kaart.Soort;            
            if (TroefNum == 1)
            {
                Card.Suit troef1 = Troef[1].Kaart.Soort;
                if (troef0 == troef1 && PlayerTroef10[0] > -1)
                {// New troef is same as old troef. Player that has troef 10 is now forced to Play.
                    int player = PlayerTroef10[0];
                    PlayerCard[player].Border = blauw;
                    PlayerCard[player].Kaart.Change(1070);
                    PlayerPlay[player] = true;
                    PlayerFiches[player]--;
                    PotFiches++;
                    PlayerCard[player].TextFiches = PlayerFiches[player].ToString() + " fiches";
                    Pot.TextFiches = PotFiches.ToString() + " fiches";
                    await MySleep(400);
                    PlayerTroef10[TroefNum] = player;
                    Troef10Phase(player);
                    return;
                }
                else if (troef0 == troef1 && PlayerTroef10[0] == -1)
                {// New troef is same as old troef. No player has troef 10. Turn another troef.                                        
                    TroefNum++;
                    Troef[TroefNum].Border = zwart;
                    Troef[TroefNum].TextColor = blauw;
                    int kaartid = MainDeck[MainDeck.Count - 1];
                    Troef[TroefNum].Kaart.Change(kaartid);
                    MainDeck.RemoveAt(MainDeck.Count - 1);
                    Troef[TroefNum].Border = rood;
                    await MySleep(400);
                    Troef[TroefNum].Border = zwart;
                    await MySleep(400);
                    Troef[TroefNum].Border = rood;
                    await MySleep(400);
                    Troef[TroefNum].Border = zwart;
                }                
            }
            if (TroefNum == 2)
            {
                Card.Suit troef1 = Troef[1].Kaart.Soort;
                Card.Suit troef2 = Troef[2].Kaart.Soort;
                if (troef2 == troef0 || troef2 == troef1)
                {// Second new troef is same as an older troef. Player that has troef 10 is now forced to Play.
                    int player = -1;
                    if (troef2 == troef0 && PlayerTroef10[0] > -1)
                    {
                        player = PlayerTroef10[0];
                    }
                    if (troef2 == troef1 && PlayerTroef10[1] > -1)
                    {
                        player = PlayerTroef10[1];
                    }
                    if (player > -1)
                    {
                        PlayerCard[player].Border = blauw;
                        PlayerCard[player].Kaart.Change(1070);
                        PlayerPlay[player] = true;
                        PlayerFiches[player]--;
                        PotFiches++;
                        PlayerCard[player].TextFiches = PlayerFiches[player].ToString() + " fiches";
                        Pot.TextFiches = PotFiches.ToString() + " fiches";
                        await MySleep(400);
                        PlayerTroef10[TroefNum] = player;
                        Troef10Phase(player);
                        return;
                    }
                    else
                    {// Second new troef is same as an older troef, but no player has troef 10.
                     // Move on the next round.
                        MsgTxt = "   Zelfde troef \n\n Volgende ronde";
                        GamePhase = "NewRound";
                        ButtonTopTxt = "START RONDE";
                        ButtonBottomTxt = " ";
                        return;
                    }
                }
            }
            SpeelPasPhase(0);                                
        }


        public async void Troef10Phase(int player)
        {// Handles all possibilities of swapping troef 10 with a troef card.
            GamePhase = "Troef10Comp";
            // No swapping troef 10 when no player has it.
            if (player == -1)
            {
                await MySleep(200);
                ZwikCheck();
            }
            // No swapping troef 10 when player has Zwik 10 and troef 10.
            else if (PlayerHand[player][0].Punten == PlayerHand[player][1].Punten &&
                        PlayerHand[player][1].Punten == PlayerHand[player][2].Punten)
            {
                await MySleep(200);
                ZwikCheck();
            }
            else
            {// Player has troef 10, consider all possibilities.
                if (player < 3)
                {
                    int numhand = -1;
                    int count = 0;
                    foreach (Card card in PlayerHand[player])
                    {
                        if (card.Soort == Troef[TroefNum].Kaart.Soort && card.Punten == 0)
                        {
                            numhand = count;
                        }
                        count++;
                    }
                    Card.Suit troef = Troef[TroefNum].Kaart.Soort;
                    int nummaxtroef = TroefNum;
                    int maxpuntentroef = Troef[TroefNum].Kaart.Punten;
                    if (TroefNum > 0)
                    {                                            
                        for (int count1 = 0; count1 <= TroefNum; count1++)
                        {
                            if (Troef[count1].Kaart.Soort == troef && Troef[count1].Kaart.Punten > maxpuntentroef)
                            {
                                maxpuntentroef = Troef[count1].Kaart.Punten;
                                nummaxtroef = count1;
                            }
                        }
                    }
                    // Check if player can make Zwik by swapping troef 10.
                    bool maakzwikmettroef10 = false;
                    int[] cardpoints = new int[3];
                    count = 0;
                    foreach (Card card in PlayerHand[player])
                    {
                        if (count == numhand)
                        {                            
                            cardpoints[count] = Troef[TroefNum].Kaart.Punten;
                        }
                        else
                        {
                            cardpoints[count] = PlayerHand[player][count].Punten;
                        }
                        count++;
                    }
                    if (cardpoints[0] == cardpoints[1] && cardpoints[1] == cardpoints[2])
                    {
                        maakzwikmettroef10 = true;
                        maxpuntentroef = TroefNum;
                    }
                    // Do not swap troef 10 if player Passes, the troef is low and no Zwik can be made.
                    if (maxpuntentroef < 3 && PlayerPlay[player] == false && maakzwikmettroef10 == false)
                    {
                        await MySleep(200);
                        ZwikCheck();
                    }
                    else
                    {// Otherwise do swap troef 10 with the selected troef card.
                        SolidColorBrush kleur = PlayerCard[player].Border;
                        await MySleep(800);
                        MsgTxt = Names[player] + " ruilt \n  troef 10";
                        PlayerCard[player].Border = rood;
                        Troef[nummaxtroef].Border = rood;
                        await MySleep(400);
                        PlayerCard[player].Border = kleur;
                        Troef[nummaxtroef].Border = zwart;
                        await MySleep(400);
                        PlayerCard[player].Border = rood;
                        Troef[nummaxtroef].Border = rood;
                        await MySleep(400);
                        PlayerCard[player].Border = kleur;
                        Troef[nummaxtroef].Border = zwart;
                        await MySleep(400);
                        PlayerCard[player].Border = rood;
                        Troef[nummaxtroef].Border = rood;
                        await MySleep(400);
                        PlayerCard[player].Border = kleur;
                        Troef[nummaxtroef].Border = zwart;
                        int troefID = Troef[nummaxtroef].Kaart.ID;
                        Troef[nummaxtroef].Kaart.Change(PlayerHand[player][numhand].ID);
                        PlayerHand[player].RemoveAt(numhand);
                        PlayerHand[player].Add(new Card(troefID));
                        ClearTable();
                        MsgTxt = " ";
                        ZwikCheck();
                    }                              
                }
                else
                {// User has troef 10 and may decide to swap it with a troef card.
                    await MySleep(10);
                    GamePhase = "Troef10User";
                    MsgTxt = "Troef 10 ruilen ?\n\n Kies een troef";
                    ButtonTopTxt = "NIET RUILEN";
                }                
            }
        }


        public async void ZwikCheck()
        {// Checks if there are players with Zwik
            bool Zwik = false;
            bool[] PlayerZwik = new bool[4];
            for (int player = 0; player < 4; player++)
            {
                PlayerZwik[player] = false;
            }
            foreach (int player in PlayerOrder)
            {
                if (PlayerHand[player][0].Punten == PlayerHand[player][1].Punten &&
                    PlayerHand[player][1].Punten == PlayerHand[player][2].Punten && PlayerPlay[player] == true)
                {
                    if (Zwik == false)
                    {
                        await MySleep(800);
                        ClearTable();
                    }                                       
                    int zwikID = 2000 + PlayerHand[player][0].Punten;
                    PlayerCard[player].Kaart.Change(zwikID);
                    PlayerZwik[player] = true;
                    Zwik = true;
                }
            }
            if (Zwik == true)
            {// Highest Zwik wins the round. Zwik 10 is highest.
                int winner = -1;
                int hoogstezwik = -1;
                foreach (int player in PlayerOrder)
                {
                    if (PlayerZwik[player] == true && PlayerHand[player][0].Punten == 0)
                    {
                        winner = player;
                        break;
                    }
                    else if (PlayerZwik[player] == true && PlayerHand[player][0].Punten > hoogstezwik)
                    {
                        hoogstezwik = PlayerHand[player][0].Punten;
                        winner = player;
                    }
                }
                MsgTxt = Names[winner] + " wint\n de ronde !";
                PlayerCard[winner].Border = goud;
                await MySleep(400);
                PlayerCard[winner].Border = blauw;
                await MySleep(400);
                PlayerCard[winner].Border = goud;
                await MySleep(400);
                PlayerCard[winner].Border = blauw;
                await MySleep(400);
                PlayerCard[winner].Border = goud;
                await MySleep(400);
                PlayerCard[winner].Border = blauw;
                PlayerFiches[winner] += PotFiches;
                PotFiches = 0;
                PlayerCard[winner].TextFiches = PlayerFiches[winner].ToString() + " fiches";
                Pot.TextFiches = PotFiches.ToString() + " fiches";
                GamePhase = "NewRound";
                ButtonTopTxt = "START RONDE";
                ButtonBottomTxt = " ";
            }
            else
            {
                SlagPhase(0);
            }
        }


        public async void SlagPhase(int count)
        {// Handles the phase in which players play their cards.
            GamePhase = "CompTurn";
            if (count > PlayerOrder.Count - 1)
            {
                GamePhase = "Evaluate";
                ButtonTopTxt = " ";
                ButtonBottomTxt = " ";
                MsgTxt = " ";
                Evaluate();
            }
            else
            {
                int player = PlayerOrder[count];
                if (player == 3)
                {// User may select a card to play
                    GamePhase = "UserTurn";
                    ButtonTopTxt = " ";
                    ButtonBottomTxt = " ";
                    MsgTxt = "Kies een kaart";
                    await MySleep(800);
                    for (int num = 0; num < TroefNum; num++)
                    {
                        Troef[num].EmptyBox();
                    }
                }
                else
                {// Computer player selects a card to play
                    await MySleep(800);
                    if (count == 0)
                    {
                        ClearTable();
                        MsgTxt = " ";
                        for (int num = 0; num < TroefNum; num++)
                        {
                            Troef[num].EmptyBox();
                        }
                    }
                    int card = Slag(count);
                    int kaartID = PlayerHand[player][card].ID;
                    PlayerCard[player].Kaart.Change(kaartID);
                    PlayerHand[player].RemoveAt(card);
                    SlagCard.Add(new Card(kaartID));
                    SlagPhase(count + 1);
                }
            }            
        }


        public int Slag(int count)
        {// Decides which card to play for a computer player
            int player = PlayerOrder[count];

            if (PlayerHand[player].Count == 1)
            {                
                return 0;
            }

            Card.Suit troef = Troef[TroefNum].Kaart.Soort;            
            if (count == 0)
            {
                // play first card
                return HighestPoints(player);
            }
            else
            {
                // play card by the rules, plus using strategy
                Card.Suit kleur = PlayerCard[PlayerOrder[0]].Kaart.Soort;
                if (troef != kleur)
                {
                    if (GotSuit(player, kleur) == true)
                    {
                        if (GotHigherThanPlayed(player, kleur) == true)
                        {
                            return HighestCard(player, kleur);
                        }
                        else
                        {
                            return LowestCard(player, kleur);
                        }
                    }
                    else
                    {
                        if (GotSuit(player, troef) == true)
                        {
                            if (TroefPlayed() == true)
                            {
                                if (GotHigherThanPlayed(player, troef) == true)
                                {
                                    return HighestCard(player, troef);
                                }
                                else
                                {// Ondertroeven is forbidden (when other cards can be played): 
                                 // playing lower troef card when first card is no troef and troef has already been played. 
                                    return OndertroefCheck(player, troef);
                                }
                            }
                            else
                            {
                                return LowestCard(player, troef);
                            }
                        }
                        else
                        {
                            return LowestPoints(player);
                        }
                    }
                }
                else
                {
                    if (GotSuit(player, troef) == true)
                    {
                        if (GotHigherThanPlayed(player, troef) == true)
                        {
                            return HighestCard(player, troef);
                        }
                        else
                        {
                            return LowestCard(player, troef);
                        }
                    }
                    else
                    {
                        return LowestPoints(player);
                    }
                }
            }            
        }        


        public bool GotSuit (int player, Card.Suit kleur)
        {
            foreach (Card card in PlayerHand[player])
            {
                if (card.Soort == kleur)
                {
                    return true;
                }
            }
            return false;
        }


        public int HighestCard(int player, Card.Suit kleur)
        {// assuming player has a card of this suit
            int maxpunten = -1;
            int kaartmax = -1;
            int count = 0;
            foreach (Card card in PlayerHand[player])
            {
                if (card.Soort == kleur && card.Punten > maxpunten)
                {
                    maxpunten = card.Punten;
                    kaartmax = count;
                }
                count++;
            }
            return kaartmax;
        }


        public int LowestCard(int player, Card.Suit kleur)
        {// assuming player has a card of this suit
            int minpunten = 10;
            int kaartmin = -1;
            int count = 0;
            foreach (Card card in PlayerHand[player])
            {
                if (card.Soort == kleur && card.Punten < minpunten)
                {
                    minpunten = card.Punten;
                    kaartmin = count;
                }
                count++;
            }
            return kaartmin;
        }


        public bool TroefPlayed()
        {
            Card.Suit troef = Troef[TroefNum].Kaart.Soort;
            foreach (Card card in SlagCard)
            {
                if (card.Soort == troef)
                {
                    return true;
                }
            }
            return false;
        }


        public int LowestPoints(int player)
        {
            int minpunten = 10;
            int kaartmin = -1;
            int count = 0;
            foreach (Card card in PlayerHand[player])
            {
                if (card.Punten < minpunten)
                {
                    minpunten = card.Punten;
                    kaartmin = count;
                }
                count++;
            }
            return kaartmin;
        }


        public int HighestPoints(int player)
        {
            int maxpunten = -1;
            int kaartmax = -1;
            int count = 0;
            foreach (Card card in PlayerHand[player])
            {
                if (card.Punten > maxpunten)
                {
                    maxpunten = card.Punten;
                    kaartmax = count;
                }
                count++;
            }
            return kaartmax;
        }


        public bool GotHigherThanPlayed(int player, Card.Suit kleur)
        {// assuming player has a card of this suit
            int maxpuntenplayed = -1;
            foreach (Card card in SlagCard)
            {
                if (card.Soort == kleur && card.Punten > maxpuntenplayed)
                {
                    maxpuntenplayed = card.Punten;
                }
            }
            int kaartmax = HighestCard(player, kleur);
            if (PlayerHand[player][kaartmax].Punten > maxpuntenplayed)
            {
                return true;
            }
            return false;
        }


        public int OndertroefCheck(int player, Card.Suit troef)
        {// assuming player does not have cards of suit kleur, and troef has been played, 
         // and kleur != troef, and player has cards of suit troef, but not higher than played
            int nottroefminpunten = 10;
            int kaartnottroefmin = -1;
            int count = 0;
            foreach (Card card in PlayerHand[player])
            {
                if (card.Soort != troef && card.Punten < nottroefminpunten)
                {
                    nottroefminpunten = card.Punten;
                    kaartnottroefmin = count;
                }
                count++;
            }
            if (kaartnottroefmin > -1)
            {
                return kaartnottroefmin;
            }
            else
            {
                return LowestCard(player, troef);
            }
        }


        public async void CheckUserCard(int num)
        {// Checks if the card selected to play by the user is a legal move.
            Card ChosenCard = UserHand[num].Kaart;
            int kaartID = UserHand[num].Kaart.ID;
            PlayerHand[3].Clear();
            for (int card = 0; card < 3; card++)
            {
                if (UserHand[card].Kaart.ID > 0)
                {// cards in users hand including the chosen card
                    PlayerHand[3].Add(new Card(UserHand[card].Kaart.ID));
                }
            }
            if (PlayerHand[3].Count > 1 && UserOrder > 0)
            {
                Card.Suit troef = Troef[TroefNum].Kaart.Soort;
                Card.Suit kleur = PlayerCard[PlayerOrder[0]].Kaart.Soort;
                if (ChosenCard.Soort != kleur && GotSuit(3, kleur) == true)
                {// Illegal move: need to play same suit as first card when possible
                    MsgTxt = "     Kleur \n bekennen !";
                    await MySleep(500);
                    MsgTxt = " ";
                    return;
                }
                else if (ChosenCard.Soort != troef && GotSuit(3, kleur) == false && GotSuit(3, troef) == true &&             
                    (TroefPlayed() == false || (TroefPlayed() == true && GotHigherThanPlayed(3, troef) == true)))
                {// Illegal move: need to play troef card when possible
                    MsgTxt = "     Troef \n opgooien !";
                    await MySleep(500);
                    MsgTxt = " ";
                    return;
                }                
                else if (ChosenCard.Soort == troef && GotSuit(3, kleur) == false && TroefPlayed() == true)
                {
                    int maxtroefplayed = -1;
                    foreach (Card card in SlagCard)
                    {
                        if (card.Soort == troef && card.Punten > maxtroefplayed)
                        {
                            maxtroefplayed = card.Punten;
                        }
                    }
                    if (ChosenCard.Punten < maxtroefplayed && 
                        (GotHigherThanPlayed(3, troef) == true || PlayerHand[3][OndertroefCheck(3, troef)].Soort != troef) )
                    {// Illegal move: ondertroeven forbidden when other cards are possible to play
                        MsgTxt = "          Niet \n ondertroeven !";
                        await MySleep(500);
                        MsgTxt = " ";
                        return;
                    }                       
                }              
            }
            UserHand[num].EmptyBox();
            PlayerCard[3].Kaart.Change(kaartID);
            SlagCard.Add(new Card(kaartID));
            MsgTxt = " ";
            if (UserOrder < 3)
            {
                SlagPhase(UserOrder + 1);
            }
            else
            {
                Evaluate();
            }
        }


        public void ClearTable()
        {// Empty the card boxes for each player
            foreach (int player in PlayerOrder)
            {
                PlayerCard[player].Kaart.Change(0);
            }
        }


        public async void Evaluate()
        {// Main function of evaluation after one Slag (decide winner of Slag) or after 3 Slagen (decide winner of round)
            GamePhase = "Evaluate";
            await MySleep(800);
            int winner = Winner_Slag(SlagNum);
            SlagCard.Clear();
            if (winner == 3)
            {// Display winner of Slag
                MsgTxt = "Slag gewonnen !";
            }

            SolidColorBrush kleur = PlayerCard[winner].Border;
            PlayerCard[winner].Border = rood;
            await MySleep(400);
            PlayerCard[winner].Border = kleur;
            await MySleep(400);
            PlayerCard[winner].Border = rood;
            await MySleep(400);
            PlayerCard[winner].Border = kleur;

            if (SlagNum == 3)
            {
                foreach (int player in PlayerOrder)
                {
                    int player_points = PlayerPoints[player][0] + PlayerPoints[player][1] + PlayerPoints[player][2];
                    if (PlayerPlay[player] == true)
                    {
                        PlayerCard[player].Kaart.Change(200 + player_points);
                    }
                    else
                    {
                        PlayerCard[player].Kaart.Change(100 + player_points);
                    }
                }
                // Display winner of round and winner wins the money          
                winner = Winner_Round();
                if (winner > -1)
                {
                    MsgTxt = Names[winner] + " wint\n de ronde !";
                    PlayerCard[winner].Border = goud;
                    await MySleep(400);
                    PlayerCard[winner].Border = blauw;
                    await MySleep(400);
                    PlayerCard[winner].Border = goud;
                    await MySleep(400);
                    PlayerCard[winner].Border = blauw;
                    await MySleep(400);
                    PlayerCard[winner].Border = goud;
                    await MySleep(400);
                    PlayerCard[winner].Border = blauw;
                    PlayerFiches[winner] += PotFiches;
                    PotFiches = 0;
                    PlayerCard[winner].TextFiches = PlayerFiches[winner].ToString() + " fiches";
                    Pot.TextFiches = PotFiches.ToString() + " fiches";
                    GamePhase = "NewRound";
                    ButtonTopTxt = "START RONDE";
                    ButtonBottomTxt = " ";
                }
                else
                {
                    MsgTxt = "Geen winnaar";
                    GamePhase = "NewRound";
                    ButtonTopTxt = "START RONDE";
                    ButtonBottomTxt = " ";
                }               
            }
            else
            {
                ClearTable();
                SlagNum++;
                GamePhase = "CompTurn";
                ButtonTopTxt = " ";
                ButtonBottomTxt = " ";
                MsgTxt = " ";
                SetPlayerOrder(winner);
                SlagPhase(0);
            }
        }


        public int Winner_Slag(int slag)
        {// Determine the winner of one Slag
            Card.Suit troef = Troef[TroefNum].Kaart.Soort;
            Card.Suit kleur = PlayerCard[PlayerOrder[0]].Kaart.Soort;
            int puntenslag = 0;
            int hoogstetroef = -1;
            int player_hoogstetroef = -1;
            int hoogstekleur = -1;
            int player_hoogstekleur = -1;
            foreach (int player in PlayerOrder)
            {
                if (PlayerCard[player].Kaart.Soort == troef && PlayerCard[player].Kaart.Punten > hoogstetroef)
                {
                    hoogstetroef = PlayerCard[player].Kaart.Punten;
                    player_hoogstetroef = player;
                }
                else if (PlayerCard[player].Kaart.Soort == kleur && PlayerCard[player].Kaart.Punten > hoogstekleur)
                {
                    hoogstekleur = PlayerCard[player].Kaart.Punten;
                    player_hoogstekleur = player;
                }
                puntenslag += PlayerCard[player].Kaart.Punten;
            }

            if (player_hoogstetroef > -1)
            {
                PlayerPoints[player_hoogstetroef][SlagNum - 1] = puntenslag;
                return player_hoogstetroef;
            }
            else
            {
                PlayerPoints[player_hoogstekleur][SlagNum - 1] = puntenslag;
                return player_hoogstekleur;
            }
        }


        public int Winner_Round()
        {// Determine the winner of one round
            int maxpoints = -1;
            int player_maxpoints = -1;
            int player_points = -1;
            foreach (int player in PlayerOrder)
            {
                player_points = PlayerPoints[player][0] + PlayerPoints[player][1] + PlayerPoints[player][2];               
                if ( player_points > maxpoints)
                {
                    maxpoints = player_points;
                    player_maxpoints = player;
                }
            }
            // check for equal max score
            bool winner_exists = true;
            foreach (int player in PlayerOrder)
            {
                if (PlayerPoints[player][0] + PlayerPoints[player][1] + PlayerPoints[player][2] == maxpoints
                    && player != player_maxpoints)
                {
                    winner_exists = false;
                }
            }

            if (winner_exists && PlayerPlay[player_maxpoints] == true)
            {
                return player_maxpoints;
            }
            else
            {
                return -1;
            }
        }


        public async void OutOfGameCheck()
        {// After each player in the game pays money, this function checks if some player has no money left.
         // That player is then taken out of the game.
            for (int player = 0; player < 4; player++)
            {
                if (PlayerJustOut[player] == true)
                {
                    PlayerJustOut[player] = false;
                    await MySleep(200);
                    PlayerCard[player].Border = rood;
                    PlayerCard[player].TextColor = rood;
                    PlayerCard[player].Kaart.Change(0);
                    await MySleep(400);
                    PlayerCard[player].Border = goud;
                    await MySleep(400);
                    PlayerCard[player].Border = rood;
                    await MySleep(400);
                    PlayerCard[player].Border = goud;
                    await MySleep(400);
                    PlayerCard[player].Border = rood;
                    await MySleep(400);
                    PlayerCard[player].Border = goud;
                    PlayerCard[player].TextColor = goud;
                }
            }
            if (PlayerInGame[3] == false)
            {// Game over when user is out of the game
                GameOver();
            }
            else if (PlayerInGame[0] == false && PlayerInGame[1] == false && PlayerInGame[2] == false)
            {// User wins the game when the other players are out of the game
                GameWon();
            }
            else
            {
                if (PlayerInGame[Dealer] == false)
                {// Determine new Dealer when current Dealer is out of the game
                    NewDealer();
                }
                else
                {// Determine new PlayerOrder when (possibly) some players have left the game
                    SetPlayerOrder(Dealer + 1);
                }
                await MySleep(10);
                GamePhase = "SpeelPasComp";
                ButtonTopTxt = " ";
                ButtonBottomTxt = " ";
                MsgTxt = " ";
                ShowNewTroef();                                            
            }                                        
        }

        
        public void GameOver()
        {// Display game over for user
            GamePhase = "NewGame";
            MsgTxt = "Game Over !";
            ButtonTopTxt = "NIEUW SPEL";
            ButtonBottomTxt = " ";
        }

        public void GameWon()
        {// Display user is game winner
            GamePhase = "NewGame";
            MsgTxt = "GEFELICITEERD !";
            ButtonTopTxt = "NIEUW SPEL";
            ButtonBottomTxt = " ";
        }


        // To insert pauses for visual effects we need some sleep function.
        // Thread.Sleep() blocks to GUI, so I use this asynchronous alternative (.Net Framework 4.5 or higher).
        // Another alternative is to use timers, but that is more complicated.
        private async Task MySleep(int msec)
        {
            await Task.Delay(msec);
        }
    }


    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // Method to shuffle a list is not included in C#, so here is one.
    static class MyExtension
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }


    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class CardBox : INotifyPropertyChanged
    {
        public Card Kaart { get; set; }
        private SolidColorBrush _border;
        public SolidColorBrush Border
        {
            get { return _border; }
            set
            {
                if (this._border != value)
                {
                    this._border = value;
                    this.NotifyPropertyChanged("Border");
                }
            }
        }
        private string _textname;
        public string TextName
        {
            get { return _textname; }
            set
            {
                if (this._textname != value)
                {
                    this._textname = value;
                    this.NotifyPropertyChanged("TextName");
                }
            }
        }
        private string _textfiches;
        public string TextFiches
        {
            get { return _textfiches; }
            set
            {
                if (this._textfiches != value)
                {
                    this._textfiches = value;
                    this.NotifyPropertyChanged("TextFiches");
                }
            }
        }
        private SolidColorBrush _textcolor;
        public SolidColorBrush TextColor
        {
            get { return _textcolor; }
            set
            {
                if (this._textcolor != value)
                {
                    this._textcolor = value;
                    this.NotifyPropertyChanged("TextColor");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        public CardBox(int kaartID, string deText1 = " ", string deText2 = " ")
        {
            Kaart = new Card(kaartID);
            Border = (SolidColorBrush)Application.Current.FindResource("ColorBlack");
            TextName = deText1;
            TextFiches = deText2;
            TextColor = (SolidColorBrush)Application.Current.FindResource("ColorBlue");
        }

        public CardBox()
        {
            Kaart = new Card();
            Border = (SolidColorBrush)Application.Current.FindResource("ColorGold");
            TextName = " ";
            TextFiches = " ";
            TextColor = (SolidColorBrush)Application.Current.FindResource("ColorGold");
        }

        public void EmptyBox()
        {
            Kaart.Change(0);
            Border = (SolidColorBrush)Application.Current.FindResource("ColorGold");
            TextColor = (SolidColorBrush)Application.Current.FindResource("ColorGold");
        }
    }


    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class Card : INotifyPropertyChanged
    {
        public enum Suit { leeg, harten, ruiten, schoppen, klaver };

        public int ID { get; set; }
        public Suit Soort { get; set; }
        public int Punten { get; set; }
        public string _source;
        public string Source
        {
            get { return _source; }
            set
            {
                if (this._source != value)
                {
                    this._source = value;
                    this.NotifyPropertyChanged("Source");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public Card(int kaartID = 0)
        {
            ID = kaartID;

            switch ((int)(kaartID / 10))
            {
                case 1: Soort = Suit.harten; break;
                case 2: Soort = Suit.ruiten; break;
                case 3: Soort = Suit.schoppen; break;
                case 4: Soort = Suit.klaver; break;
                default: Soort = Suit.leeg; break;
            }

            Punten = kaartID % 10;

            string cardString = " ";

            switch (kaartID % 10)
            {
                case 0: cardString = "10"; break;
                case 1: cardString = "J"; break;
                case 2: cardString = "Q"; break;
                case 3: cardString = "K"; break;
                case 4: cardString = "A"; break;
                default: cardString = "bla"; break;
            }

            Source = "/images/" + Soort.ToString() + "-" + cardString + ".png";

            if (kaartID == 0) { Punten = 0; Source = "/images/leeg.png"; }
            if (kaartID == -1) { Punten = 0; Source = "/images/back_blue.png"; }
            if (kaartID == 69) { Punten = 0; Source = "/images/pas.png"; }
            if (kaartID == 70) { Punten = 0; Source = "/images/speel.png"; }
        }


        public void Change(int kaartID)
        {
            ID = kaartID;

            switch ((int)(kaartID / 10))
            {
                case 1: Soort = Suit.harten; break;
                case 2: Soort = Suit.ruiten; break;
                case 3: Soort = Suit.schoppen; break;
                case 4: Soort = Suit.klaver; break;
                default: Soort = Suit.leeg; break;
            }

            Punten = kaartID % 10;

            string cardString = " ";

            switch (kaartID % 10)
            {
                case 0: cardString = "10"; break;
                case 1: cardString = "J"; break;
                case 2: cardString = "Q"; break;
                case 3: cardString = "K"; break;
                case 4: cardString = "A"; break;
                default: cardString = "bla"; break;
            }

            Source = "/images/" + Soort.ToString() + "-" + cardString + ".png";

            if (kaartID == 0) { Punten = 0; Source = "/images/leeg.png"; }
            else if (kaartID == -1) { Punten = 0; Source = "/images/back_blue.png"; }
            else if (kaartID == 69) { Punten = 0; Source = "/images/pas.png"; }
            else if (kaartID == 70) { Punten = 0; Source = "/images/speel.png"; }
            else if (kaartID > 99 && kaartID < 200)
            {
                Punten = 0;
                int score = kaartID - 100;
                Source = "/images/punten_P_" + score.ToString() + ".png";
            }
            else if (kaartID > 199 && kaartID < 1000)
            {
                Punten = 0;
                int score = kaartID - 200;
                Source = "/images/punten_S_" + score.ToString() + ".png";
            }
            else if (kaartID == 1069) { Punten = 0; Source = "/images/pas_t10.png"; }
            else if (kaartID == 1070) { Punten = 0; Source = "/images/speel_t10.png"; }
            else if (kaartID > 1999) { Punten = 0; Source = "/images/zwik-" + cardString + ".png"; }
        }
    }
}
