/* !!auto gen do not change
 
 */

using UnityEngine;
using System.Collections;

namespace Table
{
    public class TableLoad
    {
/*
        public static void LoadFromMemory()
        {
            AddDeviceProgess.LoadFromMemory();
            NPCConfig.LoadFromMemory();
            PortalConfig.LoadFromMemory();

        }
*/
        public static void LoadFromResources()
        {
            AddDeviceProgess.LoadFromResources();
            NPCConfig.LoadFromResources();
            PortalConfig.LoadFromResources();

        }

        public static void LoadBinFromResources()
        {
            AddDeviceProgess.LoadBinFromResources();
            NPCConfig.LoadBinFromResources();
            PortalConfig.LoadBinFromResources();

        }

        public static void LoadFromStreaming()
        {
            AddDeviceProgess.LoadFromStreaming();
            NPCConfig.LoadFromStreaming();
            PortalConfig.LoadFromStreaming();

        }

        public static void Clear()
        {
            AddDeviceProgess.Clear();
            NPCConfig.Clear();
            PortalConfig.Clear();

        }
		
        public static void UnLoad()
        {
            AddDeviceProgess.UnLoad();
            NPCConfig.UnLoad();
            PortalConfig.UnLoad();

        }
    }
}