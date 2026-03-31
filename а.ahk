#NoEnv
#SingleInstance Force
SendMode Input
SetBatchLines -1

; --- Настройки таймингов (изменяй их, если не работает) ---
shot_hold := 100      ; Сколько миллисекунд удерживается ЛКМ
c_delay := 20        ; Пауза перед нажатием C
c_hold := 20         ; Сколько удерживается клавиша C
next_shot_delay := 60 ; Пауза ПЕРЕД следующим выстрелом (самый важный параметр для спама)

Enabled := false

F3::
Enabled := !Enabled
SoundBeep, % (Enabled ? 750 : 400), 200
return

~LButton::
if (!Enabled || !GetKeyState("RButton", "P"))
    return

Loop
{
    ; Проверка: зажаты ли кнопки. Если отпустил — стоп.
    if !GetKeyState("LButton", "P") || !GetKeyState("RButton", "P")
        break

    ; 1. ВЫСТРЕЛ
    Click, Down
    Sleep %shot_hold%
    Click, Up

    ; 2. ПАУЗА ДЛЯ ВЫЛЕТА ПУЛИ
    Sleep %c_delay%

    ; 3. СБИВ (C)
    Send, {c down}
    Sleep %c_hold%
    Send, {c up}

    ; 4. ПАУЗА ПЕРЕД СЛЕДУЮЩИМ ЦИКЛОМ (Чтобы не было "одной пули")
    Sleep %next_shot_delay%
    
    ; Небольшой рандом для обхода античита
    Random, rand, 5, 15
    Sleep %rand%
}
return

End::ExitApp