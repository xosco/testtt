; --- Настройки ---
Sens := 2.0       ; Твоя чувствительность в игре
Modifier := 1.0   ; Множитель (если нужно ускорить/замедлить движение)

; Массив смещений (X, Y) для каждого выстрела (всего 30)
; Отрицательный Y - движение вниз, положительный X - вправо
global recoil_pattern := [ [0, 4], [0, 8], [0, 10], [-1, 12], [-2, 12], [-3, 10], [-5, 8], [-7, 5], [-8, 2], [-5, -1], [0, -2], [4, -1], [8, 0], [10, 1], [10, 2], [8, 1], [5, 0], [2, 0], [-2, 0], [-5, 0], [-8, 0], [-10, 0], [-8, 1], [-5, 1], [-2, 1] ]

IsActive := false

; Клавиша F1 для включения/выключения скрипта
F1:: 
    IsActive := !IsActive
    SoundBeep, % (IsActive ? 750 : 500), 200
return

~LButton::
    if (!IsActive)
        return

    Loop, % recoil_pattern.MaxIndex()
    {
        ; Проверяем, зажата ли все еще левая кнопка мыши
        if !GetKeyState("LButton", "P")
            break

        ; Получаем значения из массива
        x := recoil_pattern[A_Index][1] * Modifier
        y := recoil_pattern[A_Index][2] * Modifier

        ; Двигаем мышь относительно текущей позиции
        ; dllcall mouse_event 1 - это относительное движение (MOUSEEVENTF_MOVE)
        DllCall("mouse_event", uint, 1, int, x, int, y, uint, 0, int, 0)

        ; Задержка между выстрелами АК-47 (примерно 100мс для 600 RPM)
        Sleep, 99 
    }
return

; Клавиша для экстренного завершения скрипта
End::ExitApp