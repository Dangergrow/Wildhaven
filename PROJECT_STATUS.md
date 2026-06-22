# Wildhaven — СОСТОЯНИЕ ПРОЕКТА

> 22.06.2026 | 100+ C# скриптов | Unity 6 URP | 55/55 PlayMode тестов

## Система автотестирования геймплея

```powershell
# Запустить тесты:
Get-Process Unity -ErrorAction SilentlyContinue | Stop-Process -Force
& "Unity.exe" -projectPath "..." -executeMethod GameTestLauncher.Run -batchmode -logFile "log.log"
Get-Content "log.log" | Select-String "\[RUNTEST\]"
```

### Что тестируется (55 проверок)
- Загрузка сцены, все системы, GridManager, Camera, Directional Light
- Генерация мира (solid blocks + chunks + materials)
- Спавн колонистов (3 шт., компоненты, еда/бинты/инструменты, позиция Y)
- Пауза/скорость (0/1/2x)
- Строительство (SetBlock/RemoveBlock)
- Выделение колонистов + приказы Move (реальное движение)
- A* Pathfinder (поиск пути + движение к цели)
- Input System (Keyboard, Mouse симуляция)
- FPS (~1000 fps avg, 120/120 кадров)
- Визуал (камера включена, чанки с мешем/материалом, свет, UI Canvas)
- StabilitySystem (BFS flood-fill, 0 коллапсов)
- UI (CanvasHUD, PauseMenu, GameBar, F1-F4, портреты, инвентарь)

## Что реализовано

### Игровой мир
- Воксельный мир 100×32×100, чанки 16×16×16, greedy mesh
- Perlin-шум генерация: горы, вода, снег, биомы
- 18 типов блоков (Dirt, Grass, Stone, Wood, Sand, Water, Snow, руды...)
- Сохранение/загрузка (F5/F9, binary .sav)

### Камера и управление
- WASD/стрелки, Q/E поворот 45°, зум, MMB, Home
- Tab/Shift+Tab переключение этажей
- B = переключение BUILD/SELECT

### Строительство
- 1-9 выбор блоков, Shift+1-9 продвинутые
- ЛКМ = поставить, ПКМ = сломать
- Shift+ПКМ = 3×3 снос, Shift+ЛКМ = blueprint
- Drag ЛКМ = линия (Bresenham 3D)
- Ctrl+C/V = копировать/вставить структуру

### Колонисты
- 3 колониста при старте, случайные имена/возраст
- 13 навыков, перки/недостатки
- 7 потребностей (голод, жажда, усталость, настроение, комфорт, социализация, рекреация)
- Стартовые ресурсы: еда (RationPack×5, Bread×2, Berries×4), бинты×3, инструменты
- Инвентарь со стаками
- AI: блуждание, авто-еда, авто-атака врагов
- Ментальные срывы, смерть, трупы

### Приказы и режимы
- F1 = Architect (строительство)
- F2 = Work (приоритеты работ 14×4, зелёная подсветка)
- F3 = Zone (4 типа: Stockpile/Dump/Farm/Room)
- F4 = Orders (6 типов: Mine/Chop/Harvest/Hunt/Haul/Deconstruct)
- ПКМ по колонисту → контекстное меню:
  Move here / Attack nearest / Pick up / Prioritize work / Heal / Deselect

### Ресурсы и крафт
- 125+ типов предметов (ItemType enum)
- Блоки дропают ресурсы при добыче (BlockDropManager)
- Кулинария: 31 рецепт (стейки, супы, пироги, напитки, brownie...)
- Фермерство: 12 культур (PlantGrowth, TryHarvestAt, бонус-дроп семян)
- Собирательство (ForageSpawner)
- 20 типов животных (спавн по типу планеты: пустыня→верблюды, лёд→мамонты...)
- Оружие/броня по слотам (Equipment)
- Охота/приручение (AnimalManager)

### Бой
- Enemy AI (атака, урон, дроп)
- RaidManager (волны рейдеров, частота зависит от сложности)
- 4 типа ловушек (Trap)
- Визуализация урона: красные/жёлтые/зелёные цифры (FloatingText)
- Колонисты авто-атакуют врагов в радиусе

### Глобальная карта
- Hex-сетка, 15 биомов, Perlin-noise генерация
- 10+ фракций с территориями и репутацией
- Караваны (движение, припасы, события)
- M = карта мира (MapOverlay):
  - Цветные хексы по биомам
  - При наведении: биом, климат (темп/осадки/высота), фракция, дороги
  - Зелёный = поселение игрока, жёлтый = NPC-поселения

### Создание мира (WorldSettings)
3 страницы:
1. **Выбор планеты**: Earthlike, Desert World, Ice World, Jungle World, Dead World
2. **Размер карты**: 50×50, 100×100, 200×200 + описание климата планеты
3. **Сложность и моды**: Peaceful/Normal/Brutal, частота рейдов, Apocalyptic mode, PvP

### Настройки (GameSettings)
- Аудио: Music Volume ±, SFX Volume ±
- Видео: Quality Low/Med/High, Fullscreen, FPS cap (30/60/120/Unlimited)
- Язык: ENG/RUS переключение
- Key Bindings (заглушка — кнопка есть, биндинг через Input Actions позже)

### Об игре (About)
Детальное описание: версия, движок, фичи (воксельный мир, биомы, фракции, исследования, кулинария, животные, стабильность, электричество, религия)

### Время и события
- День/ночь, 24h цикл, 4 сезона
- Space = пауза, Num1/2/3 = скорость
- EventManager: 25+ типов событий
- QuestManager: 5 типов квестов

### Исследования
- Тех-древо: 60+ узлов, 5 эпох (Выживание→Поселение→Развитие→Индустрия→Прогресс)
- Секретная ветка (находится в руинах)

### Уникальные системы (сверх Going Medieval)
- Электричество (EnergyNetwork: генераторы, провода, аккумуляторы)
- Религия (ReligionSystem: верования, ритуалы, храмы)
- Валюта Копейки (EconomyManager: динамические цены)
- Семья/дети (FamilySystem: брак, беременность)
- Порча еды (FoodSpoilage)
- Стабильность построек (StabilitySystem: BFS flood-fill от bedrock)
- Дальний бой (луки/арбалеты)
- Копирование структур (StructureClipboard)

### UI
- MainMenu: New Game / Continue / Multiplayer / Settings / About / Quit
- PauseMenu (Esc): Continue / Save / Load / Settings / Main Menu / Quit
- CanvasHUD: время, ресурсы (Wood/Stone/Food/Metal), портреты колонистов
- GameBar: F1-F4 панель с кнопками
- WorkPanel: F2 приоритеты
- CharacterCreator: создание 3 колонистов
- TradeUI: торговля с караванами

## Для новой сессии
```powershell
# Запустить все тесты:
Get-Process Unity -ErrorAction SilentlyContinue | Stop-Process -Force
& "C:\Program Files\Unity\Hub\Editor\6000.5.0f1\Editor\Unity.exe" `
  -projectPath "C:\Users\Vladimir Kamashev\Desktop\12\Wildhaven" `
  -executeMethod GameTestLauncher.Run -batchmode `
  -logFile "$env:TEMP\WildhavenTest.log"
Get-Content "$env:TEMP\WildhavenTest.log" | Select-String "\[RUNTEST\]"
```
