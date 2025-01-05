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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace kanban_v2
{
    /// <summary>
    /// Логика взаимодействия для IncomeBox.xaml
    /// </summary>
    public partial class IncomeBox : UserControl
    {

        // Поля для перетаскивания
        private Point? _startMousePosition; // Начальная позиция мыши
        private Point? _startElementPosition; // Начальная позиция элемента
        public IncomeBox()
        {
            InitializeComponent();

            // Привязываем обработчики событий для перетаскивания
            this.MouseLeftButtonDown += Block_MouseLeftButtonDown;
            this.MouseMove += Block_MouseMove;
            this.MouseLeftButtonUp += Block_MouseLeftButtonUp;
            MessageBox.Show("IncomeBox создан"); // Диагностика
        }


        public event Action<IncomeBox>? OnDeleteRequestedIncome;

        private void DeleteBlockIncomve(object sender, RoutedEventArgs e)
        {
            {
                MessageBox.Show("Кнопка удаления нажата."); // Диагностическое сообщение

                if (OnDeleteRequestedIncome != null)
                {
                    OnDeleteRequestedIncome.Invoke(this);
                }
                else
                {
                    MessageBox.Show("Событие удаления не подписано.");
                }
            }
        }

        private void HideZoneText(object sender, RoutedEventArgs e)
        {
            if (ZoneHide.Visibility == Visibility.Visible)
            {
                ZoneHide.Visibility = Visibility.Collapsed;
            }
            else // Если элемент скрыт, показываем его
            {
                ZoneHide.Visibility = Visibility.Visible;
            }
        }

        private void ibWrite_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var ibVar1 = ibLink1.Text;
                ibPole1.Text = ibVar1;
                e.Handled = true;
            }
        }
        private void ibWrite_2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var ibVar2 = ibLink2.Text;
                ibPole2.Text = ibVar2;
                e.Handled = true;
            }
        }

        private void ibWrite_3(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var ibVar3 = ibLink3.Text;
                ibPole3.Text = ibVar3;
                e.Handled = true;
            }
        }

        // Свойства для текста в TextBlock'ах
        public string Pole1Text
        {
            get => ibPole1.Text; // Получаем текст из TextBlock "ibPole1"
            set => ibPole1.Text = value; // Устанавливаем текст в TextBlock "ibPole1"
        }

        public string Pole2Text
        {
            get => ibPole2.Text; // Получаем текст из TextBlock "ibPole2"
            set => ibPole2.Text = value; // Устанавливаем текст в TextBlock "ibPole2"
        }

        public string Pole3Text
        {
            get => ibPole3.Text; // Получаем текст из TextBlock "ibPole3"
            set => ibPole3.Text = value; // Устанавливаем текст в TextBlock "ibPole3"
        }

        // Начало перетаскивания
        private void Block_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Запоминаем начальную позицию мыши
            _startMousePosition = e.GetPosition(this.Parent as UIElement);

            // Запоминаем начальную позицию элемента
            _startElementPosition = new Point(
                Canvas.GetLeft(this),
                Canvas.GetTop(this)
            );

            // Захватываем мышь
            this.CaptureMouse();
        }

        // Перемещение
        private void Block_MouseMove(object sender, MouseEventArgs e)
        {
            if (_startMousePosition.HasValue && _startElementPosition.HasValue && e.LeftButton == MouseButtonState.Pressed)
            {
                // Текущая позиция мыши
                var currentMousePosition = e.GetPosition(this.Parent as UIElement);

                // Вычисляем новое положение элемента
                var newLeft = _startElementPosition.Value.X + (currentMousePosition.X - _startMousePosition.Value.X);
                var newTop = _startElementPosition.Value.Y + (currentMousePosition.Y - _startMousePosition.Value.Y);

                // Ограничиваем положение в пределах Canvas
                var parentCanvas = this.Parent as Canvas;
                if (parentCanvas != null)
                {
                    newLeft = Math.Max(0, Math.Min(newLeft, parentCanvas.ActualWidth - this.ActualWidth));
                    newTop = Math.Max(0, Math.Min(newTop, parentCanvas.ActualHeight - this.ActualHeight));
                }

                // Устанавливаем новые координаты
                Canvas.SetLeft(this, newLeft);
                Canvas.SetTop(this, newTop);
            }
        }

        // Завершение перетаскивания
        private void Block_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_startMousePosition.HasValue)
            {
                // Освобождаем мышь
                this.ReleaseMouseCapture();
                _startMousePosition = null;
                _startElementPosition = null;
            }
        }


    }
}
