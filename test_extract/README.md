# WuRuDisplayTouch 🖐️🐈 & 🖥️✨ (v2.0)

*(For Thai version, please scroll down / สำหรับภาษาไทยกรุณาเลื่อนลงด้านล่าง)*

**WuRuDisplayTouch** is a lightweight, automatic background utility originally designed to prevent "ghost touches" from cats touching Windows gaming handhelds (like the **Lenovo Legion Go / Legion Go 2**) when connected to an external monitor. It has now evolved to include **Global Display Enforcer**, a powerful feature that automatically locks and applies your preferred resolution and refresh rate specifically for each external monitor you connect. 

---

## 🇬🇧 English Documentation

### 🌟 Why this exists?
If you own a Lenovo Legion Go 2 and often connect it to an external monitor, you face two common issues:
1. **Ghost Touches:** In "Second screen only" mode, the built-in touchscreen remains active. If your cat touches the screen, it ruins your gameplay/work. (WuRuDisplayTouch automatically disables the touchscreen when the internal display is not in use).
2. **Messy Display Scaling:** When connecting to an external monitor or changing settings via the device's Quick Settings, the external monitor's scale often becomes messy or is pulled incorrectly. (WuRuDisplayTouch allows you to "lock" the resolution and Hz for each individual monitor. Whenever you plug in a monitor, the program remembers and applies the perfectly correct settings every time!)

### 🚀 Features
- **Bilingual Support:** Switch instantly between English and Thai directly in the Settings.
- **Smart Touchscreen Auto Mode:** Automatically detects if the internal display is active and toggles the touchscreen device accordingly.
- **Global Display Enforcer:** Lock the perfect Resolution and Refresh rate for your external monitors.
- **Hardware-Based Monitor Memory:** Remembers settings per-monitor based on their unique Hardware ID. Connect your office monitor during the day and your home monitor at night—the program seamlessly switches to the correct saved settings for each.
- **Centralized Settings UI:** Double-click the tray icon to easily configure everything.
- **Run on Startup (Bypass UAC):** Can be set to run automatically on Windows startup using a Scheduled Task, bypassing the annoying Administrator prompt.

### 🛠 How to Use
1. Download the latest release `.zip` and extract it.
2. Right-click on `WuRuDisplayTouch.exe` and choose **"Run as administrator"**.
3. The app icon will appear in the system tray (bottom right corner). Double-click the icon to open the Settings window.
4. Choose your preferred language (English / ภาษาไทย) from the dropdown at the top.
5. Check **"Run on Startup"** so you never have to manually run it again.
6. Select your connected monitor from the dropdown and configure your preferred locked resolution/Hz.

---
---

## 🇹🇭 คู่มือภาษาไทย

**WuRuDisplayTouch** เป็นโปรแกรมขนาดเล็กที่ทำงานเบื้องหลัง เริ่มแรกถูกสร้างขึ้นเพื่อแก้ปัญหา "น้องแมว" ไปโดนจอทัชสกรีนบนเครื่องเล่นเกมพกพา (เช่น **Lenovo Legion Go / Legion Go 2**) เวลาต่อจอนอก ปัจจุบันได้รับการพัฒนาเพิ่มฟีเจอร์ **Global Display Enforcer** ซึ่งเป็นระบบช่วยล็อคและปรับความละเอียด (Resolution) รวมไปถึงอัตรารีเฟรช (Refresh Rate) ของจอนอกแต่ละตัวให้ถูกต้องแบบอัตโนมัติ

### 🌟 ทำไมถึงต้องมีโปรแกรมนี้?
หากใช้ Lenovo Legion Go 2 และต่อจอนอก (External Monitor) เพื่อทำงานหรือเล่นเกม มักจะเจอปัญหา 2 อย่างนี้:
1. **ปัญหาทัชสกรีนลั่น:** เมื่อตั้งจอเป็น "Second screen only" จอของเครื่องจะดับลง แต่ระบบสัมผัส (Touchscreen) มักจะยังคงทำงานอยู่ หากน้องแมวมาโดนจอ เมาส์ก็จะขยับมั่วไปหมด (WuRuDisplayTouch ช่วยปิดการทำงานของ Touchscreen ให้ทันทีเมื่อจอหลักไม่ได้แสดงผล)
2. **ปัญหาสเกลจอเพี้ยนเมื่อต่อจอนอก:** เวลาต่อจอนอกหรือเวลาเปลี่ยนการตั้งค่าผ่านเมนูด่วน (Quick Settings) ของตัวเครื่อง สเกลของจอนอกมักจะเพี้ยนหรือถูกดึงไม่ถูกต้อง (WuRuDisplayTouch ช่วยให้สามารถ "ล็อค" ค่าความละเอียดและ Hz ของจอแต่ละตัวได้ เมื่อเสียบจอ โปรแกรมจะจดจำและปรับค่าให้ถูกต้องเป๊ะๆ ทุกครั้ง)

### 🚀 คุณสมบัติ
- **รองรับ 2 ภาษา:** สลับภาษา (ไทย/อังกฤษ) ได้ทันทีในหน้าตั้งค่า
- **ระบบเปิด/ปิด Touchscreen อัตโนมัติ:** ป้องกันแมวแตะโดนจอ โดยไม่กระทบการทำงานของทัชแพด/เมาส์
- **ระบบปรับตั้งค่าหน้าจออัตโนมัติ (Global Display Enforcer):** ล็อคความละเอียดหน้าจอและ Hz สำหรับจอนอก
- **จดจำการตั้งค่าแยกตามจอ:** จำค่าแยกตามรุ่น (Device ID) เสียบจอที่ทำงานตอนเช้า เสียบจอที่บ้านตอนกลางคืน โปรแกรมก็รู้และเปลี่ยนค่าให้ตรงกับจอนั้นๆ โดยอัตโนมัติ
- **รวมการตั้งค่าทุกอย่างไว้ในหน้าต่างเดียว:** เปิดง่ายๆ แค่ดับเบิลคลิกไอคอน
- **เปิดโปรแกรมพร้อม Windows:** สามารถตั้งให้เปิดพร้อม Windows ได้ทันทีโดยไม่ติดหน้าต่างถามสิทธิ์ Admin (UAC)

### 🛠 วิธีการใช้งาน
1. ดาวน์โหลดไฟล์ `.zip` จากหน้า Release แล้วแตกไฟล์
2. คลิกขวาที่ไฟล์ `WuRuDisplayTouch.exe` เลือก **"Run as administrator"** (โปรแกรมต้องใช้สิทธิ์ในการปิด/เปิดฮาร์ดแวร์และปรับตั้งค่าระบบ)
3. ดับเบิลคลิกไอคอนรูปโล่ที่มุมขวาล่างเพื่อเปิดหน้าต่างตั้งค่า
4. เลือกภาษาที่ต้องการจากกล่องตัวเลือกด้านบนสุด
5. แนะนำให้ติ๊กช่อง **"เปิดโปรแกรมพร้อม Windows"** ไว้
6. เลือกจอนอกที่ต้องการตั้งค่า แล้วเลือกล็อคความละเอียดและ Refresh Rate ได้ตามต้องการ
