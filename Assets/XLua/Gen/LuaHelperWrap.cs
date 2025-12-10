#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class LuaHelperWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(LuaHelper);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 110, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateLuaTable", _m_CreateLuaTable_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadLuaUI", _m_LoadLuaUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadUI", _m_LoadUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ShowUI", _m_ShowUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "HideLuaUI", _m_HideLuaUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ShowLuaUI", _m_ShowLuaUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "HideUI", _m_HideUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnloadLuaUI", _m_UnloadLuaUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnloadUI", _m_UnloadUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "HideAllUI", _m_HideAllUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadResource", _m_LoadResource_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadResourceAsync", _m_LoadResourceAsync_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadGameObject", _m_LoadGameObject_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadTexture", _m_LoadTexture_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadSprite", _m_LoadSprite_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadAudioClip", _m_LoadAudioClip_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadTextAsset", _m_LoadTextAsset_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadMaterial", _m_LoadMaterial_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadGameObjectAsync", _m_LoadGameObjectAsync_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadTextureAsync", _m_LoadTextureAsync_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadSpriteAsync", _m_LoadSpriteAsync_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadAudioClipAsync", _m_LoadAudioClipAsync_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Instantiate", _m_Instantiate_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Log", _m_Log_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LogWarning", _m_LogWarning_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LogError", _m_LogError_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateGameObject", _m_CreateGameObject_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DestroyGameObject", _m_DestroyGameObject_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "FindGameObject", _m_FindGameObject_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "FindGameObjectWithTag", _m_FindGameObjectWithTag_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DelayCall", _m_DelayCall_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadScene", _m_LoadScene_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadSceneAsync", _m_LoadSceneAsync_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "PreloadScene", _m_PreloadScene_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnloadScene", _m_UnloadScene_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetCurrentSceneName", _m_GetCurrentSceneName_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "IsSceneLoading", _m_IsSceneLoading_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ReloadCurrentScene", _m_ReloadCurrentScene_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ReloadCurrentSceneAsync", _m_ReloadCurrentSceneAsync_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "PlayMusic", _m_PlayMusic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "StopMusic", _m_StopMusic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "PauseMusic", _m_PauseMusic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ResumeMusic", _m_ResumeMusic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SwitchMusic", _m_SwitchMusic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "PlaySound", _m_PlaySound_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "StopAllSounds", _m_StopAllSounds_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "StopSound", _m_StopSound_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetMusicVolume", _m_SetMusicVolume_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetSoundVolume", _m_SetSoundVolume_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetMusicVolume", _m_GetMusicVolume_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetSoundVolume", _m_GetSoundVolume_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetMusicMuted", _m_SetMusicMuted_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetSoundMuted", _m_SetSoundMuted_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "IsMusicMuted", _m_IsMusicMuted_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "IsSoundMuted", _m_IsSoundMuted_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "PreloadAudio", _m_PreloadAudio_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnloadAudio", _m_UnloadAudio_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ClearAudioCache", _m_ClearAudioCache_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "IsMusicPlaying", _m_IsMusicPlaying_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetCurrentMusicName", _m_GetCurrentMusicName_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetActiveSoundCount", _m_GetActiveSoundCount_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SaveInt", _m_SaveInt_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadInt", _m_LoadInt_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SaveFloat", _m_SaveFloat_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadFloat", _m_LoadFloat_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SaveString", _m_SaveString_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadString", _m_LoadString_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SaveBool", _m_SaveBool_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadBool", _m_LoadBool_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SaveToJson", _m_SaveToJson_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadFromJson", _m_LoadFromJson_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DeleteData", _m_DeleteData_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "HasData", _m_HasData_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DeleteAll", _m_DeleteAll_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetEncryptionEnabled", _m_SetEncryptionEnabled_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetSaveDataPath", _m_GetSaveDataPath_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreatePool", _m_CreatePool_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetFromPool", _m_GetFromPool_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ReleaseToPool", _m_ReleaseToPool_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "PreloadPool", _m_PreloadPool_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AutoReleasePool", _m_AutoReleasePool_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DestroyPool", _m_DestroyPool_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ClearPool", _m_ClearPool_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetPoolInfo", _m_GetPoolInfo_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetPoolMaxSize", _m_SetPoolMaxSize_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetPoolDefaultAutoReleaseTime", _m_SetPoolDefaultAutoReleaseTime_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ReleaseAllFromPool", _m_ReleaseAllFromPool_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterEvent", _m_RegisterEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterEventWithPriority", _m_RegisterEventWithPriority_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnregisterEvent", _m_UnregisterEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnregisterEventWithPriority", _m_UnregisterEventWithPriority_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnregisterAllEvents", _m_UnregisterAllEvents_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "TriggerEvent", _m_TriggerEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "HasEvent", _m_HasEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetEventListenerCount", _m_GetEventListenerCount_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ClearAllEvents", _m_ClearAllEvents_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "StartCoroutine", _m_StartCoroutine_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "StartCoroutineWithParam", _m_StartCoroutineWithParam_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "StopCoroutine", _m_StopCoroutine_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "StopAllCoroutines", _m_StopAllCoroutines_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "WaitForSeconds", _m_WaitForSeconds_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "WaitForFrames", _m_WaitForFrames_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "WaitForEndOfFrame", _m_WaitForEndOfFrame_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "WaitForFixedUpdate", _m_WaitForFixedUpdate_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "WaitSeconds", _m_WaitSeconds_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "WaitFrames", _m_WaitFrames_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "WaitUntil", _m_WaitUntil_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Repeat", _m_Repeat_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "StopRepeat", _m_StopRepeat_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "LuaHelper does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateLuaTable_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    object _luaTable = translator.GetObject(L, 1, typeof(object));
                    
                        var gen_ret = LuaHelper.CreateLuaTable( _luaTable );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadLuaUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<object>(L, 3)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 4)) 
                {
                    string _uiName = LuaAPI.lua_tostring(L, 1);
                    string _uiPath = LuaAPI.lua_tostring(L, 2);
                    object _luaCallbacks = translator.GetObject(L, 3, typeof(object));
                    bool _showImmediately = LuaAPI.lua_toboolean(L, 4);
                    
                        var gen_ret = LuaHelper.LoadLuaUI( _uiName, _uiPath, _luaCallbacks, _showImmediately );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<object>(L, 3)) 
                {
                    string _uiName = LuaAPI.lua_tostring(L, 1);
                    string _uiPath = LuaAPI.lua_tostring(L, 2);
                    object _luaCallbacks = translator.GetObject(L, 3, typeof(object));
                    
                        var gen_ret = LuaHelper.LoadLuaUI( _uiName, _uiPath, _luaCallbacks );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _uiName = LuaAPI.lua_tostring(L, 1);
                    string _uiPath = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = LuaHelper.LoadLuaUI( _uiName, _uiPath );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.LoadLuaUI!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)) 
                {
                    string _uiTypeName = LuaAPI.lua_tostring(L, 1);
                    string _uiPath = LuaAPI.lua_tostring(L, 2);
                    bool _showImmediately = LuaAPI.lua_toboolean(L, 3);
                    
                        var gen_ret = LuaHelper.LoadUI( _uiTypeName, _uiPath, _showImmediately );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _uiTypeName = LuaAPI.lua_tostring(L, 1);
                    string _uiPath = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = LuaHelper.LoadUI( _uiTypeName, _uiPath );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.LoadUI!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ShowUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _uiTypeName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.ShowUI( _uiTypeName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HideLuaUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _uiName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.HideLuaUI( _uiName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ShowLuaUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _uiName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.ShowLuaUI( _uiName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HideUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _uiTypeName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.HideUI( _uiTypeName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnloadLuaUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _uiName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.UnloadLuaUI( _uiName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnloadUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _uiTypeName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.UnloadUI( _uiTypeName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HideAllUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    LuaHelper.HideAllUI(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadResource_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    string _typeName = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = LuaHelper.LoadResource( _path, _typeName );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadResourceAsync_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    string _typeName = LuaAPI.lua_tostring(L, 2);
                    System.Action<UnityEngine.Object> _onComplete = translator.GetDelegate<System.Action<UnityEngine.Object>>(L, 3);
                    
                    LuaHelper.LoadResourceAsync( _path, _typeName, _onComplete );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadGameObject_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.LoadGameObject( _path );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadTexture_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.LoadTexture( _path );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadSprite_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.LoadSprite( _path );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAudioClip_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.LoadAudioClip( _path );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadTextAsset_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.LoadTextAsset( _path );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadMaterial_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.LoadMaterial( _path );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadGameObjectAsync_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    System.Action<UnityEngine.GameObject> _onComplete = translator.GetDelegate<System.Action<UnityEngine.GameObject>>(L, 2);
                    
                    LuaHelper.LoadGameObjectAsync( _path, _onComplete );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadTextureAsync_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    System.Action<UnityEngine.Texture2D> _onComplete = translator.GetDelegate<System.Action<UnityEngine.Texture2D>>(L, 2);
                    
                    LuaHelper.LoadTextureAsync( _path, _onComplete );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadSpriteAsync_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    System.Action<UnityEngine.Sprite> _onComplete = translator.GetDelegate<System.Action<UnityEngine.Sprite>>(L, 2);
                    
                    LuaHelper.LoadSpriteAsync( _path, _onComplete );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAudioClipAsync_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    System.Action<UnityEngine.AudioClip> _onComplete = translator.GetDelegate<System.Action<UnityEngine.AudioClip>>(L, 2);
                    
                    LuaHelper.LoadAudioClipAsync( _path, _onComplete );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Instantiate_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1&& translator.Assignable<UnityEngine.GameObject>(L, 1)) 
                {
                    UnityEngine.GameObject _prefab = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    
                        var gen_ret = LuaHelper.Instantiate( _prefab );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.GameObject>(L, 1)&& translator.Assignable<UnityEngine.Transform>(L, 2)) 
                {
                    UnityEngine.GameObject _prefab = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    UnityEngine.Transform _parent = (UnityEngine.Transform)translator.GetObject(L, 2, typeof(UnityEngine.Transform));
                    
                        var gen_ret = LuaHelper.Instantiate( _prefab, _parent );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.Instantiate!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Log_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    object _message = translator.GetObject(L, 1, typeof(object));
                    
                    LuaHelper.Log( _message );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogWarning_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    object _message = translator.GetObject(L, 1, typeof(object));
                    
                    LuaHelper.LogWarning( _message );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogError_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    object _message = translator.GetObject(L, 1, typeof(object));
                    
                    LuaHelper.LogError( _message );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateGameObject_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.CreateGameObject( _name );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DestroyGameObject_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    
                    LuaHelper.DestroyGameObject( _obj );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FindGameObject_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.FindGameObject( _name );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FindGameObjectWithTag_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _tag = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.FindGameObjectWithTag( _tag );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DelayCall_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<System.Action>(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 1);
                    float _delay = (float)LuaAPI.lua_tonumber(L, 2);
                    
                    LuaHelper.DelayCall( _callback, _delay );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& translator.Assignable<System.Action>(L, 1)) 
                {
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 1);
                    
                    LuaHelper.DelayCall( _callback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.DelayCall!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadScene_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _sceneName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.LoadScene( _sceneName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadSceneAsync_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<float>>(L, 2)&& translator.Assignable<System.Action>(L, 3)) 
                {
                    string _sceneName = LuaAPI.lua_tostring(L, 1);
                    System.Action<float> _onProgress = translator.GetDelegate<System.Action<float>>(L, 2);
                    System.Action _onComplete = translator.GetDelegate<System.Action>(L, 3);
                    
                    LuaHelper.LoadSceneAsync( _sceneName, _onProgress, _onComplete );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<float>>(L, 2)) 
                {
                    string _sceneName = LuaAPI.lua_tostring(L, 1);
                    System.Action<float> _onProgress = translator.GetDelegate<System.Action<float>>(L, 2);
                    
                    LuaHelper.LoadSceneAsync( _sceneName, _onProgress );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _sceneName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.LoadSceneAsync( _sceneName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.LoadSceneAsync!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PreloadScene_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action>(L, 2)) 
                {
                    string _sceneName = LuaAPI.lua_tostring(L, 1);
                    System.Action _onComplete = translator.GetDelegate<System.Action>(L, 2);
                    
                    LuaHelper.PreloadScene( _sceneName, _onComplete );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _sceneName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.PreloadScene( _sceneName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.PreloadScene!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnloadScene_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action>(L, 2)) 
                {
                    string _sceneName = LuaAPI.lua_tostring(L, 1);
                    System.Action _onComplete = translator.GetDelegate<System.Action>(L, 2);
                    
                    LuaHelper.UnloadScene( _sceneName, _onComplete );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _sceneName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.UnloadScene( _sceneName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.UnloadScene!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetCurrentSceneName_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.GetCurrentSceneName(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsSceneLoading_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.IsSceneLoading(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ReloadCurrentScene_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    LuaHelper.ReloadCurrentScene(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ReloadCurrentSceneAsync_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<System.Action<float>>(L, 1)&& translator.Assignable<System.Action>(L, 2)) 
                {
                    System.Action<float> _onProgress = translator.GetDelegate<System.Action<float>>(L, 1);
                    System.Action _onComplete = translator.GetDelegate<System.Action>(L, 2);
                    
                    LuaHelper.ReloadCurrentSceneAsync( _onProgress, _onComplete );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& translator.Assignable<System.Action<float>>(L, 1)) 
                {
                    System.Action<float> _onProgress = translator.GetDelegate<System.Action<float>>(L, 1);
                    
                    LuaHelper.ReloadCurrentSceneAsync( _onProgress );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 0) 
                {
                    
                    LuaHelper.ReloadCurrentSceneAsync(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.ReloadCurrentSceneAsync!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PlayMusic_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    string _clipName = LuaAPI.lua_tostring(L, 1);
                    bool _loop = LuaAPI.lua_toboolean(L, 2);
                    bool _fadeIn = LuaAPI.lua_toboolean(L, 3);
                    float _fadeTime = (float)LuaAPI.lua_tonumber(L, 4);
                    
                    LuaHelper.PlayMusic( _clipName, _loop, _fadeIn, _fadeTime );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)) 
                {
                    string _clipName = LuaAPI.lua_tostring(L, 1);
                    bool _loop = LuaAPI.lua_toboolean(L, 2);
                    bool _fadeIn = LuaAPI.lua_toboolean(L, 3);
                    
                    LuaHelper.PlayMusic( _clipName, _loop, _fadeIn );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)) 
                {
                    string _clipName = LuaAPI.lua_tostring(L, 1);
                    bool _loop = LuaAPI.lua_toboolean(L, 2);
                    
                    LuaHelper.PlayMusic( _clipName, _loop );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _clipName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.PlayMusic( _clipName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.PlayMusic!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_StopMusic_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    bool _fadeOut = LuaAPI.lua_toboolean(L, 1);
                    float _fadeTime = (float)LuaAPI.lua_tonumber(L, 2);
                    
                    LuaHelper.StopMusic( _fadeOut, _fadeTime );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool _fadeOut = LuaAPI.lua_toboolean(L, 1);
                    
                    LuaHelper.StopMusic( _fadeOut );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 0) 
                {
                    
                    LuaHelper.StopMusic(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.StopMusic!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PauseMusic_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    LuaHelper.PauseMusic(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ResumeMusic_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    LuaHelper.ResumeMusic(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SwitchMusic_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    string _newClipName = LuaAPI.lua_tostring(L, 1);
                    bool _loop = LuaAPI.lua_toboolean(L, 2);
                    float _fadeTime = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    LuaHelper.SwitchMusic( _newClipName, _loop, _fadeTime );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)) 
                {
                    string _newClipName = LuaAPI.lua_tostring(L, 1);
                    bool _loop = LuaAPI.lua_toboolean(L, 2);
                    
                    LuaHelper.SwitchMusic( _newClipName, _loop );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _newClipName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.SwitchMusic( _newClipName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.SwitchMusic!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PlaySound_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    string _clipName = LuaAPI.lua_tostring(L, 1);
                    float _volume = (float)LuaAPI.lua_tonumber(L, 2);
                    float _pitch = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    LuaHelper.PlaySound( _clipName, _volume, _pitch );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    string _clipName = LuaAPI.lua_tostring(L, 1);
                    float _volume = (float)LuaAPI.lua_tonumber(L, 2);
                    
                    LuaHelper.PlaySound( _clipName, _volume );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _clipName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.PlaySound( _clipName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.PlaySound!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_StopAllSounds_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    LuaHelper.StopAllSounds(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_StopSound_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _clipName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.StopSound( _clipName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetMusicVolume_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float _volume = (float)LuaAPI.lua_tonumber(L, 1);
                    
                    LuaHelper.SetMusicVolume( _volume );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetSoundVolume_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float _volume = (float)LuaAPI.lua_tonumber(L, 1);
                    
                    LuaHelper.SetSoundVolume( _volume );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetMusicVolume_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.GetMusicVolume(  );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetSoundVolume_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.GetSoundVolume(  );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetMusicMuted_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    bool _muted = LuaAPI.lua_toboolean(L, 1);
                    
                    LuaHelper.SetMusicMuted( _muted );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetSoundMuted_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    bool _muted = LuaAPI.lua_toboolean(L, 1);
                    
                    LuaHelper.SetSoundMuted( _muted );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsMusicMuted_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.IsMusicMuted(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsSoundMuted_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.IsSoundMuted(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PreloadAudio_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<bool>>(L, 2)) 
                {
                    string _clipName = LuaAPI.lua_tostring(L, 1);
                    System.Action<bool> _onComplete = translator.GetDelegate<System.Action<bool>>(L, 2);
                    
                    LuaHelper.PreloadAudio( _clipName, _onComplete );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _clipName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.PreloadAudio( _clipName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.PreloadAudio!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnloadAudio_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _clipName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.UnloadAudio( _clipName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearAudioCache_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    LuaHelper.ClearAudioCache(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsMusicPlaying_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.IsMusicPlaying(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetCurrentMusicName_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.GetCurrentMusicName(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetActiveSoundCount_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.GetActiveSoundCount(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SaveInt_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    int _value = LuaAPI.xlua_tointeger(L, 2);
                    
                    LuaHelper.SaveInt( _key, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadInt_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    int _defaultValue = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = LuaHelper.LoadInt( _key, _defaultValue );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.LoadInt( _key );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.LoadInt!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SaveFloat_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    float _value = (float)LuaAPI.lua_tonumber(L, 2);
                    
                    LuaHelper.SaveFloat( _key, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadFloat_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    float _defaultValue = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        var gen_ret = LuaHelper.LoadFloat( _key, _defaultValue );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.LoadFloat( _key );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.LoadFloat!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SaveString_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    string _value = LuaAPI.lua_tostring(L, 2);
                    
                    LuaHelper.SaveString( _key, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadString_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    string _defaultValue = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = LuaHelper.LoadString( _key, _defaultValue );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.LoadString( _key );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.LoadString!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SaveBool_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    bool _value = LuaAPI.lua_toboolean(L, 2);
                    
                    LuaHelper.SaveBool( _key, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadBool_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)) 
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    bool _defaultValue = LuaAPI.lua_toboolean(L, 2);
                    
                        var gen_ret = LuaHelper.LoadBool( _key, _defaultValue );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.LoadBool( _key );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.LoadBool!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SaveToJson_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)) 
                {
                    string _fileName = LuaAPI.lua_tostring(L, 1);
                    string _jsonString = LuaAPI.lua_tostring(L, 2);
                    bool _encrypt = LuaAPI.lua_toboolean(L, 3);
                    
                    LuaHelper.SaveToJson( _fileName, _jsonString, _encrypt );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _fileName = LuaAPI.lua_tostring(L, 1);
                    string _jsonString = LuaAPI.lua_tostring(L, 2);
                    
                    LuaHelper.SaveToJson( _fileName, _jsonString );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.SaveToJson!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadFromJson_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)) 
                {
                    string _fileName = LuaAPI.lua_tostring(L, 1);
                    bool _encrypted = LuaAPI.lua_toboolean(L, 2);
                    
                        var gen_ret = LuaHelper.LoadFromJson( _fileName, _encrypted );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _fileName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.LoadFromJson( _fileName );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.LoadFromJson!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DeleteData_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.DeleteData( _key );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HasData_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _key = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.HasData( _key );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DeleteAll_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    LuaHelper.DeleteAll(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetEncryptionEnabled_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    bool _enabled = LuaAPI.lua_toboolean(L, 1);
                    
                    LuaHelper.SetEncryptionEnabled( _enabled );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetSaveDataPath_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.GetSaveDataPath(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreatePool_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<UnityEngine.GameObject>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    UnityEngine.GameObject _prefab = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    int _maxSize = LuaAPI.xlua_tointeger(L, 3);
                    float _defaultAutoReleaseTime = (float)LuaAPI.lua_tonumber(L, 4);
                    
                    LuaHelper.CreatePool( _poolName, _prefab, _maxSize, _defaultAutoReleaseTime );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<UnityEngine.GameObject>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    UnityEngine.GameObject _prefab = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    int _maxSize = LuaAPI.xlua_tointeger(L, 3);
                    
                    LuaHelper.CreatePool( _poolName, _prefab, _maxSize );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<UnityEngine.GameObject>(L, 2)) 
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    UnityEngine.GameObject _prefab = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    
                    LuaHelper.CreatePool( _poolName, _prefab );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.CreatePool!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetFromPool_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<UnityEngine.GameObject>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    UnityEngine.GameObject _prefab = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    float _autoReleaseTime = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        var gen_ret = LuaHelper.GetFromPool( _poolName, _prefab, _autoReleaseTime );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<UnityEngine.GameObject>(L, 2)) 
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    UnityEngine.GameObject _prefab = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    
                        var gen_ret = LuaHelper.GetFromPool( _poolName, _prefab );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.GetFromPool( _poolName );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.GetFromPool!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ReleaseToPool_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1&& translator.Assignable<UnityEngine.GameObject>(L, 1)) 
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    
                    LuaHelper.ReleaseToPool( _obj );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<UnityEngine.GameObject>(L, 2)) 
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    
                    LuaHelper.ReleaseToPool( _poolName, _obj );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.ReleaseToPool!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PreloadPool_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    UnityEngine.GameObject _prefab = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    int _count = LuaAPI.xlua_tointeger(L, 3);
                    
                    LuaHelper.PreloadPool( _poolName, _prefab, _count );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AutoReleasePool_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    float _delay = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    LuaHelper.AutoReleasePool( _poolName, _obj, _delay );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DestroyPool_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.DestroyPool( _poolName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearPool_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.ClearPool( _poolName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetPoolInfo_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.GetPoolInfo( _poolName );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetPoolMaxSize_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    int _maxSize = LuaAPI.xlua_tointeger(L, 2);
                    
                    LuaHelper.SetPoolMaxSize( _poolName, _maxSize );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetPoolDefaultAutoReleaseTime_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    float _autoReleaseTime = (float)LuaAPI.lua_tonumber(L, 2);
                    
                    LuaHelper.SetPoolDefaultAutoReleaseTime( _poolName, _autoReleaseTime );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ReleaseAllFromPool_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _poolName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.ReleaseAllFromPool( _poolName );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 0) 
                {
                    
                    LuaHelper.ReleaseAllFromPool(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.ReleaseAllFromPool!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action>(L, 2)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 2);
                    
                    LuaHelper.RegisterEvent( _eventName, _callback );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object>>(L, 2)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    System.Action<object> _callback = translator.GetDelegate<System.Action<object>>(L, 2);
                    
                    LuaHelper.RegisterEvent( _eventName, _callback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.RegisterEvent!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterEventWithPriority_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 2);
                    int _priority = LuaAPI.xlua_tointeger(L, 3);
                    
                    LuaHelper.RegisterEventWithPriority( _eventName, _callback, _priority );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object>>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    System.Action<object> _callback = translator.GetDelegate<System.Action<object>>(L, 2);
                    int _priority = LuaAPI.xlua_tointeger(L, 3);
                    
                    LuaHelper.RegisterEventWithPriority( _eventName, _callback, _priority );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.RegisterEventWithPriority!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnregisterEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action>(L, 2)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 2);
                    
                    LuaHelper.UnregisterEvent( _eventName, _callback );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object>>(L, 2)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    System.Action<object> _callback = translator.GetDelegate<System.Action<object>>(L, 2);
                    
                    LuaHelper.UnregisterEvent( _eventName, _callback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.UnregisterEvent!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnregisterEventWithPriority_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 2);
                    int _priority = LuaAPI.xlua_tointeger(L, 3);
                    
                    LuaHelper.UnregisterEventWithPriority( _eventName, _callback, _priority );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object>>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    System.Action<object> _callback = translator.GetDelegate<System.Action<object>>(L, 2);
                    int _priority = LuaAPI.xlua_tointeger(L, 3);
                    
                    LuaHelper.UnregisterEventWithPriority( _eventName, _callback, _priority );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.UnregisterEventWithPriority!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnregisterAllEvents_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.UnregisterAllEvents( _eventName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TriggerEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    
                    LuaHelper.TriggerEvent( _eventName );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<object>(L, 2)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    object _data = translator.GetObject(L, 2, typeof(object));
                    
                    LuaHelper.TriggerEvent( _eventName, _data );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.TriggerEvent!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HasEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.HasEvent( _eventName );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetEventListenerCount_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _eventName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = LuaHelper.GetEventListenerCount( _eventName );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearAllEvents_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    LuaHelper.ClearAllEvents(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_StartCoroutine_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.Func<System.Collections.IEnumerator> _coroutineFunc = translator.GetDelegate<System.Func<System.Collections.IEnumerator>>(L, 1);
                    
                        var gen_ret = LuaHelper.StartCoroutine( _coroutineFunc );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_StartCoroutineWithParam_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.Func<object, System.Collections.IEnumerator> _coroutineFunc = translator.GetDelegate<System.Func<object, System.Collections.IEnumerator>>(L, 1);
                    object _param = translator.GetObject(L, 2, typeof(object));
                    
                        var gen_ret = LuaHelper.StartCoroutineWithParam( _coroutineFunc, _param );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_StopCoroutine_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.Collections.IEnumerator _coroutine = (System.Collections.IEnumerator)translator.GetObject(L, 1, typeof(System.Collections.IEnumerator));
                    
                    LuaHelper.StopCoroutine( _coroutine );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_StopAllCoroutines_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    LuaHelper.StopAllCoroutines(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WaitForSeconds_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    float _seconds = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        var gen_ret = LuaHelper.WaitForSeconds( _seconds );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WaitForFrames_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    int _frames = LuaAPI.xlua_tointeger(L, 1);
                    
                        var gen_ret = LuaHelper.WaitForFrames( _frames );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WaitForEndOfFrame_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.WaitForEndOfFrame(  );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WaitForFixedUpdate_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    
                        var gen_ret = LuaHelper.WaitForFixedUpdate(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WaitSeconds_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    float _seconds = (float)LuaAPI.lua_tonumber(L, 1);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 2);
                    
                    LuaHelper.WaitSeconds( _seconds, _callback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WaitFrames_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    int _frames = LuaAPI.xlua_tointeger(L, 1);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 2);
                    
                    LuaHelper.WaitFrames( _frames, _callback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WaitUntil_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<System.Func<bool>>(L, 1)&& translator.Assignable<System.Action>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    System.Func<bool> _condition = translator.GetDelegate<System.Func<bool>>(L, 1);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 2);
                    float _timeout = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    LuaHelper.WaitUntil( _condition, _callback, _timeout );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& translator.Assignable<System.Func<bool>>(L, 1)&& translator.Assignable<System.Action>(L, 2)) 
                {
                    System.Func<bool> _condition = translator.GetDelegate<System.Func<bool>>(L, 1);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 2);
                    
                    LuaHelper.WaitUntil( _condition, _callback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.WaitUntil!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Repeat_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<System.Action>(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 1);
                    float _interval = (float)LuaAPI.lua_tonumber(L, 2);
                    int _repeatCount = LuaAPI.xlua_tointeger(L, 3);
                    
                        var gen_ret = LuaHelper.Repeat( _callback, _interval, _repeatCount );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& translator.Assignable<System.Action>(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 1);
                    float _interval = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        var gen_ret = LuaHelper.Repeat( _callback, _interval );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to LuaHelper.Repeat!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_StopRepeat_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.Collections.IEnumerator _coroutine = (System.Collections.IEnumerator)translator.GetObject(L, 1, typeof(System.Collections.IEnumerator));
                    
                    LuaHelper.StopRepeat( _coroutine );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
