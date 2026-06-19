# Handshake — передача контекста между сессиями

> РАБОЧИЙ ↔ ДОМАШНИЙ | Последние записи сверху

---

## ▶ Сессия: 19.06.2026 | ПК: РАБОЧИЙ

### Что сделано (Этапы 1-7)
**Этап 1:** Воксельный мир 100x32x100, чанки 16x16x16, greedy mesh, 9 типов блоков, вода, снег
**Этап 2:** Колонисты — Colonist, NeedsSystem (7 потребностей), ColonistAI (стейт-машина), DayCycle, Spawner, JobManager, RecreationManager
**Этап 3:** Ресурсы — 70+ типов предметов, Inventory (стаки, слоты), ItemData SO, RecipeData, CraftingStation, BlockDropManager (дроп при разрушении → летит к колонисту)
**Этап 4:** Глобальная карта — HexTile, WorldMapGenerator (15 биомов, острова, фракции), Caravan (путь, события, припасы), FactionManager (репутация -100..+100)
**Этап 5:** Бой — Enemy (AI, атака, дроп), RaidManager (волны), Trap (4 типа)
**Этап 7:** События — EventManager (11 типов), ResearchManager (тех-древо 5 эпох)
**Этап 8:** UI — GameHUD (время, статы колонистов, панель строительства)

### Что нужно добавить в Unity
- На GameManager добавь: **Game HUD**, **Event Manager**, **Raid Manager**, **Research Manager**
- Колонисты: красные капсулы 0.8x, спавн на поверхности

### Следующий шаг
- Домашний ПК: Этап 6 (мультиплеер Mirror + Steamworks)
- Дополнить ResearchManager всеми 60+ исследованиями из SETUP.md
- Анимации (Mixamo)
- Главное меню, настройки

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
**Агенты:** DeepSeek V4 Pro, llava:13b для скриншотов

### Ключевые баги (исправлены)
- `Camera.main` = null → serialized field `_cam`
- `blockMaterial` имя поля должно совпадать с сериализацией
- `Cull Off` вместо `Cull Back` (вининг нестабилен)
- `using var` без скобок не работает в этом C# → `using(...){ }`

### Что дальше (Этап 2)
- Колонисты: модельки, AI, потребности (SETUP.md раздел "Колонисты")
- Анимации: Mixamo + Blend Trees
- Mirror + Steamworks для мультиплеера
- UI: панели строительства, инвентарь

### Инструкция для РАБОЧЕГО ПК
1. `git pull` в папке проекта
2. Удалить `world.sav` (AppData/LocalLow/DefaultCompany/Wildhaven/)
3. Открыть SampleScene, Play — проверить мир

---

## Архив
