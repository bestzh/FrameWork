using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// UI查询系统，类似UI Toolkit的Q方法
    /// </summary>
    public static class UIQuery
    {
        /// <summary>
        /// 查询UI元素（按名称）
        /// </summary>
        public static T Q<T>(GameObject root, string name = null) where T : Component
        {
            if (root == null) return null;
            
            if (string.IsNullOrEmpty(name))
            {
                // 如果没有指定名称，直接查找第一个匹配的类型
                return root.GetComponentInChildren<T>(true);
            }
            
            // 按名称查找
            Transform found = root.transform.Find(name);
            if (found != null)
            {
                return found.GetComponent<T>();
            }
            
            // 递归查找
            return FindInChildren<T>(root.transform, name);
        }
        
        /// <summary>
        /// 查询所有匹配的UI元素
        /// </summary>
        public static List<T> Query<T>(GameObject root, string name = null) where T : Component
        {
            List<T> results = new List<T>();
            if (root == null) return results;
            
            if (string.IsNullOrEmpty(name))
            {
                results.AddRange(root.GetComponentsInChildren<T>(true));
            }
            else
            {
                FindAllInChildren(root.transform, name, results);
            }
            
            return results;
        }
        
        /// <summary>
        /// 按类名查询（通过Tag或Layer）
        /// </summary>
        public static List<T> QueryByClass<T>(GameObject root, string className) where T : Component
        {
            List<T> results = new List<T>();
            if (root == null || string.IsNullOrEmpty(className)) return results;
            
            T[] allComponents = root.GetComponentsInChildren<T>(true);
            foreach (var component in allComponents)
            {
                // 可以通过Tag、Layer或自定义属性来判断
                if (component.gameObject.tag == className || 
                    component.gameObject.layer == LayerMask.NameToLayer(className))
                {
                    results.Add(component);
                }
            }
            
            return results;
        }
        
        /// <summary>
        /// 递归查找子元素
        /// </summary>
        private static T FindInChildren<T>(Transform parent, string name) where T : Component
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    T component = child.GetComponent<T>();
                    if (component != null)
                    {
                        return component;
                    }
                }
                
                T found = FindInChildren<T>(child, name);
                if (found != null)
                {
                    return found;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 递归查找所有匹配的元素
        /// </summary>
        private static void FindAllInChildren<T>(Transform parent, string name, List<T> results) where T : Component
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    T component = child.GetComponent<T>();
                    if (component != null)
                    {
                        results.Add(component);
                    }
                }
                
                FindAllInChildren(child, name, results);
            }
        }
    }
}

