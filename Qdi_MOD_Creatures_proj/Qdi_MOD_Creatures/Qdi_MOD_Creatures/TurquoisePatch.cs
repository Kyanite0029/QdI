using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Assets.Scripts.UI.Utils;
using CommandWindow;
using CreatureInfo;
using Customizing;
using Harmony;
using Inventory;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Experimental.Rendering.RenderPass;

namespace Qdi_MOD_Creatures
{
    public class TurquoisePatch
    {
        public static void Patch(HarmonyInstance harmonyInstance)
        {
            try
            {
                TurquoisePatch.path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
                TurquoisePatch.MakeDamageIcon();
                //T속성을 정상인식하게 패치
                MethodInfo method = typeof(TurquoisePatch).GetMethod("EquipmentDataLoader_ConvertToRWBP");
                harmonyInstance.Patch(typeof(EquipmentDataLoader).GetMethod("ConvertToRWBP", AccessTools.all), new HarmonyMethod(method), null, null); 
                //
                method = typeof(TurquoisePatch).GetMethod("UIComponent_SetData");
                harmonyInstance.Patch(typeof(AgentInfoWindow.UIComponent).GetMethod("SetData", AccessTools.all, null, new Type[]
                {
                    typeof(AgentModel)
                }, null), new HarmonyMethod(method), null, null);
                //
                method = typeof(TurquoisePatch).GetMethod("InventoryAgentController_SetUI");
                harmonyInstance.Patch(typeof(InventoryAgentController).GetMethod("SetUI", AccessTools.all), new HarmonyMethod(method), null, null);
                //
                method = typeof(TurquoisePatch).GetMethod("InventoryWeaponSlot_SetWeapon");
                harmonyInstance.Patch(typeof(InventoryWeaponSlot).GetMethod("SetWeapon", AccessTools.all), new HarmonyMethod(method), null);
                //
                method = typeof(TurquoisePatch).GetMethod("AgentEquipmentSlot_SetData");
                harmonyInstance.Patch(typeof(AgentEquipmentSlot).GetMethod("SetData", AccessTools.all), new HarmonyMethod(method), null, null);
                //
                method = typeof(TurquoisePatch).GetMethod("InGameModeComponent_SetUI");
                    harmonyInstance.Patch(typeof(AgentInfoWindow.InGameModeComponent).GetMethod("SetUI", AccessTools.all), new HarmonyMethod(method), null, null);
                //
                method = typeof(TurquoisePatch).GetMethod("WeaponSlot_SetModel");
                harmonyInstance.Patch(typeof(WeaponSlot).GetMethod("SetModel", AccessTools.all, null, new Type[]
                     {
                    typeof(EquipmentTypeInfo)
                     }, null), new HarmonyMethod(method), null, null);
                //
                method = typeof(TurquoisePatch).GetMethod("InventoryWeaponSlot_UpdateUI");
                harmonyInstance.Patch(typeof(InventoryWeaponSlot).GetMethod("UpdateUI", AccessTools.all), new HarmonyMethod(method), null, null);
                //
                method = typeof(TurquoisePatch).GetMethod("CreatureModel_TakeDamage");
                harmonyInstance.Patch(typeof(CreatureModel).GetMethod("TakeDamage", AccessTools.all), new HarmonyMethod(method), null, null);
                //
                method = typeof(TurquoisePatch).GetMethod("WorkerModel_TakeDamage");
                harmonyInstance.Patch(typeof(WorkerModel).GetMethod("TakeDamage", AccessTools.all), new HarmonyMethod(method), null, null);
                //
                method = typeof(TurquoisePatch).GetMethod("RabbitModel_TakeDamage");
                harmonyInstance.Patch(typeof(RabbitModel).GetMethod("TakeDamage", AccessTools.all), new HarmonyMethod(method), null, null);
                //
                method = typeof(TurquoisePatch).GetMethod("InventoryAgentController_SetUI");
                harmonyInstance.Patch(typeof(InventoryAgentController).GetMethod("SetUI", AccessTools.all), new HarmonyMethod(method), null, null);
                //
                method = typeof(TurquoisePatch).GetMethod("WorkerSuppressRegion_SetData");
                harmonyInstance.Patch(typeof(WorkerSuppressRegion).GetMethod("SetData", AccessTools.all, null, new Type[]
                {
            typeof(WorkerModel)
                }, null), new HarmonyMethod(method), null, null);
                //
                method = typeof(TurquoisePatch).GetMethod("UnitModel_MakeDamageEffect");
                harmonyInstance.Patch(typeof(UnitModel).GetMethod("MakeDamageEffect", AccessTools.all), new HarmonyMethod(method), null, null);

                LobotomyBaseMod.ModDebug.Log("QDI.Turquoise Loaded");

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }


        }

        public static bool AgentEquipmentSlot_SetData(AgentEquipmentSlot __instance, AgentModel agent) //Pre
        {
            UnitEquipSpace equipment = agent.Equipment;
            bool flag = equipment.weapon.metaInfo.damageInfo.type == (RwbpType)654321;
            if (flag)
            {

                __instance.WeaponName.text = agent.Equipment.weapon.metaInfo.Name;
                global::DamageInfo damage = agent.Equipment.weapon.GetDamage(agent);

                __instance.TypeFill.enabled = true;
                __instance.TypeFill.color = Color.white;
                __instance.TypeFill.sprite = TurquoisePatch.TurquoiseIcon;
                __instance.TypeText.color = TurquoisePatch.TurquoiseDamageColor;
                __instance.TypeText.text = "TURQUOISE";

                Inventory.InventoryItemController.SetGradeText(agent.Equipment.weapon.metaInfo.Grade, __instance.WeaponGrade);
                Inventory.InventoryItemController.SetGradeText(agent.Equipment.armor.metaInfo.Grade, __instance.ArmorGrade);
                string grade = agent.Equipment.weapon.metaInfo.grade;
                foreach (Text text in __instance.Vanlia)
                {
                    text.text = grade;
                }
                __instance.DualValue.SetActive(false);
                global::DefenseInfo defense = agent.defense;
                global::UIUtil.DefenseSetOnlyText(defense, __instance.DefenseType);
                global::UIUtil.SetDefenseTypeIcon(defense, __instance.DefenseFactorRenderer);
                bool flag2 = agent.Equipment.armor != null;
                if (flag2)
                {
                    __instance.ArmorName.text = agent.Equipment.armor.metaInfo.Name;
                }
                return false;
            }

            return true;

        }

