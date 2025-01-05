using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
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
using Xceed.Wpf.AvalonDock.Themes;

namespace kanban_v2
{
    /// <summary>
    /// Логика взаимодействия для OutcomeBox.xaml
    /// </summary>
    public partial class OutcomeBox : UserControl
    {
        public int? BlockId { get; set; } // Уникальный идентификатор из базы данных
        public string? IncomeMoney { get; set; }
        public string? Owner { get; set; }
        public string? Date { get; set; }



        // Поля для перетаскивания
        private Point? _startMousePosition; // Начальная позиция мыши
        private Point? _startElementPosition; // Начальная позиция элемента

        public OutcomeBox(double x, double y, bool fromDB)
        {



            InitializeComponent();

            // Привязываем обработчики событий для перетаскивания
            this.MouseLeftButtonDown += Block_MouseLeftButtonDown;
            this.MouseMove += Block_MouseMove;
            this.MouseLeftButtonUp += Block_MouseLeftButtonUp;
            MessageBox.Show("OutcomeBox создан"); // Диагностика
            if (!fromDB)
            {
                getIdOutcomeBox(x, y); //получаем ID блока после вставки
            }



        }

        public void InitializeData1(bool x)
        {
            // Используем значения свойств, заданных через инициализатор
            ibPole1.Text = IncomeMoney;
            ibPole2.Text = Date;
            ibPole3.Text = Owner;
            ibPole4.Text = Convert.ToString(BlockId);
        }

        //Логика для удаления блока
        public event Action<OutcomeBox>? OnDeleteRequestedOutcome;
        private void DeleteBlockOutcome(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить этот блок?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeleteOutcomeBoxFromDatabase(); // Удаляем запись из базы
                OnDeleteRequestedOutcome?.Invoke(this); // Уведомляем о необходимости удаления блока
            }
        }

        private void DeleteOutcomeBoxFromDatabase()
        {
            if (BlockId == null)
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

                    string query = "DELETE FROM OutcomeBox WHERE ID = @id";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", BlockId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Блок с ID {BlockId} успешно удалён из базы данных.", "Успех");
                        }
                        else
                        {
                            MessageBox.Show($"Не удалось найти строку с ID {BlockId} для удаления.", "Предупреждение");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении блока из базы данных: {ex.Message}", "Ошибка");
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
                UPDATE OutcomeBox 
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
                        command1.Parameters.AddWithValue("@z", BlockId);
                        command1.ExecuteNonQuery();
                    }
                }
            }

        }

        /*Логика записи значений пользователя и взаимодействия с БД. 
         Предполагается, что когда блок создается, он получает свой уникальный ID в БД. */
        private void getIdOutcomeBox(double x, double y)
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
            INSERT INTO OutcomeBox (X, Y, incMoney, incOwner, incDate) 
            VALUES (@x, @y, @money, @owner, @date)";

                    using (var insertCommand = new SqliteCommand(insertQuery, line))
                    {
                        insertCommand.Parameters.AddWithValue("@x", x);
                        insertCommand.Parameters.AddWithValue("@y", y);
                        insertCommand.Parameters.AddWithValue("@money", "");
                        insertCommand.Parameters.AddWithValue("@owner", "");
                        insertCommand.Parameters.AddWithValue("@date", "");

                        insertCommand.ExecuteNonQuery();
                    }

                    // Получаем последний ID
                    string getLastIdQuery = "SELECT last_insert_rowid()";
                    using (var getLastIdCommand = new SqliteCommand(getLastIdQuery, line))
                    {
                        var result = getLastIdCommand.ExecuteScalar();
                        if (result != null)
                        {
                            BlockId = Convert.ToInt32(result);
                            MessageBox.Show($"Блоку присвоен ID: {BlockId}");
                            ibPole4.Text = Convert.ToString(BlockId);
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
        private void UpdateOutcomeBoxInDatabase(string column, object value)
        {
            if (BlockId == null)
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

                    string query = $"UPDATE OutcomeBox SET {column} = @value WHERE ID = @id";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@value", value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@id", BlockId);

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


        private void ibWrite_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ibPole1.Text = ibLink1.Text; // Получаем введённое значение
                UpdateOutcomeBoxInDatabase("incMoney", ibPole1.Text); // Обновляем в БД
                e.Handled = true;
            }
        }

        private void ibWrite_2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ibPole2.Text = ibLink2.Text;
                UpdateOutcomeBoxInDatabase("incDate", ibPole2.Text); // Обновляем дату в БД
                e.Handled = true;
            }
        }

        private void ibWrite_3(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ibPole3.Text = ibLink3.Text;
                UpdateOutcomeBoxInDatabase("incOwner", ibPole3.Text); // Обновляем отправителя в БД
                e.Handled = true;
            }
        }



        public string Pole4Text
        {
            //get => BlockId; // Получаем текст из TextBlock "ibPole3"
            set => ibPole4.Text = Convert.ToString(BlockId); // Устанавливаем текст в TextBlock "ibPole3"
        }



    }

}
