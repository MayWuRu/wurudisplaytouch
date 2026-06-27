# WuRuDisplayTouch 🖐️🐈

**WuRuDisplayTouch** is a lightweight, automatic background utility designed specifically for Windows gaming handhelds and tablets (like the **Lenovo Legion Go / Legion Go 2**) to automatically **disable the touchscreen** when connected to an external monitor in "Second screen only" mode. 

**WuRuDisplayTouch** เป็นโปรแกรมขนาดเล็กที่ทำงานเบื้องหลัง สร้างขึ้นมาเพื่อเครื่องเล่นเกมพกพาและแท็บเล็ตระบบ Windows (เช่น **Lenovo Legion Go / Legion Go 2**) โดยเฉพาะ โปรแกรมจะทำหน้าที่ **ปิดระบบสัมผัสหน้าจอ (Touchscreen)** ให้อัตโนมัติเมื่อมีการต่อจอนอกและเลือกโหมด "Second screen only" (ใช้จอที่สองเท่านั้น)

---

## 🌟 Why this exists / ทำไมถึงต้องมีโปรแกรมนี้?

### 🇬🇧 English
If you own a Lenovo Legion Go 2 and often connect it to an external monitor to work or play games, you might set your display to "Second screen only". However, Windows often leaves the built-in touchscreen active even when the screen is black. 
If you have a cat that likes to sleep on your desk and accidentally steps on your Legion Go's screen, it will cause "ghost touches" or interfere with your mouse cursor! 
**WuRuDisplayTouch** solves this by seamlessly disabling the touchscreen device automatically when you project to an external monitor, and enabling it back when you disconnect. It does **not** disable the built-in touchpad, so you always have a fallback.

**Keywords for search engines:** *Disable touchscreen Legion Go 2, Auto turn off touch screen Windows 11 external monitor, Legion Go ghost touch second screen, disable HID-compliant touch screen, cat stepping on tablet screen fix, Legion Go external display issues.*

### 🇹🇭 ภาษาไทย
หากคุณใช้ Lenovo Legion Go 2 และต่อจอนอก (External Monitor) เพื่อทำงานหรือเล่นเกม โดยตั้งค่าจอเป็น "Second screen only" (จอที่สองเท่านั้น) หน้าจอของเครื่องจะดับลง แต่ระบบสัมผัส (Touchscreen) มักจะยังคงทำงานอยู่
ปัญหาจะเกิดเมื่อคุณมี **"น้องแมว"** ที่ชอบมานอนเล่นบนโต๊ะทำงาน แล้วเผลอเอาเท้าไปเหยียบหรือนอนทับจอ Legion Go ของคุณ ทำให้เมาส์ขยับเอง หรือเกิดการรบกวนการทำงาน (Ghost touch)
**WuRuDisplayTouch** แก้ปัญหานี้โดยการตรวจจับสถานะหน้าจอ และสั่งปิด Touchscreen ให้ทันทีที่จอเครื่องไม่ได้ถูกใช้งาน และจะเปิดกลับมาอัตโนมัติเมื่อคุณถอดสายจอออก โดยตัวโปรแกรมจะ**ไม่ไปยุ่งกับ Touchpad (แป้นสัมผัส)** ของเครื่อง ทำให้คุณยังมีเมาส์สำรองไว้ใช้งานเสมอ

**คีย์เวิร์ดสำหรับค้นหา:** *วิธีปิดหน้าจอสัมผัส Legion Go, โปรแกรมปิดทัชสกรีนอัตโนมัติเวลาต่อจอนอก, แมวเหยียบจอเมาส์ขยับ, ปิด Touch screen Windows 11, Legion Go 2 ต่อจอนอกปิดทัชไม่ได้, วิธีแก้จอสัมผัสลั่น*

---

## 🚀 Features / คุณสมบัติ

- **Auto-Detect Display Topology:** Automatically detects if the internal display is attached to the Windows desktop. / *ระบบตรวจจับหน้าจออัตโนมัติ รู้ได้ทันทีว่าจอหลักทำงานอยู่หรือไม่*
- **Targeted Disable:** Disables only the "HID-compliant touch screen" and leaves the Trackpad/Mouse fully functional. / *เจาะจงปิดเฉพาะทัชสกรีน โดยไม่กระทบการทำงานของทัชแพด*
- **System Tray Integration:** Runs quietly in the system tray with minimal resource usage. / *ทำงานเงียบๆ ซ่อนอยู่ใน System Tray ไม่กินสเปคเครื่อง*
- **Bypass UAC at Startup:** Can be set to run automatically on Windows startup using a Scheduled Task, bypassing the annoying Administrator prompt. / *สามารถตั้งค่าให้เปิดพร้อมเปิดเครื่องได้ทันที โดยไม่มีหน้าต่างขอสิทธิ์ Admin มากวนใจ*

## 🛠 How to Use / วิธีการใช้งาน

1. Download or compile the `WuRuDisplayTouch.exe`.
2. Right-click and choose **"Run as administrator"** (Run as admin is required to enable/disable hardware devices).
3. The app icon will appear in the system tray (bottom right corner). 
4. Right click the tray icon and select **"เปิดพร้อม Windows (Run on Startup)"** to ensure it starts automatically with high privileges on every boot.

---
1. ดาวน์โหลดหรือคอมไพล์ไฟล์ `WuRuDisplayTouch.exe`
2. คลิกขวาที่ไฟล์ เลือก **"Run as administrator"** (จำเป็นต้องใช้สิทธิ์ผู้ดูแลระบบเพื่อสั่งปิด/เปิดอุปกรณ์ฮาร์ดแวร์)
3. ไอคอนโปรแกรมจะปรากฏที่ System Tray (มุมขวาล่างของจอ)
4. คลิกขวาที่ไอคอน แล้วเลือก **"เปิดพร้อม Windows (Run on Startup)"** เพื่อให้โปรแกรมทำงานอัตโนมัติทุกครั้งที่เปิดเครื่องโดยไม่ต้องมากดเอง
