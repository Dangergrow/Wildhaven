# Handshake — передача контекста между сессиями

> РАБОЧИЙ ↔ ДОМАШНИЙ | Последние записи сверху

---

## ▶ Сессия: 18.06.2026 | ПК: ДОМАШНИЙ → ПК: РАБОЧИЙ

### Что сделано на ДОМАШНЕМ ПК

**Рендер:**
- Мир 100×32×100, Perlin-шум, горы со снежными шапками (y≥27)
- Submeshes per block type + URP/Unlit материалы с `_BaseColor`
- Шейдер `Wildhaven/Block`: Cull Off, диффузное освещение, текстурный _BaseMap
- Процедурный текстурный atlas 128×64 (Point filter, 16×16 per cell, 8×4 grid)

**Сетка:**
- Чанки 16×16×16: каждый чанк — отдельный GameObject с MeshFilter/MeshRenderer
- SetBlock/Rebuild — пересчитывается только грязный чанк + соседи на границе
- Raycast по сетке (Amanatides-Woo упрощённый), без физики

**Камера:**
- Новый Input System: WASD/arrows, Q/E rotate 45°, scroll zoom, MMB free rotate
- Home = сброс на (50, 50, 50)

**Стройка (BuildManager):**
- ЛКМ — поставить блок, ПКМ — сломать, 1-9 — выбор типа (Dirt/Grass/Stone/Wood/Glass/StoneBrick/WoodPlanks/Sand/Snow)
- Ссылка на камеру через `_cam` (serialized field), не Camera.main
- OnGUI: подсказка в левом верхнем углу

**Сохранения:**
- BinaryWriter в `Application.persistentDataPath/world.sav`
- Формат: 'W''H''V''N' + width(int) + height(int) + depth(int) + seed(int) + grid(byte[])
- F5 = сохранить, F9 = загрузить
- Автозагрузка при Awake если файл есть

### Важные файлы
- `Assets/Scripts/World/GridManager.cs` — всё: сетка, чанки, генерация, меш, сохранения
- `Assets/Scripts/World/BuildManager.cs` — стройка, выбор блоков
- `Assets/Scripts/World/GridCell.cs` — структура клетки
- `Assets/Scripts/Core/BlockType.cs` — enum 24 типа
- `Assets/Scripts/Camera/CameraController.cs` — камера
- `Assets/Shaders/Block.shader` — шейдер (Cull Off, NdotL, текстура)
- `Assets/Material/BlockMaterial.mat` — материал (Shader=Wildhaven/Block, Color=белый)

### Настройка сцены (уже сделано)
- **World**: Position(0,0,0), компонент GridManager, Block Material = BlockMaterial
- **Camera**: Position(50,55,50), Rotation(75,45,0), компонент CameraController
- **BuildManager**: компонент BuildManager, GridManager → World, Cam → Camera

### Ключевые баги исправлены
- `Camera.main` = null → serialized field `_cam` в BuildManager
- `blockMaterial` vs `baseMaterial` → имя поля должно совпадать с сериализацией
- `Cull Back` скрывал грани → `Cull Off`
- `using var` без скобок → `using (...) { }`

### Что дальше (на РАБОЧЕМ ПК)
- Greedy meshing внутри чанков (убрано при переходе на чанки, сейчас per-face)
- Настоящие текстуры (PNG atlas) вместо процедурных
- Этап 2 SETUP.md: колонисты, AI, потребности
- Mirror + Steamworks для мультиплеера
- UI: инвентарь, панели строительства

---

## Архив
