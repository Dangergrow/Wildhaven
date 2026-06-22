# Handshake — передача контекста между сессиями

> РАБОЧИЙ ↔ ДОМАШНИЙ | Последние записи сверху

---

## ⚠️ ЖЕЛЕЗНОЕ ПРАВИЛО ДЛЯ ВСЕХ ИИ ⚠️

**При ЛЮБОМ изменении кода, ЛЮБОМ написании, ЛЮБОМ создании — сначала прочитай документацию в `.docs/`!**

Без исключений. Прежде чем писать код, открой соответствующий .md файл в `.docs/` и изучи API.

Критические правила Unity 6:
- `?.` и `??` НЕ РАБОТАЮТ с Unity-объектами после Destroy
- `OnGUI()` вызывается МНОГО раз за кадр — только GUI, не логика
- `MonoBehaviour` с Update/OnGUI имеет чекбокс вкл/выкл в инспекторе
- `Camera.main` может быть null
- `Material.color` может не мапиться на `_BaseColor` в URP — использовать `SetColor`

---

---

## ▶ Сессия: 22.06.2026 | ПК: РАБОЧИЙ → ПК: ДОМАШНИЙ

### Что сделано (6 фич)

1. **ПКМ контекстное меню** — SelectionManager: RMB по колонисту в SELECT-режиме показывает меню (Move here / Attack nearest / Pick up / Prioritize / Heal / Deselect). Реализовано через `OrderType.Haul` и `OrderType.Prioritize` в ColonistAI. Attack ищет ближайшего Enemy, Heal лечит +20HP мгновенно.
   - Файлы: `UI/SelectionManager.cs`, `Colonists/ColonistAI.cs`
2. **F4 Orders панель** — GameBar: кнопки [1-6] Mine/Chop/Harvest/Hunt/Haul/Deconstruct, ЛКМ ставит цветной куб-маркер. `OrderMarkerSystem` (World/) хранит список заказов, `ColonistAI` выполняет ближайший. Mine=RemoveBlock, Chop=добыча дерева, Harvest=сбор PlantGrowth, Hunt=AnimalManager.Hunt(), Deconstruct=слом блока.
   - Файлы: `World/OrderMarkerSystem.cs`, `UI/GameBar.cs`
3. **Стартовые ресурсы** — `ColonistSpawner.GiveStartingResources()`: 5 RationPacks, 2 Bread, 4 Berries, 3 Bandages. Инструменты: 1-й=Pickaxe, 2-й=Axe, 3-й=Knife.
   - Файл: `Colonists/ColonistSpawner.cs`
4. **Визуализация урона** — `FloatingText.Spawn()` создаёт TextMesh, летит вверх, затухает 1.5с. Enemy.TakeDamage=красный, Colonist.TakeDamage=жёлтый, Colonist.Heal=зелёный.
   - Файлы: `UI/FloatingText.cs`, `Combat/Enemy.cs`, `Colonists/Colonist.cs`
5. **Кулинария** — +16 рецептов (Steak, Fish Fillet, Berry Jam, Pemmican, Cannabis Brownie и др.), всего 31 рецепт.
   - Файл: `Resources/CookingSystem.cs`
6. **Животные** — +8 типов (Camel, Mammoth, Llama, Ostrich, Tiger, Crocodile, GiantSpider, Eagle), всего 20. Capsule-модели с уникальными цветами и размерами, 25 при старте.
   - Файл: `World/AnimalManager.cs`

### Важные изменения в API
- `ColonistAI.OrderType` расширен: `Haul`, `Prioritize` добавлены
- `ColonistAI.orderEnemy` — новое поле для Attack-приказов
- `SelectionManager` использует `GiveOrder()` вместо прямого присвоения полей
- `GameBar` зависит от `OrderMarkerSystem` (авто-создаётся в Start если нет)

### Файлы созданы
- `World/OrderMarkerSystem.cs`
- `UI/FloatingText.cs`

---

## ▶ Сессия: 19.06.2026 | ПК: РАБОЧИЙ (ФИНАЛ)

