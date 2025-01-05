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

namespace kanban_v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private UIElement? _draggedElement; // Текущий перетаскиваемый элемент
        //private Point _startMousePosition; // Начальная позиция мыши
       // private Point _startElementPosition; // Начальная позиция элемента
        private int _blockProjectCounter = 0;
        private int _blockContractCounter = 0;
        private int _blockIncomeCounter = 0;
        private int _blockOutcomeCounter = 0;

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

        //Вставка блока IncomeBox
        private void AddIncomeBox_Click(object sender, RoutedEventArgs e)
        {
            // Создаём экземпляр ProjectBoxControl
            var incomeBox = new IncomeBox
            {
                Name = $"IncomeBox{_blockIncomeCounter++}"

            };
            // Подписываемся на удаление блока
            incomeBox.OnDeleteRequestedIncome += RemoveBlockIncome;

            // Получаем позицию мыши относительно Canvas
            var mousePosition = Mouse.GetPosition(Canvas2);

            // Устанавливаем позицию блока на Canvas
            Canvas.SetLeft(incomeBox, mousePosition.X);
            Canvas.SetTop(incomeBox, mousePosition.Y);

            

            // Подписываемся на события мыши для перемещения
            //incomeBox.MouseLeftButtonDown += Block_MouseLeftButtonDown;
            //incomeBox.MouseMove += Block_MouseMove;
            //incomeBox.MouseLeftButtonUp += Block_MouseLeftButtonUp;

            // Добавляем блок на Canvas
            Canvas2.Children.Add(incomeBox);
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

    }

}




    

