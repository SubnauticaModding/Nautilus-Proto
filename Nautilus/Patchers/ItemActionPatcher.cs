﻿using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Utility;

namespace Nautilus.Patchers;

internal class ItemActionPatcher
{
    internal class CustomItemAction
    {
        internal Action<InventoryItem> Action;
        internal string LanguageLineKey;
        internal Predicate<InventoryItem> Condition;

        internal CustomItemAction(Action<InventoryItem> action, string tooltipKey, Predicate<InventoryItem> condition)
        {
            Action = action;
            LanguageLineKey = tooltipKey;
            Condition = condition;
        }
    }

    #region Internal Fields

    private static readonly Func<TechType, string> AsStringFunction = (t) => t.AsString();

    /// <summary>
    /// A constant <see cref="ItemAction"/> value to represent a custom middle click item action
    /// </summary>
    internal const ItemAction CustomMiddleClickItemAction = (ItemAction)1337;
    /// <summary>
    /// A constant <see cref="ItemAction"/> value to represent a custom left click item action
    /// </summary>
    internal const ItemAction CustomLeftClickItemAction = (ItemAction)1338;

    internal const string LeftClickMouseIcon = "<color=#ADF8FFFF></color>";
    internal const string MiddleClickMouseIcon = "<color=#ADF8FFFF></color>";

    internal static readonly IDictionary<TechType, CustomItemAction> MiddleClickActions = new SelfCheckingDictionary<TechType, CustomItemAction>("MiddleClickActions", AsStringFunction);
    internal static readonly IDictionary<TechType, CustomItemAction> LeftClickActions = new SelfCheckingDictionary<TechType, CustomItemAction>("LeftClickActions", AsStringFunction);

    #endregion

    #region Patching

    internal static void Patch(Harmony harmony)
    {
        // Direct access to private fields made possible by https://github.com/CabbageCrow/AssemblyPublicizer/
        // See README.md for details.

        harmony.Patch(AccessTools.Method(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnPointerClick)), 
            prefix: new HarmonyMethod(AccessTools.Method(typeof(ItemActionPatcher), nameof(ItemActionPatcher.OnPointerClick_Prefix))));

        harmony.Patch(AccessTools.Method(typeof(Inventory), nameof(Inventory.ExecuteItemAction),new Type[] { typeof(ItemAction), typeof(InventoryItem) }), 
            prefix: new HarmonyMethod(AccessTools.Method(typeof(ItemActionPatcher), nameof(ItemActionPatcher.ExecuteItemAction_Prefix))));

        harmony.Patch(AccessTools.Method(typeof(TooltipFactory), nameof(TooltipFactory.ItemActions)), 
            postfix: new HarmonyMethod(AccessTools.Method(typeof(ItemActionPatcher), nameof(ItemActionPatcher.ItemActions_Postfix))));

        if (MiddleClickActions.Count > 0 && LeftClickActions.Count > 0)
        {
            InternalLogger.Log($"Added {LeftClickActions.Count} left click actions and {MiddleClickActions.Count} middle click actions.");
        }
        else if (LeftClickActions.Count > 0)
        {
            InternalLogger.Log($"Added {LeftClickActions.Count} left click actions.");
        }
        else if (MiddleClickActions.Count > 0)
        {
            InternalLogger.Log($"Added {MiddleClickActions.Count} middle click actions.");
        }

        InternalLogger.Log("ItemActionPatcher is done.", LogLevel.Debug);
    }

    internal static bool OnPointerClick_Prefix(InventoryItem item, int button)
    {
        if (ItemDragManager.isDragging)
        {
            return true;
        }
        if (button == 0 && LeftClickActions.ContainsKey(item.item.GetTechType()))
        {
            Inventory.main.ExecuteItemAction(CustomLeftClickItemAction, item);
            return false;
        }
        if (button == 2 && MiddleClickActions.ContainsKey(item.item.GetTechType()))
        {
            Inventory.main.ExecuteItemAction(CustomMiddleClickItemAction, item);
            return false;
        }
        return true;
    }

    internal static bool ExecuteItemAction_Prefix(ItemAction action, InventoryItem item)
    {
        if (action != CustomLeftClickItemAction && action != CustomMiddleClickItemAction)
        {
            return true;
        }

        TechType itemTechType = item.item.GetTechType();

        if (action == CustomLeftClickItemAction)
        {
            if (LeftClickActions.TryGetValue(itemTechType, out CustomItemAction customItemAction))
            {
                if (customItemAction.Condition(item))
                {
                    customItemAction.Action(item);
                }
            }
            return false;
        }

        if (action == CustomMiddleClickItemAction)
        {
            if (MiddleClickActions.TryGetValue(itemTechType, out CustomItemAction customItemAction))
            {
                if (customItemAction.Condition(item))
                {
                    customItemAction.Action(item);
                }
            }
            return false;
        }

        return true;
    }

    internal static void ItemActions_Postfix(StringBuilder sb, InventoryItem item)
    {
        TechType itemTechType = item.item.GetTechType();
        bool hasLeftAction = false;

        if (LeftClickActions.TryGetValue(itemTechType, out CustomItemAction action))
        {
            if (action.Condition(item))
            {
                sb.Append("\n");
                TooltipFactory.WriteAction(sb, LeftClickMouseIcon, Language.main.Get(action.LanguageLineKey));
                hasLeftAction = true;
            }
        }

        if (MiddleClickActions.TryGetValue(itemTechType, out action))
        {
            if (action.Condition(item))
            {
                if (!hasLeftAction)
                {
                    sb.Append("\n");
                }

                TooltipFactory.WriteAction(sb, MiddleClickMouseIcon, Language.main.Get(action.LanguageLineKey));
            }
        }
    }

    #endregion
}