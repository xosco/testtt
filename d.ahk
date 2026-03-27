#Requires AutoHotkey v2.0
#SingleInstance Force
ListLines 0
ProcessSetPriority "High"

; --- Настройки ---
global modifier := 1  ; Тонкая настройка силы (для сенсы 4.0 начни с 0.4 - 0.5)
global isEnabled := false
global smoothingSteps := 10 ; На сколько микро-шагов разбивать один выстрел (чем больше, тем плавнее)

; Паттерн AK-47 (X, Y)
; Значения подобраны с учетом того, что они будут дробиться
AK47_Pattern := [
    [0, 9], [0, 16], [0, 19], [0, 22], [-2, 22], 
    [-4, 19], [-7, 16], [-9, 13], [-10, 8], [-7, 4], 
    [-3, 2], [3, 2], [8, 2], [11, 0], [13, 0], 
    [10, 2], [7, 3], [2, 3], [-3, 3], [-8, 3], 
    [-11, 2], [-13, 2], [-10, 2], [-5, 2], [0, 2], 
    [3, 2], [6, 2], [8, 2], [5, 2], [2, 2]
]

F1:: {
    global isEnabled := !isEnabled
    SoundBeep(isEnabled ? 750 : 500, 200)
}

End:: ExitApp()

#HotIf isEnabled
~LButton:: {
    ; AK-47 стреляет примерно каждые 100 мс
    loop AK47_Pattern.Length {
        if !GetKeyState("LButton", "P")
            break
        
        targetX := AK47_Pattern[A_Index][1] * modifier
        targetY := AK47_Pattern[A_Index][2] * modifier
        
        ; Плавное перемещение внутри одного выстрела
        SmoothMove(targetX, targetY, 100) 
    }
}
#HotIf

; Функция для плавного движения
SmoothMove(totalX, totalY, duration) {
    global smoothingSteps
    stepX := totalX / smoothingSteps
    stepY := totalY / smoothingSteps
    sleepTime := duration / smoothingSteps
    
    ; Накопители для дробных пикселей (т.к. мышь движется только по целым числам)
    static accX := 0.0
    static accY := 0.0
    
    loop smoothingSteps {
        if !GetKeyState("LButton", "P")
            break
            
        accX += stepX
        accY += stepY
        
        moveX := Round(accX)
        moveY := Round(accY)
        
        if (moveX != 0 || moveY != 0) {
            DllCall("mouse_event", "UInt", 0x0001, "Int", moveX, "Int", moveY, "UInt", 0, "UPtr", 0)
            accX -= moveX
            accY -= moveY
        }
        
        DllCall("Sleep", "UInt", sleepTime) ; Более точный сон через DLL
    }
}