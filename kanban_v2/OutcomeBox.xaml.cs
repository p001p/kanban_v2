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
    /// Логика взаимодействия для OutcomeBox.xaml
    /// </summary>
    public partial class OutcomeBox : UserControl
    {
        public OutcomeBox()
        {
            InitializeComponent();
        }


        public event Action<OutcomeBox>? OnDeleteRequestedOutcome;

        private void DeleteBlockOutcomve(object sender, RoutedEventArgs e)
        {
            OnDeleteRequestedOutcome?.Invoke(this); // Уведомляем, что блок хочет быть удалён
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
    }
}
