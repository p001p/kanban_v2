using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kanban_v2
{
    public class CanvasElementData
    {
        public string? ControlType { get; set; } // Тип контрола, например "GroupBox"
        public double X { get; set; } // Позиция по оси X
        public double Y { get; set; } // Позиция по оси Y
        public double Width { get; set; } // Ширина элемента
        public double Height { get; set; } // Высота элемента
        public string? Data { get; set; } // Данные, сохраненные в GroupBox

        // Новые поля для данных TextBlock'ов
        public string? Pole1Text { get; set; }
        public string? Pole2Text { get; set; }
        public string? Pole3Text { get; set; }
    }
}
