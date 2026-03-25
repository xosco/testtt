#NoEnv
#Persistent
#SingleInstance Force
#MaxHotkeysPerInterval 200
SetBatchLines, -1
SetKeyDelay, -1, -1
SendMode Event
Process, Priority,, High

; =============================================
; Максимально точный Bhop
; Space = спам WheelDown с идеальным таймингом
; F6 — вкл/выкл
; =============================================

toggle := true
tickrate := 15.625  ; CS2 работает на 64 тик (1000/64 ≈ 15.625 мс)

F6::
    toggle := !toggle
    ToolTip % (toggle ? "Bhop ON" : "Bhop OFF")
    SetTimer, ClearTip, 1000
    return

ClearTip:
    ToolTip
    SetTimer, ClearTip, Off
    return

$Space::
    if (!toggle)
    {
        Send {Space}
        return
    }

    ; Первый прыжок мгновенно
    Send {WheelDown}

    while GetKeyState("Space", "P")
    {
        ; Два скролла за тик — один попадёт точно на момент приземления
        Send {WheelDown}
        DllCall("Sleep", "UInt", 6)
        Send {WheelDown}
        DllCall("Sleep", "UInt", 10)
    }
    return