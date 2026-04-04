using System;
using System.Runtime.InteropServices;
using System.Threading;

class BhopApp
{
    // --- Импорт WinAPI ---
    [DllImport("user32.dll", SetLastError = true)]
    static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);

    // --- Структуры для SendInput ---
    [StructLayout(LayoutKind.Sequential)]
    struct INPUT
    {
        public uint type;
        public InputUnion u;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
        [FieldOffset(0)] public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MOUSEINPUT { public int dx; public int dy; public uint mouseData; public uint dwFlags; public uint time; public IntPtr dwExtraInfo; }
    [StructLayout(LayoutKind.Sequential)] struct HARDWAREINPUT { public uint uMsg; public ushort wParamL; public ushort wParamH; }

    // Константы
    const int INPUT_KEYBOARD = 1;
    const uint KEYEVENTF_SCANCODE = 0x0008;
    const uint KEYEVENTF_KEYUP = 0x0002;
    const int VK_SPACE = 0x20;
    const ushort SCAN_SPACE = 0x39;

    // Настройки
    static bool isRunning = true;
    static int jumpDelay = 20;
    static int targetKey = VK_SPACE;

    static void Main(string[] args)
    {
        Console.WriteLine("=== C# Bhop Console ===");
        Console.WriteLine("Hold SPACE to hop. Press END to exit.");

        // Запуск потока логики
        Thread bhopThread = new Thread(BhopLoop);
        bhopThread.IsBackground = true;
        bhopThread.Start();

        // Ожидание завершения (нажать End)
        while (isRunning)
        {
            if ((GetAsyncKeyState(0x23) & 0x8000) != 0) // VK_END
            {
                isRunning = false;
            }
            Thread.Sleep(100);
        }

        Console.WriteLine("Exiting...");
    }

    static void BhopLoop()
    {
        while (isRunning)
        {
            // Проверка зажатия клавиши (Space)
            if ((GetAsyncKeyState(targetKey) & 0x8000) != 0)
            {
                SendPerfectJump();
                Thread.Sleep(jumpDelay);
            }
            else
            {
                Thread.Sleep(10);
            }
        }
    }

    static void SendPerfectJump()
    {
        INPUT[] inputs = new INPUT[2];

        // 1. ОТПУСТИТЬ ПРОБЕЛ
        inputs[0].type = INPUT_KEYBOARD;
        inputs[0].u.ki.wScan = SCAN_SPACE;
        inputs[0].u.ki.dwFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP;

        // 2. НАЖАТЬ ПРОБЕЛ
        inputs[1].type = INPUT_KEYBOARD;
        inputs[1].u.ki.wScan = SCAN_SPACE;
        inputs[1].u.ki.dwFlags = KEYEVENTF_SCANCODE;

        SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT)));
    }
}
