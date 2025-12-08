/* !!auto gen do not change
 
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Table
{
    public partial class AddDeviceProgess
    {
	    
        public uint ID;
        public string Name;
        public string LoadPath;
        public string DeviceInfo;

        static int memberCount = 4 ; 
        public AddDeviceProgess()
        {
        }

        public AddDeviceProgess( uint ID, string Name, string LoadPath, string DeviceInfo)
        {
            this.ID = ID;
            this.Name = Name;
            this.LoadPath = LoadPath;
            this.DeviceInfo = DeviceInfo;

        }
        public static Dictionary<uint, AddDeviceProgess> _datas = new Dictionary<uint, AddDeviceProgess>();
		public static bool loaded = false;
		public static  Dictionary<uint, AddDeviceProgess> datas
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
                
                AddDeviceProgess dataAddDeviceProgess = new AddDeviceProgess();
                dataAddDeviceProgess.ID = br.ReadUInt32();
                dataAddDeviceProgess.Name = br.ReadString();
                dataAddDeviceProgess.LoadPath = br.ReadString();
                dataAddDeviceProgess.DeviceInfo = br.ReadString();
                if (_datas.ContainsKey(dataAddDeviceProgess.ID))
                {
                #if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPaused = true;
                #endif
                    throw new ArgumentException("数据有误,主键重复:" + dataAddDeviceProgess.ID);
                }
                _datas.Add(dataAddDeviceProgess.ID,dataAddDeviceProgess);
                
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
                    Debug.LogError("AddDeviceProgess严重错误，表头和表数据长度不一样");
#if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPaused = true;
#endif
                    throw new ArgumentException("AddDeviceProgess严重错误，表头和表数据长度不一样");
                    return;
                }
                AddDeviceProgess dataAddDeviceProgess = new AddDeviceProgess();
                if(!uint.TryParse(values[0],out dataAddDeviceProgess.ID))
                {

#if UNITY_EDITOR
                    Debug.LogError("数据有误:" + values[0] + " to uint");
                    UnityEditor.EditorApplication.isPaused = true;
#endif
                    throw new ArgumentException("数据有误:" + values[0] + " to uint" + " 第"+ i + "行,第0列");
                 
                }
                dataAddDeviceProgess.Name = values[1];
                dataAddDeviceProgess.LoadPath = values[2];
                dataAddDeviceProgess.DeviceInfo = values[3];
                if (_datas.ContainsKey(dataAddDeviceProgess.ID))
                {
               #if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPaused = true;
                #endif
                    throw new ArgumentException("数据有误,主键重复:" + dataAddDeviceProgess.ID);
                }
                _datas.Add(dataAddDeviceProgess.ID,dataAddDeviceProgess);
            }

        }

        public static void LoadFromResources()
        {           
			Clear();
			string path = "";
			TextAsset data = null;

			path = "Table/AddDeviceProgess.csv"; 
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
                AddDeviceProgess.LoadFromString(text);
        }

        public static void LoadBinFromResources()
        {           
			Clear();
			loaded = true;
			string path = "";
			TextAsset data = null;
			path = "TableBin/AddDeviceProgess.bytes"; 
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
                AddDeviceProgess.LoadFromBinanry(text);
        }

        public static void LoadFromStreaming()
        {
            try
            {
                string url = "Table/AddDeviceProgess.csv";
                string content = FileUtils.ReadStringFromStreaming(url);

                LoadFromString(content);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("表AddDeviceProgess数据有误! ({0})",ex.Message));
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

        public static AddDeviceProgess Get(uint ID)
        {
#if UNITY_EDITOR
            if (!Contains(ID))
            {

                Debug.LogError("表AddDeviceProgess没有元素" + ID + ",检测一下Excel表");
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
        public static string GetLoadPath(uint ID)
        {
            return Get(ID).LoadPath;
        }
        public static string GetDeviceInfo(uint ID)
        {
            return Get(ID).DeviceInfo;
        }

    }
}