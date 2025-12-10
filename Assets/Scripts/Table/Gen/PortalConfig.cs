/* !!auto gen do not change
 
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Table
{
    public partial class PortalConfig
    {
	    
        public uint ID;
        public string Name;
        public string TargetSceneName;
        public string Description;
        public string InteractionDistance;
        public string TeleportDelay;
        public string InteractionKey;
        public string PositionX;
        public string PositionY;
        public string PositionZ;
        public string RotationY;

        static int memberCount = 11 ; 
        public PortalConfig()
        {
        }

        public PortalConfig( uint ID, string Name, string TargetSceneName, string Description, string InteractionDistance, string TeleportDelay, string InteractionKey, string PositionX, string PositionY, string PositionZ, string RotationY)
        {
            this.ID = ID;
            this.Name = Name;
            this.TargetSceneName = TargetSceneName;
            this.Description = Description;
            this.InteractionDistance = InteractionDistance;
            this.TeleportDelay = TeleportDelay;
            this.InteractionKey = InteractionKey;
            this.PositionX = PositionX;
            this.PositionY = PositionY;
            this.PositionZ = PositionZ;
            this.RotationY = RotationY;

        }
        public static Dictionary<uint, PortalConfig> _datas = new Dictionary<uint, PortalConfig>();
		public static bool loaded = false;
		public static  Dictionary<uint, PortalConfig> datas
        {
            get
            {
				if(!loaded)
				{
				LoadBinFromResources();
				}
                return _datas;
            }
			
			set
			{
				_datas = value;
			}
        }
        
        public static void LoadFromBinanry(byte[] bytes)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
            System.IO.BinaryReader br = new System.IO.BinaryReader(ms);
            int length = br.ReadInt32();
            
            for (int i = 0; i < length; i++)
            {
                br.ReadByte();
            }

            int looplength = br.ReadInt32();
            for (int i = 0; i < looplength; i++)
            {
                
                PortalConfig dataPortalConfig = new PortalConfig();
                dataPortalConfig.ID = br.ReadUInt32();
                dataPortalConfig.Name = br.ReadString();
                dataPortalConfig.TargetSceneName = br.ReadString();
                dataPortalConfig.Description = br.ReadString();
                dataPortalConfig.InteractionDistance = br.ReadString();
                dataPortalConfig.TeleportDelay = br.ReadString();
                dataPortalConfig.InteractionKey = br.ReadString();
                dataPortalConfig.PositionX = br.ReadString();
                dataPortalConfig.PositionY = br.ReadString();
                dataPortalConfig.PositionZ = br.ReadString();
                dataPortalConfig.RotationY = br.ReadString();
                if (_datas.ContainsKey(dataPortalConfig.ID))
                {
                #if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPaused = true;
                #endif
                    throw new ArgumentException("数据有误,主键重复:" + dataPortalConfig.ID);
                }
                _datas.Add(dataPortalConfig.ID,dataPortalConfig);
                
            }
            br.Close();
            ms.Close();
        }
		
        public static void LoadFromString(string data)
        {

            string content = data;
            string[] lines = content.Split('\n');
       

            for (int i = 2; i < lines.Length; i++)
            {
                string line = lines[i];
                line = line.Replace("\r", "");
                if(string.IsNullOrEmpty(line)) continue;
                string[] values = line.Split('\t');
                if(values.Length != memberCount)
                {
                    Debug.LogError("PortalConfig严重错误，表头和表数据长度不一样");
#if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPaused = true;
#endif
                    throw new ArgumentException("PortalConfig严重错误，表头和表数据长度不一样");
                    return;
                }
                PortalConfig dataPortalConfig = new PortalConfig();
                if(!uint.TryParse(values[0],out dataPortalConfig.ID))
                {

#if UNITY_EDITOR
                    Debug.LogError("数据有误:" + values[0] + " to uint");
                    UnityEditor.EditorApplication.isPaused = true;
#endif
                    throw new ArgumentException("数据有误:" + values[0] + " to uint" + " 第"+ i + "行,第0列");
                 
                }
                dataPortalConfig.Name = values[1];
                dataPortalConfig.TargetSceneName = values[2];
                dataPortalConfig.Description = values[3];
                dataPortalConfig.InteractionDistance = values[4];
                dataPortalConfig.TeleportDelay = values[5];
                dataPortalConfig.InteractionKey = values[6];
                dataPortalConfig.PositionX = values[7];
                dataPortalConfig.PositionY = values[8];
                dataPortalConfig.PositionZ = values[9];
                dataPortalConfig.RotationY = values[10];
                if (_datas.ContainsKey(dataPortalConfig.ID))
                {
               #if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPaused = true;
                #endif
                    throw new ArgumentException("数据有误,主键重复:" + dataPortalConfig.ID);
                }
                _datas.Add(dataPortalConfig.ID,dataPortalConfig);
            }

        }

        public static void LoadFromResources()
        {           
			Clear();
			string path = "";
			TextAsset data = null;

			path = "Table/PortalConfig.csv"; 
                data = ResManager.Load<TextAsset>(path);
				if(data == null)
				{
				    Debug.LogError(path + " 不存在！！！！");
					#if UNITY_EDITOR
                    	UnityEditor.EditorApplication.isPaused = true;
					#endif
					return;
				}
                string text = data.text;
				if(string.IsNullOrEmpty(text))
				{
					
				    Debug.LogError(path + " 没有内容");
					#if UNITY_EDITOR
                    	UnityEditor.EditorApplication.isPaused = true;
					#endif
					return;
				}
                PortalConfig.LoadFromString(text);
        }

        public static void LoadBinFromResources()
        {           
			Clear();
			loaded = true;
			string path = "";
			TextAsset data = null;
			path = "TableBin/PortalConfig.bytes"; 
                data = ResManager.Load<TextAsset>(path);
				if(data == null)
				{
				    Debug.LogError(path + " 不存在！！！！");
					#if UNITY_EDITOR
                    	UnityEditor.EditorApplication.isPaused = true;
					#endif
					return;
				}
                byte [] text = data.bytes;
				if(text == null || text.Length == 0)
				{
					
				    Debug.LogError(path + " 没有内容");
					#if UNITY_EDITOR
                    	UnityEditor.EditorApplication.isPaused = true;
					#endif
					return;
				}
                PortalConfig.LoadFromBinanry(text);
        }

        public static void LoadFromStreaming()
        {
            try
            {
                string url = "Table/PortalConfig.csv";
                string content = FileUtils.ReadStringFromStreaming(url);

                LoadFromString(content);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("表PortalConfig数据有误! ({0})",ex.Message));
            }
        }


		public static void UnLoad()
		{
			Clear();
		}
        public static void Clear()
        {
        	if(_datas != null && _datas.Count != 0)
            	_datas.Clear();
        }

        public static bool Contains(uint ID)
        {    
            return datas.ContainsKey(ID);
        }

        public static PortalConfig Get(uint ID)
        {
#if UNITY_EDITOR
            if (!Contains(ID))
            {

                Debug.LogError("表PortalConfig没有元素" + ID + ",检测一下Excel表");
                #if UNITY_EDITOR
                      UnityEditor.EditorApplication.isPaused = true;
                #endif
                return null;
            }
#endif
            return datas[ID];
        }


        public static string GetName(uint ID)
        {
            return Get(ID).Name;
        }
        public static string GetTargetSceneName(uint ID)
        {
            return Get(ID).TargetSceneName;
        }
        public static string GetDescription(uint ID)
        {
            return Get(ID).Description;
        }
        public static string GetInteractionDistance(uint ID)
        {
            return Get(ID).InteractionDistance;
        }
        public static string GetTeleportDelay(uint ID)
        {
            return Get(ID).TeleportDelay;
        }
        public static string GetInteractionKey(uint ID)
        {
            return Get(ID).InteractionKey;
        }
        public static string GetPositionX(uint ID)
        {
            return Get(ID).PositionX;
        }
        public static string GetPositionY(uint ID)
        {
            return Get(ID).PositionY;
        }
        public static string GetPositionZ(uint ID)
        {
            return Get(ID).PositionZ;
        }
        public static string GetRotationY(uint ID)
        {
            return Get(ID).RotationY;
        }

    }
}