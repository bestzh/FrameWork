using UnityEngine;
using System.Collections;

namespace Framework.ResourceLoader
{
    /// <summary>
    /// 资源加载器接口，支持多种加载方式
    /// </summary>
    public interface IResourceLoader
    {
        /// <summary>
        /// 同步加载资源
        /// </summary>
        T Load<T>(string path) where T : Object;
        
        /// <summary>
        /// 异步加载资源
        /// </summary>
        IEnumerator LoadAsync<T>(string path, System.Action<T> onComplete) where T : Object;
        
        /// <summary>
        /// 卸载资源
        /// </summary>
        void Unload(string path);
        
        /// <summary>
        /// 卸载所有资源
        /// </summary>
        void UnloadAll();
    }
}

