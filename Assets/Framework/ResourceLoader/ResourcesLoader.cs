using UnityEngine;
using System.Collections;

namespace Framework.ResourceLoader
{
    /// <summary>
    /// Resources 资源加载器
    /// </summary>
    public class ResourcesLoader : IResourceLoader
    {
        public T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
        
        public IEnumerator LoadAsync<T>(string path, System.Action<T> onComplete) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(path);
            yield return request;
            onComplete?.Invoke(request.asset as T);
        }
        
        public void Unload(string path)
        {
            // Resources 不需要手动卸载
        }
        
        public void UnloadAll()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}

