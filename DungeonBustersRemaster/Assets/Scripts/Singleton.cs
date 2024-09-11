using UnityEngine;

public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                _instance.Init();
            }
            return _instance;
        }
    }

    protected Singleton() { }   //new를 통한 외부 인스턴스 생성 방지

    protected virtual void Init() { }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticVariables()
    {
        _instance = null;   //재시작, 새로운 씬 로드시 초기화.
    }
}
