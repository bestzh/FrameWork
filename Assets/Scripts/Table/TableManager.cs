
  using UnityEngine;
  using System.Collections;
  using System.Threading;
  namespace Table
  {
  
      public class TableManager
      {
          private static TableManager m_instance;
          public static TableManager Instance
          {
              get
              {
                  if (m_instance == null)
                  {
                      m_instance = new TableManager();
                      m_instance.Initialize();
                  }
                  return m_instance;
              }
          }
  
      
          public bool m_isLoad = false;
          public void Initialize()
          {
              m_isLoad = false;
          }
  
          public void LoadTable()
          {
              
                  //TableLoad.LoadFromResources();
                //  IosLimit.Init();
             
          }
  
          public void Load()
          {
              if (!m_isLoad)
              {
                  LoadTable();
                  m_isLoad = true;
              }
          }
  
          public void Clear()
          {
              TableLoad.Clear();
              m_isLoad = false;
          }
      }
  }
  
 