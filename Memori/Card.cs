using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Memori
{
    public class Card : Button
    {
        MediaPlayer clickSound = new MediaPlayer();
        Image face = new Image();
        Image back = new Image();    

        public Card(int i) : base()
        {
            Margin = new Thickness(5);

            face.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/{i}.jpg"));
            back.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/0.jpg"));

            clickSound.Open(new Uri("Resources\\Щелчёк.mp3", UriKind.Relative));
        }

        protected override void OnClick()
        {         
            FaceUp();
            clickSound.Stop();
            clickSound.Play();
            base.OnClick();   
        }

        public void FaceUp()
        {
            IsEnabled = false;
            Content = face;         
        }

        public void FaceDown()
        {
            IsEnabled = true;           
            Content = back;        
        }

        public static bool Equals(Card a, Card b)
        {
            if(a.face.Source.ToString() == b.face.Source.ToString()) return true;
            else return false;
        }
    }
}
