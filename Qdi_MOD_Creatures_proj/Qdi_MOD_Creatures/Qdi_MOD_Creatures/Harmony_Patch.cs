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
    [HarmonyPatch(typeof(InventorySlot), ("get_info"))]
    public class Harmony_Patch
    {
        public Harmony_Patch()
        {
            try
            {
                
                HarmonyInstance harmonyInstance = HarmonyInstance.Create("Lobotomy.QDI.QDI");
                TurquoisePatch.Patch(harmonyInstance);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }

            LobotomyBaseMod.ModDebug.Log("QDI Loaded");


        }




    }
}
