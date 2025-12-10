/* !!auto gen do not change
 
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Table
{
    public partial class NPCConfig
    {
	    
        public uint ID;
        public string Name;
        public string DialogueText;
        public string InteractionDistance;
        public string InteractionKey;
        public string PositionX;
        public string PositionY;
        public string PositionZ;
        public string RotationY;

        static int memberCount = 9 ; 
        public NPCConfig()
        {
        }

        public NPCConfig( uint ID, string Name, string DialogueText, string InteractionDistance, string InteractionKey, string PositionX, string PositionY, string PositionZ, string RotationY)
        {
            this.ID = ID;
            this.Name = Name;
            this.DialogueText = DialogueText;
            this.InteractionDistance = InteractionDistance;
            this.InteractionKey = InteractionKey;
            this.PositionX = PositionX;
            this.PositionY = PositionY;
            this.PositionZ = PositionZ;
            this.RotationY = RotationY;

        }
        public static Dictionary<uint, NPCConfig> _datas = new Dictionary<uint, NPCConfig>();
		public static bool loaded = false;
		public static  Dictionary<uint, NPCConfig> datas
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
                
                NPCConfig dataNPCConfig = new NPCConfig();
                dataNPCConfig.ID = br.ReadUInt32();
                dataNPCConfig.Name = br.ReadString();
                dataNPCConfig.DialogueText = br.ReadString();
                dataNPCConfig.InteractionDistance = br.ReadString();
                dataNPCConfig.InteractionKey = br.ReadString();
                dataNPCConfig.PositionX = br.ReadString();
                dataNPCConfig.PositionY = br.ReadString();
                dataNPCConfig.PositionZ = br.ReadString();
                dataNPCConfig.RotationY = br.ReadString();
                if (_datas.ContainsKey(dataNPCConfig.ID))
                {
                #if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPaused = true;
                #endif
                    throw new ArgumentException("数据有误,主键重复:" + dataNPCConfig.ID);
                }
                _datas.Add(dataNPCConfig.ID,dataNPCConfig);
                
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
                    Debug.LogError("NPCConfig严重错误，表头和表数据长度不一样");
#if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPaused = true;
#endif
                    throw new ArgumentException("NPCConfig严重错误，表头和表数据长度不一样");
                    return;
                }
                NPCConfig dataNPCConfig = new NPCConfig();
                if(!uint.TryParse(values[0],out dataNPCConfig.ID))
                {

#if UNITY_EDITOR
                    Debug.LogError("数据有误:" + values[0] + " to uint");
                    UnityEditor.EditorApplication.isPaused = true;
#endif
                    throw new ArgumentException("数据有误:" + values[0] + " to uint" + " 第"+ i + "行,第0列");
                 
                }
                dataNPCConfig.Name = values[1];
                dataNPCConfig.DialogueText = values[2];
                dataNPCConfig.InteractionDistance = values[3];
                dataNPCConfig.InteractionKey = values[4];
                dataNPCConfig.PositionX = values[5];
                dataNPCConfig.PositionY = values[6];
                dataNPCConfig.PositionZ = values[7];
                dataNPCConfig.RotationY = values[8];
                if (_datas.ContainsKey(dataNPCConfig.ID))
                {
               #if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPaused = true;
                #endif
                    throw new ArgumentException("数据有误,主键重复:" + dataNPCConfig.ID);
                }
                _datas.Add(dataNPCConfig.ID,dataNPCConfig);
            }

        }

        public static void LoadFromResources()
        {           
			Clear();
			string path = "";
			TextAsset data = null;

			path = "Table/NPCConfig.csv"; 
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
                NPCConfig.LoadFromString(text);
        }

        public static void LoadBinFromResources()
        {           
			Clear();
			loaded = true;
			string path = "";
			TextAsset data = null;
			path = "TableBin/NPCConfig.bytes"; 
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
                NPCConfig.LoadFromBinanry(text);
        }

        public static void LoadFromStreaming()
        {
            try
            {
                string url = "Table/NPCConfig.csv";
                string content = FileUtils.ReadStringFromStreaming(url);

                LoadFromString(content);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("表NPCConfig数据有误! ({0})",ex.Message));
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

        public static NPCConfig Get(uint ID)
        {
#if UNITY_EDITOR
            if (!Contains(ID))
            {

                Debug.LogError("表NPCConfig没有元素" + ID + ",检测一下Excel表");
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
        public static string GetDialogueText(uint ID)
        {
            return Get(ID).DialogueText;
        }
        public static string GetInteractionDistance(uint ID)
        {
            return Get(ID).InteractionDistance;
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