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
    public class PlayerControllerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(PlayerController);
			Utils.BeginObjectRegister(type, L, translator, 0, 4, 16, 7);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetIsJumping", _m_SetIsJumping);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetIsRolling", _m_SetIsRolling);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "TakeDamage", _m_TakeDamage);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Heal", _m_Heal);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "StateMachine", _g_get_StateMachine);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "InputDir", _g_get_InputDir);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsRunning", _g_get_IsRunning);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsGrounded", _g_get_IsGrounded);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "CurrentHealth", _g_get_CurrentHealth);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsJumping", _g_get_IsJumping);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsRolling", _g_get_IsRolling);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "AnimationController", _g_get_AnimationController);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "SkillSystem", _g_get_SkillSystem);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "CharacterData", _g_get_CharacterData);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "animator", _g_get_animator);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "MoveSpeed", _g_get_MoveSpeed);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "MaxHealth", _g_get_MaxHealth);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "rb", _g_get_rb);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "mouseSensitivity", _g_get_mouseSensitivity);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "RollForce", _g_get_RollForce);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "CharacterData", _s_set_CharacterData);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "animator", _s_set_animator);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "MoveSpeed", _s_set_MoveSpeed);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "MaxHealth", _s_set_MaxHealth);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "rb", _s_set_rb);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "mouseSensitivity", _s_set_mouseSensitivity);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "RollForce", _s_set_RollForce);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new PlayerController();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to PlayerController constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetIsJumping(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    bool _jumping = LuaAPI.lua_toboolean(L, 2);
                    
                    gen_to_be_invoked.SetIsJumping( _jumping );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetIsRolling(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    bool _rolling = LuaAPI.lua_toboolean(L, 2);
                    
                    gen_to_be_invoked.SetIsRolling( _rolling );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TakeDamage(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _damage = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.TakeDamage( _damage );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Heal(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _amount = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.Heal( _amount );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StateMachine(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.StateMachine);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_InputDir(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                translator.PushUnityEngineVector2(L, gen_to_be_invoked.InputDir);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsRunning(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsRunning);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsGrounded(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsGrounded);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CurrentHealth(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.CurrentHealth);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsJumping(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsJumping);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsRolling(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsRolling);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_AnimationController(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.AnimationController);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_SkillSystem(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.SkillSystem);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CharacterData(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.CharacterData);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_animator(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.animator);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_MoveSpeed(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.MoveSpeed);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_MaxHealth(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.MaxHealth);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_rb(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.rb);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_mouseSensitivity(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.mouseSensitivity);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_RollForce(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.RollForce);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CharacterData(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.CharacterData = (CharacterData)translator.GetObject(L, 2, typeof(CharacterData));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_animator(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.animator = (UnityEngine.Animator)translator.GetObject(L, 2, typeof(UnityEngine.Animator));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_MoveSpeed(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.MoveSpeed = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_MaxHealth(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.MaxHealth = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_rb(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.rb = (UnityEngine.Rigidbody)translator.GetObject(L, 2, typeof(UnityEngine.Rigidbody));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_mouseSensitivity(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.mouseSensitivity = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_RollForce(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                PlayerController gen_to_be_invoked = (PlayerController)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.RollForce = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
