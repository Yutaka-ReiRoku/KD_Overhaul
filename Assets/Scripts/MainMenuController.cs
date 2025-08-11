// Yutaka ReiRoku
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    // Định nghĩa các hằng số cho tên class CSS để dễ quản lý và tránh lỗi
    private const string PANEL_HIDDEN_CLASS = "panel-hidden";
    private const string NAV_PANEL_HIDDEN_CLASS = "nav-panel-hidden";
    private const string VIEW_ABOVE_CLASS = "content-view--above";
    private const string VIEW_BOTTOM_CLASS = "content-view--bottom";
    private const string INSTA_ABOVE_CLASS = "content-view--insta-above";
    private const string INSTA_BOTTOM_CLASS = "content-view--insta-bottom";

    // Class nhỏ để chứa thông tin về một panel
    [System.Serializable]
    public class PanelInfo
    {
        public string Name;
        public int Order;
        [HideInInspector] public VisualElement Element;
    }

    // Quản lý tất cả các panel trong một danh sách duy nhất.
    // Dễ dàng thêm, bớt hoặc thay đổi thứ tự.
    [SerializeField]
    private List<PanelInfo> panels = new List<PanelInfo>
    {
        new PanelInfo { Name = "LoginForm", Order = 1 },
        new PanelInfo { Name = "RegisterForm", Order = 2 },
        new PanelInfo { Name = "LevelSelectPanel", Order = 3 },
        new PanelInfo { Name = "ShopPanel", Order = 4 },
        new PanelInfo { Name = "AchievementsPanel", Order = 5 },
        new PanelInfo { Name = "QuestsPanel", Order = 6 },
        new PanelInfo { Name = "SettingsPanel", Order = 7 }
    };

    private VisualElement root;
    private VisualElement currentPanel;
    private int currentOrder = 0;

    // Overlay và NavPanel có thể được quản lý riêng
    private VisualElement exitConfirmationOverlay;
    private VisualElement navPanel;

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // Tự động query tất cả các panel và các element quan trọng khác
        InitializeElements();

        // Đăng ký tất cả các sự kiện cho button
        RegisterButtonCallbacks();

        // Hiển thị panel đầu tiên một cách an toàn sau khi UI đã được render
        root.schedule.Execute(() => ShowPanelByOrder(1)).ExecuteLater(1500);
    }

    private void InitializeElements()
    {
        foreach (var panelInfo in panels)
        {
            panelInfo.Element = root.Q(panelInfo.Name);
        }
        exitConfirmationOverlay = root.Q("ExitConfirmationOverlay");
        navPanel = root.Q("NavPanel");
    }

    private void RegisterButtonCallbacks()
    {
        // Liên kết các button điều hướng chính (dùng .clicked cho Button là OK)
        root.Q<Button>("StartButton").clicked += () => ShowPanelByOrder(3);
        root.Q<Button>("ShopButton").clicked += () => ShowPanelByOrder(4);
        root.Q<Button>("AchievementsButton").clicked += () => ShowPanelByOrder(5);
        root.Q<Button>("QuestsButton").clicked += () => ShowPanelByOrder(6);
        root.Q<Button>("SettingsButton").clicked += () => ShowPanelByOrder(7);

        // SỬA LẠI Ở ĐÂY: Dùng RegisterCallback cho các element không phải là Button
        // Cách này đúng với code gốc của bạn và hoạt động cho mọi VisualElement.
        root.Q("SwitchToRegister").RegisterCallback<ClickEvent>(evt => ShowPanelByOrder(2));
        root.Q("SwitchToLogin").RegisterCallback<ClickEvent>(evt => ShowPanelByOrder(1));

        // Logic thoát game
        root.Q<Button>("ExitButton").clicked += () => exitConfirmationOverlay.RemoveFromClassList(PANEL_HIDDEN_CLASS);
        root.Q<Button>("ConfirmExitButton").clicked += () => Application.Quit();
        root.Q<Button>("CancelExitButton").clicked += () => exitConfirmationOverlay.AddToClassList(PANEL_HIDDEN_CLASS);

        // Logic đăng nhập / đăng xuất
        root.Q<Button>("LoginSubmitButton").clicked += PerformLogin;
        root.Q<Button>("LogoutButton").clicked += PerformLogout;

        // Logic vào màn chơi
        root.Query<Button>("LevelButton").ForEach(button => {
            button.clicked += () => SceneManager.LoadScene(1);
        });
    }

    private void PerformLogin()
    {
        navPanel.RemoveFromClassList(NAV_PANEL_HIDDEN_CLASS);
        ShowPanelByOrder(3); // Chuyển đến màn hình chọn level sau khi đăng nhập
    }

    private void PerformLogout()
    {
        navPanel.AddToClassList(NAV_PANEL_HIDDEN_CLASS);
        ShowPanelByOrder(1); // Quay về màn hình đăng nhập
    }

    // Hàm tìm và hiển thị panel dựa trên Order
    private void ShowPanelByOrder(int order)
    {
        var panelInfo = panels.Find(p => p.Order == order);
        if (panelInfo != null)
        {
            ShowPanel(panelInfo.Element, panelInfo.Order);
        }
    }

    private void ShowPanel(VisualElement panelToShow, int order)
    {
        if (currentPanel == panelToShow || panelToShow == null)
        {
            return;
        }

        // 1. Reset trạng thái của tất cả các panel
        foreach (var panelInfo in panels)
        {
            panelInfo.Element?.AddToClassList(PANEL_HIDDEN_CLASS);
            panelInfo.Element?.RemoveFromClassList(VIEW_ABOVE_CLASS);
            panelInfo.Element?.RemoveFromClassList(VIEW_BOTTOM_CLASS);
            panelInfo.Element?.RemoveFromClassList(INSTA_ABOVE_CLASS);
            panelInfo.Element?.RemoveFromClassList(INSTA_BOTTOM_CLASS);
        }

        bool isGoingForward = order > currentOrder;

        // 2. Animate panel hiện tại (nếu có) ra khỏi màn hình
        if (currentPanel != null)
        {
            currentPanel.RemoveFromClassList(PANEL_HIDDEN_CLASS);
            currentPanel.AddToClassList(isGoingForward ? VIEW_ABOVE_CLASS : VIEW_BOTTOM_CLASS);
        }

        // 3. Chuẩn bị panel mới để animate vào
        panelToShow.RemoveFromClassList(PANEL_HIDDEN_CLASS);
        panelToShow.AddToClassList(isGoingForward ? INSTA_BOTTOM_CLASS : INSTA_ABOVE_CLASS);

        // 4. Thực thi animation vào ở frame tiếp theo
        panelToShow.schedule.Execute(() =>
        {
            panelToShow.RemoveFromClassList(isGoingForward ? INSTA_BOTTOM_CLASS : INSTA_ABOVE_CLASS);
        });

        // 5. Cập nhật trạng thái
        currentPanel = panelToShow;
        currentOrder = order;
    }
}