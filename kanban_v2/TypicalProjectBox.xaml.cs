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
    /// Логика взаимодействия для TypicalProjectBox.xaml
    /// </summary>
    public partial class TypicalProjectBox : UserControl
    {
        public TypicalProjectBox()
        {
            InitializeComponent();
        }

        private void SaveName(object sender, RoutedEventArgs e)
        {
            var inputText = NameToBox.Text;
            NameBox.Text = inputText;
        }

        private void SaveOpis(object sender, RoutedEventArgs e)
        {
            var inputText1 = OpisToBox.Text;
            OpisBox.Text = inputText1;
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

        public event Action<TypicalProjectBox>? OnDeleteRequestedProject;

        private void DeleteBlockProject(object sender, RoutedEventArgs e)
        {
            OnDeleteRequestedProject?.Invoke(this); // Уведомляем, что блок хочет быть удалён
        }
    }




}
