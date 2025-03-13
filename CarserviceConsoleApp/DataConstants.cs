using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarserviceConsoleApp
{
    public static class DataConstants
    {
        public static readonly Dictionary<string, string[]> ServiceToParts = new()
        {
            { "Замена масла", new[] { "Масляный фильтр" } },
            { "Замена тормозных колодок", new[] { "Тормозные колодки" } },
            { "Ремонт двигателя", new[] { "Свечи зажигания", "Топливный фильтр" } },
            { "Ремонт ходовой части", new[] { "Амортизаторы", "Шаровые опоры", "Подшипники ступицы" } },
            { "Диагностика автомобиля", new[] { "Датчик давления масла", "Лямбда-зонд" } },
            { "Шиномонтаж", new[] { "Тормозные диски", "Тормозные колодки" } },
            { "Замена воздушного фильтра", new[] { "Воздушный фильтр" } },
            { "Ремонт электроники", new[] { "Датчик давления масла" } },
            { "Покраска кузова", new[] { "Лобовое стекло" } },
            { "Ремонт топливной системы", new[] { "Топливный фильтр" } },
            { "Замена свечей зажигания", new[] { "Свечи зажигания" } },
            { "Ремонт коробки передач", new[] { "Выжимной подшипник", "Колодки сцепления" } },
            { "Установка сигнализации", new[] { "Аккумулятор" } },
            { "Ремонт кондиционера", new[] { "Насос гидроусилителя" } },
            { "Замена лобового стекла", new[] { "Лобовое стекло" } },
            { "Чистка радиатора", new[] { "Радиатор охлаждения" } },
            { "Замена ремня ГРМ", new[] { "Ремень ГРМ" } },
            { "Ремонт подвески", new[] { "Амортизаторы", "Стойки стабилизатора" } },
            { "Замена аккумулятора", new[] { "Аккумулятор" } },
            { "Техническое обслуживание", new[] { "Масляный фильтр", "Воздушный фильтр", "Свечи зажигания" } }
        };

        public static readonly Dictionary<string, (int Min, int Max)> PartPrices = new()
        {
            { "Тормозные колодки", (1000, 3000) },
            { "Масляный фильтр", (200, 800) },
            { "Воздушный фильтр", (300, 1000) },
            { "Свечи зажигания", (500, 2000) },
            { "Аккумулятор", (4000, 15000) },
            { "Ремень ГРМ", (1500, 5000) },
            { "Топливный фильтр", (400, 1200) },
            { "Амортизаторы", (2000, 8000) },
            { "Стойки стабилизатора", (1000, 4000) },
            { "Радиатор охлаждения", (3000, 12000) },
            { "Термостат", (800, 3000) },
            { "Насос гидроусилителя", (2500, 10000) },
            { "Шаровые опоры", (1500, 6000) },
            { "Рулевые тяги", (1000, 5000) },
            { "Тормозные диски", (2000, 8000) },
            { "Подшипники ступицы", (1000, 4000) },
            { "Колодки сцепления", (1500, 6000) },
            { "Выжимной подшипник", (2000, 8000) },
            { "Лямбда-зонд", (1500, 7000) },
            { "Датчик давления масла", (500, 2500) },
            { "Лобовое стекло", (5000, 20000) }
        };

        public static readonly Dictionary<string, string[]> carData = new() 
        {
            { "Toyota", new[] { "Corolla", "Camry", "RAV4", "Land Cruiser", "Prius", "Highlander", "Yaris", "Tacoma", "Sienna", "Tundra" } },
            { "Porsche", new[] { "911", "Cayenne", "Panamera", "Macan", "Boxster", "Taycan", "718 Cayman" } },
            { "Volvo", new[] { "XC90", "XC60", "S60", "V90", "S90", "XC40", "V60", "C40" } },
            { "BMW", new[] { "X5", "3 Series", "5 Series", "7 Series", "X3", "X7", "M3", "M5", "Z4", "iX" } },
            { "Mercedes-Benz", new[] { "C-Class", "E-Class", "S-Class", "GLC", "GLE", "GLA", "G-Class", "CLA", "A-Class", "EQS" } },
            { "Ford", new[] { "F-150", "Mustang", "Explorer", "Escape", "Focus", "Fiesta", "Bronco", "Ranger", "Edge", "Transit" } },
            { "Chevrolet", new[] { "Silverado", "Equinox", "Malibu", "Tahoe", "Suburban", "Corvette", "Camaro", "Trailblazer", "Spark", "Blazer" } },
            { "Honda", new[] { "Civic", "Accord", "CR-V", "Pilot", "Odyssey", "HR-V", "Ridgeline", "Fit", "Passport", "Insight" } },
            { "Nissan", new[] { "Altima", "Maxima", "Sentra", "Rogue", "Pathfinder", "Murano", "Titan", "Frontier", "Leaf", "GT-R" } },
            { "Hyundai", new[] { "Elantra", "Sonata", "Tucson", "Santa Fe", "Kona", "Palisade", "Venue", "Ioniq", "Nexo", "Veloster" } },
            { "Audi", new[] { "A3", "A4", "A6", "A8", "Q3", "Q5", "Q7", "Q8", "TT", "R8" } },
            { "Volkswagen", new[] { "Golf", "Passat", "Jetta", "Tiguan", "Atlas", "Arteon", "Beetle", "ID.4", "Polo", "Up!" } },
            { "Lexus", new[] { "ES", "IS", "RX", "NX", "GX", "LX", "UX", "LC", "LS", "LM" } },
            { "Subaru", new[] { "Outback", "Forester", "Impreza", "Legacy", "Crosstrek", "WRX", "BRZ", "Ascent", "Levorg", "Exiga" } },
            { "Mazda", new[] { "CX-5", "CX-9", "Mazda3", "Mazda6", "MX-5 Miata", "CX-30", "CX-3", "RX-7", "RX-8", "BT-50" } },
            { "Kia", new[] { "Optima", "Sorento", "Sportage", "Soul", "Telluride", "Stinger", "Rio", "Niro", "Carnival", "EV6" } },
        };
    }
}
