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
    public class CharacterManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(CharacterManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 8, 1, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CreateCharacter", _m_CreateCharacter);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetCharacter", _m_GetCharacter);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetPlayerCharacter", _m_SetPlayerCharacter);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LevelUp", _m_LevelUp);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddExp", _m_AddExp);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SaveCharacter", _m_SaveCharacter);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadCharacter", _m_LoadCharacter);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SaveAllCharacters", _m_SaveAllCharacters);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "PlayerCharacter", _g_get_PlayerCharacter);
            
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 1, 0);
			
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Instance", _g_get_Instance);
            
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new CharacterManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to CharacterManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateCharacter(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CharacterManager gen_to_be_invoked = (CharacterManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _characterId = LuaAPI.xlua_tointeger(L, 2);
                    string _name = LuaAPI.lua_tostring(L, 3);
                    
                        var gen_ret = gen_to_be_invoked.CreateCharacter( _characterId, _name );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetCharacter(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CharacterManager gen_to_be_invoked = (CharacterManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _characterId = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetCharacter( _characterId );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetPlayerCharacter(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CharacterManager gen_to_be_invoked = (CharacterManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    CharacterData _character = (CharacterData)translator.GetObject(L, 2, typeof(CharacterData));
                    
                    gen_to_be_invoked.SetPlayerCharacter( _character );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LevelUp(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CharacterManager gen_to_be_invoked = (CharacterManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    CharacterData _character = (CharacterData)translator.GetObject(L, 2, typeof(CharacterData));
                    
                    gen_to_be_invoked.LevelUp( _character );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddExp(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CharacterManager gen_to_be_invoked = (CharacterManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    CharacterData _character = (CharacterData)translator.GetObject(L, 2, typeof(CharacterData));
                    int _exp = LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.AddExp( _character, _exp );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SaveCharacter(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CharacterManager gen_to_be_invoked = (CharacterManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    CharacterData _character = (CharacterData)translator.GetObject(L, 2, typeof(CharacterData));
                    
                    gen_to_be_invoked.SaveCharacter( _character );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadCharacter(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CharacterManager gen_to_be_invoked = (CharacterManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _characterId = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.LoadCharacter( _characterId );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SaveAllCharacters(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CharacterManager gen_to_be_invoked = (CharacterManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.SaveAllCharacters(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Instance(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, CharacterManager.Instance);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_PlayerCharacter(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                CharacterManager gen_to_be_invoked = (CharacterManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.PlayerCharacter);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}
