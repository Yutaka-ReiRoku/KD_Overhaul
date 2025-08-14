
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class GameUIManager : MonoBehaviour
{
    public enum GamePhase { SquadSelection, Gameplay }
    private GamePhase currentPhase;
    private const int ANIMATION_DURATION = 500;

    [SerializeField] private UIDocument uiDocument;

    [Header("Asset References")]
    public TowerDatabase towerDatabase;
    public VisualTreeAsset towerCardTemplate;
    public VisualTreeAsset cooldownOverlayTemplate;

    [Header("Game State Managers")]
    public CurrencyManager currencyManager;
    public CurrentPlayerDataSO currentPlayer;

    [Header("Gameplay Components")]
    [SerializeField] private WaveSpawner waveSpawner;

    [SerializeField] private Texture2D shovelCursor;

    [SerializeField] private VisualTreeAsset towersBar;
    [SerializeField] private VisualTreeAsset towersPanel;


    [Header("Camera Movement")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform initialPos;
    [SerializeField] private Transform targetPos;

    private VisualElement root;
    private VisualElement squadSelectionPanel;
    private VisualElement cardGrid;
    private Button confirmButton;
    private VisualElement towerSelectionBar;
    private VisualElement barCardContainer;
    private List<Button> barSlots;
    private Label currencyLabel;
    private Dictionary<Button, Button> activeCardsMap = new Dictionary<Button, Button>();
    private Dictionary<Button, Coroutine> activeCooldowns = new Dictionary<Button, Coroutine>();
    private Button selectedGameplayCard = null;
    private TowerData towerToPlace = null;
    private Tile lastHoveredTile = null;
    private Vector2 pointerPosition;
    private Button shovelButton;
    private bool isShovelSelected = false;
    private VisualElement loadingPanel;


    private Coroutine currencyAnimationCoroutine;
    private int lastDisplayedCurrency = 0;


    private TemplateContainer towersBarInstance;
    private TemplateContainer towersPanelInstance;
    private void OnEnable()
    {

        root = uiDocument.rootVisualElement;

        
    }
    void Start()
    {
        Time.timeScale = 1f;
        UpdateUI();

        Sequence introSequence = DOTween.Sequence();
        introSequence.AppendInterval(2f)
                     .AppendCallback(MoveScreen)
                     .JoinCallback(ShowUIElements);


        loadingPanel = root.Q("LoadingPanel");

        root.schedule.Execute(() => loadingPanel.RemoveFromClassList("loading-panel-active")).ExecuteLater(1500);

        squadSelectionPanel = root.Q<VisualElement>("squad-selection-panel");
        towerSelectionBar = root.Q<VisualElement>("tower-selection-bar");
        cardGrid = root.Q<VisualElement>("card-grid");
        confirmButton = root.Q<Button>("confirm-button");
        barCardContainer = root.Q<VisualElement>("card-container");
        currencyLabel = root.Q<Label>("currency-amount");
        shovelButton = root.Q<Button>("ShovelCard");
        if (shovelButton != null)
        {
            shovelButton.RegisterCallback<ClickEvent>(OnShovelClicked);
        }

        currencyManager.OnCurrencyChanged += UpdateAllCardStates;




        InitializeSquadSelectionPanel();
        InitializeTowerSelectionBar();

        confirmButton.RegisterCallback<ClickEvent>(OnConfirmButtonClick);

        confirmButton.SetEnabled(false);


        currentPhase = GamePhase.SquadSelection;
        currencyManager.UpdateInitalCurrency();
        lastDisplayedCurrency = currencyManager.GetCurrentCurrency();
        UpdateAllCardStates(lastDisplayedCurrency);
    }


    private void UpdateUI()
    {
        towersBarInstance = CreateAndAddUIElement(towersBar, "fullscreen");
        towersPanelInstance = CreateAndAddUIElement(towersPanel, "fullscreen");
    }

    /// <summary>
    /// </summary>
    private TemplateContainer CreateAndAddUIElement(VisualTreeAsset visualTree, string className)
    {
        var newInstance = visualTree.CloneTree();
        newInstance.AddToClassList(className);
        newInstance.pickingMode = PickingMode.Ignore;
        root.Add(newInstance);
        return newInstance;
    }

    /// <summary>
    /// </summary>
    private void ShowUIElements()
    {
        currencyManager.UpdateInitalCurrency();

        var squadPanel = towersPanelInstance.Q<VisualElement>("squad-selection-panel");
        squadPanel?.schedule.Execute(() =>
        {
            squadPanel.RemoveFromClassList("squad-selection-panel--hidden");
        });

        var towerBar = towersBarInstance.Q<VisualElement>("tower-selection-bar");
        towerBar?.schedule.Execute(() =>
        {
            towerBar.RemoveFromClassList("tower-selection-bar--hidden");
        });
    }

    /// <summary>
    /// </summary>
    private void MoveScreen()
    {
        cameraTransform.DOMove(targetPos.position, 2f).SetEase(Ease.InOutQuad);
    }

    private void MoveIniScreen()
    {
        cameraTransform.DOMove(initialPos.position, 2f).SetEase(Ease.InOutQuad);
    }

    private void OnDisable()
    {
        currencyManager.OnCurrencyChanged -= UpdateAllCardStates;
    }

    public void OnPointerMove(InputAction.CallbackContext context)
    {
        if (currentPhase != GamePhase.Gameplay) return;
        pointerPosition = context.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider != null)
        {
            Tile currentTile = hit.collider.GetComponent<Tile>();
            if (currentTile != null)
            {
                if (lastHoveredTile != currentTile)
                {
                    lastHoveredTile?.HandlePointerExit();
                    currentTile.HandlePointerEnter();
                    lastHoveredTile = currentTile;
                }
            }
            else
            {
                lastHoveredTile?.HandlePointerExit();
                lastHoveredTile = null;
            }
        }
        else
        {
            lastHoveredTile?.HandlePointerExit();
            lastHoveredTile = null;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed || currentPhase != GamePhase.Gameplay) return;

        Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider == null) return;


        if (hit.collider.TryGetComponent<ICollectible>(out ICollectible collectible))
        {
            collectible.Collect();
        }
        else if (hit.collider.TryGetComponent<Tile>(out Tile tile))
        {
            if (tile == lastHoveredTile)
            {
                tile.HandleClick();
            }
        }
    }

    private void InitializeTowerSelectionBar()
    {
        barSlots = barCardContainer.Query<Button>("TowerCard").ToList();
        foreach (var slot in barSlots)
        {
            slot.style.display = DisplayStyle.Flex;
            if (cooldownOverlayTemplate != null)
            {
                VisualElement overlayInstance = cooldownOverlayTemplate.CloneTree();
                VisualElement cooldownOverlay = overlayInstance.Q("cooldown-overlay");
                if (cooldownOverlay != null)
                {
                    cooldownOverlay.style.display = DisplayStyle.Flex;
                    cooldownOverlay.style.position = Position.Absolute;
                    slot.Add(cooldownOverlay);
                }
            }
            slot.style.visibility = Visibility.Hidden;
            slot.RegisterCallback<ClickEvent>(OnBarCardClick);
        }
    }

    private void InitializeSquadSelectionPanel()
    {
        if (towerCardTemplate == null) { Debug.LogError("Chưa gán 'Tower Card Template' trong Inspector!"); return; }
        cardGrid.Clear();



        foreach (string towerID in currentPlayer.ownedTowerIDs)
        {
            TowerData towerData = towerDatabase.GetTowerDataByID(towerID);

            if (towerData != null)
            {
                VisualElement newCardInstance = towerCardTemplate.CloneTree();
                Button newCard = newCardInstance.Q<Button>("AvailableTower_Instance");
                newCard.style.display = DisplayStyle.Flex;
                newCard.userData = towerData;
                newCard.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(towerData.towerIcon);
                newCard.Q<Label>("cost").text = towerData.cost.ToString();
                newCard.RegisterCallback<ClickEvent>(OnSquadCardClick);
                cardGrid.Add(newCard);
            }
        }
    }

    private void OnConfirmButtonClick(ClickEvent evt)
    {
        currentPhase = GamePhase.Gameplay;
        squadSelectionPanel.AddToClassList("squad-selection-panel--hidden");
        MoveIniScreen();

        UpdateAllCardStates(currencyManager.GetCurrentCurrency());
        if (waveSpawner != null)
        {
            waveSpawner.BeginSpawning();
        }
        else
        {
            Debug.LogError("Chưa gán WaveSpawner trong Inspector của GameUIManager!");
        }
    }

    private void UpdateAllCardStates(int newCurrency)
    {
        if (currencyAnimationCoroutine != null)
        {
            StopCoroutine(currencyAnimationCoroutine);
        }
        currencyAnimationCoroutine = StartCoroutine(AnimateCurrencyText(newCurrency));


        currencyLabel.text = newCurrency.ToString();

        foreach (var barCard in barSlots)
        {
            if (barCard.style.visibility == Visibility.Visible)
            {
                var towerData = barCard.userData as TowerData;
                if (towerData != null)
                {
                    bool isOnCooldown = activeCooldowns.ContainsKey(barCard);
                    barCard.SetEnabled(currencyManager.CanAfford(towerData.cost) && !isOnCooldown);
                }
            }
        }
    }


    private System.Collections.IEnumerator AnimateCurrencyText(int targetValue)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        int startValue = lastDisplayedCurrency;

        IStyle style = currencyLabel.style;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            int currentValue = (int)Mathf.Lerp(startValue, targetValue, progress);
            currencyLabel.text = currentValue.ToString();

            float scaleValue = 1f + Mathf.Sin(progress * Mathf.PI) * 0.5f;
            style.scale = new Scale(new Vector2(scaleValue, scaleValue));

            yield return null;
        }
        currencyLabel.text = targetValue.ToString();
        style.scale = new Scale(Vector2.one);

        lastDisplayedCurrency = targetValue;
        currencyAnimationCoroutine = null;
    }

    private System.Collections.IEnumerator CooldownCoroutine(Button card, float duration)
    {
        VisualElement overlay = card.Q("cooldown-overlay");
        if (overlay == null) { yield break; }

        card.SetEnabled(false);
        overlay.style.height = new Length(100, LengthUnit.Percent);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = 1f - (elapsedTime / duration);
            overlay.style.height = new Length(Mathf.Clamp01(percentage) * 100f, LengthUnit.Percent);
            yield return null;
        }

        overlay.style.height = 0;
        activeCooldowns.Remove(card);

        UpdateAllCardStates(currencyManager.GetCurrentCurrency());
    }

    private void OnSquadCardClick(ClickEvent evt)
    {
        if (currentPhase != GamePhase.SquadSelection) return;
        Button clickedPanelCard = evt.currentTarget as Button;
        TowerData towerData = clickedPanelCard.userData as TowerData;
        Button emptyBarSlot = barSlots.FirstOrDefault(s => s.style.visibility == Visibility.Hidden && !s.ClassListContains("slot--reserved"));
        emptyBarSlot.AddToClassList("slot--reserved");
        clickedPanelCard.SetEnabled(false);
        AnimateMove(clickedPanelCard, emptyBarSlot, towerData, () => {
            emptyBarSlot.userData = towerData;
            emptyBarSlot.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(towerData.towerIcon);
            emptyBarSlot.Q<Label>("cost").text = towerData.cost.ToString();
            emptyBarSlot.style.visibility = Visibility.Visible;
            activeCardsMap[emptyBarSlot] = clickedPanelCard;
            emptyBarSlot.RemoveFromClassList("slot--reserved");
            UpdateConfirmButtonState();
        });
    }

    private void OnBarCardClick(ClickEvent evt)
    {
        switch (currentPhase)
        {
            case GamePhase.SquadSelection: HandleUnselectTower(evt); break;
            case GamePhase.Gameplay: HandleSelectTowerForPlacement(evt); break;
        }
    }

    private void HandleUnselectTower(ClickEvent evt)
    {
        Button clickedBarCard = evt.currentTarget as Button;
        if (activeCardsMap.TryGetValue(clickedBarCard, out Button correspondingPanelCard))
        {
            clickedBarCard.style.visibility = Visibility.Hidden;
            activeCardsMap.Remove(clickedBarCard);
            AnimateMove(clickedBarCard, correspondingPanelCard, clickedBarCard.userData as TowerData, () => {
                correspondingPanelCard.SetEnabled(true);
                clickedBarCard.userData = null;
                UpdateConfirmButtonState();
            });
        }
    }

    private void HandleSelectTowerForPlacement(ClickEvent evt)
    {
        if (isShovelSelected)
        {
            DeselectShovel();
        }
        Button clickedButton = evt.currentTarget as Button;
        if (selectedGameplayCard != null) { selectedGameplayCard.RemoveFromClassList("selected"); }
        if (selectedGameplayCard == clickedButton)
        {
            selectedGameplayCard = null;
            towerToPlace = null;
        }
        else
        {
            selectedGameplayCard = clickedButton;
            selectedGameplayCard.AddToClassList("selected");
            towerToPlace = selectedGameplayCard.userData as TowerData;
        }
    }

    public void OnTileClicked(Tile clickedTile)
    {
        if (isShovelSelected)
        {
            HandleShovelClick(clickedTile);
            return;
        }

        if (currentPhase != GamePhase.Gameplay) return;

        if (towerToPlace.towerPrefab != null)
        {
            GameObject towerGO = Instantiate(towerToPlace.towerPrefab, clickedTile.transform.position, Quaternion.identity);
            TowerBase newTower = towerGO.GetComponent<TowerBase>();

            if (newTower != null)
            {
                clickedTile.SetOccupied(newTower, towerToPlace);
            }

            currencyManager.SpendCurrency(towerToPlace.cost);
            if (selectedGameplayCard != null) { StartCooldown(selectedGameplayCard, towerToPlace); }
        }
        else { Debug.LogError($"TowerData '{towerToPlace.towerName}' chưa được gán Prefab!"); }

        if (selectedGameplayCard != null) { selectedGameplayCard.RemoveFromClassList("selected"); }
        selectedGameplayCard = null;
        towerToPlace = null;
    }

    private void StartCooldown(Button card, TowerData towerData)
    {
        if (activeCooldowns.TryGetValue(card, out Coroutine existingCoroutine)) { StopCoroutine(existingCoroutine); }
        Coroutine newCooldownCoroutine = StartCoroutine(CooldownCoroutine(card, towerData.cooldownTime));
        activeCooldowns[card] = newCooldownCoroutine;
    }

    public bool IsHoldingTower()
    {
        return towerToPlace != null;
    }

    private void AnimateMove(VisualElement startElement, VisualElement endElement, TowerData towerData, System.Action onAnimationComplete)
    {
        if (towerData == null) return;
        VisualElement flyingDummyInstance = towerCardTemplate.CloneTree();
        Button flyingDummy = flyingDummyInstance.Q<Button>("AvailableTower_Instance");
        flyingDummy.style.display = DisplayStyle.Flex;
        flyingDummy.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(towerData.towerIcon);
        flyingDummy.Q<Label>("cost").text = towerData.cost.ToString();
        flyingDummy.style.width = startElement.resolvedStyle.width;
        flyingDummy.style.height = startElement.resolvedStyle.height;
        flyingDummy.style.position = Position.Absolute;
        flyingDummy.style.marginLeft = 0; flyingDummy.style.marginRight = 0; flyingDummy.style.marginTop = 0; flyingDummy.style.marginBottom = 0;
        Vector2 startPos = startElement.ChangeCoordinatesTo(root, Vector2.zero);
        Vector2 endPos = endElement.ChangeCoordinatesTo(root, Vector2.zero);
        flyingDummy.style.left = startPos.x;
        flyingDummy.style.top = startPos.y;
        root.Add(flyingDummy);
        flyingDummy.experimental.animation
            .Start(new StyleValues { left = startPos.x, top = startPos.y},
                   new StyleValues { left = endPos.x, top = endPos.y},
                   ANIMATION_DURATION)
            .Ease(Easing.OutCubic)
            .OnCompleted(() => {
                flyingDummy.RemoveFromHierarchy();
                onAnimationComplete?.Invoke();
            });
    }


    private void OnShovelClicked(ClickEvent evt)
    {
        if (currentPhase != GamePhase.Gameplay) return;

        isShovelSelected = !isShovelSelected;

        if (isShovelSelected)
        {
            DeselectTowerCard();

            shovelButton.AddToClassList("selected");
            UnityEngine.Cursor.SetCursor(shovelCursor, Vector2.zero, CursorMode.Auto);
            Debug.Log("Shovel selected.");
        }
        else
        {
            DeselectShovel();
        }
    }

    private void HandleShovelClick(Tile clickedTile)
    {
        if (clickedTile.IsOccupied && clickedTile.TowerOnTile != null)
        {
            TowerData removedTowerData = clickedTile.TowerOnTile.Data;
            int refundAmount = Mathf.RoundToInt(removedTowerData.cost * 0.5f);
            currencyManager.AddCurrency(refundAmount);
            Debug.Log($"Removing tower {clickedTile.TowerOnTile.name} from tile.");
            Destroy(clickedTile.TowerOnTile.gameObject);
            clickedTile.SetEmpty();
        }
        else
        {
            Debug.Log("Clicked on an empty tile with shovel.");
        }

        DeselectShovel();
    }

    private void DeselectShovel()
    {
        isShovelSelected = false;
        shovelButton.RemoveFromClassList("selected");
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Debug.Log("Shovel deselected.");
    }

    private void DeselectTowerCard()
    {
        if (selectedGameplayCard != null)
        {
            selectedGameplayCard.RemoveFromClassList("selected");
            selectedGameplayCard = null;
            towerToPlace = null;
        }
    }
    public bool IsShovelSelected()
    {
        return isShovelSelected;
    }



    private void UpdateConfirmButtonState()
    {
        bool allBarSlotsFull = barSlots.All(slot => slot.style.visibility == Visibility.Visible);

        var panelCards = cardGrid.Query<Button>().ToList();
        bool allPanelCardsSelected = panelCards.All(card => !card.enabledSelf);

        if (allBarSlotsFull || allPanelCardsSelected)
        {
            confirmButton.SetEnabled(true);
        }
        else
        {
            confirmButton.SetEnabled(false);
        }
    }
}