### Что сделано (Этапы 1-7)
**Этап 1:** Воксельный мир 100x32x100, чанки 16x16x16, greedy mesh, 9 типов блоков, вода, снег
**Этап 2:** Колонисты — Colonist, NeedsSystem (7 потребностей), ColonistAI (стейт-машина, блуждание, еда из инвентаря), DayCycle, Spawner, JobManager, RecreationManager, MentalState (срывы), ColonistGravity, WaterInteraction, BuildBlocker
**Этап 3:** Ресурсы — 70+ типов предметов, Inventory (стаки, слоты), ItemData SO, RecipeData, CraftingStation, BlockDropManager (дроп при разрушении → физика → летит к колонисту), Equipment (оружие/броня)
**Этап 4:** Глобальная карта — HexTile, WorldMapGenerator (15 биомов, острова, фракции), Caravan (путь, события, припасы), FactionManager (репутация -100..+100)
**Этап 5:** Бой — Enemy (AI, атака, дроп), RaidManager (волны), Trap (4 типа), колонисты автоматически атакуют врагов
**Этап 7:** События — EventManager (20+ типов), ResearchManager (тех-древо 60+ исследований, 5 эпох)
**Доп. системы:** PlantGrowth (8 культур, фермерство), SocialSystem (отношения, брак, драки), Equipment (слоты экипировки)
**Этап 8:** UI — GameHUD (время, статы колонистов, панель строительства), SelectionManager (B = вкл/выкл стройку, выделение колонистов)

### ПРОБЛЕМЫ (нужно исправить дома)
1. **Выделение колонистов не работает**: SelectionManager пытается искать лучом от камеры, но не находит колонистов. Пробовал Physics.Raycast, GridRaycast, ray-distance math — ничего. Колонисты имеют CapsuleCollider (добавляется в SpawnColonist). Нужно переписать SelectionManager.HandleInput с нуля.
2. **Колонисты иногда проваливаются**: ColonistGravity и WaterInteraction на префабе, но 1-2 всё равно уходят под текстуры. Возможно конфликт с AI-блужданием.
3. **Пауза**: работает через проверку IsPaused в каждом Update, но не все скрипты проверяют (BuildManager не паузится).
4. **Скорость времени**: Num1/2/3 меняет gameSpeed, но не все системы учитывают (движение колонистов учитывает, но не всё).

### Что нужно добавить в Unity
- На GameManager: **Game HUD**, **Event Manager**, **Raid Manager**, **Research Manager**, **Selection Manager**
- На World: **Block Drop Manager**
- На префаб Colonist: **Colonist**, **ColonistAI**, **NeedsSystem**, **ColonistGravity**, **WaterInteraction**, **BuildBlocker**, **MentalState**, **Inventory**, **UnitPhysics** (опционально)
- Префаб: `Assets/Colonist.prefab` (без пробела в имени!)
- Старый сейв удалять: `%APPDATA%/LocalLow/DefaultCompany/Wildhaven/world.sav`

### Следующий шаг (для ДОМАШНЕГО)
- Починить выделение колонистов (SelectionManager) — это приоритет
- Этап 6: Мультиплеер Mirror + Steamworks
- Довести до ума UI (Canvas вместо OnGUI)
- Анимации (Mixamo)
- Главное меню, настройки

### Личное сообщение от РАБОЧЕГО
Я отупел на выделении колонистов. Три раза переписывал SelectionManager — физика, грид-рейкаст, луч-дистанция — и всё равно не выбирает. Остальное работает. Пожалуйста, почини это и посмейся надо мной.

---

## ▶ Сессия: 18.06.2026 | ПК: ДОМАШНИЙ → ПК: РАБОЧИЙ

### Что сделано (полный список)
**Рендер:** мир 100×32×100, Perlin-шум, горы+снег(y≥27), вода(seaLevel=10)
**Чанки:** 16×16×16, greedy meshing, SetBlock → только грязные чанки
**Стройка:** 9 типов (1-9), ЛКМ поставить ПКМ сломать, рейкаст по сетке
**Камера:** новый Input System, WASD/QE/зум/MMB/Home
**Сохранения:** BinaryWriter, F5/F9, автозагрузка
**Архитектура:** BlockDatabase ScriptableObject для цветов блоков
**Шейдер:** Wildhaven/Block — Cull Off, NdotL диффуз, текстурный atlas

### Ключевые баги (исправлены)
- `Camera.main` = null → serialized field `_cam`
- `blockMaterial` имя поля должно совпадать с сериализацией
- `Cull Off` вместо `Cull Back` (вининг нестабилен)
- `using var` без скобок не работает в этом C# → `using(...){ }`

---

## Архив
