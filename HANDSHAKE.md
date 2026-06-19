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
