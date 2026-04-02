using System;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    // Импорт функций WinAPI
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport("kernel32.dll")]
    static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

    [DllImport("kernel32.dll")]
    static extern bool QueryPerformanceFrequency(out long lpFrequency);

    [StructLayout(LayoutKind.Sequential)]
    struct INPUT {
        public uint type;
        public MOUSEINPUT mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MOUSEINPUT {
        public int dx; public int dy; public uint mouseData;
        public uint dwFlags; public uint time; public IntPtr dwExtraInfo;
    }

    const uint INPUT_MOUSE = 0;
    const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    const uint MOUSEEVENTF_LEFTUP = 0x0004;
    const int VK_F6 = 0x75; // F6 для сброса настроек

    static bool g_running = true;
    static bool g_active = false;
    static bool g_configuring = true;
    static int g_cps = 100;
    static int g_toggleKey = 0;

    static void Main(string[] args)
    {
        Console.Title = "Ultra High-Speed Clicker";

        // Запускаем поток кликера
        Thread clickThread = new Thread(ClickerThread);
        clickThread.Priority = ThreadPriority.Highest;
        clickThread.IsBackground = true;
        clickThread.Start();

        bool keyWasPressed = false; // Переменная теперь объявлена правильно

        while (g_running)
        {
            if (g_configuring)
            {
                ConfigureSettings();
                keyWasPressed = false; // Сброс состояния клавиши после настройки
            }

            // Проверка F6 (перенастройка)
            if ((GetAsyncKeyState(VK_F6) & 0x8000) != 0)
            {
                g_active = false;
                g_configuring = true;
                Thread.Sleep(500); 
                continue;
            }

            // Проверка выбранной клавиши активации
            if (g_toggleKey != 0)
            {
                bool keyIsPressed = (GetAsyncKeyState(g_toggleKey) & 0x8000) != 0;
                
                if (keyIsPressed && !keyWasPressed)
                {
                    g_active = !g_active;
                    Console.ForegroundColor = g_active ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine(g_active ? "[СТАТУС: РАБОТАЕТ]" : "[СТАТУС: ПАУЗА]");
                    Console.ResetColor();
                }
                keyWasPressed = keyIsPressed;
            }

            Thread.Sleep(10); 
        }
    }

    static void ConfigureSettings()
    {
        g_active = false;
        Console.Clear();
        Console.WriteLine("=== НАСТРОЙКИ КЛИКЕРА ===");
        Console.WriteLine("\n1. Нажмите ЛЮБУЮ клавишу на клавиатуре, чтобы сделать её кнопкой ВКЛ/ВЫКЛ...");
        
        // Очищаем буфер нажатий перед выбором
        Thread.Sleep(500);

        bool found = false;
        while (!found)
        {
            for (int i = 8; i < 190; i++) 
            {
                if (i == VK_F6) continue; 
                if ((GetAsyncKeyState(i) & 0x8000) != 0)
                {
                    g_toggleKey = i;
                    Console.WriteLine("Клавиша выбрана! (Код: " + i + ")");
                    found = true;
                    break;
                }
            }
            Thread.Sleep(10);
        }

        Thread.Sleep(500);
        Console.Write("\n2. Введите желаемое количество CPS (например, 100): ");
        string input = Console.ReadLine();
        if (!int.TryParse(input, out g_cps) || g_cps <= 0) g_cps = 100;

        Console.Clear();
        Console.WriteLine("=== КЛИКЕР ГОТОВ ===");
        Console.WriteLine("Бинд: [Код " + g_toggleKey + "]");
        Console.WriteLine("Скорость: " + g_cps + " CPS");
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("Нажмите F6, если захотите изменить настройки.");
        Console.WriteLine("-------------------------------------------");
        
        g_configuring = false;
    }

    static void ClickerThread()
    {
        long frequency;
        QueryPerformanceFrequency(out frequency);

        INPUT[] inputs = new INPUT[2];
        inputs[0].type = INPUT_MOUSE;
        inputs[0].mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
        inputs[1].type = INPUT_MOUSE;
        inputs[1].mi.dwFlags = MOUSEEVENTF_LEFTUP;

        while (g_running)
        {
            if (g_active && !g_configuring)
            {
                long t1, t2;
                double interval = 1.0 / g_cps;
                QueryPerformanceCounter(out t1);

                SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT)));

                // Прецизионное ожидание (как в твоем C++ коде)
                do
                {
                    QueryPerformanceCounter(out t2);
                } while ((double)(t2 - t1) / frequency < interval);
            }
            else
            {
                Thread.Sleep(10);
            }
        }
    }
}