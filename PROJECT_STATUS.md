# Wildhaven — ТЕКУЩЕЕ СОСТОЯНИЕ ПРОЕКТА

> Последнее обновление: 19.06.2026 | РАБОЧИЙ ПК

## Что это за проект

**Wildhaven** — колони-сим в духе Going Medieval с глобальной картой как в RimWorld. Мультиплеер до 3 игроков через Steam (Spacewar). Unity 6 (6000.5.0f1), URP, C#.

## Как запустить

1. Открыть Unity Hub → проект `C:\Users\Vladimir Kamashev\Desktop\12\Wildhaven`
2. Открыть сцену `Assets/Scenes/SampleScene.unity`
3. Play — игра запустится сразу (меню и создание персонажа авто-пропускаются в редакторе)

## Архитектура

### Ядро (авто-запуск)
- **`CentralIntegration.cs`** (`Assets/Scripts/Core/`) — `[RuntimeInitializeOnLoadMethod]` создаёт себя автоматически. Оркестрирует все системы с тиками 1с/5с/30с.
- **`GameManager.cs`** — авто-создаёт 30+ систем через `EnsureSystem<T>()`. Не требует ручной настройки сцены.
- **`GridManager.cs`** — воксельный мир 100×32×100, чанки 16×16×16, greedy mesh. На GameObject "World" в сцене.

### Системы (все в `Assets/Scripts/`)

