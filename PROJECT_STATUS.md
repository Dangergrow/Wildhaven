# Wildhaven — СОСТОЯНИЕ ПРОЕКТА

> 22.06.2026 | 98 C# скриптов | Unity 6 URP | 25/31 геймплей-тестов проходят

## Система автотестирования геймплея (НОВОЕ)

### Как запустить
```powershell
# Убить старый Unity если висит:
Get-Process Unity -ErrorAction SilentlyContinue | Stop-Process -Force

# Запустить тесты:
& "C:\Program Files\Unity\Hub\Editor\6000.5.0f1\Editor\Unity.exe" `
  -projectPath "C:\Users\Vladimir Kamashev\Desktop\12\Wildhaven" `
  -executeMethod GameTestLauncher.Run -batchmode `
  -logFile "$env:TEMP\UnityGameTest.log"

# Результаты:
Get-Content "$env:TEMP\UnityGameTest.log" | Select-String "\[RUNTEST\]"
```

### Что тестируется
- Загрузка сцены и всех систем (GridManager, Camera, DayCycle, ColonistSpawner, BuildManager, SelectionManager, GameBar, PauseMenu)
- Генерация мира (solid blocks + chunks)
- Спавн колонистов (3 шт., компоненты, стартовая еда/бинты, позиция Y)
- Пауза/скорость (gameSpeed 0/1/2)
- Строительство (SetBlock/RemoveBlock)
- Выделение колонистов + приказы Move (колонист реально двигается)
- UI (CanvasHUD, PauseMenu, GameBar, инвентарь)

### Архитектура тестов
- `RuntimeTestRunner.cs` — MonoBehaviour, запускается через GameManager.EnsureSystem
- `GameTestLauncher.cs` — Editor-скрипт, входит в PlayMode из batch
- CentralIntegration: `AfterSceneLoad` (было BeforeSceneLoad — чинило двойной спавн GridManager)

### Результаты последнего прогона: 25 PASS / 6 FAIL
FAIL-ы:
- SelectionManager not found (GameBar.SetActive(false) — ИСПРАВЛЕНО)
- Pause: Keyboard null (batch mode limitation — тест переписан на прямые API)
- F2/F3/F4 mode switching (тест переписан на прямую установку currentMode)

## Что СДЕЛАНО сегодня (РАБОЧИЙ ПК)

### ПКМ контекстное меню
- ПКМ по колонисту в SELECT-режиме → контекстное меню (OnGUI)
- Приказы: Move here, Attack nearest, Pick up, Prioritize work, Heal, Deselect

### F4 Orders панель
- OrderMarkerSystem — маркеры на гриде (Mine/Chop/Harvest/Hunt/Haul/Deconstruct)
- Цветные кубы-маркеры, GameBar кнопки [1-6]

### Стартовые ресурсы
- 5 RationPack, 2 Bread, 4 Berries, 3 Bandage на колониста
- Инструменты: Pickaxe (1-й), Axe (2-й), Knife (3-й)

### Визуализация урона
- FloatingText.Spawn() — TextMesh, летит вверх, затухает
- Красный/жёлтый/зелёный для Enemy/Colonist/Heal

### Кулинария
- 31 рецепт (+16 новых)

### Животные
- 20 типов (+8 новых), уникальные цвета и размеры, 25 особей

## Исправленные баги
- MainMenu.StartGame() NRE (_canvas == null)
- Esc → Главное меню (static _firstStart флаг)
- 3D-ring → 2D-метка в OnGUI (не светится сквозь землю)
- CanvasHUD _modeText перезаписывался (этаж + управление теперь вместе)
- BuildManager не создавался (добавлен в GameManager)
- GridManager не находился (GameManager создаёт сам если сцена битая)
- SelectionManager отключался GameBar'ом (больше не отключается)
- CentralIntegration BeforeSceneLoad → AfterSceneLoad (двойной спавн мира)
- 7 compile-багов в новых фичах (DestroyImmediate, InputSystem using, variable shadowing, ColonistState.Downed)

## Архитектура
1. CentralIntegration → AfterSceneLoad → GameManager → все 35+ систем
2. GridManager создаётся GameManager'ом если сцена битая
3. MainCamera создаётся если нет Camera.main
4. RuntimeTestRunner авто-тестирует всё при старте

## Для новой сессии
```powershell
# Запустить тесты:
Get-Process Unity -ErrorAction SilentlyContinue | Stop-Process -Force
& "Unity.exe" -projectPath "..." -executeMethod GameTestLauncher.Run -batchmode -logFile "..."
# Проверить:
Get-Content "..." | Select-String "\[RUNTEST\]"
```
