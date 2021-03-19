using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using LibGit2Sharp;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace GitGUI.Logic
{
    public class CommitNodeModel : GraphItemModel
    {
        double _margin = 8;
        public Commit Commit { get; private set; }
        BitmapImage _picture;
        bool _inProgress = false;
        public string Message { get { return Commit.Message; } }
        public string Sha { get { return Commit.Sha; } }
        public string Author {  get { return Commit.Author.Name; } }
        public string Email { get { return Commit.Author.Email; } }
        public string Time { get { return Commit.Author.When.DateTime.ToString(); } }
        public event Action<CommitNodeModel> CopyShaRequested;
        public event Action<CommitNodeModel> ShowChanges;
        public RelayCommand CopySha { get; private set; }
        public RelayCommand OnShowChanges { get; private set; }
        public bool InProgress { get { return _inProgress; } set { _inProgress = value; OnPropertyChanged(); } }
        public double MessageWidth { get { return MeasureMessageWidth(); } }
        double Height { get { return 40; } }
        public double LeftContactDist { get { return Height / 2; } }
        public double TextStartDist { get { return LeftContactDist + _margin + (EnabledPhoto ? Height / 2 : 0); } }
        double TextEndDist { get { return TextStartDist + TextWidth; } }
        public double RightContactDist { get { return TextEndDist + _margin; } }
        public double MaxWidth { get { return 2 * _margin + TextWidth + Height + (EnabledPhoto ? Height / 2 : 0); } }
        public double MaxW { get { return 150; } }
        public double TextWidth
        {
            get
            {
                double w = MessageWidth;
                return (w <= MaxW) ? w : Math.Min(MaxW, w / 2);
            }
        }
        double MeasureMessageWidth()
        {
            var tb = new TextBlock();
            var formattedText = new FormattedText(
                Message,
                CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                tb.FontSize,
                System.Windows.Media.Brushes.White,
                new NumberSubstitution(),
                TextFormattingMode.Display);
            return formattedText.Width;
        }

        public bool EnabledPhoto
        {
            get { return BitmapImage != null && ((App)Application.Current).Settings.ShowAuthorMiniatures; }
        }

        public BitmapImage BitmapImage
        {
            get { return _picture; }
            set { _picture = value; OnPropertyChanged("BitmapImage"); }
        }

        public CommitNodeModel(Commit c, BitmapImage picture)
        {
            PlusButton = true;
            Commit = c;
            BitmapImage = picture;
            CopySha = new RelayCommand(() => CopyShaRequested?.Invoke(this));
            OnShowChanges = new RelayCommand(
                () => 
                ShowChanges?.Invoke(this));
        }
    }
}
