using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UIElement? _draggedElement; // Текущий перетаскиваемый элемент
        private Point _startMousePosition; // Начальная позиция мыши
        private Point _startElementPosition; // Начальная позиция элемента
        private int _blockProjectCounter = 0;
        private int _blockContractCounter = 0;

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
                Name = $"ContractBox{_blockProjectCounter++}"
            }; 

            // Получаем позицию мыши относительно Canvas
            var mousePosition = Mouse.GetPosition(Canvas2);

            // Устанавливаем позицию блока на Canvas
            Canvas.SetLeft(projectBox, mousePosition.X);
            Canvas.SetTop(projectBox, mousePosition.Y);


            // Подписываемся на удаление блока
            projectBox.OnDeleteRequestedProject += RemoveBlockProject;

            // Подписываемся на события мыши для перемещения
            projectBox.MouseLeftButtonDown += Block_MouseLeftButtonDown;
            projectBox.MouseMove += Block_MouseMove;
            projectBox.MouseLeftButtonUp += Block_MouseLeftButtonUp;

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
            contractBox.MouseLeftButtonDown += Block_MouseLeftButtonDown;
            contractBox.MouseMove += Block_MouseMove;
            contractBox.MouseLeftButtonUp += Block_MouseLeftButtonUp;

            // Добавляем блок на Canvas
            Canvas2.Children.Add(contractBox);
        }

        //Начало перетаскивания
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
    }

        






}
