using System.Collections.Generic;

/// <summary>Simple localization with RUS/ENG support. Add strings here, use L10n.Get("key").</summary>
public static class L10n
{
    public static int language; // 0=ENG, 1=RUS

    static Dictionary<string, string[]> _dict = new()
    {
        // MainMenu
        {"menu_title",    new[]{"WILDHAVEN", "WILDHAVEN"}},
        {"menu_subtitle", new[]{"Colony Simulator", "Колониальный симулятор"}},
        {"menu_newgame",  new[]{"New Game", "Новая игра"}},
        {"menu_continue", new[]{"Continue", "Продолжить"}},
        {"menu_multi",    new[]{"Multiplayer", "Мультиплеер"}},
        {"menu_settings", new[]{"Settings", "Настройки"}},
        {"menu_about",    new[]{"About", "Об игре"}},
        {"menu_quit",     new[]{"Quit", "Выход"}},
        // GameSettings
        {"set_title",     new[]{"SETTINGS", "НАСТРОЙКИ"}},
        {"set_audio",     new[]{"AUDIO", "ЗВУК"}},
        {"set_music",     new[]{"Music", "Музыка"}},
        {"set_sfx",       new[]{"SFX", "Эффекты"}},
        {"set_video",     new[]{"VIDEO", "ВИДЕО"}},
        {"set_quality",   new[]{"Quality", "Качество"}},
        {"set_fullscreen",new[]{"Fullscreen", "Полный экран"}},
        {"set_fps",       new[]{"FPS Cap", "Огр. FPS"}},
        {"set_lang",      new[]{"LANGUAGE", "ЯЗЫК"}},
        {"set_langval",   new[]{"ENG", "РУС"}},
        {"set_keybinds",  new[]{"KEY BINDINGS", "КЛАВИШИ"}},
        {"set_close",     new[]{"CLOSE", "ЗАКРЫТЬ"}},
        // About
        {"about_text",    new[]{"WILDHAVEN v0.1\n\nColony Simulator | Unity 6 URP | C#\n\nInspired by Going Medieval & RimWorld\nBuilt with OpenCode AI Agents",
                                "WILDHAVEN v0.1\n\nКолониальный симулятор | Unity 6 URP | C#\n\nВдохновлён Going Medieval и RimWorld\nСоздан с помощью OpenCode AI Agents"}},
        // WorldSettings
        {"ws_planet",     new[]{"CHOOSE PLANET", "ВЫБОР ПЛАНЕТЫ"}},
        {"ws_earth",      new[]{"Earthlike", "Землеподобная"}},
        {"ws_desert",     new[]{"Desert World", "Пустынный мир"}},
        {"ws_ice",        new[]{"Ice World", "Ледяной мир"}},
        {"ws_jungle",     new[]{"Jungle World", "Мир джунглей"}},
        {"ws_dead",       new[]{"Dead World", "Мёртвый мир"}},
        {"ws_size",       new[]{"MAP SIZE", "РАЗМЕР КАРТЫ"}},
        {"ws_climate",    new[]{"CLIMATE & BIOMES", "КЛИМАТ И БИОМЫ"}},
        {"ws_diff",       new[]{"DIFFICULTY & MODIFIERS", "СЛОЖНОСТЬ И МОДИФИКАТОРЫ"}},
        {"ws_peaceful",   new[]{"Peaceful", "Мирный"}},
        {"ws_normal",     new[]{"Normal", "Нормальный"}},
        {"ws_brutal",     new[]{"Brutal", "Жестокий"}},
        {"ws_raids",      new[]{"Raid Frequency", "Частота рейдов"}},
        {"ws_rare",       new[]{"Rare", "Редко"}},
        {"ws_often",      new[]{"Often", "Часто"}},
        {"ws_apo_off",    new[]{"Apocalyptic: OFF", "Апокалипсис: ВЫКЛ"}},
        {"ws_apo_on",     new[]{"APOCALYPTIC: ON", "АПОКАЛИПСИС: ВКЛ"}},
        {"ws_pvp_off",    new[]{"PvP: Disabled", "PvP: Выключен"}},
        {"ws_pvp_on",     new[]{"PvP: ENABLED", "PvP: ВКЛЮЧЁН"}},
        {"ws_seed",       new[]{"Seed", "Ключ"}},
        {"ws_back",       new[]{"BACK", "НАЗАД"}},
        {"ws_next",       new[]{"NEXT", "ДАЛЕЕ"}},
        {"ws_start",      new[]{"START GAME", "НАЧАТЬ ИГРУ"}},
        // PauseMenu
        {"pause_title",   new[]{"PAUSE", "ПАУЗА"}},
        {"pause_continue",new[]{"Continue", "Продолжить"}},
        {"pause_save",    new[]{"Save Game", "Сохранить"}},
        {"pause_load",    new[]{"Load Game", "Загрузить"}},
        {"pause_settings",new[]{"Settings", "Настройки"}},
        {"pause_menu",    new[]{"Main Menu", "Главное меню"}},
        {"pause_quit",    new[]{"Quit", "Выход"}},
        // CharacterCreator
        {"cc_points",     new[]{"Points", "Очки"}},
        {"cc_colonist",   new[]{"Colonist", "Колонист"}},
        {"cc_skills",     new[]{"Skills", "Навыки"}},
        {"cc_perk",       new[]{"Perk", "Перк"}},
        {"cc_flaw",       new[]{"Flaw", "Недост."}},
        {"cc_backstory",  new[]{"Backstory", "Прошлое"}},
        {"cc_name",       new[]{"Name", "Имя"}},
        {"cc_age",        new[]{"Age", "Возраст"}},
        {"cc_hair",       new[]{"Hair", "Причёска"}},
        {"cc_body",       new[]{"Body", "Тело"}},
        {"cc_random",     new[]{"Random Colonist", "Случайный колонист"}},
        {"cc_start",      new[]{"START GAME", "НАЧАТЬ ИГРУ"}},
        // HUD
        {"hud_paused",    new[]{"PAUSED", "ПАУЗА"}},
        {"hud_day",       new[]{"Day", "День"}},
        {"hud_floor",     new[]{"Floor", "Этаж"}},
        {"hud_colonists", new[]{"Colonists", "Колонистов"}},
        // Build/Orders/Zones
        {"build_mode",    new[]{"BUILD", "СТРОЙКА"}},
        {"select_mode",   new[]{"SELECT", "ВЫБОР"}},
        {"arch_mode",     new[]{"Architect", "Архитектор"}},
        {"work_mode",     new[]{"Work", "Работа"}},
        {"zone_mode",     new[]{"Zone", "Зона"}},
        {"order_mode",    new[]{"Orders", "Приказы"}},
        // Context menu
        {"ctx_move",      new[]{"Move here", "Идти сюда"}},
        {"ctx_attack",    new[]{"Attack nearest", "Атаковать"}},
        {"ctx_pickup",    new[]{"Pick up", "Подобрать"}},
        {"ctx_prioritize",new[]{"Prioritize work", "Приоритет"}},
        {"ctx_heal",      new[]{"Heal", "Лечить"}},
        {"ctx_deselect",  new[]{"Deselect", "Снять выбор"}},
    };

    public static string Get(string key)
    {
        if (_dict.TryGetValue(key, out string[] vals))
            return vals[language < vals.Length ? language : 0];
        return key;
    }

    public static void SetLanguage(int lang)
    {
        language = lang;
        var gs = UnityEngine.Object.FindFirstObjectByType<GameSettings>();
        if (gs != null) gs.languageIndex = lang;
    }
}
