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
    public class EventManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(EventManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 9, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RegisterEvent", _m_RegisterEvent);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "UnregisterEvent", _m_UnregisterEvent);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "UnregisterAll", _m_UnregisterAll);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ClearAllEvents", _m_ClearAllEvents);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "TriggerEvent", _m_TriggerEvent);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "HasEvent", _m_HasEvent);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetListenerCount", _m_GetListenerCount);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetAllEventNames", _m_GetAllEventNames);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetLogEvents", _m_SetLogEvents);
			
			
			
			
			
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
					
					var gen_ret = new EventManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to EventManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterEvent(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventManager gen_to_be_invoked = (EventManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object>>(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action<object> _callback = translator.GetDelegate<System.Action<object>>(L, 3);
                    
                    gen_to_be_invoked.RegisterEvent( _eventName, _callback );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action>(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 3);
                    
                    gen_to_be_invoked.RegisterEvent( _eventName, _callback );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 3);
                    int _priority = LuaAPI.xlua_tointeger(L, 4);
                    
                    gen_to_be_invoked.RegisterEvent( _eventName, _callback, _priority );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object>>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action<object> _callback = translator.GetDelegate<System.Action<object>>(L, 3);
                    int _priority = LuaAPI.xlua_tointeger(L, 4);
                    
                    gen_to_be_invoked.RegisterEvent( _eventName, _callback, _priority );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to EventManager.RegisterEvent!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnregisterEvent(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventManager gen_to_be_invoked = (EventManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object>>(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action<object> _callback = translator.GetDelegate<System.Action<object>>(L, 3);
                    
                    gen_to_be_invoked.UnregisterEvent( _eventName, _callback );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action>(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 3);
                    
                    gen_to_be_invoked.UnregisterEvent( _eventName, _callback );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action _callback = translator.GetDelegate<System.Action>(L, 3);
                    int _priority = LuaAPI.xlua_tointeger(L, 4);
                    
                    gen_to_be_invoked.UnregisterEvent( _eventName, _callback, _priority );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object>>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action<object> _callback = translator.GetDelegate<System.Action<object>>(L, 3);
                    int _priority = LuaAPI.xlua_tointeger(L, 4);
                    
                    gen_to_be_invoked.UnregisterEvent( _eventName, _callback, _priority );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to EventManager.UnregisterEvent!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnregisterAll(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventManager gen_to_be_invoked = (EventManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.UnregisterAll( _eventName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearAllEvents(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventManager gen_to_be_invoked = (EventManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.ClearAllEvents(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TriggerEvent(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventManager gen_to_be_invoked = (EventManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.TriggerEvent( _eventName );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<object>(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    object _data = translator.GetObject(L, 3, typeof(object));
                    
                    gen_to_be_invoked.TriggerEvent( _eventName, _data );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.TriggerEvent( _eventName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to EventManager.TriggerEvent!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HasEvent(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventManager gen_to_be_invoked = (EventManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.HasEvent( _eventName );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetListenerCount(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventManager gen_to_be_invoked = (EventManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetListenerCount( _eventName );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetAllEventNames(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventManager gen_to_be_invoked = (EventManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetAllEventNames(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetLogEvents(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventManager gen_to_be_invoked = (EventManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    bool _enabled = LuaAPI.lua_toboolean(L, 2);
                    
                    gen_to_be_invoked.SetLogEvents( _enabled );
                    
                    
                    
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
			    translator.Push(L, EventManager.Instance);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}
