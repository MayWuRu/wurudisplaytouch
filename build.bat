@echo off
set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist %CSC% (
    echo ไม่พบ Compiler C# (csc.exe) ในเครื่อง!
    pause
    exit /b
)

echo กำลังสร้าง WuRuDisplayTouch...
%CSC% /nologo /target:winexe /out:WuRuDisplayTouch.exe /win32icon:icon.ico /resource:icon.ico,WuRuDisplayTouch.icon.ico /win32manifest:app.manifest /reference:System.Management.dll /reference:System.Windows.Forms.dll /reference:System.Drawing.dll App.cs

if %ERRORLEVEL% EQU 0 (
    echo.
    echo สร้างโปรแกรมสำเร็จ! ได้ไฟล์ WuRuDisplayTouch.exe
) else (
    echo.
    echo เกิดข้อผิดพลาดในการสร้างโปรแกรม
)