        public static bool CreatureModel_TakeDamage(CreatureModel __instance, UnitModel actor, DamageInfo dmg) //Pre
        {
            bool flag = dmg.type == (RwbpType)654321;
            bool result;
            float weakres = -10.0f;
            if (flag)
            {
                result = false;
                dmg = dmg.Copy();
                if (!__instance.script.CanTakeDamage(actor, dmg))
                {
                    return result;
                }
                if (__instance.Equipment.armor != null)
                {
                    __instance.Equipment.armor.OnTakeDamage(actor, ref dmg);
                }
                if (__instance.Equipment.weapon != null)
                {
                    __instance.Equipment.weapon.OnTakeDamage(actor, ref dmg);
                }
                __instance.Equipment.gifts.OnTakeDamage(actor, ref dmg);
                float num = 0f;
                float num2 = 1f;
                if (actor != null)
                {
                    num2 = global::UnitModel.GetDmgMultiplierByEgoLevel(actor.GetAttackLevel(), __instance.GetDefenseLevel());
                }
                num2 *= __instance.GetBufDamageMultiplier(actor, dmg);
                num2 *= __instance.script.GetDamageFactor(actor, dmg);
                weakres = __instance.defense.GetMultiplier(RwbpType.R);
                if (weakres < __instance.defense.GetMultiplier(RwbpType.W))
                {
                    weakres = __instance.defense.GetMultiplier(RwbpType.W);
                }
                if (weakres < __instance.defense.GetMultiplier(RwbpType.B))
                {
                    weakres = __instance.defense.GetMultiplier(RwbpType.B);
                }
                if (weakres < __instance.defense.GetMultiplier(RwbpType.P))
                {
                    weakres = __instance.defense.GetMultiplier(RwbpType.P);
                }
                num = dmg.GetDamage() * weakres * num2;
                if (__instance.hp > 0f)
                {
                    if (num >= 0f)
                    {
                        float num3 = (float)((int)__instance.hp);
                        __instance.hp -= num;
                        float num4 = num3 - (float)((int)__instance.hp);
                        __instance.MakeDamageEffect(dmg.type, num4, __instance.defense.GetDefenseType(dmg.type));
                        __instance.script.OnTakeDamage(actor, dmg, num);
                    }
                    else if (num < 0f)
                    {
                        float num5 = -num;
                        if ((float)__instance.maxHp - __instance.hp >= num5)
                        {
                            __instance.hp += num5;
                        }
                        else
                        {
                            __instance.hp = (float)__instance.maxHp;
                        }
                        GameObject gameObject = global::Prefab.LoadPrefab("Effect/RecoverHP");
                        gameObject.transform.SetParent(__instance.Unit.animTarget.transform);
                        gameObject.transform.localPosition = Vector3.zero;
                        gameObject.transform.localScale = Vector3.one;
                        gameObject.transform.localRotation = Quaternion.identity;
                    }
                }
                __instance.hp = Mathf.Clamp(__instance.hp, 0f, (float)__instance.maxHp);
                if (__instance.state == global::CreatureState.ESCAPE && __instance.hp <= 0f)
                {
                    __instance.Suppressed();
                }
                if (__instance.Equipment.armor != null)
                {
                    __instance.Equipment.armor.OnTakeDamage_After(num, dmg.type);
                }
                if (__instance.Equipment.weapon != null)
                {
                    __instance.Equipment.weapon.OnTakeDamage_After(num, dmg.type);
                }
                __instance.Equipment.gifts.OnTakeDamage_After(num, dmg.type);
            }
            else
            {
                result = true;
            }
            return result;
        }

        public static bool EquipmentDataLoader_ConvertToRWBP(ref RwbpType __result, string text) //Pre
        {
            if (text == "T")
            {
                __result = (RwbpType)654321;

                return false;
            }
            return true;
        }

