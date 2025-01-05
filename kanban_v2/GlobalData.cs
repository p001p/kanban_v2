using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kanban_v2
{
    public static class GlobalData
    {
        public static string? globalDatabasePath { get; set; } //Ссылка на базу данных при открытии.
        public static bool isConnected { get; set; } //Если базу загрузили, то тут есть bool'ьк
    }
}
