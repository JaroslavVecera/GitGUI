using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GitGUI
{
    class DataToCommitNode : IValueConverter
    {
        double  _height = 40;
        double _margin;
        double _maxWidth = 200;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            try { _margin = Double.Parse((string)parameter); }
            catch (Exception) { }
            CommitNodeConverterData data = (CommitNodeConverterData)value;
            Logic.AppSettings s = ((App)Application.Current).Settings;
            if (!s.ShowAuthorMiniatures)
                data.PhotoPath = null;
            TextBlock block = MessageBlock(data);
            double textWidth = MeasureTextWidth(block);
            block.Width = textWidth;
            block.Height = block.LineHeight * 2;
            Path p = new Path() { Data = CommitGeometry(data.PhotoPath != null, textWidth)};
            Grid g = new Grid() { Height = _height, Margin = new Thickness(Leftmost(data.PhotoPath != null, textWidth), -_height / 2, 0, 0) };
            g.Children.Add(p);
            g.Children.Add(block);
            if (data.PhotoPath != null)
                g.Children.Add(Image(data.PhotoPath));
            return g;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        PathGeometry CommitGeometry(bool hasPhoto, double textLength)
        {
            List<PathSegment> segments = Segments(textLength, hasPhoto);
            PathFigure figure = new PathFigure(new Point(0, 0), segments, true);
            List<PathFigure> collection = new List<PathFigure> { figure };
            PathGeometry geometry = new PathGeometry() { Figures = new PathFigureCollection(collection) };
            return geometry;
        }

        Ellipse Image(string path)
        {
            ImageBrush b = new ImageBrush() { ImageSource = new BitmapImage(new Uri(path)), Stretch = Stretch.UniformToFill };
            return new Ellipse()
            {
                Height = _height * 0.9,
                Width = _height * 0.9,
                Fill = b,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(-_height, 0 , 0, 0)
            };
        }

        TextBlock MessageBlock(CommitNodeConverterData data)
        {
            return new TextBlock
            {
                Text = data.Message,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.CharacterEllipsis,
            };
        }

        List<PathSegment> Segments(double textLength, bool hasPhoto)
        {
            return new List<PathSegment>() { Top(textLength), RightArc(textLength), Bottom(hasPhoto), LeftArc(hasPhoto) };
        }


        LineSegment Top(double textLength)
        {
            return new LineSegment(new Point(textLength + _margin, 0), false);
        }

        ArcSegment RightArc(double textLength)
        {
            return new ArcSegment()
            {
                Point = new Point(textLength + _margin, _height),
                Size = new Size(_height / 2, _height / 2),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };
        }

        LineSegment Bottom(bool hasPhoto)
        {
            return new LineSegment(new Point(-_margin - (hasPhoto ? _height / 2 : 0), _height), false);
        }

        ArcSegment LeftArc(bool hasPhoto)
        {
            return new ArcSegment()
            {
                Point = new Point(-_margin - (hasPhoto ? _height / 2 : 0), 0),
                Size = new Size(_height / 2, _height / 2),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };
        }

        double MeasureTextWidth(TextBlock b)
        {
            b.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            if (b.DesiredSize.Width <= _maxWidth)
                return b.DesiredSize.Width;
            return Math.Min(_maxWidth, b.DesiredSize.Width / 2);
        }

        double Leftmost(bool hasPhoto, double textLength)
        {
            return -textLength / 2;
        }
    }
}
