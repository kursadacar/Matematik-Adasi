using UnityEngine;

[CreateAssetMenu(fileName = "Text Database", menuName = "MatOgretici/Text Database")]
public class Language : ScriptableObject
{
    [Header("Language Prefix")]
    public string languagePrefix;

    [Header("Language Icon")]
    public Sprite languageIcon;

    [Header("Common")]
    public string yes;
    public string no;
    public string _continue;
    public string close;
    public string end;
    public string back;

    [Header("Main Menu")]
    public string main_menu_header;
    public string main_menu_resume_game;
    public string main_menu_start_game;
    public string main_menu_settings;
    public string main_menu_credits;
    public string main_menu_reset;
    public string main_menu_exit;
    public string main_menu_are_you_sure_to_reset;

    [Header("Settings Menu")]
    public string settings_menu_master_volume;
    public string settings_menu_music_volume;
    public string settings_graphics_quality;
    public string settings_quality_level_1;
    public string settings_quality_level_2;
    public string settings_quality_level_3;
    public string settings_quality_level_4;

    [Header("In Game Menu")]
    public string in_game_menu_go_fishing;
    public string in_game_menu_closest_market;
    public string in_game_menu_inventory;
    public string in_game_menu_stop;

    [Header("Timeout")]
    public string timeout_text;

    [Header("Credits")]
    public string credits_designer;
    public string credits_graphics;
    public string credits_programming;
    public string credits_ui;
    public string credits_testing;

    [Header("Fishing")]
    public string fishing_rod_broken;
    public string fishing_correct_answer;
    public string fishing_wrong_answer;
    public string fishing_caught_fish;
    public string fishing_you_need_rod;
    public string fishing_rod_health_reduced;

    [Header("Shop")]
    public string shop_buy;
    public string shop_sell_equipment;
    public string shop_sell_fish;
    public string shop_not_enough_money;
    public string shop_sell_item_one;
    public string shop_sell_item_all;
    public string shop_preview_item;
    public string shop_stop_preview;
    public string shop_you_have_bought;
    public string shop_cant_but_rod;

    [Header("Inventory")]
    public string inventory_view_equipment;
    public string inventory_view_fish;
    public string inventory_wear;
    public string inventory_take_off;

    [Header("Question")]
    public string question_ask_result_of_equation;


    [Header("Tutorial Menus")]
    public string tutorial_main_welcome;
    public string tutorial_main_ask_for_name;
    public string tutorial_main_ask_for_name_placeholder;
    public string tutorial_main_shops_and_fishing;
    public string tutorial_main_items_to_buy;
    public string tutorial_main_to_fish;
    public string tutorial_main_to_buy;

    public string tutorial_shop_welcome;
    public string tutorial_shop_can_buy_equipment;
    public string tutorial_shop_can_overview_equipment;
    public string tutorial_shop_buy_tutorial;
    public string tutorial_shop_sell_tutorial;
    public string tutorial_shop_final;

    public string tutorial_fish_welcome;
    public string tutorial_fish_how_to;
    public string tutorial_fish_consequenses;
    public string tutorial_fish_consequenses_deep1;
    public string tutorial_fish_consequenses_deep2;
    public string tutorial_fish_fish_final;

    public string tutorial_common_click_to_continue;
    public string tutorial_common_click_to_start_game;
    public string tutorial_common_click_to_end;

    [Header("Tip Menus")]
    public string tip_natural_numbers_addition;
    public string tip_natural_numbers_substraction;
    public string tip_natural_numbers_multiplication;
    public string tip_natural_numbers_division;

    public string tip_fractions_addition;
    public string tip_fractions_substraction;
    public string tip_fractions_multiplication;
    public string tip_fractions_division;
}
