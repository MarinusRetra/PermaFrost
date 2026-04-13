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
            if(_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if(_instance != null)
                {
                    return _instance;
                }
            
                T newInstance = new GameObject(typeof(T).Name).AddComponent<T>();
                Debug.LogError($"No instance of {typeof(T)} found in scene. Created a new instance: {newInstance.name}");
                return newInstance;
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if(_instance != this)
        {
            Debug.LogError($"Multiple instances found of {typeof(T)} in scene. Killing {gameObject.name}");
            Destroy(gameObject);
        }

        _instance = this as T;
        if(PersistThroughSceneLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
        Debug.Log($"{typeof(T).Name} initialized as {gameObject.name}");
    }
}
}
 