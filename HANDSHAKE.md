# Handshake — передача контекста между сессиями

> РАБОЧИЙ (openocode на рабочем ПК) ↔ ДОМАШНИЙ (opencode на домашнем ПК)
> Формат: последние записи сверху.

---

## ▶ Сессия: 18.06.2026 | ПК: ДОМАШНИЙ

### Что сделано
- Изучена вся документация проекта (SETUP.md, HANDSHAKE.md, opencode.json, agent configs)
- Изучен существующий код: GridManager, GridCell, BlockType, BlockData, CameraController
- Все агенты переведены на DeepSeek V4 Pro: explore, general, ui-designer (баг Ollama #18423)
- Всем агентам прописаны Wildhaven-специфичные промты: структура проекта, код-стандарты, UI/UX правила из SETUP.md
- Добавлен `instructions` в opencode.json: SETUP.md + HANDSHAKE.md (как и требовалось по доке)
- Основной ИИ: DeepSeek V4 Pro (стратегия/архитектура/контроль)

### Что в процессе
- Установка Unity Hub + Unity 6 LTS на ДОМАШНЕМ ПК
- Steam установлен, ожидается запуск Spacewar (steam://run/480)
- Ждём завершения установки Unity для старта Этапа 1

### Следующий шаг
- Завершить установку Unity 6 LTS + Windows Build Support
- Запустить Spacewar через Steam
- Открыть проект в Unity, проверить что сцена работает (генерация + камера)
- Начать Этап 1: воксельный мир + строительство (добить меш-генерацию, систему строительства)

---

## ▶ Сессия: 18.06.2026 | ПК: РАБОЧИЙ

### Что сделано
- Настроен проект Wildhaven: репозиторий, гейм-дизайн документ (SETUP.md), система handshake
- Установлены: Ollama, Node.js, Bun, opencode-ollama плагин
- Модели Ollama: gemma2:9b, neo-code:27b (NEO-CODE), qwen2.5:14b, qwen2.5-coder:7b, nomic-embed-text
- Агенты opencode: explore, general, ui-designer (все на deepseek/deepseek-v4-pro)
- Причина: Ollama агенты не работают из-за бага opencode #18423
- Конфиг Ollama готов (`npm: @ai-sdk/openai-compatible`), ждём фикса
- @ai-sdk/openai-compatible установлен глобально

### Что в процессе
- Установка Unity Hub + Unity 6 LTS
- Ожидание: Steam + Spacewar, Mirror, ParrelSync

### Следующий шаг
- Завершить установку Unity
- Скачать Steam + Spacewar для мультиплеера
- Начать Этап 1: Воксельный мир + строительство

---

## Архив

<!-- Завершённые сессии ниже -->
