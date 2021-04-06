using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GitGUI
{
    /// <summary>
    /// Interakční logika pro ProgressBarDialog.xaml
    /// </summary>
    public partial class ProgressBarDialog : Window
    {
        public void MinorThreadClose()
        {
            Dispatcher.Invoke(() => Close());
        }

        public void MinorThreadSetBytes(long bytes)
        {
            Dispatcher.Invoke(() => Bytes = bytes);
        }

        public void MinorThreadSetTotal(int total)
        {
            Dispatcher.Invoke(() => Total = total);
        }

        public void MinorThreadSetCount(int count)
        {
            Dispatcher.Invoke(() => Count = count);
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
          "Message",
          typeof(string),
          typeof(ProgressBarDialog),
          new FrameworkPropertyMetadata(""));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty BytesProperty = DependencyProperty.Register(
          "Bytes",
          typeof(long),
          typeof(ProgressBarDialog),
          new FrameworkPropertyMetadata((long)0));

        public long Bytes
        {
            get { return (long)GetValue(BytesProperty); }
            set { SetValue(BytesProperty, value); }
        }

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
          "Count",
          typeof(int),
          typeof(ProgressBarDialog),
          new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnCountPropertyChanged)));

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public static readonly DependencyProperty TotalProperty = DependencyProperty.Register(
          "Total",
          typeof(int),
          typeof(ProgressBarDialog),
          new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnTotalPropertyChanged)));

        public int Total
        {
            get { return (int)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }

        public ProgressBarDialog()
        {
            InitializeComponent();
            ProgressChanged();
        }

        static void OnCountPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressBarDialog)sender).ProgressChanged();
        }

        static void OnTotalPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressBarDialog)sender).ProgressChanged();
        }

        void ProgressChanged()
        {
            double barWidth = progressBar.ActualWidth;
            double progressPercentage = Total == 0 ? 0 : (double)Count / (double)Total;
            progressColumn.Width = new GridLength(barWidth * progressPercentage);
        }
    }
}
