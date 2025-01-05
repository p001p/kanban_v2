using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace kanban_v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        public MainWindow()
        {
            InitializeComponent();
        }

        //Вставка блока IncomeBox (с записью в БД)
        private void AddIncomeBox_Click(object sender, RoutedEventArgs e)
        {
            // Получаем позицию мыши относительно Canvas
            var mousePosition = Mouse.GetPosition(Canvas2);

            // Создаём экземпляр ProjectBoxControl
            var incomeBox = new IncomeBox(mousePosition.X, mousePosition.Y, false);

            // Подписываемся на удаление блока
            incomeBox.OnDeleteRequestedIncome += RemoveBlockIncome;

            // Устанавливаем позицию блока на Canvas
            Canvas.SetLeft(incomeBox, mousePosition.X);
            Canvas.SetTop(incomeBox, mousePosition.Y);

            // Добавляем блок на Canvas
            Canvas2.Children.Add(incomeBox);
                       
        }
        //Удаление IncomeBlock
        private void RemoveBlockIncome(IncomeBox incomeBox)
        {
            // Удаляем блок с Canvas
            Canvas2.Children.Remove(incomeBox);
            MessageBox.Show($"Блок с ID {incomeBox.BlockId} удалён.", "Успех");
        }


        //Создаем DB по кнопке
        private void CreateNewDB(object sender, RoutedEventArgs e)
        {
            //Открываем диалоговое окно
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Укажите папку для хранения вновь созданной базы данных",
                Filter = "SQLite Database (*.sqlite)|*.sqlite",
                DefaultExt = ".sqlite"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string databasePath = saveFileDialog.FileName;

                try
                {
                    //Создание базы данных
                    using (var connection = new SqliteConnection($"Data Source = {databasePath}"))
                    {
                        connection.Open();

                        //Создаем таблицу и добавляем первую запись
                        string createTableQuery = @"CREATE TABLE IF NOT EXISTS Info (Id INTEGER PRIMARY KEY AUTOINCREMENT,DatabaseName TEXT NOT NULL)";
                        using (var command = new SqliteCommand(createTableQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        // Запрашиваем имя базы у пользователя
                        string databaseName = Microsoft.VisualBasic.Interaction.InputBox(
                            "Введите название базы данных:", "Название базы данных", "Моя База");

                        // Добавляем запись с названием базы
                        string insertQuery = "INSERT INTO Info (DatabaseName) VALUES (@name)";
                        using (var command = new SqliteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@name", databaseName);
                            command.ExecuteNonQuery();
                        }

                        string createProjectBox = @"CREATE TABLE IF NOT EXISTS ProjectBox (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        X INTEGER NOT NULL,
                        Y INTEGER NOT NULL,
                        ProjectName TEXT NOT NULL,
                        Description TEXT NOT NULL)";

                        using (var command = new SqliteCommand(createProjectBox, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        string createContractBox = @"CREATE TABLE IF NOT EXISTS ContractBox (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        X INTEGER NOT NULL,
                        Y INTEGER NOT NULL,
                        ContractNumber TEXT NOT NULL,
                        ContractDate TEXT NOT NULL,
                        Customer TEXT NOT NULL,
                        Builser TEXT NOT NULL,
                        DataStartWork TEXT NOT NULL,
                        DataEndWork TEXT NOT NULL,
                        Igk TEXT NOT NULL,
                        mbMoney TEXT NOT NULL,
                        mbAvans TEXT NOT NULL,
                        mbGarant TEXT NOT NULL)";

                        using (var command = new SqliteCommand(createContractBox, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        string createIncomeBox = @"CREATE TABLE IF NOT EXISTS IncomeBox (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        X INTEGER NOT NULL,
                        Y INTEGER NOT NULL,
                        incMoney TEXT NOT NULL,
                        incOwner TEXT NOT NULL,
                        incDate TEXT NOT NULL)";

                        using (var command = new SqliteCommand(createIncomeBox, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        string createOutcomeBox = @"CREATE TABLE IF NOT EXISTS OutcomeBox (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        X INTEGER NOT NULL,
                        Y INTEGER NOT NULL,
                        outMoney TEXT NOT NULL,
                        outOwner TEXT NOT NULL,
                        outDate TEXT NOT NULL)";

                        using (var command = new SqliteCommand(createOutcomeBox, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        string createTxtBox = @"CREATE TABLE IF NOT EXISTS txtBox (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        X INTEGER NOT NULL,
                        Y INTEGER NOT NULL,
                        txt TEXT NOT NULL)";

                        using (var command = new SqliteCommand(createTxtBox, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"База данных успешно создана: {databasePath}", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании базы данных: {ex.Message}", "Ошибка");
                }
            }
        }
        // Обработчик кнопки "Открыть базу данных"
        private void OpenDB(object sender, RoutedEventArgs e)
        {
            // Открываем диалоговое окно для выбора файла
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите существующую базу данных",
                Filter = "SQLite Database (*.sqlite)|*.sqlite",
                DefaultExt = ".sqlite"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Canvas2.Children.Clear();
                string databasePath = openFileDialog.FileName;
                GlobalData.globalDatabasePath = databasePath;
                try
                {
                    // Проверяем подключение
                    using (var connection = new SqliteConnection($"Data Source={databasePath}"))
                    {
                        connection.Open();

                        // Проверяем, есть ли таблица Info
                        string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='Info'";
                        using (var command = new SqliteCommand(checkTableQuery, connection))
                        {
                            var result = command.ExecuteScalar();
                            if (result != null)
                            {
                                MessageBox.Show($"База данных успешно открыта: {databasePath}", "Успех");
                            }
                            else
                            {
                                MessageBox.Show("В базе данных отсутствует таблица Info.", "Предупреждение");
                            }
                        }

                        // Считываем название базы данных из таблицы Info
                        string selectQuery = "SELECT DatabaseName FROM Info LIMIT 1";
                        using (var command = new SqliteCommand(selectQuery, connection))
                        {
                            var databaseName = command.ExecuteScalar()?.ToString();
                            if (!string.IsNullOrEmpty(databaseName))
                            {
                                DatabaseNameTextBlock.Text = $"Название базы данных: {databaseName}";
                            }
                            else
                            {
                                DatabaseNameTextBlock.Text = "Название базы данных не найдено.";
                            }
                        }

                        // Проверяем, есть ли таблица IncomeBox
                        string checkTableQueryIncomeBox = "SELECT name FROM sqlite_master WHERE type='table' AND name='IncomeBox'";
                        using (var command = new SqliteCommand(checkTableQueryIncomeBox, connection))
                        {
                            var result = command.ExecuteScalar();
                            if (result != null)
                            {
                                MessageBox.Show($"В базе данных присутствует таблица IncomeBox.", "Успех");
                            }
                            else
                            {
                                MessageBox.Show("В базе данных отсутствует таблица IncomeBox.", "Предупреждение");
                            }
                        }

                        // Считываем записи из таблицы IncomeBox
                        string selectIncomeBoxQuery = "SELECT Id, X, Y, incMoney, incOwner, incDate FROM IncomeBox";
                        using (var command = new SqliteCommand(selectIncomeBoxQuery, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Получаем значения из таблицы
                                    int id = reader.GetInt32(0);
                                    double x = reader.GetDouble(1);
                                    double y = reader.GetDouble(2);
                                    string incMoney = reader.GetString(3);
                                    string incOwner = reader.GetString(4);
                                    string incDate = reader.GetString(5);

                                    // Проверяем полученные значения
                                    Debug.WriteLine($"Создание блока1: ID={id}, X={x}, Y={y}, Money={incMoney}, Owner={incOwner}, Date={incDate}");

                                    // Создаём блок IncomeBox
                                    var incomeBox = new IncomeBox(x, y, true)
                                    {
                                        BlockId = id, // Привязываем ID из базы данных
                                        IncomeMoney = incMoney, // Ваши свойства IncomeMoney и другие должны быть добавлены в класс IncomeBox
                                        Owner = incOwner,
                                        Date = incDate,
                                    };

                                    incomeBox.InitializeData(true);
                                    Debug.WriteLine($"Создание блока2: ID={incomeBox.BlockId}, Money={incomeBox.IncomeMoney}, Owner={incomeBox.Owner}, Date={incomeBox.Date}");

                                    // Устанавливаем позицию блока на Canvas
                                    Canvas.SetLeft(incomeBox, x);
                                    Canvas.SetTop(incomeBox, y);

                                    // Подписываемся на удаление блока
                                    incomeBox.OnDeleteRequestedIncome += RemoveBlockIncome;

                                    // Добавляем блок на Canvas
                                    Canvas2.Children.Add(incomeBox);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии базы данных: {ex.Message}", "Ошибка");
                }
            }
        }
        private bool DatabaseExists(string databasePath)
        {
            return File.Exists(databasePath);
        }
        private bool TableExists(string tableName, string connectionString)
        {
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@tableName", tableName);
                        var result = command.ExecuteScalar();
                        return result != null; // true, если таблица существует
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке таблицы: {ex.Message}", "Ошибка");
                return false;
            }
        }

        


    }
        

    }