        public static bool InGameModeComponent_SetUI(AgentInfoWindow.InGameModeComponent __instance, AgentModel agent) //Pre
        {
            bool flag = agent != null;
            if (flag)
            {
                bool flag2 = agent.Equipment.weapon.metaInfo.damageInfo.type == (RwbpType)654321; ;
                if (flag2)
                {
                    __instance.AgentTitle.enabled = true;
                    __instance.GradeImage.sprite = global::DeployUI.GetAgentGradeSprite(agent);
                    __instance.AgentName.text = agent.GetUnitName();
                    string str = string.Empty;
                    global::Sefira sefira = global::SefiraManager.instance.GetSefira(agent.lastServiceSefira);
                    bool flag3 = sefira != null;
                    if (flag3)
                    {
                        global::SefiraEnum sefiraEnum = sefira.sefiraEnum;
                        bool flag4 = sefiraEnum == global::SefiraEnum.TIPERERTH2;
                        if (flag4)
                        {
                            sefiraEnum = global::SefiraEnum.TIPERERTH1;
                        }
                        str = string.Format(global::LocalizeTextDataModel.instance.GetText("continous_service_ability_cur_title2"), global::LocalizeTextDataModel.instance.GetTextAppend(new string[]
                        {
                    global::SefiraName.GetSefiraByEnum(sefiraEnum),
                    "Name"
                        }), agent.continuousServiceDay) + " ";
                    }
                    __instance.AgentTitle.text = str + global::LocalizeTextDataModel.instance.GetText("continous_service_ability_cur_blank") + agent.GetTitle();
                    __instance.portrait.SetWorker(agent);
                    global::AgentInfoWindow.WorkerPrimaryStatUI[] array = __instance.statUI;
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].SetStat(agent);
                    }
                    __instance.Weapon.StatName.text = agent.Equipment.weapon.metaInfo.Name;
                    global::DamageInfo damage = agent.Equipment.weapon.GetDamage(agent);

                    __instance.Weapon.Fill.enabled = true;
                    __instance.Weapon.Fill.sprite = TurquoisePatch.TurquoiseIcon;
                    __instance.Weapon.Fill_Inner.text = "TURQUOISE";
                    __instance.Weapon.Fill_Inner.color = TurquoisePatch.TurquoiseDamageColor;
                    __instance.Weapon.Fill.color = Color.white;
                    float num2 = agent.GetDamageFactorByEquipment();
                    num2 *= agent.GetDamageFactorBySefiraAbility();
                    float reinforcementDmg2 = agent.Equipment.weapon.script.GetReinforcementDmg();
                    string text2 = string.Format("{0}-{1}", (int)(damage.min * num2 * reinforcementDmg2), (int)(damage.max * num2 * reinforcementDmg2));
                    __instance.Weapon.slots[0].SetText(text2);

                    global::DefenseInfo defense = agent.defense;
                    global::UIUtil.DefenseSetFactor(defense, __instance.DefenseType, true);
                    global::UIUtil.SetDefenseTypeIcon(defense, __instance.DefenseIcon);
                    bool flag5 = agent.Equipment.armor != null;
                    if (flag5)
                    {
                        __instance.ArmorName.text = agent.Equipment.armor.metaInfo.Name;
                    }
                    else
                    {
                        __instance.ArmorName.text = "Armor is missing";
                    }
                    Inventory.InventoryItemController.SetGradeText(agent.Equipment.weapon.metaInfo.Grade, __instance.WeaponGrade);
                    Inventory.InventoryItemController.SetGradeText(agent.Equipment.armor.metaInfo.Grade, __instance.ArmorGrade);
                    for (int j = 0; j < __instance.StatTooltips.Length; j++)
                    {
                        string text3 = global::LocalizeTextDataModel.instance.GetText(__instance.StatTooltips[j].ID);
                        string arg = "?";
                        switch (j)
                        {
                            case 0:
                                arg = agent.fortitudeLevel.ToString();
                                break;
                            case 1:
                                arg = agent.prudenceLevel.ToString();
                                break;
                            case 2:
                                arg = agent.temperanceLevel.ToString();
                                break;
                            case 3:
                                arg = agent.justiceLevel.ToString();
                                break;
                            case 4:
                                arg = (agent.workProb / 5).ToString();
                                break;
                            case 5:
                                arg = (agent.workSpeed / 5).ToString();
                                break;
                            case 6:
                                arg = (agent.attackSpeed / 5f).ToString();
                                break;
                            case 7:
                                arg = (agent.movement / 5f).ToString();
                                break;
                        }
                        string dynamicTooltip = string.Format(text3, arg);
                        __instance.StatTooltips[j].SetDynamicTooltip(dynamicTooltip);
                    }
                    for (int k = 0; k < __instance.DefenseTooltips.Length; k++)
                    {
                        string text4 = global::LocalizeTextDataModel.instance.GetText(__instance.DefenseTooltips[k].ID);
                        string defenseTypeText = __instance.GetDefenseTypeText(agent.defense, k + global::RwbpType.R);
                        string dynamicTooltip2 = string.Format(text4, defenseTypeText);
                        __instance.DefenseTooltips[k].SetDynamicTooltip(dynamicTooltip2);
                    }
                    return false;
                }
            }
            return true;
        }


        public static bool InventoryWeaponSlot_SetWeapon(InventoryWeaponSlot __instance, WeaponModel weapon) //Pre
        {
            bool flag = weapon.metaInfo.damageInfo.type == (RwbpType)654321;
            if (flag)
            {

                global::UnitModel owner = weapon.owner;
                __instance.Name.text = weapon.metaInfo.Name;
                string text = ((int)weapon.GetDamage(owner).min).ToString() + "-" + ((int)weapon.GetDamage(owner).max).ToString();

                __instance.Type.text = "T";
                __instance.Type.color = TurquoisePatch.TurquoiseDamageColor;
                string empty = string.Empty;
                string empty2 = string.Empty;
                InventoryItemDescGetter.GetWeaponDesc(weapon, out empty2, out empty);
                __instance.Range.text = empty2;
                __instance.AttackSpeed.text = empty;
                __instance.DamageRange.text = text;
                InventoryItemController.SetGradeText(weapon.metaInfo.Grade, __instance.Grade);
                __instance.Grade.text = weapon.metaInfo.Grade.ToString();
                __instance.SetEquipmentText();
                __instance.TooltipButton.interactable = true;
                bool flag2 = weapon.metaInfo.weaponClassType == global::WeaponClassType.FIST;
                Sprite sprite;
                if (flag2)
                {
                    int id = (int)float.Parse(weapon.metaInfo.sprite);
                    Sprite[] fistSprite = global::WorkerSprite.WorkerSprite_WorkerSpriteManager.instance.GetFistSprite(id);
                    bool flag3 = fistSprite[0] == null || fistSprite[1] == null;
                    if (flag3)
                    {
                        return false;
                    }
                    sprite = fistSprite[1];
                }
                else
                {
                    LobotomyBaseMod.KeyValuePairSS ss = new LobotomyBaseMod.KeyValuePairSS(global::EquipmentTypeInfo.GetLcId(weapon.metaInfo).packageId, weapon.metaInfo.sprite);
                    sprite = ((global::WorkerSpriteManager)global::WorkerSpriteManager.instance).GetWeaponSprite_Mod(weapon.metaInfo.weaponClassType, ss);
                }
                string str = "Weapon sprite ";
                Sprite sprite2 = sprite;
                Debug.Log(str + ((sprite2 != null) ? sprite2.ToString() : null));
                __instance.Icon.sprite = sprite;
                __instance.Icon.SetNativeSize();
                __instance.Icon.preserveAspect = true;
                bool flag4 = sprite == null;
                if (flag4)
                {
                    __instance.Icon.gameObject.SetActive(false);
                }
                else
                {
                    __instance.Icon.gameObject.SetActive(true);
                }
                __instance.RequireInit(weapon.metaInfo);

                return false;
            }
            return true;
        }

        public static bool InventoryWeaponSlot_UpdateUI(InventoryWeaponSlot __instance) //Pre
        {
            bool flag = __instance.Info.damageInfo.type == (RwbpType)654321;
            if (flag)
            {
                __instance.Name.text = __instance.Info.Name;
                InventoryItemController.SetGradeText(__instance.Info.Grade, __instance.Grade);
                __instance.ApplyPortrait();
                string text = string.Format("{0}-{1}", (int)__instance.Info.damageInfo.min, (int)__instance.Info.damageInfo.max);

                __instance.Type.text = "TURQUOISE";
                __instance.Type.color = TurquoisePatch.TurquoiseDamageColor;
                __instance.DamageTypeImage.enabled = true;
                __instance.DamageTypeImage.sprite = TurquoisePatch.TurquoiseIcon;
                __instance.DamageTypeImage.color = TurquoisePatch.TurquoiseDamageColor;
                __instance.DamageRange.text = text;
                string empty = string.Empty;
                string empty2 = string.Empty;
                InventoryItemDescGetter.GetWeaponDesc(__instance.Info, out empty, out empty2);
                __instance.Range.text = empty2;
                __instance.AttackSpeed.text = empty;
                __instance.TooltipButton.interactable = true;
                __instance.TooltipButton.OnPointerExit(null);
                return false;
            }
            return true;
        }

        public static bool InventoryAgentController_SetUI(InventoryAgentController __instance) //Pre
        {
            bool flag = __instance.CurrentAgent.Equipment.weapon.metaInfo.damageInfo.type == (RwbpType)654321 && __instance.CurrentAgent != null;
            if (flag)
            {
                DamageInfo damage = __instance.CurrentAgent.Equipment.weapon.GetDamage(__instance.CurrentAgent);
                try
                {
                    __instance.TypeFill.color = Color.white;
                    __instance.TypeFill.enabled = true;
                    __instance.TypeText.color = TurquoiseDamageColor;
                    __instance.TypeFill.sprite = TurquoisePatch.TurquoiseIcon;
                    __instance.TypeText.text = "T";

                    string text = string.Format("{0}-{1}", (int)damage.min, (int)damage.max);
                    __instance.DamageText.text = text;
                    global::WorkerPrimaryStatBonus titleBonus = __instance.CurrentAgent.titleBonus;
                    int num = __instance.CurrentAgent.primaryStat.maxHP + titleBonus.maxHP;
                    int num2 = __instance.CurrentAgent.primaryStat.maxMental + titleBonus.maxMental;
                    int num3 = __instance.CurrentAgent.primaryStat.workProb + titleBonus.workProb;
                    int num4 = __instance.CurrentAgent.primaryStat.cubeSpeed + titleBonus.cubeSpeed;
                    int num5 = __instance.CurrentAgent.primaryStat.attackSpeed + titleBonus.attackSpeed;
                    int num6 = __instance.CurrentAgent.primaryStat.movementSpeed + titleBonus.movementSpeed;
                    int num7 = __instance.CurrentAgent.maxHp - num;
                    int num8 = __instance.CurrentAgent.maxMental - num2;
                    int num9 = __instance.CurrentAgent.workProb - num3;
                    int num10 = __instance.CurrentAgent.workSpeed - num4;
                    int num11 = (int)__instance.CurrentAgent.attackSpeed - num5;
                    int num12 = (int)__instance.CurrentAgent.movement - num6;
                    bool flag3 = num7 > 0;
                    if (flag3)
                    {
                        __instance.Stats[0].slots[0].SetText(num.ToString() + string.Empty, "+" + num7.ToString());
                    }
                    else
                    {
                        bool flag4 = num7 < 0;
                        if (flag4)
                        {
                            __instance.Stats[0].slots[0].SetText(num.ToString() + string.Empty, "-" + (-num7).ToString());
                        }
                        else
                        {
                            __instance.Stats[0].slots[0].SetText(num.ToString() + string.Empty);
                        }
                    }
                    bool flag5 = num8 > 0;
                    if (flag5)
                    {
                        __instance.Stats[1].slots[0].SetText(num2.ToString() + string.Empty, "+" + num8.ToString());
                    }
                    else
                    {
                        bool flag6 = num8 < 0;
                        if (flag6)
                        {
                            __instance.Stats[1].slots[0].SetText(num2.ToString() + string.Empty, "-" + (-num8).ToString());
                        }
                        else
                        {
                            __instance.Stats[1].slots[0].SetText(num2.ToString() + string.Empty);
                        }
                    }
                    bool flag7 = num9 > 0;
                    if (flag7)
                    {
                        __instance.Stats[2].slots[0].SetText(num3.ToString() + string.Empty, "+" + num9.ToString());
                    }
                    else
                    {
                        bool flag8 = num9 < 0;
                        if (flag8)
                        {
                            __instance.Stats[2].slots[0].SetText(num3.ToString() + string.Empty, "-" + (-num9).ToString());
                        }
                        else
                        {
                            __instance.Stats[2].slots[0].SetText(num3.ToString() + string.Empty);
                        }
                    }
                    bool flag9 = num10 > 0;
                    if (flag9)
                    {
                        __instance.Stats[2].slots[1].SetText(num4.ToString() + string.Empty, "+" + num10.ToString());
                    }
                    else
                    {
                        bool flag10 = num10 < 0;
                        if (flag10)
                        {
                            __instance.Stats[2].slots[1].SetText(num4.ToString() + string.Empty, "-" + (-num10).ToString());
                        }
                        else
                        {
                            __instance.Stats[2].slots[1].SetText(num4.ToString() + string.Empty);
                        }
                    }
                    bool flag11 = num12 > 0;
                    if (flag11)
                    {
                        __instance.Stats[3].slots[0].SetText(num6.ToString() + string.Empty, "+" + num12.ToString());
                    }
                    else
                    {
                        bool flag12 = num12 < 0;
                        if (flag12)
                        {
                            __instance.Stats[3].slots[0].SetText(num6.ToString() + string.Empty, "-" + (-num12).ToString());
                        }
                        else
                        {
                            __instance.Stats[3].slots[0].SetText(num6.ToString() + string.Empty);
                        }
                    }
                    bool flag13 = num11 > 0;
                    if (flag13)
                    {
                        __instance.Stats[3].slots[1].SetText(num5.ToString() + string.Empty, "+" + num11.ToString());
                    }
                    else
                    {
                        bool flag14 = num11 < 0;
                        if (flag14)
                        {
                            __instance.Stats[3].slots[1].SetText(num5.ToString() + string.Empty, "-" + (-num11).ToString());
                        }
                        else
                        {
                            __instance.Stats[3].slots[1].SetText(num5.ToString() + string.Empty);
                        }
                    }
                    __instance.Stats[0].Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Rstat"), global::AgentModel.GetLevelGradeText(__instance.CurrentAgent.Rstat));
                    __instance.Stats[1].Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Wstat"), global::AgentModel.GetLevelGradeText(__instance.CurrentAgent.Wstat));
                    __instance.Stats[2].Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Bstat"), global::AgentModel.GetLevelGradeText(__instance.CurrentAgent.Bstat));
                    __instance.Stats[3].Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Pstat"), global::AgentModel.GetLevelGradeText(__instance.CurrentAgent.Pstat));
                    global::DefenseInfo defense = __instance.CurrentAgent.defense;
                    global::UIUtil.DefenseSetOnlyText(defense, __instance.DefenseType);
                    global::UIUtil.DefenseSetFactor(defense, __instance.DefenseFactor, true);
                    string name = __instance.CurrentAgent.Equipment.weapon.metaInfo.Name;
                    bool flag15 = name == "UNKNOWN";
                    if (flag15)
                    {
                        __instance.WeaponTitle.text = global::LocalizeTextDataModel.instance.GetText("Inventory_WeaponTitle");
                    }
                    else
                    {
                        __instance.WeaponTitle.text = name;
                    }
                    string name2 = __instance.CurrentAgent.Equipment.armor.metaInfo.Name;
                    bool flag16 = name2 == "UNKNOWN";
                    if (flag16)
                    {
                        __instance.ArmorTitle.text = global::LocalizeTextDataModel.instance.GetText("Inventory_ArmorTitle");
                    }
                    else
                    {
                        __instance.ArmorTitle.text = name2;
                    }
                    __instance.SubEquipTitle.text = global::LocalizeTextDataModel.instance.GetText("Inventory_GiftTitle");
                    IEnumerator enumerator = __instance.SubEquipListParent.transform.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            object obj = enumerator.Current;
                            Transform transform = (Transform)obj;
                            UnityEngine.Object.Destroy(transform.gameObject);
                        }
                    }
                    finally
                    {
                        IDisposable disposable;
                        bool flag17 = (disposable = (enumerator as IDisposable)) != null;
                        if (flag17)
                        {
                            disposable.Dispose();
                        }
                    }
                    foreach (global::EquipmentModel equipmentModel in __instance.CurrentAgent.Equipment.gifts.addedGifts)
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.attachUnit);
                        global::InventoryAttachmentUnit component = gameObject.GetComponent<global::InventoryAttachmentUnit>();
                        component.text.text = equipmentModel.metaInfo.Name + " : " + equipmentModel.metaInfo.Description;
                        gameObject.transform.SetParent(__instance.SubEquipListParent.transform);
                        gameObject.transform.localScale = Vector3.one;
                    }
                    foreach (global::EquipmentModel equipmentModel2 in __instance.CurrentAgent.Equipment.gifts.replacedGifts)
                    {
                        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(__instance.attachUnit);
                        global::InventoryAttachmentUnit component2 = gameObject2.GetComponent<global::InventoryAttachmentUnit>();
                        component2.text.text = equipmentModel2.metaInfo.Name + " : " + equipmentModel2.metaInfo.Description;
                        gameObject2.transform.SetParent(__instance.SubEquipListParent.transform);
                        gameObject2.transform.localScale = Vector3.one;
                    }
                    InventoryItemController.SetGradeText(__instance.CurrentAgent.Equipment.weapon.metaInfo.Grade, __instance.WeaponGrade);
                    __instance.WeaponImage.sprite = __instance.CurrentAgent.GetWeaponSprite();
                    __instance.ArmorImage.SetArmor(__instance.CurrentAgent.Equipment.armor.metaInfo);
                    VerticalLayoutGroup component3 = __instance.SubEquipListParent.transform.GetComponent<VerticalLayoutGroup>();
                    component3.enabled = false;
                    component3.enabled = true;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                return false;

            }

            return true;
        }








        public bool WorkerModel_TakeDamage(WorkerModel __instance, UnitModel actor, DamageInfo dmg) //Pre
        {

            bool flag = dmg.type == (RwbpType)654321;
            bool result;
            float weakres = -10.0f;

            if (flag)
            {
                result = false;
                dmg = dmg.Copy();
                if (__instance.invincible && !__instance.HasUnitBuf(global::UnitBufType.OTHER_WORLD_PORTRAIT_VICTIM))
                {
                    return result;
                }
                if (__instance.IsDead())
                {
                    return result;
                }
                if (__instance.Equipment.armor != null)
                {
                    __instance.Equipment.armor.OnTakeDamage(actor, ref dmg);
                }
                if (__instance.Equipment.weapon != null)
                {
                    __instance.Equipment.weapon.OnTakeDamage(actor, ref dmg);
                }
                __instance.Equipment.gifts.OnTakeDamage(actor, ref dmg);
                float num = 0f;
                float num2 = 0.75f;
                if (actor != null)
                {
                    num2 = global::UnitModel.GetDmgMultiplierByEgoLevel(actor.GetAttackLevel(), __instance.GetDefenseLevel());
                }
                num2 *= __instance.GetBufDamageMultiplier(actor, dmg);
                num = dmg.GetDamage() * weakres * num2;

                if (__instance.hp > 0f)
                {
                    if (num >= 0f)
                    {
                        float num3 = (float)((int)__instance.hp);
                        __instance.AddUnitBuf(new TurquoiseDebuf(num));
                        float num4 = num3 - (float)((int)__instance.hp);
                        __instance.MakeDamageEffect(dmg.type, num4, __instance.defense.GetDefenseType(dmg.type));
                    }
                    else if (num < 0f)
                    {

                    }
                }

                if (__instance.Equipment.armor != null)
                {
                    __instance.Equipment.armor.OnTakeDamage_After(num, dmg.type);
                }
                if (__instance.Equipment.weapon != null)
                {
                    __instance.Equipment.weapon.OnTakeDamage_After(num, dmg.type);
                }
                __instance.Equipment.gifts.OnTakeDamage_After(num, dmg.type);
            }
            else
            {
                result = true;
            }

            return result;
        }

        public bool RabbitModel_TakeDamage(RabbitModel __instance, UnitModel actor, DamageInfo dmg) //Pre
        {

            bool flag = dmg.type == (RwbpType)654321;
            bool result;
            if (flag)
            {
                result = false;
                dmg = dmg.Copy();
                if (__instance.IsDead())
                {
                    return result;
                }
                if (__instance.Equipment.armor != null)
                {
                    __instance.Equipment.armor.OnTakeDamage(actor, ref dmg);
                }
                if (__instance.Equipment.weapon != null)
                {
                    __instance.Equipment.weapon.OnTakeDamage(actor, ref dmg);
                }
                __instance.Equipment.gifts.OnTakeDamage(actor, ref dmg);
                float num = 0f;
                float num2 = 0.75f;
                if (actor != null)
                {
                    num2 = global::UnitModel.GetDmgMultiplierByEgoLevel(actor.GetAttackLevel(), __instance.GetDefenseLevel());
                }
                num2 *= __instance.GetBufDamageMultiplier(actor, dmg);
                num = dmg.GetDamage() * num2;

                if (__instance.hp > 0f)
                {
                    if (num >= 0f)
                    {
                        float num3 = (float)((int)__instance.hp);
                        __instance.AddUnitBuf(new TurquoiseDebuf(num));
                        float num4 = num3 - (float)((int)__instance.hp);
                        __instance.MakeDamageEffect(dmg.type, num4, __instance.defense.GetDefenseType(dmg.type));
                    }
                    else if (num < 0f)
                    {

                    }
                }

                if (__instance.Equipment.armor != null)
                {
                    __instance.Equipment.armor.OnTakeDamage_After(num, dmg.type);
                }
                if (__instance.Equipment.weapon != null)
                {
                    __instance.Equipment.weapon.OnTakeDamage_After(num, dmg.type);
                }
                __instance.Equipment.gifts.OnTakeDamage_After(num, dmg.type);
            }
            else
            {
                result = true;
            }

            return result;
        }

        public static bool WeaponSlot_SetModel(WeaponSlot __instance, EquipmentTypeInfo info)
        {
            bool flag = info.damageInfo.type == (RwbpType)654321;
            if (flag)
            {
                string empty = string.Empty;
                string empty2 = string.Empty;
                Inventory.InventoryItemDescGetter.GetWeaponDesc(info, out empty2, out empty);
                __instance.ItemGrade.text = info.Grade.ToString();
                Inventory.InventoryItemController.SetGradeText(info.Grade, __instance.ItemGrade);
                __instance.DamageRange.text = ((int)info.damageInfo.min).ToString() + "-" + ((int)info.damageInfo.max).ToString();
                __instance.AttackSpeed.text = empty2;
                __instance.AttackRange.text = empty;
                __instance.ItemName.text = info.Name;
                global::RwbpType type = info.damageInfo.type;
                Color white = Color.white;
                Color white2 = Color.white;
                global::UIColorManager.instance.GetRWBPTypeColor(type, out white, out white2);
                __instance.TypeText.text = "TURQUOISE";
                __instance.TypeText.color = TurquoisePatch.TurquoiseDamageColor;
                __instance.TypeFill.sprite = TurquoisePatch.TurquoiseIcon;
                __instance.TypeFill.color = Color.white;

                Sprite weaponSprite_Mod = ((global::WorkerSpriteManager)global::WorkerSpriteManager.instance).GetWeaponSprite_Mod(info.weaponClassType, new LobotomyBaseMod.KeyValuePairSS(global::EquipmentTypeInfo.GetLcId(info).packageId, info.sprite));
                __instance.ItemImage.sprite = weaponSprite_Mod;
                __instance.ItemImage.SetNativeSize();
                bool flag2 = weaponSprite_Mod == null;
                if (flag2)
                {
                    __instance.ItemImage.enabled = false;
                }
                else
                {
                    __instance.ItemImage.enabled = true;
                }

                return false;
            }
            return true;
        }

        public static bool WorkerSuppressRegion_SetData(WorkerSuppressRegion __instance, WorkerModel worker) //Pre
        {
            bool flag = worker is AgentModel;
            if (flag)
            {
                AgentModel agentModel = (AgentModel)worker;
                bool flag2 = agentModel.Equipment.weapon.metaInfo.damageInfo.type == (RwbpType)654321;
                if (flag2)
                {
                    __instance.GradeImage.sprite = global::DeployUI.GetAgentGradeSprite(agentModel);
                    __instance.AgentName.text = agentModel.GetUnitName();
                    __instance.portrait.SetWorker(worker);
                    __instance.Title.text = agentModel.GetTitle();
                    int num = agentModel.maxHp - agentModel.primaryStat.maxHP;
                    int num2 = agentModel.maxMental - agentModel.primaryStat.maxMental;
                    int num3 = agentModel.workProb - agentModel.primaryStat.workProb;
                    int num4 = agentModel.workSpeed - agentModel.primaryStat.cubeSpeed;
                    if (num > 0)
                    {
                        __instance.Stat_R.slots[0].SetText(agentModel.primaryStat.maxHP + string.Empty, "+" + num);
                    }
                    else if (num < 0)
                    {
                        __instance.Stat_R.slots[0].SetText(agentModel.primaryStat.maxHP + string.Empty, "-" + -num);
                    }
                    else
                    {
                        __instance.Stat_R.slots[0].SetText(agentModel.primaryStat.maxHP + string.Empty);
                    }
                    if (num2 > 0)
                    {
                        __instance.Stat_W.slots[0].SetText(agentModel.primaryStat.maxMental + string.Empty, "+" + num2);
                    }
                    else if (num2 < 0)
                    {
                        __instance.Stat_W.slots[0].SetText(agentModel.primaryStat.maxMental + string.Empty, "-" + -num2);
                    }
                    else
                    {
                        __instance.Stat_W.slots[0].SetText(agentModel.primaryStat.maxMental + string.Empty);
                    }
                    if (num3 > 0)
                    {
                        __instance.Stat_B.slots[0].SetText(agentModel.primaryStat.workProb + string.Empty, "+" + num3);
                    }
                    else if (num3 < 0)
                    {
                        __instance.Stat_B.slots[0].SetText(agentModel.primaryStat.workProb + string.Empty, "-" + -num3);
                    }
                    else
                    {
                        __instance.Stat_B.slots[0].SetText(agentModel.primaryStat.workProb + string.Empty);
                    }
                    if (num4 > 0)
                    {
                        __instance.Stat_B.slots[1].SetText(agentModel.primaryStat.cubeSpeed + string.Empty, "+" + num4);
                    }
                    else if (num4 < 0)
                    {
                        __instance.Stat_B.slots[1].SetText(agentModel.primaryStat.cubeSpeed + string.Empty, "-" + -num4);
                    }
                    else
                    {
                        __instance.Stat_B.slots[1].SetText(agentModel.primaryStat.cubeSpeed + string.Empty);
                    }
                    __instance.Stat_P.slots[0].SetText(agentModel.primaryStat.attackSpeed + string.Empty);
                    __instance.Stat_P.slots[1].SetText((int)agentModel.movement + string.Empty);
                    __instance.Stat_R.Fill_Inner.text = global::AgentModel.GetLevelGradeText(agentModel.Rstat);
                    __instance.Stat_W.Fill_Inner.text = global::AgentModel.GetLevelGradeText(agentModel.Wstat);
                    __instance.Stat_B.Fill_Inner.text = global::AgentModel.GetLevelGradeText(agentModel.Bstat);
                    __instance.Weapon.StatName.text = agentModel.Equipment.weapon.metaInfo.Name;
                    global::DamageInfo damage = agentModel.Equipment.weapon.GetDamage(agentModel);
                    global::RwbpType type = damage.type;
                    __instance.Weapon.Fill_Inner.text = "TURQUOISE";
                    __instance.Weapon.Fill_Inner.color = TurquoisePatch.TurquoiseDamageColor;
                    __instance.Weapon.Fill.color = TurquoisePatch.TurquoiseDamageColor;
                    string text = string.Format("{0}-{1}", (int)damage.min, (int)damage.max);
                    __instance.Weapon.slots[0].SetText(text);
                    global::DefenseInfo defense = agentModel.defense;
                    global::UIUtil.DefenseSetOnlyText(defense, __instance.DefenseType);
                    global::UIUtil.DefenseSetFactor(defense, __instance.DefenseFactor, true);
                    if (agentModel.Equipment.armor != null)
                    {
                        __instance.ArmorName.text = agentModel.Equipment.armor.metaInfo.Name;
                    }
                    else
                    {
                        __instance.ArmorName.text = "Armor is missing";
                    }
                    Inventory.InventoryItemController.SetGradeText(agentModel.Equipment.weapon.metaInfo.Grade, __instance.WeaponGrade);
                    Inventory.InventoryItemController.SetGradeText(agentModel.Equipment.armor.metaInfo.Grade, __instance.ArmorGrade);
                    __instance.Weapon.Fill_Inner.text = "TURQUOISE";
                    __instance.Weapon.Fill_Inner.color = TurquoisePatch.TurquoiseDamageColor;
                    __instance.Weapon.Fill.color = TurquoisePatch.TurquoiseDamageColor;

                    return false;

                }
            }
            return true;
        }


        public static bool UIComponent_SetData(AgentInfoWindow.UIComponent __instance, AgentModel agent) //Pre
        {
            bool flag = agent.Equipment.weapon.metaInfo.damageInfo.type == (RwbpType)654321;
            if (flag)
            {
                __instance.SetColorData();
                __instance.AgentTitle.enabled = true;
                __instance.GradeImage.sprite = global::DeployUI.GetAgentGradeSprite(agent);
                __instance.AgentName.text = agent.GetUnitName();
                __instance.AgentTitle.text = agent.GetTitle();
                __instance.portrait.SetWorker(agent);
                global::WorkerPrimaryStatBonus titleBonus = agent.titleBonus;
                int originFortitudeStat = agent.originFortitudeStat;
                int originPrudenceStat = agent.originPrudenceStat;
                int originTemperanceStat = agent.originTemperanceStat;
                int originTemperanceStat2 = agent.originTemperanceStat;
                int originJusticeStat = agent.originJusticeStat;
                int originJusticeStat2 = agent.originJusticeStat;
                int num = agent.maxHp - originFortitudeStat;
                int num2 = agent.maxMental - originPrudenceStat;
                int num3 = agent.workProb - originTemperanceStat;
                int num4 = agent.workSpeed - originTemperanceStat2;
                int num5 = (int)agent.attackSpeed - originJusticeStat;
                int num6 = (int)agent.movement - originJusticeStat2;
                bool flag2 = num > 0;
                if (flag2)
                {
                    __instance.Stat_R.slots[0].SetText(originFortitudeStat.ToString() + string.Empty, "+" + num.ToString());
                }
                else
                {
                    bool flag3 = num < 0;
                    if (flag3)
                    {
                        __instance.Stat_R.slots[0].SetText(originFortitudeStat.ToString() + string.Empty, "-" + (-num).ToString());
                    }
                    else
                    {
                        __instance.Stat_R.slots[0].SetText(originFortitudeStat.ToString() + string.Empty);
                    }
                }
                bool flag4 = num2 > 0;
                if (flag4)
                {
                    __instance.Stat_W.slots[0].SetText(originPrudenceStat.ToString() + string.Empty, "+" + num2.ToString());
                }
                else
                {
                    bool flag5 = num2 < 0;
                    if (flag5)
                    {
                        __instance.Stat_W.slots[0].SetText(originPrudenceStat.ToString() + string.Empty, "-" + (-num2).ToString());
                    }
                    else
                    {
                        __instance.Stat_W.slots[0].SetText(originPrudenceStat.ToString() + string.Empty);
                    }
                }
                bool flag6 = num3 > 0;
                if (flag6)
                {
                    __instance.Stat_B.slots[0].SetText(originTemperanceStat.ToString() + string.Empty, "+" + num3.ToString());
                }
                else
                {
                    bool flag7 = num3 < 0;
                    if (flag7)
                    {
                        __instance.Stat_B.slots[0].SetText(originTemperanceStat.ToString() + string.Empty, "-" + (-num3).ToString());
                    }
                    else
                    {
                        __instance.Stat_B.slots[0].SetText(originTemperanceStat.ToString() + string.Empty);
                    }
                }
                bool flag8 = num4 > 0;
                if (flag8)
                {
                    __instance.Stat_B.slots[1].SetText(originTemperanceStat2.ToString() + string.Empty, "+" + num4.ToString());
                }
                else
                {
                    bool flag9 = num4 < 0;
                    if (flag9)
                    {
                        __instance.Stat_B.slots[1].SetText(originTemperanceStat2.ToString() + string.Empty, "-" + (-num4).ToString());
                    }
                    else
                    {
                        __instance.Stat_B.slots[1].SetText(originTemperanceStat2.ToString() + string.Empty);
                    }
                }
                bool flag10 = num5 > 0;
                if (flag10)
                {
                    __instance.Stat_P.slots[0].SetText(originJusticeStat.ToString() + string.Empty, "+" + num5.ToString());
                }
                else
                {
                    bool flag11 = num5 < 0;
                    if (flag11)
                    {
                        __instance.Stat_P.slots[0].SetText(originJusticeStat.ToString() + string.Empty, "-" + (-num5).ToString());
                    }
                    else
                    {
                        __instance.Stat_P.slots[0].SetText(originJusticeStat.ToString() + string.Empty);
                    }
                }
                bool flag12 = num6 > 0;
                if (flag12)
                {
                    __instance.Stat_P.slots[1].SetText(originJusticeStat2.ToString() + string.Empty, "+" + num6.ToString());
                }
                else
                {
                    bool flag13 = num6 < 0;
                    if (flag13)
                    {
                        __instance.Stat_P.slots[1].SetText(originJusticeStat2.ToString() + string.Empty, "-" + (-num6).ToString());
                    }
                    else
                    {
                        __instance.Stat_P.slots[1].SetText(originJusticeStat2.ToString() + string.Empty);
                    }
                }
                __instance.Stat_R.Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Rstat"), global::AgentModel.GetLevelGradeText(agent.Rstat));
                __instance.Stat_W.Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Wstat"), global::AgentModel.GetLevelGradeText(agent.Wstat));
                __instance.Stat_B.Fill_Inner.text = string.Format("{0}{2}{1}", global::LocalizeTextDataModel.instance.GetText("Bstat"), global::AgentModel.GetLevelGradeText(agent.Bstat), "\n");
                __instance.Stat_P.Fill_Inner.text = string.Format("{0}{2}{1}", global::LocalizeTextDataModel.instance.GetText("Pstat"), global::AgentModel.GetLevelGradeText(agent.Pstat), "\n");

                __instance.Weapon.StatName.text = agent.Equipment.weapon.metaInfo.Name;
                global::DamageInfo damage = agent.Equipment.weapon.GetDamage(agent);
                __instance.Weapon.Fill.enabled = true;
                __instance.Weapon.Fill_Inner.text = "T";
                __instance.Weapon.Fill_Inner.color = TurquoisePatch.TurquoiseDamageColor;
                __instance.Weapon.Fill_Inner.text = "TURQUOISE";
                __instance.Weapon.Fill.sprite = TurquoisePatch.TurquoiseIcon;
                string text = string.Format("{0}-{1}", (int)damage.min, (int)damage.max);
                __instance.Weapon.slots[0].SetText(text);
                global::DefenseInfo defense = agent.defense;
                global::UIUtil.DefenseSetOnlyText(defense, __instance.DefenseType);
                global::UIUtil.SetDefenseTypeIcon(defense, __instance.DefenseTypeRenderer);
                bool flag15 = agent.Equipment.armor != null;
                if (flag15)
                {
                    __instance.ArmorName.text = agent.Equipment.armor.metaInfo.Name;
                }
                else
                {
                    __instance.ArmorName.text = "Armor is missing";
                }
                Inventory.InventoryItemController.SetGradeText(agent.Equipment.weapon.metaInfo.Grade, __instance.WeaponGrade);
                Inventory.InventoryItemController.SetGradeText(agent.Equipment.armor.metaInfo.Grade, __instance.ArmorGrade);
                for (int i = 0; i < __instance.StatTooltips.Length; i++)
                {
                    string text3 = global::LocalizeTextDataModel.instance.GetText(__instance.StatTooltips[i].ID);
                    string arg = "?";
                    switch (i)
                    {
                        case 0:
                            arg = agent.fortitudeLevel.ToString();
                            break;
                        case 1:
                            arg = agent.prudenceLevel.ToString();
                            break;
                        case 2:
                            arg = agent.temperanceLevel.ToString();
                            break;
                        case 3:
                            arg = agent.justiceLevel.ToString();
                            break;
                        case 4:
                            arg = (agent.workProb / 5).ToString();
                            break;
                        case 5:
                            arg = (agent.workSpeed / 5).ToString();
                            break;
                        case 6:
                            arg = (agent.attackSpeed / 5f).ToString();
                            break;
                        case 7:
                            arg = (agent.movement / 5f).ToString();
                            break;
                    }
                    string dynamicTooltip = string.Format(text3, arg);
                    __instance.StatTooltips[i].SetDynamicTooltip(dynamicTooltip);
                }
                for (int j = 0; j < __instance.DefenseTooltips.Length; j++)
                {
                    string text4 = global::LocalizeTextDataModel.instance.GetText(__instance.DefenseTooltips[j].ID);

                    global::DefenseInfo.Type defenseType = agent.defense.GetDefenseType(j + RwbpType.R);
                    string res = "?";
                    switch (defenseType)
                    {
                        case global::DefenseInfo.Type.NONE:
                            res = global::LocalizeTextDataModel.instance.GetText("DefenseType_None");
                            break;
                        case global::DefenseInfo.Type.WEAKNESS:
                            res = global::LocalizeTextDataModel.instance.GetText("DefenseType_Weak");
                            break;
                        case global::DefenseInfo.Type.SUPER_WEAKNESS:
                            res = global::LocalizeTextDataModel.instance.GetText("DefenseType_SuperWeak");
                            break;
                        case global::DefenseInfo.Type.ENDURE:
                            res = global::LocalizeTextDataModel.instance.GetText("DefenseType_Endure");
                            break;
                        case global::DefenseInfo.Type.RESISTANCE:
                            res = global::LocalizeTextDataModel.instance.GetText("DefenseType_Resist");
                            break;
                        case global::DefenseInfo.Type.IMMUNE:
                            res = global::LocalizeTextDataModel.instance.GetText("DefenseType_Immune");
                            break;
                    }
                    string defenseTypeText = res;
                    string dynamicTooltip2 = string.Format(text4, defenseTypeText);
                    __instance.DefenseTooltips[j].SetDynamicTooltip(dynamicTooltip2);
                }
                return false;
            }

            return true;
        }

        public static bool UnitModel_MakeDamageEffect(UnitModel __instance, RwbpType type, float value, DefenseInfo.Type defense) //Pre
        {
            bool flag = type == (RwbpType)654321;
            bool result;
            if (flag)
            {
                bool flag5 = ResearchDataModel.instance.IsUpgradedAbility("damage_text") || (GlobalGameManager.instance.gameMode == GameMode.TUTORIAL && GlobalGameManager.instance.tutorialStep > 1);
                if (flag5)
                {
                    DamageEffect damageEffect = DamageEffect.Invoker(__instance.GetMovableNode());
                    TurquoisePatch.SetCustomDamageEffect(damageEffect, (int)value, __instance);
                    damageEffect.Dettach();
                    damageEffect.transform.localPosition = new Vector3(damageEffect.transform.localPosition.x + UnityEngine.Random.Range(-0.5f, 0.5f), damageEffect.transform.localPosition.y, damageEffect.transform.localPosition.z);
                }
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }




        public static void SetCustomDamageEffect(DamageEffect damageEffect, int damage, UnitModel unit)
        {
            damageEffect.DefenseTypeInner.gameObject.SetActive(false);
            damageEffect.DamageCount.rectTransform.anchoredPosition = new Vector2(-83f, damageEffect.DamageCount.rectTransform.anchoredPosition.y);
            damageEffect.Icon.rectTransform.anchoredPosition = new Vector2(5f, damageEffect.Icon.rectTransform.anchoredPosition.y);
            damageEffect.IconOut.rectTransform.anchoredPosition = damageEffect.Icon.rectTransform.anchoredPosition;
            damageEffect.DefenseTypeText.enabled = false;
            Sprite gazeIcon = TurquoisePatch.TurquoiseIcon;
            damageEffect.DamageCount.text = damage.ToString();
            damageEffect.Icon.sprite = gazeIcon;
            damageEffect.IconOut.sprite = null;
            Graphic frame = damageEffect.Frame;
            Color color = Color.black;
            Color black = Color.black;

            color = TurquoisePatch.TurquoiseDamageColor;
            damageEffect.Icon.sprite = TurquoisePatch.TurquoiseIcon;

            damageEffect.DamageCount.color = color;
            damageEffect.DamageContext.color = color;
            Graphic fill = damageEffect.Fill;
            color = black;
            damageEffect.DamageCountOutline.effectColor = color;
            fill.color = color;
            damageEffect.IconOut.color = black;
            damageEffect.DefenseTypeText.color = color;
            damageEffect.DefenseTypeInner.color = black;
            damageEffect.IconOut.rectTransform.anchoredPosition = damageEffect.Icon.rectTransform.anchoredPosition;
            damageEffect.Icon.color = Color.white;
            frame.color = color;
        }
        public static void MakeDamageIcon()
        {
            Texture2D texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(File.ReadAllBytes(TurquoisePatch.path + "/Image/AttackType_Turquoise.png"));
            TurquoisePatch.TurquoiseIcon = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
        }


        public static Color TurquoiseDamageColor = new Color(0.3f, 0.8f, 0.9f);

        public static string path;

        public static Sprite TurquoiseIcon;

        public static Sprite TurquoiseWorkDamageRoomSprite;




    }
}
