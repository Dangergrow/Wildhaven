# Wildhaven — СОСТОЯНИЕ ПРОЕКТА

> 19.06.2026 | 91 C# скрипт | Unity 6 URP | 0 заглушек (кроме Multiplayer)

## Что ИСПРАВЛЕНО сегодня (РАБОЧИЙ ПК)
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
8. SelectionManager — B=BUILD/SELECT, грид-выделение
9. BuildManager — стройка (1-9, ЛКМ/ПКМ, Shift=blueprint/demolish)
10. PauseMenu — Esc (OnGUI)

## Управление (полное)
WASD/стрелки=камера | Q/E=поворот | Колёсико=зум | MMB=свободный поворот | Home=сброс
Tab=этаж↑ | Shift+Tab=этаж↓
1-9=блок | Shift+1-9=продвинутый | ЛКМ=поставить | ПКМ=сломать | Shift+ПКМ=3×3 | Shift+ЛКМ=blueprint | ЛКМ+drag=линия
F1=Architect | F2=Work | F3=Zone | F4=Orders | []=страницы
Space=пауза | Num1/2/3=скорость | B=BUILD/SELECT | M=карта | Esc=меню | F5=сохранить | F9=загрузить

## Поток игры
MainMenu(6кнопок) → WorldSettings(seed/размер/сложность) → CharacterCreator(3 колониста) → Игра
В редакторе: авто-пропуск меню, сразу игра.

## Что осталось (НЕ заглушки, а крупные фичи)
1. Multiplayer (Mirror + Steamworks) — требуется домашний ПК
2. Ассеты — 3D модели (Mixamo/Kenney), звуки (Freesound), анимации
3. Pathfinder A* — есть, частично интегрирован для приказов
4. Торговля — TradeUI готов, ждёт события каравана
5. Визуализация температуры/качества комнат — логика есть, нет отображения

## Для новой сессии
Прочитай: PROJECT_STATUS.md → HANDSHAKE.md → SETUP.md → .docs/DEVELOPER_GUIDE.md
Запусти: открыть SampleScene → Play (авто-старт в редакторе)
Проверь: F2, F3, B, M, Esc, Shift+ЛКМ, Shift+ПКМ
