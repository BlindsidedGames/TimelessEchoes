using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TimelessEchoes.UI
{
    /// <summary>
    ///     Instantiates and manages sorting buttons based on the active stats tab.
    /// </summary>
    public class StatSortingManager : MonoBehaviour
    {
        [SerializeField] private Transform buttonParent;
        [SerializeField] private StatSortButton buttonPrefab;

        [SerializeField] private Button generalButton;
        [SerializeField] private Button enemiesButton;
        [SerializeField] private Button tasksButton;
        [SerializeField] private Button itemsButton;

        [SerializeField] private EnemyStatsPanelUI enemyPanel;
        [SerializeField] private TaskStatsPanelUI taskPanel;
        [SerializeField] private ItemStatsPanelUI itemPanel;
        [SerializeField] private RunStatsPanelUI runStatsPanel;

        private readonly List<StatSortButton> buttons = new();
        private readonly Dictionary<StatSortButton, Enum> buttonModes = new();

        private int currentTab = -1;
        private EnemyStatsPanelUI.SortMode enemyMode = EnemyStatsPanelUI.SortMode.Default;
        private TaskStatsPanelUI.SortMode taskMode = TaskStatsPanelUI.SortMode.Default;
        private ItemStatsPanelUI.SortMode itemMode = ItemStatsPanelUI.SortMode.Default;
        private RunStatsPanelUI.GraphMode runMode = RunStatsPanelUI.GraphMode.Distance;

        private void Awake()
        {
            if (generalButton != null)
                generalButton.onClick.AddListener(ShowGeneral);
            if (enemiesButton != null)
                enemiesButton.onClick.AddListener(ShowEnemies);
            if (tasksButton != null)
                tasksButton.onClick.AddListener(ShowTasks);
            if (itemsButton != null)
                itemsButton.onClick.AddListener(ShowItems);
        }

        private void Start()
        {
            if (runStatsPanel != null)
                runStatsPanel.SetGraphMode(runMode);
            SetTab(0);
        }

        private void OnDestroy()
        {
            if (generalButton != null)
                generalButton.onClick.RemoveListener(ShowGeneral);
            if (enemiesButton != null)
                enemiesButton.onClick.RemoveListener(ShowEnemies);
            if (tasksButton != null)
                tasksButton.onClick.RemoveListener(ShowTasks);
            if (itemsButton != null)
                itemsButton.onClick.RemoveListener(ShowItems);
        }

        private void ShowGeneral()
        {
            SetTab(0);
        }

        private void ShowEnemies()
        {
            SetTab(1);
        }

        private void ShowTasks()
        {
            SetTab(2);
        }

        private void ShowItems()
        {
            SetTab(3);
        }

        private void SetTab(int tab)
        {
            if (currentTab == tab) return;
            currentTab = tab;
            BuildButtons();
        }

        private void BuildButtons()
        {
            ClearButtons();
            switch (currentTab)
            {
                case 0:
                    BuildGeneralButtons();
                    break;
                case 1:
                    BuildEnemyButtons();
                    break;
                case 2:
                    BuildTaskButtons();
                    break;
                case 3:
                    BuildItemButtons();
                    break;
            }
        }

        private void ClearButtons()
        {
            foreach (var b in buttons)
                if (b != null)
                    Destroy(b.gameObject);
            buttons.Clear();
            buttonModes.Clear();
        }

        private void BuildEnemyButtons()
        {
            foreach (EnemyStatsPanelUI.SortMode mode in Enum.GetValues(typeof(EnemyStatsPanelUI.SortMode)))
                CreateButton(mode, () =>
                {
                    enemyMode = mode;
                    if (enemyPanel != null) enemyPanel.SetSortMode(mode);
                    UpdateButtonStates();
                });
            UpdateButtonStates();
        }

        private void BuildTaskButtons()
        {
            foreach (TaskStatsPanelUI.SortMode mode in Enum.GetValues(typeof(TaskStatsPanelUI.SortMode)))
                CreateButton(mode, () =>
                {
                    taskMode = mode;
                    if (taskPanel != null) taskPanel.SetSortMode(mode);
                    UpdateButtonStates();
                });
            UpdateButtonStates();
        }

        private void BuildItemButtons()
        {
            foreach (ItemStatsPanelUI.SortMode mode in Enum.GetValues(typeof(ItemStatsPanelUI.SortMode)))
                CreateButton(mode, () =>
                {
                    itemMode = mode;
                    if (itemPanel != null) itemPanel.SetSortMode(mode);
                    UpdateButtonStates();
                });
            UpdateButtonStates();
        }

        private void BuildGeneralButtons()
        {
            CreateButton(RunStatsPanelUI.GraphMode.Distance, () =>
            {
                runMode = RunStatsPanelUI.GraphMode.Distance;
                if (runStatsPanel != null) runStatsPanel.SetGraphMode(runMode);
                UpdateButtonStates();
            });
            CreateButton(RunStatsPanelUI.GraphMode.Duration, () =>
            {
                runMode = RunStatsPanelUI.GraphMode.Duration;
                if (runStatsPanel != null) runStatsPanel.SetGraphMode(runMode);
                UpdateButtonStates();
            });
            CreateButton(RunStatsPanelUI.GraphMode.Resources, () =>
            {
                runMode = RunStatsPanelUI.GraphMode.Resources;
                if (runStatsPanel != null) runStatsPanel.SetGraphMode(runMode);
                UpdateButtonStates();
            });
            CreateButton(RunStatsPanelUI.GraphMode.Kills, () =>
            {
                runMode = RunStatsPanelUI.GraphMode.Kills;
                if (runStatsPanel != null) runStatsPanel.SetGraphMode(runMode);
                UpdateButtonStates();
            });
            UpdateButtonStates();
        }

        private void CreateButton(Enum mode, UnityAction action)
        {
            if (buttonParent == null || buttonPrefab == null) return;

            var btn = Instantiate(buttonPrefab, buttonParent);
            btn.SetLabel(mode.ToString());
            btn.Button.onClick.AddListener(action);
            buttons.Add(btn);
            buttonModes[btn] = mode;
        }

        private void UpdateButtonStates()
        {
            if (buttons.Count == 0) return;

            Enum selected = null;
            switch (currentTab)
            {
                case 0:
                    selected = runMode;
                    break;
                case 1:
                    selected = enemyMode;
                    break;
                case 2:
                    selected = taskMode;
                    break;
                case 3:
                    selected = itemMode;
                    break;
            }

            if (selected == null) return;

            foreach (var pair in buttonModes)
                pair.Key.SetInteractable(!Equals(pair.Value, selected));
        }
    }
}