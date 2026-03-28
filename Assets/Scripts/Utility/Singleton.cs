using Unity.VisualScripting;
using UnityEngine;

namespace Gameplay
{
 /// <summary>
 /// A generic singleton pattern, meaning:
 /// There will only ever be one instance and that instance is globally accessible.
 /// Only use this when:
 ///  - You need to have only one instance of something forever.
 ///  - Must be globally accessible.
 ///  - You need to use a piece of code that should never run twice at the same time (Like a save and load system).
 /// 
 ///  - If you want to make one persist through scene transitions you need to add it in the main menu scene and then never again.
 /// </summary>

using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected bool PersistThroughSceneLoad;
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                #if UNITY_EDITOR
                if (_instance == null)
                {
                    Debug.LogError($"No instance of {typeof(T)} found in scene.");
                }
                #endif
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if(PersistThroughSceneLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
            Debug.Log($"Singleton {typeof(T)} initialized as {gameObject.name}");
        }

        #if UNITY_EDITOR
        if (_instance != this)
        {
            Debug.LogError($"Multiple instances found of {typeof(T)} in scene.");
        }
        #endif
    }
}
}
 