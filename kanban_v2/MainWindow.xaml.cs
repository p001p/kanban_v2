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

namespace kanban_v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _blockProjectCounter = 0;
        private int _blockContractCounter = 0;
        private int _blockOutcomeCounter = 0;
        private string? _databasePath;


        public MainWindow()
        {
            InitializeComponent();
        }

        //Вставка блока TypicalProjectBox
        private void AddProjectBox_Click(object sender, RoutedEventArgs e)
        {
            // Создаём экземпляр ProjectBoxControl
            var projectBox = new TypicalProjectBox
            {
                Name = $"ProjectBox{_blockProjectCounter++}"
            };

            // Получаем позицию мыши относительно Canvas
            var mousePosition = Mouse.GetPosition(Canvas2);

            // Устанавливаем позицию блока на Canvas
            Canvas.SetLeft(projectBox, mousePosition.X);
            Canvas.SetTop(projectBox, mousePosition.Y);


            // Подписываемся на удаление блока
            projectBox.OnDeleteRequestedProject += RemoveBlockProject;

            // Подписываемся на события мыши для перемещения
            //projectBox.MouseLeftButtonDown += Block_MouseLeftButtonDown;
           // projectBox.MouseMove += Block_MouseMove;
          //  projectBox.MouseLeftButtonUp += Block_MouseLeftButtonUp;

            // Добавляем блок на Canvas
            Canvas2.Children.Add(projectBox);
        }

        //Вставка блока TypicalContractBox
        private void AddContractBox_Click(object sender, RoutedEventArgs e)
        {
            // Создаём экземпляр ProjectBoxControl
            var contractBox = new TypicalContractBox
            {
                Name = $"ContractBox{_blockContractCounter++}"
            };

            // Получаем позицию мыши относительно Canvas
            var mousePosition = Mouse.GetPosition(Canvas2);

            // Устанавливаем позицию блока на Canvas
            Canvas.SetLeft(contractBox, mousePosition.X);
            Canvas.SetTop(contractBox, mousePosition.Y);

            // Подписываемся на удаление блока
            contractBox.OnDeleteRequested += RemoveBlock;

            // Подписываемся на события мыши для перемещения
            //contractBox.MouseLeftButtonDown += Block_MouseLeftButtonDown;
            //contractBox.MouseMove += Block_MouseMove;
            //contractBox.MouseLeftButtonUp += Block_MouseLeftButtonUp;

            // Добавляем блок на Canvas
            Canvas2.Children.Add(contractBox);
        }

        //Вставка блока IncomeBox (с записью в БД)
        private void AddIncomeBox_Click(object sender, RoutedEventArgs e)
        {
            // Получаем позицию мыши относительно Canvas
            var mousePosition = Mouse.GetPosition(Canvas2);

            // Создаём экземпляр ProjectBoxControl
            var incomeBox = new IncomeBox();
            
            // Подписываемся на удаление блока
            incomeBox.OnDeleteRequestedIncome += RemoveBlockIncome;

         

            // Устанавливаем позицию блока на Canvas
            Canvas.SetLeft(incomeBox, mousePosition.X);
            Canvas.SetTop(incomeBox, mousePosition.Y);
                    
            // Добавляем блок на Canvas
            Canvas2.Children.Add(incomeBox);

            // Сохраняем данные о блоке в таблицу IncomeBox
            SaveIncomeBoxToDatabase(mousePosition.X, mousePosition.Y);
        }
        //Метод записи данных в DB для IncomeBox
        private void SaveIncomeBoxToDatabase(double x, double y)
        {
            string connectionString1 = $"Data Source={_databasePath}";

            // Проверяем, что база данных существует
            if (!DatabaseExists(_databasePath))
            {
                MessageBox.Show("Файл базы данных не найден. Проверьте путь.", "Ошибка");
                return;
            }

            // Проверяем, что таблица IncomeBox существует
            if (!TableExists("IncomeBox", connectionString1))
            {
                MessageBox.Show("Таблица IncomeBox отсутствует в базе данных. Создайте её перед записью.", "Ошибка");
                return;
            }

            try
            {
                using (var connection = new SqliteConnection(connectionString1))
                {
                    connection.Open();

                    // SQL-запрос для вставки данных
                    string query = "INSERT INTO IncomeBox (X, Y, incMoney, incOwner, incDate) VALUES (@x, @y, @incMoney, @incOwner, @incDate)";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        // Устанавливаем значения параметров
                        command.Parameters.AddWithValue("@x", x);
                        command.Parameters.AddWithValue("@y", y);
                        command.Parameters.AddWithValue("@incMoney", 0); // Значение по умолчанию
                        command.Parameters.AddWithValue("@incOwner", ""); // Пустое значение
                        command.Parameters.AddWithValue("@incDate", ""); // Текущая дата

                        // Выполняем запрос
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Блок IncomeBox добавлен в таблицу.", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи в таблицу IncomeBox: {ex.Message}", "Ошибка");
            }
        }





        //Вставка блока OutcomeBox
        private void AddOutcomeBox_Click(object sender, RoutedEventArgs e)
        {
            // Создаём экземпляр ProjectBoxControl
            var outcomeBox = new OutcomeBox
            {
                Name = $"OutcomeBox{_blockOutcomeCounter++}"
            };

            // Получаем позицию мыши относительно Canvas
            var mousePosition = Mouse.GetPosition(Canvas2);

            // Устанавливаем позицию блока на Canvas
            Canvas.SetLeft(outcomeBox, mousePosition.X);
            Canvas.SetTop(outcomeBox, mousePosition.Y);

            // Подписываемся на удаление блока
            outcomeBox.OnDeleteRequestedOutcome += RemoveBlockOutcome;

            // Подписываемся на события мыши для перемещения
          // outcomeBox.MouseLeftButtonDown += Block_MouseLeftButtonDown;
          //  outcomeBox.MouseMove += Block_MouseMove;
           // outcomeBox.MouseLeftButtonUp += Block_MouseLeftButtonUp;

            // Добавляем блок на Canvas
            Canvas2.Children.Add(outcomeBox);
        }

        /*Начало перетаскивания
        private void Block_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _draggedElement = sender as UIElement;

            if (_draggedElement != null)
            {
                // Запоминаем начальную позицию мыши и элемента
                _startMousePosition = e.GetPosition(Canvas2);
                _startElementPosition = new Point(
                    Canvas.GetLeft(_draggedElement),
                    Canvas.GetTop(_draggedElement)
                );

                // Захватываем мышь
                _draggedElement.CaptureMouse();
            }
        }
        //Переммещение
        private void Block_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedElement != null && e.LeftButton == MouseButtonState.Pressed)
            {
                // Текущая позиция мыши
                var currentMousePosition = e.GetPosition(Canvas2);

                // Вычисляем новое положение элемента
                var newLeft = _startElementPosition.X + (currentMousePosition.X - _startMousePosition.X);
                var newTop = _startElementPosition.Y + (currentMousePosition.Y - _startMousePosition.Y);

                // Ограничиваем положение в пределах Canvas2
                newLeft = Math.Max(0, Math.Min(newLeft, Canvas2.ActualWidth - _draggedElement.RenderSize.Width));
                newTop = Math.Max(0, Math.Min(newTop, Canvas2.ActualHeight - _draggedElement.RenderSize.Height));

                // Устанавливаем новые координаты
                Canvas.SetLeft(_draggedElement, newLeft);
                Canvas.SetTop(_draggedElement, newTop);
            }
        }
        //Завершение перетаскивания
        private void Block_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_draggedElement != null)
            {
                // Освобождаем мышь
                _draggedElement.ReleaseMouseCapture();
                _draggedElement = null;
            }
        }
        */

        //Удаление ContractBlock
        private void RemoveBlock(TypicalContractBox blockToRemove)
        {
            // Удаляем блок с Canvas
            Canvas2.Children.Remove(blockToRemove);
        }

        //Удаление ProjectBlock
        private void RemoveBlockProject(TypicalProjectBox blockToRemove)
        {
            // Удаляем блок с Canvas
            Canvas2.Children.Remove(blockToRemove);
        }

        //Удаление IncomeBlock
        private void RemoveBlockIncome(IncomeBox incomeBox)
        {
            // Удаляем блок с Canvas
            Canvas2.Children.Remove(incomeBox);
        }

        //Удаление OutcomeBlock
        private void RemoveBlockOutcome(OutcomeBox blockToRemove)
        {
            // Удаляем блок с Canvas
            Canvas2.Children.Remove(blockToRemove);
        }


        //Сохранение по кнопке
        private void SaveData(object sender, RoutedEventArgs e)
        {
            // Создаем список для хранения информации о всех элементах
            var elementsData = new List<CanvasElementData>();

            // Перебираем элементы на Canvas
            foreach (UIElement child in Canvas2.Children)
            {
                if (child is IncomeBox incomeBox)
                {
                    // Сохраняем данные из IncomeBox
                    elementsData.Add(new CanvasElementData
                    {
                        ControlType = "IncomeBox", // Указываем тип элемента
                        X = Canvas.GetLeft(incomeBox), // Позиция по X
                        Y = Canvas.GetTop(incomeBox), // Позиция по Y
                        Width = incomeBox.Width, // Ширина
                        Height = incomeBox.Height, // Высота

                        // Сохраняем текст из TextBlock'ов
                        Pole1Text = incomeBox.Pole1Text,
                        Pole2Text = incomeBox.Pole2Text,
                        Pole3Text = incomeBox.Pole3Text
                    });
                }
            }

                // Указываем путь для сохранения файла
                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = System.IO.Path.Combine(folderPath, "canvasData.json");

            // Сохраняем данные в JSON
            var json = JsonSerializer.Serialize(elementsData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);

            MessageBox.Show($"Данные сохранены в файл: {filePath}");
        }

        //Загружаем по кнопке
        private void LoadData(object sender, RoutedEventArgs e)
        {
            // Путь к файлу
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = System.IO.Path.Combine(folderPath, "canvasData.json");

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл с данными не найден.");
                return;
            }

            // Читаем JSON из файла
            var json = File.ReadAllText(filePath);
            var elementsData = JsonSerializer.Deserialize<List<CanvasElementData>>(json);

            if (elementsData == null || elementsData.Count == 0)
            {
                MessageBox.Show("Данные пусты или файл поврежден.");
                return;
            }

            // Очищаем Canvas перед добавлением элементов
            Canvas2.Children.Clear();

            // Восстанавливаем элементы
            foreach(var elementData in elementsData)
    {
                if (elementData.ControlType == "IncomeBox")
                {
                    // Создаем экземпляр IncomeBox
                    var incomeBox = new IncomeBox
                    {
                        Width = elementData.Width,
                        Height = elementData.Height
                    };

                    // Восстанавливаем текст в TextBlock'ах, если данные не null
                    if (!string.IsNullOrEmpty(elementData.Pole1Text))
                    {
                        incomeBox.Pole1Text = elementData.Pole1Text; // Текст для "ibPole1"
                    }

                    if (!string.IsNullOrEmpty(elementData.Pole2Text))
                    {
                        incomeBox.Pole2Text = elementData.Pole2Text; // Текст для "ibPole2"
                    }

                    if (!string.IsNullOrEmpty(elementData.Pole3Text))
                    {
                        incomeBox.Pole3Text = elementData.Pole3Text; // Текст для "ibPole3"
                    }

                    // Подписываемся на удаление блока
                    incomeBox.OnDeleteRequestedIncome += RemoveBlockIncome;

                    // Устанавливаем положение на Canvas
                    Canvas.SetLeft(incomeBox, elementData.X);
                    Canvas.SetTop(incomeBox, elementData.Y);

                    // Добавляем на Canvas
                    Canvas2.Children.Add(incomeBox);
                }
                else
                {
                    MessageBox.Show($"Неизвестный тип элемента: {elementData.ControlType}");
                }
            }

            MessageBox.Show("Данные успешно загружены.");
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
                string databasePath = openFileDialog.FileName;
                _databasePath = databasePath;
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






