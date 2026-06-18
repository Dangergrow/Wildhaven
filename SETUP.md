# Wildhaven — колони-сим с мультиплеером (Unity + AI Agents)

## Что мы делаем

**Wildhaven** — игра в духе Going Medieval, но со своей вселенной, расширенными механиками и мультиплеером до трёх игроков через Steam (Spacewar).

### Ключевые фичи

- Воксельный мир: строительство, терраформинг, слои земли/камня/руды
- Колонисты с потребностями, навыками, расписанием дня
- Ресурсы, крафт, цепочки производства
- Враги, рейды, оборона, ловушки
- Мультиплеер до 3 человек через Mirror + Steamworks (Spacewar AppID)
- Свой лор: 3 фракции, биомы, тех-древо, дипломатия, события
- Сезоны, погода, животные, охота

### Архитектура разработки

- **Основной ИИ (DeepSeek V4 Pro)** — стратегия, архитектура, распределение задач между агентами, контроль качества
- **general-агент (NEO-CODE 27B)** — пишет код, реализует механики
- **ui-designer (NEO-CODE 27B)** — интерфейсы, HUD, меню
- **explore (gemma2:9b)** — поиск по кодовой базе, анализ

### План этапов (подробно в плане)

| Этап | Что | Время |
|---|---|---|
| 0 | Подготовка окружения | Сегодня |
| 1 | Воксельный мир + строительство | 2-3 недели |
| 2 | Колонисты, AI, потребности | 2 недели |
| 3 | Ресурсы, крафт, производство | 2 недели |
| 4 | Враги, бой, оборона | 2-3 недели |
| 5 | Мультиплеер (Mirror + Steam) | 6-8 недель |
| 6 | Свой сеттинг + доп. механики | 4-6 недель |
| 7 | UI/UX, звук, полировка | 3-4 недели |
| 8 | Сборка, тестирование, релиз | 2-3 недели |

## Установка окружения с нуля

### 1. Ollama + модели

```powershell
# Установить Ollama с https://ollama.com/download/windows

# Модель для explore-агента (быстрая, ~5.4 GB, влезает в VRAM)
ollama pull gemma2:9b

# Модель для general + ui-designer (кодовая, 16.9 GB, ~70% на GPU)
ollama run hf.co/DavidAU/Qwen3.6-27B-Heretic-Uncensored-FINETUNE-NEO-CODE-Di-IMatrix-MAX-GGUF:Q4_K_M
```

### 2. Unity

```powershell
# Скачать Unity Hub с https://unity.com/download
# В Unity Hub → Installs → Install Editor → Unity 6 LTS
# При установке выбрать модули: Windows Build Support, WebGL (опционально)
```

### 3. Visual Studio Code

```powershell
# Скачать с https://code.visualstudio.com
# Расширения: C#, C# Dev Kit, Unity
```

### 4. Git

```powershell
git clone https://github.com/Dangergrow/Wildhaven.git
```

### 5. Steam + Spacewar (для мультиплеера)

```powershell
# Установить Steam. Затем в браузере:
steam://run/480
# Дождаться установки Spacewar в библиотеке Steam
```

### 6. Открыть проект

```powershell
cd Wildhaven
# Запустить opencode в этой папке
# HANDSHAKE.md загрузится автоматически (прописан в opencode.json → instructions)
# При первом запуске также прочитает SETUP.md для понимания проекта
```

## Агенты

Конфиг в `.opencode/agents/`. Агенты загружаются автоматически при запуске opencode в папке проекта.

| Агент | Модель | Назначение |
|---|---|---|
| explore | gemma2:9b | Поиск по коду, анализ файлов |
| general | NEO-CODE 27B | Написание кода, логика, архитектура |
| ui-designer | NEO-CODE 27B | UI/UX, верстка, интерфейсы |

## Технологический стек

- **Движок**: Unity 6 LTS (C#)
- **Мультиплеер**: Mirror + Steamworks (Spacewar AppID)
- **Графика**: Voxel-движок, стилизация low-poly
- **Ассеты**: Kenney.nl, Quaternius.com (бесплатно)
- **Контроль версий**: Git + GitHub
