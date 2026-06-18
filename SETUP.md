# Wildhaven — Colony Sim (Unity + AI Agents)

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
# Перед работой сказать opencode: "Прочитай HANDSHAKE.md"
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
