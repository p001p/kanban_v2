using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.VisualBasic; // Для использования InputBox

namespace kanban_v2
{
    /// <summary>
    /// Логика взаимодействия для txtBlock.xaml
    /// </summary>
    public partial class txtBlock : UserControl
    {
        public int? BlockId { get; set; } // Уникальный идентификатор из базы данных
        public string? dtxtNotice { get; set; }

        // Поля для перетаскивания
        private Point? _startMousePosition; // Начальная позиция мыши
        private Point? _startElementPosition; // Начальная позиция элемента

        public txtBlock(double x, double y, bool fromDB)
        {
            InitializeComponent();
            // Привязываем обработчики событий для перетаскивания
            this.MouseLeftButtonDown += Block_MouseLeftButtonDown;
            this.MouseMove += Block_MouseMove;
            this.MouseLeftButtonUp += Block_MouseLeftButtonUp;
            MessageBox.Show("txtBox создан"); // Диагностика
            if (!fromDB)
            {
                getIdTxtBox(x, y); //получаем ID блока после вставки
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            // Увеличиваем ширину и высоту UserControl
            double newWidth = this.Width + e.HorizontalChange;
            double newHeight = this.Height + e.VerticalChange;

            // Устанавливаем минимальные размеры
            this.Width = newWidth > 50 ? newWidth : 200;
            this.Height = newHeight > 50 ? newHeight : 30;
        }

        public void InitializeData1(bool x)
        {
            // Используем значения свойств, заданных через инициализатор
            txtNotice.Text = dtxtNotice;
            idTxtBlock.Text = Convert.ToString(BlockId);
            
        }

        //Логика для удаления блока
        public event Action<txtBlock>? OnDeleteRequestedTxtBox;
        private void delTxt(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить этот блок?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeleteTxtBoxFromDatabase(); // Удаляем запись из базы
                OnDeleteRequestedTxtBox?.Invoke(this); // Уведомляем о необходимости удаления блока
            }
        }

        private void DeleteTxtBoxFromDatabase()
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

                    string query = "DELETE FROM TxtBox WHERE ID = @id";
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


        /*  private void NeedTextChanger(object sender, RoutedEventArgs e)
          {
              {
                  // Вызов InputBox
                  string userInput = Interaction.InputBox(
                      "Введите значение:",    // Сообщение
                      "InputBox Example",     // Заголовок окна
                      "");                    // Значение по умолчанию

                  // Проверяем, что пользователь ввёл текст (не нажал "Cancel")
                  if (!string.IsNullOrEmpty(userInput))
                  {
                      txtNotice.Text = userInput; // Записываем значение в TextBox
                  }
              }
          }

          */

        private void NeedTextChanger(object sender, RoutedEventArgs e)
        {
            TextInputPopup.IsOpen = true; // Открываем Popup
        }

        private void ConfirmPopup_Click(object sender, RoutedEventArgs e)
        {
            txtNotice.Text = PopupTextBox.Text; // Переносим текст из Popup
            UpdateTxtBoxInDatabase("txt", txtNotice.Text); // Обновляем в БД
            e.Handled = true;
            TextInputPopup.IsOpen = false; // Закрываем Popup
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
                UPDATE TxtBox 
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
        private void getIdTxtBox(double x, double y)
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
            INSERT INTO TxtBox (X, Y, txt) 
            VALUES (@x, @y, @data)";

                    using (var insertCommand = new SqliteCommand(insertQuery, line))
                    {
                        insertCommand.Parameters.AddWithValue("@x", x);
                        insertCommand.Parameters.AddWithValue("@y", y);
                        insertCommand.Parameters.AddWithValue("@data", "");

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
                            idTxtBlock.Text = Convert.ToString(BlockId);
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
        private void UpdateTxtBoxInDatabase(string column, object value)
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

                    string query = $"UPDATE txtBox SET {column} = @value WHERE ID = @id";
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
   /*     private void ibWrite_55(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
               
                UpdateTxtBoxInDatabase("txt", txtNotice.Text); // Обновляем в БД
                e.Handled = true;
            }
        }
   */
        
    }
}
