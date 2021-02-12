using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AngelMP3.mv;

namespace AngelMP3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Resources.Add("songList", ModelView.Songs);
            playButton.AddHandler(Button.MouseEnterEvent, new MouseEventHandler(PlayButton_MouseEnterOrLeave));
            playButton.AddHandler(Button.MouseLeaveEvent, new MouseEventHandler(PlayButton_MouseEnterOrLeave));
        }

        private void SearchField_TextInput(object sender, TextCompositionEventArgs e)
        {

        }
        private void PlayButton_MouseEnterOrLeave(object sender, MouseEventArgs e) {
            Button playBtn = (Button)e.Source;
            Image bgImg = (Image)playBtn.Content;
            Uri currentBgUri = new Uri(bgImg.Source.ToString());
            string imgName = currentBgUri.Segments[currentBgUri.Segments.Length-1];
            if (imgName == "play_icon.png") {
                bgImg.Source = new BitmapImage(new Uri("img/pause_icon.png", UriKind.RelativeOrAbsolute));
            } else if (imgName == "pause_icon.png") {
                bgImg.Source = new BitmapImage(new Uri("img/play_icon.png", UriKind.RelativeOrAbsolute));
            }
        }
    }
}