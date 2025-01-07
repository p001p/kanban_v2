using Microsoft.Data.Sqlite;
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
        //Пересылаем данные для конструктора, у нас дохуя переменных
        public int? BlockID { get; set; } //Уникальный идентификатора из базы данных
        public string? dContractNumber { get; set; }
        public string? dCustomerName { get; set; }
        public string? dBuilderName { get; set; }
        public string? dStartDate { get; set; }
        public string? dStartWorkDate { get; set; }
        public string? dEndWorkDate { get; set; }
        public string? dIGK { get; set; }
        public string? dContractMoney { get; set; }
        public string? dmbAvans { get; set; }
        public string? dgarantMoney { get; set; }

        // Поля для перетаскивания
        private Point? _startMousePosition; // Начальная позиция мыши
        private Point? _startElementPosition; // Начальная позиция элемента

        public TypicalContractBox(double x, double y, bool fromDB)
        {
            InitializeComponent();
            // Привязываем обработчики событий для перетаскивания
            this.MouseLeftButtonDown += Block_MouseLeftButtonDown;
            this.MouseMove += Block_MouseMove;
            this.MouseLeftButtonUp += Block_MouseLeftButtonUp;
            MessageBox.Show("ContractBox создан"); // Диагностика
            if (!fromDB)
            {
                getIdContractBox(x, y); //получаем ID блока после вставки
            }
        }

        public void InitializeData1(bool x)
        {
            // Используем значения свойств, заданных через инициализатор
            ContractNumber.Text = dContractNumber;
            CustomerName.Text = dCustomerName;
            BuilderName.Text = dBuilderName;
            StartDate.Text = dStartDate;
            StartWorkDate.Text = dStartWorkDate;
            EndWorkDate.Text = dEndWorkDate;
            IGK.Text = dIGK;
            ContractMoney.Text = dContractMoney;
            mbAvans.Text = dmbAvans;
            garantMoney.Text = dgarantMoney;
            ContractID.Text = Convert.ToString(BlockID);
            Corrector.Visibility = Visibility.Collapsed;
            Money.Visibility = Visibility.Collapsed;
        }

        //Логика для удаления блока
        public event Action<TypicalContractBox>? OnDeleteRequestedContract;
        private void DeleteBlock(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить этот блок?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeleteContractBoxFromDatabase(); // Удаляем запись из базы
                OnDeleteRequestedContract?.Invoke(this); // Уведомляем о необходимости удаления блока
            }
        }

        private void DeleteContractBoxFromDatabase()
        {
            if (BlockID == null)
            {
                MessageBox.Show("ID блока не установлен. Удаление невозможно.", "Ошибка");
                return;
            }

            string connectionString = $"Data Source={GlobalData.globalDatabasePath}";

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM ContractBox WHERE ID = @id";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", BlockID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Блок с ID {BlockID} успешно удалён из базы данных.", "Успех");
                        }
                        else
                        {
                            MessageBox.Show($"Не удалось найти строку с ID {BlockID} для удаления.", "Предупреждение");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении блока из базы данных: {ex.Message}", "Ошибка");
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

                double xNew = Canvas.GetLeft(this);
                double yNew = Canvas.GetTop(this);

                string connectionString = $"Data Source={GlobalData.globalDatabasePath}";
                string query = $@"
                UPDATE ContractBox 
                SET [X] = @x, 
                [Y] = @y
                WHERE [ID] = @z";

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    using (var command1 = new SqliteCommand(query, connection))
                    {
                        command1.Parameters.AddWithValue("@x", xNew);
                        command1.Parameters.AddWithValue("@y", yNew);
                        command1.Parameters.AddWithValue("@z", BlockID);
                        command1.ExecuteNonQuery();
                    }
                }
            }

        }

        /*Логика записи значений пользователя и взаимодействия с БД. 
        Предполагается, что когда блок создается, он получает свой уникальный ID в БД. */
        private void getIdContractBox(double x, double y)
        {
            string connectionString2 = $"Data Source={GlobalData.globalDatabasePath}";

            try
            {
                using (var line = new SqliteConnection(connectionString2))
                {
                    line.Open();
                    MessageBox.Show("Соединение с базой данных успешно установлено.", "Успех");

                    // Вставляем новую запись
                    string insertQuery = @"
            INSERT INTO ContractBox (X, Y, ContractNumber, ContractDate, Customer, Builser, DataStartWork, DataEndWork, Igk, mbMoney, mbAvans, mbGarant) 
            VALUES (@x, @y, @contractNumber, @contractDate, @customer, @builser, @dataStartWork, @dataEndWork, @igk, @mmbMoney, @mmbAvans, @mmbGarant)";

                    using (var insertCommand = new SqliteCommand(insertQuery, line))
                    {
                        insertCommand.Parameters.AddWithValue("@x", x);
                        insertCommand.Parameters.AddWithValue("@y", y);
                        insertCommand.Parameters.AddWithValue("@contractNumber", "");
                        insertCommand.Parameters.AddWithValue("@contractDate", "");
                        insertCommand.Parameters.AddWithValue("@customer", "");
                        insertCommand.Parameters.AddWithValue("@builser", "");
                        insertCommand.Parameters.AddWithValue("@dataStartWork", "");
                        insertCommand.Parameters.AddWithValue("@dataEndWork", "");
                        insertCommand.Parameters.AddWithValue("@igk", "");
                        insertCommand.Parameters.AddWithValue("@mmbMoney", "");
                        insertCommand.Parameters.AddWithValue("@mmbAvans", "");
                        insertCommand.Parameters.AddWithValue("@mmbGarant", "");



                        insertCommand.ExecuteNonQuery();
                    }

                    // Получаем последний ID
                    string getLastIdQuery = "SELECT last_insert_rowid()";
                    using (var getLastIdCommand = new SqliteCommand(getLastIdQuery, line))
                    {
                        var result = getLastIdCommand.ExecuteScalar();
                        if (result != null)
                        {
                            BlockID = Convert.ToInt32(result);
                            MessageBox.Show($"Блоку присвоен ID: {BlockID}");
                            ContractID.Text = Convert.ToString(BlockID);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось получить последний ID.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при установлении соединения: {ex.Message}", "Ошибка");
            }
        }

        //Обновление значений. Общий метод.
        private void UpdateContractBoxInDatabase(string column, object value)
        {
            if (BlockID == null)
            {
                MessageBox.Show("ID блока не установлен. Обновление невозможно.", "Ошибка");
                return;
            }

            string connectionString = $"Data Source={GlobalData.globalDatabasePath}";

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string query = $"UPDATE ContractBox SET {column} = @value WHERE ID = @id";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@value", value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@id", BlockID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Значение {column} успешно обновлено.", "Успех");
                        }
                        else
                        {
                            MessageBox.Show($"Не удалось обновить значение {column}.", "Ошибка");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении значения {column}: {ex.Message}", "Ошибка");
            }
        }

        private void enterValue_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ContractNumber.Text = ContractNumberToBox.Text;
                UpdateContractBoxInDatabase("contractNumber", ContractNumber.Text);
                e.Handled = true;
            }
        }
        private void enterValue_2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CustomerName.Text = CustomerToBox.Text;
                UpdateContractBoxInDatabase("customer", CustomerName.Text);
                e.Handled = true;
            }
        }

        private void enterValue_3(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BuilderName.Text = BuilderToBox.Text;
                UpdateContractBoxInDatabase("builser", BuilderName.Text);
                e.Handled = true;
            }
        }

        private void enterValue_4(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartDate.Text = StartDateToBox.Text;
                UpdateContractBoxInDatabase("contractDate", StartDate.Text);
                e.Handled = true;
            }
        }

        private void enterValue_5(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartWorkDate.Text = StartWorkDateToBox.Text;
                UpdateContractBoxInDatabase("dataStartWork", StartWorkDate.Text);
                e.Handled = true;
            }
        }

        private void enterValue_6(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EndWorkDate.Text = EndWorkDateToBox.Text;
                UpdateContractBoxInDatabase("dataEndWork", EndWorkDate.Text);
                e.Handled = true;
            }
        }

        private void enterValue_7(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IGK.Text = IGKToBox.Text;
                UpdateContractBoxInDatabase("IGK", IGK.Text);
                e.Handled = true;
            }
        }

        private void enterValue_8(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ContractMoney.Text = ContractMoneyToBox.Text;
                UpdateContractBoxInDatabase("mbMoney", ContractMoney.Text);
                e.Handled = true;
            }
        }

        private void enterValue_9(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                mbAvans.Text = mbAvansToBox.Text;
                UpdateContractBoxInDatabase("mbAvans", mbAvans.Text);
                e.Handled = true;
            }
        }

        private void enterValue_10(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                garantMoney.Text = garantMoneyToBox.Text;
                UpdateContractBoxInDatabase("mbGarant", garantMoney.Text);
                e.Handled = true;
            }
        }

        public string Pole4Text
        {
            //get => BlockId; // Получаем текст из TextBlock "ibPole3"
            set => ContractID.Text = Convert.ToString(BlockID); // Устанавливаем текст в TextBlock "ibPole3"
        }

    }
}