| Папка | Ключевые файлы | Статус |
|---|---|---|
| **World/** | GridManager, BuildManager, BlockDatabase | ✅ Работает |
| **Colonists/** | Colonist, ColonistAI, NeedsSystem, ColonistSpawner, DayCycle, ColonistGravity, WaterInteraction, MentalState, ColonistSchedule, Pathfinder | ✅ Ядро работает, Pathfinder есть но не интегрирован |
| **Resources/** | ItemType (70+), ItemData, Inventory, RecipeData, CraftingStation, BlockDropManager, Equipment, CookingSystem, EconomyManager, FoodSpoilage, ItemQuality | ✅ Работает |
| **Combat/** | Enemy, RaidManager, Trap, CombatEnums | ✅ Работает |
| **WorldMap/** | WorldMapGenerator, HexTile, MapData, Caravan, FactionManager | ✅ Генерация работает, караваны есть, UI карты через MapOverlay (M) |
| **Events/** | EventManager, QuestManager, EventDef | ✅ События тикают |
| **Research/** | ResearchManager (60+ нод), ResearchNode, ResearchEffects | ✅ Работает |
| **Farming/** | PlantGrowth (8 культур) | ✅ Работает |
| **Social/** | SocialSystem, FamilySystem, ReligionSystem | ✅ Ядро работает |
| **UI/** | CanvasHUD, GameBar, MainMenu, PauseMenu, SelectionManager, CharacterCreator, WorldSettings, GameSettings, MapOverlay, ColonistPanel, WorkPanel, TradeUI, UIFont | ✅ Ядро работает |
| **Core/** | CentralIntegration, GameManager, ColonyServices | ✅ Работает |
| **Camera/** | CameraController, FloorController | ✅ Работает |

### Префабы
- **`Assets/Colonist.prefab`** — префаб колониста. Компоненты: Colonist, ColonistAI, NeedsSystem, ColonistGravity, WaterInteraction, BuildBlocker, MentalState, Inventory (добавляются авто при спавне если отсутствуют)
- **Важно**: CapsuleCollider добавляется в `ColonistSpawner.SpawnColonist()` программно

### Сцена
- **World** — GameObject с GridManager, MeshFilter, MeshRenderer, MeshCollider
- **Directional Light** — тег MainCamera (исправлен домашним ПК)
- Всё остальное создаётся автоматически через CentralIntegration/GameManager

## Управление (полное)

### Камера
- WASD / стрелки — движение
- Q / E — поворот на 45°
- Колёсико — зум
- MMB (зажать) — свободный поворот
- Home — сброс камеры
- **Tab** — вверх на этаж
- **Shift+Tab** — вниз на этаж

### Стройка
- **1-9** — выбор блока
- **Shift+1-9** — продвинутые блоки (мрамор, обсидиан и т.д.)
- **ЛКМ** — поставить блок
- **ЛКМ + drag** — линия/область блоков
- **ПКМ** — сломать блок
- **Shift+ПКМ** — сломать область 3×3
- **Shift+ЛКМ** — blueprint (план без ресурсов)
- **F1** — Architect
- **F2** — Work
- **F3** — Zone
- **F4** — Orders
- **[ / ]** — переключение страниц блоков

### Игра
- **Space** — пауза
- **Num1/2/3** — скорость 1x/2x/4x
- **B** — переключение стройка/выбор (BUILD/SELECT)
- **M** — карта мира
- **Esc** — меню паузы (Continue, Save, Load, Settings, Main Menu, Quit)
- **F5** — сохранить
- **F9** — загрузить

### Выделение
- **B** (режим SELECT) → **ЛКМ** по колонисту → зелёное кольцо + инфо-панель

## Поток новой игры (в билде, не в редакторе)

1. **MainMenu** — 6 кнопок (New Game, Continue, Multiplayer, Settings, About, Quit)
2. **WorldSettings** — выбор seed, размера карты (50/100/200), сложности
3. **CharacterCreator** — 3 колониста: имена, навыки (30 очков), перки, недостатки, внешность
4. **Игра** — мир генерируется заново с указанными параметрами, колонисты спавнятся с шаблонов

## Сохранения

- **GameSaveManager** — `game.sav` (полное: мир + колонисты + инвентарь + исследования + время)
- Старый формат: `world.sav` (F5/F9 через GridManager)
- Путь: `%APPDATA%/LocalLow/DefaultCompany/Wildhaven/`

## Что РАБОТАЕТ (проверено)

- Генерация мира (Perlin noise, вода, горы, снег)
- Стройка/разрушение блоков
- Спавн 3 колонистов
- Колонисты: ходят, добывают блоки, едят из инвентаря, имеют потребности,重力, вода
- AABB-коллизия (не ходят сквозь стены, не падают с обрывов)
- День/ночь, смена сезонов
- Пауза, скорость времени
- Дроп предметов (физика: подпрыгивают, падают, летят к колонисту)
- Инвентарь колонистов
- Все UI-панели (меню, HUD, портреты, нижняя панель, настройки, карта)
- Сохранение/загрузка
- Выделение колонистов (грид-метод)
- Этажи (Tab/Shift+Tab)
- Враги (спавн, AI, атака)
- События (тики каждый 60с)
- Тех-древо (60+ исследований)
- Фермерство (рост культур)
- Экономика (копейки, цены)

## Что НЕ РАБОТАЕТ / ЗАГЛУШКИ

- **Pathfinder (A*)** — код есть, но не используется для движения (колонисты ходят по прямой)
- **TradeUI** — скрипт есть, но торговля не вызывается (нужен NPC-караван)
- **WorkPanel (F2)** — показывает заглушку, приоритеты не редактируются
- **StructureClipboard (Ctrl+C/V)** — скрипт есть, не подключен к BuildManager
- **Кнопки Multiplayer/Settings/About в главном меню** — показывают "coming soon"
- **Выбор причёски/тела в CharacterCreator** — кнопки есть, но визуально не меняют модель (нет ассетов)
- **Визуальный рост растений** — логика есть, модели нет
- **RoomQuality, TemperatureLight** — логика есть, не визуализирована

## Что НУЖНО СДЕЛАТЬ (по приоритету)

### Критическое
1. **Выделение колонистов** — работает через грид, но иногда неточно. Нужно доработать радиус поиска или добавить Physics.Raycast как запасной вариант.
2. **Pathfinding** — интегрировать Pathfinder.cs в ColonistAI для нормального обхода препятствий

### Важное
3. **WorkPanel (F2)** — сделать функциональным: перетаскивание приоритетов, сохранение
4. **TradeUI** — вызывать при событии "торговый караван"
5. **Ассеты** — модели (Mixamo), текстуры (Kenney.nl), звуки (Freesound)

### Желательное
6. **Главное меню для билда** — кнопки Multiplayer, Settings, About должны что-то делать
7. **Анимации** — Mixamo + Blend Trees
8. **Мультиплеер** — Mirror + Steamworks (Spacewar)
9. **Визуализация комнат, температуры, качества**

## Ключевые файлы для быстрого старта

Если ты новый AI в этом проекте — прочитай эти файлы по порядку:
1. `HANDSHAKE.md` — история сессий, что сделано
2. `SETUP.md` — полный гейм-дизайн документ
3. `.docs/DEVELOPER_GUIDE.md` — архитектура от ДОМАШНЕГО ПК
4. `PLAYER_GUIDE.md` — руководство игрока
5. `Assets/Scripts/Core/CentralIntegration.cs` — точка входа
6. `Assets/Scripts/Core/GameManager.cs` — авто-создание систем
