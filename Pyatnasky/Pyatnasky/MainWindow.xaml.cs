using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Fifteens
{
    public class FButton : Button
    {
        public int X;
        public int Y;
    }

    public partial class MainWindow : Window
    {
        private int _x;
        private int _y;

        private Dictionary<int, FButton> _buttons = new Dictionary<int, FButton>(16);

        public MainWindow()
        {
            InitializeComponent();
            gameItem.Click += (s, e) => { Random(); Notify(); };
            gameItem2.Click += (s, e) => Close();

            int i = 1;
            foreach (var obj in grid.Children)
                if (obj is FButton)
                {
                    var btn = (FButton)obj;
                    btn.X = Grid.GetRow(btn);
                    btn.Y = Grid.GetColumn(btn);
                    btn.Padding = new Thickness(10);
                    btn.Click += OnFButtonClick;
                    
                    _buttons.Add(i++, btn); 
                }

            _buttons.Add(0, null);

            Random();
        }

        protected void OnFButtonClick(object sender, RoutedEventArgs e)
        {
            var button = (FButton)sender;
            int x = Grid.GetRow(button);
            int y = Grid.GetColumn(button);

            // При нажатии на левый Ctrl можно играть по диагонали
            var down = Keyboard.IsKeyDown(Key.LeftCtrl);

            if ((down && (Math.Abs(_x - x) == 1
            || Math.Abs(_y - y) == 1)) ||
            ((Math.Abs(_x - x) == 1 && _y == y)
            || (Math.Abs(_y - y) == 1 && _x == x)))
            {
                Grid.SetRow(button, _x);
                Grid.SetColumn(button, _y);
                _x = x; _y = y;
            }
            else return;

            if (!_new) return;

            Anim(button, e);

            bool ok = _buttons.Values
            .Where(b => b != null)
            .All(b => b.X == Grid.GetRow(b)
            && b.Y == Grid.GetColumn(b));

            if (!ok) return;

            label.Visibility = Visibility.Visible;
            restart.Visibility = Visibility.Visible;
            circle_load.Visibility = Visibility.Visible;
            AnimatebleWin();
            _new = false;

        }

        private bool _new;

        private void Random()
        {
            _new = true;

            var r = new Random();
            var a = new List<int>(16);
            var v = new List<int>(_buttons.Keys);

            int k = 0, n = 0;
            for (var x = 0; x < 4; x++)
                for (var y = 0; y < 4; y++)
                {
                    do
                    {
                        k = r.Next(0, v.Count);
                    }
                    while (a.Any(o => o == v[k]));

                    a.Add(v[k]); v.RemoveAt(k);

                    var button = _buttons[a[n]];

                    if (button == null)
                    {
                        _x = x; _y = y;
                    }
                    else
                    {
                        Grid.SetRow(button, x);
                        Grid.SetColumn(button, y);
                    }
                    n++;
                }
        }

        void Notify()
        {
            System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("notify.ico");
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(500,"Клавиатура","При нажатии на Ctrl можно играть по диагонали",System.Windows.Forms.ToolTipIcon.Info);
        }

        void Anim(object sender, RoutedEventArgs e)
        {
            var button = (FButton)sender;

            DoubleAnimation animateble = new DoubleAnimation();
            animateble.From = 0;
            animateble.By = 120;
            animateble.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            button.BeginAnimation(FButton.MaxHeightProperty, animateble);
            button.BeginAnimation(FButton.MaxWidthProperty, animateble);
        }

        void restart_Click(object sender, RoutedEventArgs e)
        {
            label.Visibility = Visibility.Collapsed;
            restart.Visibility = Visibility.Collapsed;
            circle_load.Visibility = Visibility.Collapsed;
            Random();
        }

        void AnimatebleWin()
        {
            Thread.Sleep(450);
            DoubleAnimation animateble = new DoubleAnimation();
            animateble.From = 0;
            animateble.By = 468.224;
            animateble.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            label.BeginAnimation(FButton.MaxWidthProperty, animateble);
        }
        
    }
}