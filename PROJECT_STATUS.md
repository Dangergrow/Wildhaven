# Wildhaven — СОСТОЯНИЕ ПРОЕКТА

> 22.06.2026 | 95 C# скрипт | Unity 6 URP | 0 заглушек (кроме Multiplayer)

## Что СДЕЛАНО сегодня (РАБОЧИЙ ПК)

### ПКМ контекстное меню
- ПКМ по колонисту в SELECT-режиме → контекстное меню (OnGUI)
- Приказы: Move here, Attack nearest, Pick up, Prioritize work, Heal, Deselect
- Heal мгновенное (+20 HP), Attack находит ближайшего Enemy
- ЛКМ вне меню → закрывает меню

### F4 Orders панель
- OrderMarkerSystem — маркеры на гриде (Mine/Chop/Harvest/Hunt/Haul/Deconstruct)
- Цветные кубы-маркеры: коричневый=шахта, зелёный=рубка, жёлтый=сбор, красный=охота, голубой=переноска, пурпурный=снос
- GameBar: кнопки [1-6] Orders, ЛКМ ставит маркер
- ColonistAI выполняет заказы: Mine=RemoveBlock, Chop=добыча дерева, Harvest=сбор растений, Hunt=охота в AnimalManager, Deconstruct=слом

### Стартовые ресурсы колонистам
- ColonistSpawner.GiveStartingResources() — каждому колонисту при старте:
  - 5 RationPack, 2 Bread, 4 Berries (еда на 3 дня)
  - 3 Bandage
  - Инструменты: первый=Pickaxe, второй=Axe, третий=Knife

### Визуализация урона
- FloatingText.Spawn() — world-space TextMesh, летит вверх, затухает за 1.5с
- Красный "-N" на Enemy.TakeDamage(), жёлтый на Colonist.TakeDamage()
- Зелёный "+N" на Colonist.Heal()

### Кулинария — больше рецептов
- +16 рецептов: Steak, Fish Fillet, Berry Jam, Pemmican, Cannabis Brownie, Oatmeal, Baked Potato, Sushi, Mushroom Tea, Fish Stew, Mutton Chop, Roasted Boar, Hardtack, Honeyed Berries, Vegetable Soup
- Всего теперь 31 рецепт (было 16)

### Животные — больше видов
- +8 типов: Camel, Mammoth, Llama, Ostrich, Tiger, Crocodile, GiantSpider, Eagle
- Всего 20 типов животных
- Capsule-модельки с размером по типу, уникальные цвета
- 25 животных при старте (было 15)

## Что ИСПРАВЛЕНО в прошлой сессии (РАБОЧИЙ ПК 19.06)
- BlueprintManager → колонисты строят blueprint'ы ✅
- MainMenu → Settings открывает GameSettings, About показывает версию ✅
- ZoneDesignator → F3 режим зон, 4 типа, видимые маркеры ✅
- WorkPanel → зелёная подсветка выбранного приоритета ✅
- CentralIntegration → уважает паузу ✅
- EventSystem → авто-создаётся для Canvas UI ✅
- CanvasHUD → видим в редакторе ✅
- BuildManager → авто-находит камеру и грид ✅
- Шрифты → UIFont.Get() с fallback на Arial ✅
- Старый GameHUD удалён (конфликт с CanvasHUD) ✅
- Сохранения → Continue проверяет наличие файла ✅
- Портреты → плавное обновление без мерцания ✅
- FloorController → Tab/Shift+Tab этажи ✅
- GameSettings → доступны из PauseMenu ✅

## Архитектура (ключевые точки входа)
1. CentralIntegration (Core/) — оркестратор, авто-создаётся
2. GameManager (Core/) — создаёт 30+ систем
3. GridManager (World/) — на "World" GameObject
4. DayCycle — время, пауза, скорость
5. ColonistSpawner — 3 колониста (gameStarted=true в редакторе)
6. CanvasHUD — время, ресурсы, портреты
7. GameBar — F1-F4 нижняя панель с кнопками блоков
8. SelectionManager — B=BUILD/SELECT, грид-выделение, ПКМ-меню
9. BuildManager — стройка (1-9, ЛКМ/ПКМ, Shift=blueprint/demolish)
10. PauseMenu — Esc (OnGUI)
11. OrderMarkerSystem — F4 маркеры на гриде

## Управление (полное)
WASD/стрелки=камера | Q/E=поворот | Колёсико=зум | MMB=свободный поворот | Home=сброс
Tab=этаж↑ | Shift+Tab=этаж↓
1-9=блок | Shift+1-9=продвинутый | ЛКМ=поставить | ПКМ=сломать | Shift+ПКМ=3×3 | Shift+ЛКМ=blueprint | ЛКМ+drag=линия
F1=Architect | F2=Work | F3=Zone | F4=Orders | []=страницы
Space=пауза | Num1/2/3=скорость | B=BUILD/SELECT | M=карта | Esc=меню | F5=сохранить | F9=загрузить
ПКМ по колонисту = контекстное меню (Move/Attack/Pickup/Prioritize/Heal)

## Поток игры
MainMenu(6кнопок) → WorldSettings(seed/размер/сложность) → CharacterCreator(3 колониста) → Игра
В редакторе: авто-пропуск меню, сразу игра.

## Что осталось
1. Multiplayer (Mirror + Steamworks) — требуется домашний ПК
2. Ассеты — 3D модели (Mixamo/Kenney), звуки (Freesound), анимации
3. Pathfinder A* — есть, частично интегрирован для приказов
4. Торговля — TradeUI готов, ждёт события каравана
5. Визуализация температуры/качества комнат — логика есть, нет отображения

## Для новой сессии
Прочитай: PROJECT_STATUS.md → HANDSHAKE.md → SETUP.md → .docs/DEVELOPER_GUIDE.md
Запусти: открыть SampleScene → Play (авто-старт в редакторе)
Проверь: F2, F3, B, M, Esc, Shift+ЛКМ, Shift+ПКМ, ПКМ по колонисту, F4 Orders
