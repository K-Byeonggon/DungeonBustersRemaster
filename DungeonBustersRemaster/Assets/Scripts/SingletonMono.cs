using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + "(Singleton)";

                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 인스턴스가 이미 존재하는 경우 현재 오브젝트를 파괴
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
