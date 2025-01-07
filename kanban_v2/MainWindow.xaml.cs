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
using System;

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
            this.WindowState = WindowState.Maximized;

            // Запрещаем изменение размеров окна
            this.ResizeMode = ResizeMode.NoResize;

            
        }

        //Вставка блока IncomeBox (с записью в БД)
        private void AddIncomeBox_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalData.isConnected)
            {
                var mixedList = new List<object>
            {
                false, 0, 0,0, "null", "null","null",
            };

                NewIncomeBox(mixedList);
            }
            else
            {
                MessageBox.Show("База данных ещё не подключена!", "Предупреждение");

            }

        }
        //Создаем блок IncomBox
        private void NewIncomeBox<T>(List<T> items)
        {

            //Условие создания, если вызов был от кнопки пользователя.
            if (items[0] is bool firstItem && firstItem == false)
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

            //Условие создания, если вызов был от открытия БД.
            if (items[0] is bool firstItem1 && firstItem1 == true)
            {
                // Создаём блок IncomeBox
                var incomeBox = new IncomeBox(Convert.ToDouble(items[2]), Convert.ToDouble(items[3]), true)
                {
                    BlockId = Convert.ToInt32(items[1]), // Привязываем ID из базы данных
                    IncomeMoney = Convert.ToString(items[4]), // Ваши свойства IncomeMoney и другие должны быть добавлены в класс IncomeBox
                    Owner = Convert.ToString(items[5]),
                    Date = Convert.ToString(items[6]),
                };

                incomeBox.InitializeData(true);
                Debug.WriteLine($"Создание блока2: ID={incomeBox.BlockId}, Money={incomeBox.IncomeMoney}, Owner={incomeBox.Owner}, Date={incomeBox.Date}");

                // Устанавливаем позицию блока на Canvas
                Canvas.SetLeft(incomeBox, Convert.ToDouble(items[2]));
                Canvas.SetTop(incomeBox, Convert.ToDouble(items[3]));

                // Подписываемся на удаление блока
                incomeBox.OnDeleteRequestedIncome += RemoveBlockIncome;

                // Добавляем блок на Canvas
                Canvas2.Children.Add(incomeBox);


            }

        }
        //Удаление IncomeBlock
        private void RemoveBlockIncome(IncomeBox incomeBox)
        {
            // Удаляем блок с Canvas
            Canvas2.Children.Remove(incomeBox);
            MessageBox.Show($"Блок с ID {incomeBox.BlockId} удалён.", "Успех");
        }


        //Вставка блока OutcomeBox (с записью в БД)
        private void AddOutcomeBox_Click (object sender, RoutedEventArgs e)
        {
            if (GlobalData.isConnected)
            {
                var mixedList1 = new List<object>
            {
                false, 0, 0,0, "null", "null","null",
            };

                NewOutcomeBox(mixedList1);
            }
            else
            {
                MessageBox.Show("База данных ещё не подключена!", "Предупреждение");

            }
        }
        //Создаем блок OutcomBox
        private void NewOutcomeBox<T>(List<T> items)
        {

            //Условие создания, если вызов был от кнопки пользователя.
            if (items[0] is bool firstItem && firstItem == false)
            {
                // Получаем позицию мыши относительно Canvas
                var mousePosition = Mouse.GetPosition(Canvas2);

                // Создаём экземпляр OutcomeBox
                var outcomeBox = new OutcomeBox(mousePosition.X, mousePosition.Y, false);

                // Подписываемся на удаление блока
                outcomeBox.OnDeleteRequestedOutcome += RemoveBlockOutcome;

                // Устанавливаем позицию блока на Canvas
                Canvas.SetLeft(outcomeBox, mousePosition.X);
                Canvas.SetTop(outcomeBox, mousePosition.Y);

                // Добавляем блок на Canvas
                Canvas2.Children.Add(outcomeBox);
            }

            //Условие создания, если вызов был от открытия БД.
            if (items[0] is bool firstItem1 && firstItem1 == true)
            {
                // Создаём блок OutcomeBox
                var outcomeBox = new OutcomeBox(Convert.ToDouble(items[2]), Convert.ToDouble(items[3]), true)
                {
                    BlockId = Convert.ToInt32(items[1]), // Привязываем ID из базы данных
                    IncomeMoney = Convert.ToString(items[4]), // Ваши свойства IncomeMoney и другие должны быть добавлены в класс IncomeBox
                    Owner = Convert.ToString(items[5]),
                    Date = Convert.ToString(items[6]),
                };

                outcomeBox.InitializeData1(true);
                Debug.WriteLine($"Создание блока2: ID={outcomeBox.BlockId}, Money={outcomeBox.IncomeMoney}, Owner={outcomeBox.Owner}, Date={outcomeBox.Date}");

                // Устанавливаем позицию блока на Canvas
                Canvas.SetLeft(outcomeBox, Convert.ToDouble(items[2]));
                Canvas.SetTop(outcomeBox, Convert.ToDouble(items[3]));

                // Подписываемся на удаление блока
                outcomeBox.OnDeleteRequestedOutcome += RemoveBlockOutcome;

                // Добавляем блок на Canvas
                Canvas2.Children.Add(outcomeBox);


            }

        }
        //Удаление OutcomeBlock
        private void RemoveBlockOutcome(OutcomeBox outcomeBox)
        {
            // Удаляем блок с Canvas
            Canvas2.Children.Remove(outcomeBox);
            MessageBox.Show($"Блок с ID {outcomeBox.BlockId} удалён.", "Успех");
        }


        //Вставка блока ProjectBox (с записью в БД)
        private void AddProjectBox_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalData.isConnected)
            {
                var mixedList1 = new List<object>
            {
                false, 0, 0,0, "null", "null",
            };

                NewProjectBox(mixedList1);
            }
            else
            {
                MessageBox.Show("База данных ещё не подключена!", "Предупреждение");

            }
        }
        //Создаем блок ProjectBox
        private void NewProjectBox<T>(List<T> items)
        {

            //Условие создания, если вызов был от кнопки пользователя.
            if (items[0] is bool firstItem && firstItem == false)
            {
                // Получаем позицию мыши относительно Canvas
                var mousePosition = Mouse.GetPosition(Canvas2);

                // Создаём экземпляр ProjectBox
                var projectBox = new TypicalProjectBox(mousePosition.X, mousePosition.Y, false);

                // Подписываемся на удаление блока
                projectBox.OnDeleteRequestedProject += RemoveBlockProject;

                // Устанавливаем позицию блока на Canvas
                Canvas.SetLeft(projectBox, mousePosition.X);
                Canvas.SetTop(projectBox, mousePosition.Y);

                // Добавляем блок на Canvas
                Canvas2.Children.Add(projectBox);
            }

            //Условие создания, если вызов был от открытия БД.
            if (items[0] is bool firstItem1 && firstItem1 == true)
            {
                // Создаём блок ProjectBox
                var projectBox = new TypicalProjectBox(Convert.ToDouble(items[2]), Convert.ToDouble(items[3]), true)
                {
                    BlockID = Convert.ToInt32(items[1]), // Привязываем ID из базы данных
                    ProjectName = Convert.ToString(items[4]), // Ваши свойства IncomeMoney и другие должны быть добавлены в класс IncomeBox
                    Description = Convert.ToString(items[5]),
                    
                };

                projectBox.InitializeData1(true);
                Debug.WriteLine($"Создание блока2: ID={projectBox.BlockID}, ProjectName={projectBox.ProjectName}, Description={projectBox.Description}");

                // Устанавливаем позицию блока на Canvas
                Canvas.SetLeft(projectBox, Convert.ToDouble(items[2]));
                Canvas.SetTop(projectBox, Convert.ToDouble(items[3]));

                // Подписываемся на удаление блока
                projectBox.OnDeleteRequestedProject += RemoveBlockProject;

                // Добавляем блок на Canvas
                Canvas2.Children.Add(projectBox);


            }

        }
        //Удаление ProjectBox
        private void RemoveBlockProject(TypicalProjectBox projectBox)
        {
            // Удаляем блок с Canvas
            Canvas2.Children.Remove(projectBox);
            MessageBox.Show($"Блок с ID {projectBox.BlockID} удалён.", "Успех");
        }


        //Вставка блока ContractBox (c записью в БД)
        private void AddContractBox_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalData.isConnected)
            {
                var mixedList2 = new List<object>
            {
                false, 0, 0,0, "null", "null", "null", "null","null","null","null","null","null","null",
            };

                NewContractBox(mixedList2);
            }
            else
            {
                MessageBox.Show("База данных ещё не подключена!", "Предупреждение");

            }
        }
        //Создаем блок ContractBox
        private void NewContractBox<T>(List<T> items)
        {

            //Условие создания, если вызов был от кнопки пользователя.
            if (items[0] is bool firstItem && firstItem == false)
            {
                // Получаем позицию мыши относительно Canvas
                var mousePosition = Mouse.GetPosition(Canvas2);

                // Создаём экземпляр ContractBox
                var contractBox = new TypicalContractBox(mousePosition.X, mousePosition.Y, false);

                // Подписываемся на удаление блока
                contractBox.OnDeleteRequestedContract += RemoveBlockContract;

                // Устанавливаем позицию блока на Canvas
                Canvas.SetLeft(contractBox, mousePosition.X);
                Canvas.SetTop(contractBox, mousePosition.Y);

                // Добавляем блок на Canvas
                Canvas2.Children.Add(contractBox);
            }

            //Условие создания, если вызов был от открытия БД.
            if (items[0] is bool firstItem1 && firstItem1 == true)
            {
                // Создаём блок ContractBox
                var contractBox = new TypicalContractBox(Convert.ToDouble(items[2]), Convert.ToDouble(items[3]), true)
                {
                    BlockID = Convert.ToInt32(items[1]), // Привязываем ID из базы данных
                    dContractNumber = Convert.ToString(items[4]),
                    dStartDate = Convert.ToString(items[5]),
                    dCustomerName = Convert.ToString(items[6]),
                    dBuilderName = Convert.ToString(items[7]),
                    dStartWorkDate = Convert.ToString(items[8]),
                    dEndWorkDate = Convert.ToString(items[9]),
                    dIGK = Convert.ToString(items[10]),
                    dContractMoney = Convert.ToString(items[11]),
                    dmbAvans = Convert.ToString(items[12]),
                    dgarantMoney = Convert.ToString(items[13]),
                 };

                contractBox.InitializeData1(true);
                

                // Устанавливаем позицию блока на Canvas
                Canvas.SetLeft(contractBox, Convert.ToDouble(items[2]));
                Canvas.SetTop(contractBox, Convert.ToDouble(items[3]));

                // Подписываемся на удаление блока
                contractBox.OnDeleteRequestedContract += RemoveBlockContract;

                // Добавляем блок на Canvas
                Canvas2.Children.Add(contractBox);


            }

        }

        //Удаление ContractBox
        private void RemoveBlockContract(TypicalContractBox contractBox)
        {
            // Удаляем блок с Canvas
            Canvas2.Children.Remove(contractBox);
            MessageBox.Show($"Блок с ID {contractBox.BlockID} удалён.", "Успех");
        }


        //Вставка блока TxtBlock (с записью в БД)
        private void addTxtBox(object sender, RoutedEventArgs e)
        {
            if (GlobalData.isConnected)
            {
                var mixedList33 = new List<object>
            {
                false, 0, 0,"null",
            };

                NewTxtBox(mixedList33);
            }
            else
            {
                MessageBox.Show("База данных ещё не подключена!", "Предупреждение");

            }
        }
        //Создаем блок TxTBox
        private void NewTxtBox<T>(List<T> items)
        {

            //Условие создания, если вызов был от кнопки пользователя.
            if (items[0] is bool firstItem && firstItem == false)
            {
                // Получаем позицию мыши относительно Canvas
                var mousePosition = Mouse.GetPosition(Canvas2);

                // Создаём экземпляр ContractBox
                var txtNewBox = new txtBlock(mousePosition.X, mousePosition.Y, false);

                // Подписываемся на удаление блока
                txtNewBox.OnDeleteRequestedTxtBox += RemoveBlockTxt;

                // Устанавливаем позицию блока на Canvas
                Canvas.SetLeft(txtNewBox, mousePosition.X);
                Canvas.SetTop(txtNewBox, mousePosition.Y);

                // Добавляем блок на Canvas
                Canvas2.Children.Add(txtNewBox);
            }

            //Условие создания, если вызов был от открытия БД.
            if (items[0] is bool firstItem1 && firstItem1 == true)
            {
                // Создаём блок ContractBox
                var txtNewBox = new txtBlock(Convert.ToDouble(items[2]), Convert.ToDouble(items[3]), true)
                {
                    BlockId = Convert.ToInt32(items[1]), // Привязываем ID из базы данных
                    dtxtNotice = Convert.ToString(items[4]),
                    
                };

                txtNewBox.InitializeData1(true);


                // Устанавливаем позицию блока на Canvas
                Canvas.SetLeft(txtNewBox, Convert.ToDouble(items[2]));
                Canvas.SetTop(txtNewBox, Convert.ToDouble(items[3]));

                // Подписываемся на удаление блока
                txtNewBox.OnDeleteRequestedTxtBox += RemoveBlockTxt;

                // Добавляем блок на Canvas
                Canvas2.Children.Add(txtNewBox);


            }

        }
        //Удаление TxTBox
        private void RemoveBlockTxt(txtBlock txtNewBox)
        {
            // Удаляем блок с Canvas
            Canvas2.Children.Remove(txtNewBox);
            MessageBox.Show($"Блок с ID {txtNewBox.BlockId} удалён.", "Успех");
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
                        incMoney TEXT NOT NULL,
                        incOwner TEXT NOT NULL,
                        incDate TEXT NOT NULL)";

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
                                GlobalData.isConnected = true;
                            }
                            else
                            {
                                MessageBox.Show("В базе данных отсутствует таблица Info.", "Предупреждение");
                                GlobalData.isConnected = false;
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
                                    string incMoney = string.IsNullOrEmpty(reader.IsDBNull(3) ? null : reader.GetString(3)) ? "Введите сумму" : reader.GetString(3);
                                    string incOwner = string.IsNullOrEmpty(reader.IsDBNull(4) ? null : reader.GetString(4)) ? "Отправитель" : reader.GetString(4);
                                    string incDate = string.IsNullOrEmpty(reader.IsDBNull(5) ? null : reader.GetString(5)) ? "Дата" : reader.GetString(5);


                                    // Проверяем полученные значения
                                    Debug.WriteLine($"Создание блока1: ID={id}, X={x}, Y={y}, Money={incMoney}, Owner={incOwner}, Date={incDate}");

                                    //Формируем список из переменных и вызываем метод
                                    var mixedList = new List<object>
                                        {
                                             true, id, x,y, incMoney, incOwner,incDate,
                                        };

                                    NewIncomeBox(mixedList);


                                }
                            }
                        }

                        // Проверяем, есть ли таблица OutcomeBox
                        string checkTableQueryOutcomeBox = "SELECT name FROM sqlite_master WHERE type='table' AND name='OutcomeBox'";
                        using (var command = new SqliteCommand(checkTableQueryOutcomeBox, connection))
                        {
                            var result = command.ExecuteScalar();
                            if (result != null)
                            {
                                MessageBox.Show($"В базе данных присутствует таблица OutcomeBox.", "Успех");
                            }
                            else
                            {
                                MessageBox.Show("В базе данных отсутствует таблица OutcomeBox.", "Предупреждение");
                            }
                        }

                        // Считываем записи из таблицы OutcomeBox
                        string selectOutcomeBoxQuery = "SELECT Id, X, Y, incMoney, incOwner, incDate FROM OutcomeBox";
                        using (var command = new SqliteCommand(selectOutcomeBoxQuery, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Получаем значения из таблицы
                                    int id = reader.GetInt32(0);
                                    double x = reader.GetDouble(1);
                                    double y = reader.GetDouble(2);
                                    string incMoney = string.IsNullOrEmpty(reader.IsDBNull(3) ? null : reader.GetString(3)) ? "Введите сумму" : reader.GetString(3);
                                    string incOwner = string.IsNullOrEmpty(reader.IsDBNull(4) ? null : reader.GetString(4)) ? "Получатель" : reader.GetString(4);
                                    string incDate = string.IsNullOrEmpty(reader.IsDBNull(5) ? null : reader.GetString(5)) ? "Дата" : reader.GetString(5);


                                    // Проверяем полученные значения
                                    Debug.WriteLine($"Создание блока1: ID={id}, X={x}, Y={y}, Money={incMoney}, Owner={incOwner}, Date={incDate}");

                                    //Формируем список из переменных и вызываем метод
                                    var mixedList = new List<object>
                                        {
                                             true, id, x,y, incMoney, incOwner,incDate,
                                        };

                                    NewOutcomeBox(mixedList);
                                }
                            }
                        }

                        // Проверяем, есть ли таблица ProjectBox
                        string checkTableQueryProjectBox = "SELECT name FROM sqlite_master WHERE type='table' AND name='ProjectBox'";
                        using (var command = new SqliteCommand(checkTableQueryProjectBox, connection))
                        {
                            var result = command.ExecuteScalar();
                            if (result != null)
                            {
                                MessageBox.Show($"В базе данных присутствует таблица ProjectBox.", "Успех");
                            }
                            else
                            {
                                MessageBox.Show("В базе данных отсутствует таблица ProjectBox.", "Предупреждение");
                            }
                        }

                        // Считываем записи из таблицы ProjectBox
                        string selectProjectBoxQuery = "SELECT Id, X, Y, ProjectName, Description FROM ProjectBox";
                        using (var command = new SqliteCommand(selectProjectBoxQuery, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Получаем значения из таблицы
                                    int id = reader.GetInt32(0);
                                    double x = reader.GetDouble(1);
                                    double y = reader.GetDouble(2);
                                    string projectName = string.IsNullOrEmpty(reader.IsDBNull(3) ? null : reader.GetString(3)) ? "Введите название проекта" : reader.GetString(3);
                                    string description = string.IsNullOrEmpty(reader.IsDBNull(4) ? null : reader.GetString(4)) ? "Введите описание проекта" : reader.GetString(4);
                                    


                                    // Проверяем полученные значения
                                    Debug.WriteLine($"Создание блока1: ID={id}, X={x}, Y={y}, ProjectName={projectName}, Description={description}");

                                    //Формируем список из переменных и вызываем метод
                                    var mixedList = new List<object>
                                        {
                                             true, id, x,y, projectName, description,
                                        };

                                    NewProjectBox(mixedList);
                                }
                            }
                        }

                        // Проверяем, есть ли таблица ContractBox
                        string checkTableQueryContractBox = "SELECT name FROM sqlite_master WHERE type='table' AND name='ContractBox'";
                        using (var command = new SqliteCommand(checkTableQueryContractBox, connection))
                        {
                            var result = command.ExecuteScalar();
                            if (result != null)
                            {
                                MessageBox.Show($"В базе данных присутствует таблица ContractBox.", "Успех");
                            }
                            else
                            {
                                MessageBox.Show("В базе данных отсутствует таблица ContractBox.", "Предупреждение");
                            }
                        }

                        // Считываем записи из таблицы ContractBox
                        string selectContractBoxQuery = "SELECT Id, X, Y, ContractNumber, ContractDate, Customer, Builser, DataStartWork, DataEndWork, Igk, mbMoney, mbAvans, mbGarant FROM ContractBox";
                        using (var command = new SqliteCommand(selectContractBoxQuery, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Получаем значения из таблицы
                                    int id = reader.GetInt32(0);
                                    double x = reader.GetDouble(1);
                                    double y = reader.GetDouble(2);
                                    string sContractNumber = string.IsNullOrEmpty(reader.IsDBNull(3) ? null : reader.GetString(3)) ? "Введите номер контракта" : reader.GetString(3);
                                    string sContractDate = string.IsNullOrEmpty(reader.IsDBNull(4) ? null : reader.GetString(4)) ? "Введите дату контракта" : reader.GetString(4);
                                    string sCustomer = string.IsNullOrEmpty(reader.IsDBNull(5) ? null : reader.GetString(5)) ? "Введите заказчика" : reader.GetString(5);
                                    string sBuilser = string.IsNullOrEmpty(reader.IsDBNull(6) ? null : reader.GetString(6)) ? "Введите исполнителя" : reader.GetString(6);
                                    string sDataStartWork = string.IsNullOrEmpty(reader.IsDBNull(7) ? null : reader.GetString(7)) ? "Введите дату начала работ" : reader.GetString(7);
                                    string sDataEndWork = string.IsNullOrEmpty(reader.IsDBNull(8) ? null : reader.GetString(8)) ? "Введите дату окончания работ" : reader.GetString(8);
                                    string sIgk = string.IsNullOrEmpty(reader.IsDBNull(9) ? null : reader.GetString(9)) ? "Введите номер ИГК" : reader.GetString(9);
                                    string smbMoney = string.IsNullOrEmpty(reader.IsDBNull(10) ? null : reader.GetString(10)) ? "Введите стоимость контракта" : reader.GetString(10);
                                    string smbAvans = string.IsNullOrEmpty(reader.IsDBNull(11) ? null : reader.GetString(11)) ? "Введите сумму аванса" : reader.GetString(11);
                                    string smbGarant = string.IsNullOrEmpty(reader.IsDBNull(12) ? null : reader.GetString(12)) ? "Введите сумму гарантийных обязательств" : reader.GetString(12);




                                    

                                    //Формируем список из переменных и вызываем метод
                                    var mixedList = new List<object>
                                        {
                                             true, id, x,y, sContractNumber, sContractDate, sCustomer, sBuilser, sDataStartWork, sDataEndWork, sIgk, smbMoney, smbAvans, smbGarant,
                                        };

                                    NewContractBox(mixedList);
                                }
                            }
                        }

                        // Проверяем, есть ли таблица txtBox
                        string checkTableQueryTxtBox = "SELECT name FROM sqlite_master WHERE type='table' AND name='txtBox'";
                        using (var command = new SqliteCommand(checkTableQueryTxtBox, connection))
                        {
                            var result = command.ExecuteScalar();
                            if (result != null)
                            {
                                MessageBox.Show($"В базе данных присутствует таблица txtBox.", "Успех");
                            }
                            else
                            {
                                MessageBox.Show("В базе данных отсутствует таблица txtBox.", "Предупреждение");
                            }
                        }

                        // Считываем записи из таблицы ContractBox
                        string selectTxtBoxQuery = "SELECT Id, X, Y, txt FROM txtBox";
                        using (var command = new SqliteCommand(selectTxtBoxQuery, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Получаем значения из таблицы
                                    int id = reader.GetInt32(0);
                                    double x = reader.GetDouble(1);
                                    double y = reader.GetDouble(2);
                                    string dtxtNotice = string.IsNullOrEmpty(reader.IsDBNull(3) ? null : reader.GetString(3)) ? "Введите заметку" : reader.GetString(3);
                                   





                                    //Формируем список из переменных и вызываем метод
                                    var mixedList44 = new List<object>
                                        {
                                             true, id, x,y, dtxtNotice,
                                        };

                                    NewTxtBox(mixedList44);
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







