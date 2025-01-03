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
    /// Логика взаимодействия для TypicalContractBox.xaml
    /// </summary>
    public partial class TypicalContractBox : UserControl
    {
        public TypicalContractBox()
        {
            InitializeComponent();
        }
        private void enterValue_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var inputText1 = ContractNumberToBox.Text;
                ContractNumber.Text = inputText1;
                e.Handled = true;
            }
        }
        private void enterValue_2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var inputText2 = CustomerToBox.Text;
                CustomerName.Text = inputText2;
                e.Handled = true;
            }
        }
        private void enterValue_3(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var inputText3 = BuilderToBox.Text;
                BuilderName.Text = inputText3;
                e.Handled = true;
            }
        }
        private void enterValue_4(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var inputText4 = StartDateToBox.Text;
                StartDate.Text = inputText4;
                e.Handled = true;
            }
        }
        private void enterValue_5(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var inputText5 = StartWorkDateToBox.Text;
                StartWorkDate.Text = inputText5;
                e.Handled = true;
            }
        }
        private void enterValue_6(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var inputText6 = EndWorkDateToBox.Text;
            EndWorkDate.Text = inputText6;
                e.Handled = true;
            }
        }
        private void enterValue_7(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var inputText7 = IGKToBox.Text;
                IGK.Text = inputText7;
                e.Handled = true;
            }
        }
        private void enterValue_8(object sender, KeyEventArgs e)
        {
             if (e.Key == Key.Enter)
             {
                var inputText8 = ContractMoneyToBox.Text;
                ContractMoney.Text = inputText8;
                e.Handled = true;
            }
        }
        private void enterValue_9(object sender, KeyEventArgs e)
        {
             if (e.Key == Key.Enter)
             {
                var inputText9 = mbAvansToBox.Text;
                mbAvans.Text = inputText9;
                e.Handled = true;
            }
        }
        private void enterValue_10(object sender, KeyEventArgs e)
        {
             if (e.Key == Key.Enter)
             {
                var inputText10 = garantMoneyToBox.Text;
                garantMoney.Text = inputText10;
                e.Handled = true;
            }
        }

        private void HideZone(object sender, RoutedEventArgs e)
        {
            if (Corrector.Visibility == Visibility.Visible)
            {
                Corrector.Visibility = Visibility.Collapsed;
            }
            else // Если элемент скрыт, показываем его
            {
                Corrector.Visibility = Visibility.Visible;
            }
        }

        private void HideMoney(object sender, RoutedEventArgs e)
        {
            if (Money.Visibility == Visibility.Visible)
            {
                Money.Visibility = Visibility.Collapsed;
            }
            else // Если элемент скрыт, показываем его
            {
                Money.Visibility = Visibility.Visible;
            }
        }


        public event Action<TypicalContractBox>? OnDeleteRequested;

        private void DeleteBlock(object sender, RoutedEventArgs e)
        {
            OnDeleteRequested?.Invoke(this); // Уведомляем, что блок хочет быть удалён
        }
        
    }
}
