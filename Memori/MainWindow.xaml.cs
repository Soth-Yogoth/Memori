using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Memori
{
    public partial class MainWindow : Window
    {
        List<Card> Cards = new List<Card>();
        Card? selectedCard;

        MediaPlayer music = new MediaPlayer();
        MediaPlayer timerSound = new MediaPlayer();
        MediaPlayer gameOverSound = new MediaPlayer();
        

        DispatcherTimer timer = new DispatcherTimer();
        Stopwatch stopwatch = new Stopwatch();
        TimeSpan timeSpan;

        bool flipped;
        int movesCount;
        int pairsFound;

        public MainWindow()
        {
            for (int j = 1; j < 7; j++)
            {
                Cards.Add(new Card(j));
                Cards.Add(new Card(j));
            }

            foreach (Card card in Cards) 
            { 
                card.Click += CheckPair;
            }

            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(1);

            timerSound.Open(new Uri("Resources\\Таймер.mp3", UriKind.Relative));
            music.Open(new Uri("Resources\\Музыка.mp3", UriKind.Relative));
            music.MediaEnded += new EventHandler(Media_Ended);
            music.Play();

            InitializeComponent();
        }

        private void Media_Ended(object sender, EventArgs e)
        {
            music.Stop();
            music.Play();
        }

        private async void CheckPair(object sender, RoutedEventArgs e)
        {
            Card card = (Card)sender;

            if (selectedCard == null) selectedCard = card;
            else
            {
                if (!Card.Equals(card, selectedCard))
                {
                    Table.IsEnabled = false;
                    await Task.Delay(500);
                    card.FaceDown();
                    selectedCard.FaceDown();
                    Table.IsEnabled = true;
                }
                else pairsFound++;
                
                selectedCard = null;
                movesCount++;
                Counter.Text = $"Ходов следано: {movesCount}";
            }

            if (pairsFound == 6) GameOver("Поздравляю, Вы нашли все пары!", true);
        }

        private void ShuffleCards()
        {
            Random random = new Random();

            for (int i = 0; i < 12; i++) 
            {
                int k = random.Next(12);
                Card card = Cards[i];
                Cards[i] = Cards[k];
                Cards[k] = card;
            }
        }

        private void PutCards()
        {
            Table.Children.Clear();

            foreach (Card card in Cards) 
            { 
                Table.Children.Add(card);
            }
        }

        private void ShowCards()
        {
            foreach (Card card in Cards)
                card.FaceUp();
        }

        private void HideCards()
        {
            foreach (Card card in Cards)
                card.FaceDown();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            Counter.Text = "Ходов сделано: 0";
            movesCount = 0;
            pairsFound = 0;

            stopwatch.Restart();
            timer.Start();

            ShuffleCards();            
            PutCards();    
            ShowCards();
            flipped = false;

            Table.Visibility = Visibility.Visible;
            Table.IsEnabled = true;
        }

        private void timer_Tick(object obj, EventArgs e)
        {
            timeSpan = new TimeSpan(0, 0, (int)(61 - stopwatch.Elapsed.TotalSeconds));
            Time.Text = String.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            if(timeSpan.TotalSeconds < 60 && !flipped)
            {
                HideCards();
                flipped = true;
            }

            if(timeSpan.TotalSeconds < 16 && timerSound.Position == TimeSpan.Zero)
            {   
                timerSound.Play();
            }

            if (timeSpan.TotalSeconds < 1)
            {             
                Table.IsEnabled = false;
                GameOver("Время вышло! Вы проиграли.", false);
            }
        }

        private void GameOver(string mes, bool victory)
        {
            selectedCard = null;

            timerSound.Stop();
            timer.Stop();
            stopwatch.Stop();

            if (victory) gameOverSound.Open(new Uri("Resources\\Победа.mp3", UriKind.Relative));
            else gameOverSound.Open(new Uri("Resources\\Проигрыш.mp3", UriKind.Relative));
            gameOverSound.Play();

            MessageBox.Show(mes);
            gameOverSound.Close();
        }
    }
}
