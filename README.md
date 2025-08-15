# 🛡️ Kingdom Defense

![Unity Version](https://img.shields.io/badge/Unity-6000.1.15f1-000000.svg?style=for-the-badge&logo=unity)
![UI System](https://img.shields.io/badge/UI_Toolkit-Modern_UI-2E8B57.svg?style=for-the-badge)

**Kingdom Defense** là một dự án game thủ thành (Tower Defense) 2D kết hợp các yếu tố nhập vai (RPG) được xây dựng trên nền tảng Unity. Game mang đến hệ thống phòng thủ chiến thuật, quản lý tài nguyên và lưu trữ dữ liệu đám mây qua nền tảng PlayFab.

---

## ✨ Tính Năng Nổi Bật (Features)

### 🛡️ Hệ Thống Thủ Thành (Core Defense Mechanics)
*   **Chiến thuật đặt tháp (Tower Placement):** Mô phỏng lối chơi thủ thành cổ điển, người chơi phòng thủ "Safe Zone" khỏi các đợt tấn công ngang tàng của kẻ địch.
*   **Hệ thống đợt lính (Wave Spawner):** Quản lý độ khó tăng dần qua từng đợt quái với cơ chế xuất hiện và tần suất cụ thể.
*   **Mở khóa Tháp (Reward Cards):** Người chơi sẽ nhận được Tháp phòng thủ mới sau khi vượt qua phần chơi chiến dịch định sẵn.

### 💰 Kinh Tế & Vật Phẩm (Economy & Loot)
*   **Tiền tệ trong trận (Economy Manager):** Quản lý tiền rơi ra khi hạ gục kẻ thù để xây dựng và củng cố cấu trúc phòng thủ.
*   **Khí tài đánh rơi (Loot Drop):** Kẻ địch sau khi bị tiêu diệt ngẫu nhiên văng ra chiến lợi phẩm giúp duy trì nhịp độ trận đấu.

### ☁️ Lưu Trữ Đám Mây & Tài Khoản (Cloud & Account System)
*   **Tích hợp PlayFab:** Hỗ trợ đăng nhập, đăng ký và quản lý phiên bản lưu trực tiếp trên hệ thống máy chủ PlayFab hiện đại.
*   **Cloud Save:** Lưu lại cấp độ người chơi, thẻ tháp đã thu thập và các tiến trình đạt được một cách an toàn nhất.

### 🎒 Quản Lý & Giao Diện (UI & Management)
*   **UI Toolkit Tiên Tiến:** Giao diện được thiết kế hoàn toàn bằng bộ UI Toolkit (UXML/CSS templates) mới nhất của Unity, mang lại độ mượt mà cao.
*   **Tùy Chỉnh Cấu Hình (Sound Manager):** Quản lý âm thanh hệ thống, cung cấp cơ chế tinh chỉnh thanh âm Music/SFX ngay trong Menu Pause.

---

## 📂 Kiến Trúc & Cấu Trúc Mã Nguồn

Dự án ứng dụng cấu trúc phân tầng và tập trung cấu hình bằng **Scriptable Objects (SO)**:
*   `GameManager` & `LevelSelectManager`: Chịu trách nhiệm cho toàn bộ Loop trò chơi, phân giải các tiến độ thông qua `LevelDatabase`.
*   `CurrentPlayerDataSO` / `TowerData` / `LevelData`: Tách biệt mọi chỉ số (Stat config) ra khởi cấu trúc logic giúp Game Designer dễ dàng cân bằng game.
*   `PlayFabManager` & `SaveSystem`: Thiết lập chuẩn giao thức độc lập đến máy chủ backend để bảo chứng dữ liệu trò chơi.
*   `GameUIManager`: Kiểm soát logic xử lý UI tách rời giúp Gameplay chạy trơn tru mà không sợ nghẽn cổ chai.

---

## 🛠️ Yêu Cầu Cài Đặt (Requirements)

1.  **Unity Editor:** Phiên bản `6000.1.15f1` hoặc mới hơn.
2.  **Package phụ thuộc:**
    *   UI Builder & UI Toolkit
    *   Input System
    *   PlayFab SDK (Editor Extensions)

## 📌 Hướng Dẫn Nhanh (Quick Start)

1. Clone dự án về máy: 
```bash
git clone https://github.com/Yutaka-ReiRoku/KingdomDefense.git
```
2. Mở Unity Hub, chọn **Add Project** và trỏ đường dẫn tới thư mục `KingdomDefense`.
3. Cài đặt PlayFab SDK, thiết lập Title ID của Studio nếu cần test các tính năng Backend.
4. Trong Unity, mở folder `Assets/Scenes`, khởi chạy **Main Menu** hoặc vòng lặp **GameScenes** chính để bắt đầu.

---
*Dự án thuộc bản quyền phát triển bởi [Yutaka-ReiRoku](https://github.com/Yutaka-ReiRoku), [Reider25](https://github.com/Reider25) và [BTHieu2004](https://github.com/BTHieu2004).*
