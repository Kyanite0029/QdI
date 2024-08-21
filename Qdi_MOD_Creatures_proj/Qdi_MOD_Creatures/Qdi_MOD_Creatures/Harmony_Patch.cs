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
    public class Harmony_Patch
    {
        public Harmony_Patch()
        {
            try
            {

                //하모니 인스턴스 Lobotomy.QDI.QDI
                HarmonyInstance harmonyInstance = HarmonyInstance.Create("Lobotomy.QDI.QDI");
                //T데미지 적용
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
