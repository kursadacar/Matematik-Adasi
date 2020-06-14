using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_Language_Setter : Singleton<UI_Language_Setter>
{
    [SerializeField]
    Text
        main_menu_header,
        main_menu_start_game,
        main_menu_settings,
        main_menu_credits,
        main_menu_reset,
        main_menu_exit,
        main_menu_are_you_sure_to_reset,
        main_menu_are_you_sure_to_reset_yes,
        main_menu_are_you_sure_to_reset_no,

        in_game_menu_go_fishing,
        in_game_menu_closest_market,
        in_game_menu_inventory,
        in_game_menu_stop,

        fish_question_answered_end_fishing,
        fish_quit_without_answering,

        settings_menu_master_volume,
        settings_menu_music_volume,
        settings_menu_graphis_quality,
        settings_menu_back,

        timeout,

        credits_designer,
        credits_graphics,
        credits_programming,
        credits_testing,
        credits_ui,

        shop_buy_menu_text,
        shop_sell_equipment_menu_text,
        shop_sell_fish_menu_text,
        shop_back,

        inventory_switch_type,
        inventory_close,

        tutorial_main_welcome,
        tutorial_main_ask_for_name,
        tutorial_main_ask_for_name_placeholder,
        tutorial_main_ask_for_name_continue,
        tutorial_main_shops_and_fishing,
        tutorial_main_items_to_buy,
        tutorial_main_to_fish,
        tutorial_main_to_buy,

        tutorial_shop_welcome,
        tutorial_shop_can_buy_equipment,
        tutorial_shop_can_overview_equipment,
        tutorial_shop_buy_tutorial,
        tutorial_shop_sell_tutorial,
        tutorial_shop_final,

        tutorial_fish_welcome,
        tutorial_fish_how_to,
        tutorial_fish_consequenses,
        tutorial_fish_consequenses_deep1,
        tutorial_fish_consequenses_deep2,
        tutorial_fish_fish_final,

        tip_natural_numbers_addition,
        tip_natural_numbers_substraction,
        tip_natural_numbers_multiplication,
        tip_natural_numbers_division,

        tip_fractions_addition,
        tip_fractions_substraction,
        tip_fractions_multiplication,
        tip_fractions_division;
    [SerializeField]
    Dropdown settings_menu_quality;

    [SerializeField] List<Text> clickToStartGame = new List<Text>();
    [SerializeField] List<Text> clickToContinute = new List<Text>();
    [SerializeField] List<Text> clickToEnd = new List<Text>();

    [SerializeField] List<Text> _continue = new List<Text>();

    public delegate void LanguageSwitchHandler();
    public event LanguageSwitchHandler OnLanguageSwitched;


    public void SetLanguage(Language lang)
    {
        OnLanguageSwitched?.Invoke();

        main_menu_header.text = lang.main_menu_header;
        main_menu_start_game.text = lang.main_menu_start_game;
        main_menu_settings.text = lang.main_menu_settings;
        main_menu_credits.text = lang.main_menu_credits;
        main_menu_reset.text = lang.main_menu_reset;
        main_menu_exit.text = lang.main_menu_exit;
        main_menu_are_you_sure_to_reset.text = lang.main_menu_are_you_sure_to_reset;
        main_menu_are_you_sure_to_reset_yes.text = lang.yes;
        main_menu_are_you_sure_to_reset_no.text = lang.no;

        in_game_menu_inventory.text = lang.in_game_menu_inventory;
        in_game_menu_closest_market.text = lang.in_game_menu_closest_market;
        in_game_menu_go_fishing.text = lang.in_game_menu_go_fishing;
        in_game_menu_stop.text = lang.in_game_menu_stop;

        fish_question_answered_end_fishing.text = lang.end;
        fish_quit_without_answering.text = lang.end;

        settings_menu_master_volume.text = lang.settings_menu_master_volume;
        settings_menu_music_volume.text = lang.settings_menu_music_volume;
        settings_menu_graphis_quality.text = lang.settings_graphics_quality;
        settings_menu_quality.options[0].text = lang.settings_quality_level_1;
        settings_menu_quality.options[1].text = lang.settings_quality_level_2;
        settings_menu_quality.options[2].text = lang.settings_quality_level_3;
        settings_menu_quality.options[3].text = lang.settings_quality_level_4;
        settings_menu_back.text = lang.back;

        timeout.text = lang.timeout_text;

        credits_designer.text = lang.credits_designer;
        credits_graphics.text = lang.credits_graphics;
        credits_programming.text = lang.credits_programming;
        credits_ui.text = lang.credits_ui;
        credits_testing.text = lang.credits_testing;

        shop_buy_menu_text.text = lang.shop_buy;
        shop_sell_equipment_menu_text.text = lang.shop_sell_equipment;
        shop_sell_fish_menu_text.text = lang.shop_sell_fish;
        shop_back.text = lang.back;

        inventory_switch_type.text = lang.inventory_view_equipment;
        inventory_close.text = lang.close;

        foreach(var text in clickToStartGame)
        {
            text.text = lang.tutorial_common_click_to_start_game;
        }

        foreach(var text in clickToEnd)
        {
            text.text = lang.tutorial_common_click_to_end;
        }

        foreach(var text in clickToContinute)
        {
            text.text = lang.tutorial_common_click_to_continue;
        }

        foreach(var text in _continue)
        {
            text.text = lang._continue;
        }

        tutorial_main_welcome.text = lang.tutorial_main_welcome;
        tutorial_main_ask_for_name.text = lang.tutorial_main_ask_for_name;
        tutorial_main_ask_for_name_placeholder.text = lang.tutorial_main_ask_for_name_placeholder;
        tutorial_main_ask_for_name_continue.text = lang._continue;
        tutorial_main_shops_and_fishing.text = lang.tutorial_main_shops_and_fishing;
        tutorial_main_items_to_buy.text = lang.tutorial_main_items_to_buy;
        tutorial_main_to_fish.text = lang.tutorial_main_to_fish;
        tutorial_main_to_buy.text = lang.tutorial_main_to_buy;

        tutorial_shop_welcome.text = lang.tutorial_shop_welcome;
        tutorial_shop_can_buy_equipment.text = lang.tutorial_shop_can_buy_equipment;
        tutorial_shop_can_overview_equipment.text = lang.tutorial_shop_can_overview_equipment;
        tutorial_shop_buy_tutorial.text = lang.tutorial_shop_buy_tutorial;
        tutorial_shop_sell_tutorial.text = lang.tutorial_shop_sell_tutorial;
        tutorial_shop_final.text = lang.tutorial_shop_final;

        tutorial_fish_welcome.text = lang.tutorial_fish_welcome;
        tutorial_fish_how_to.text = lang.tutorial_fish_how_to;
        tutorial_fish_consequenses.text = lang.tutorial_fish_consequenses;
        tutorial_fish_consequenses_deep1.text = lang.tutorial_fish_consequenses_deep1.Replace("<br>", "\n");
        tutorial_fish_consequenses_deep2.text = lang.tutorial_fish_consequenses_deep2;
        tutorial_fish_fish_final.text = lang.tutorial_fish_fish_final;
    }

}
