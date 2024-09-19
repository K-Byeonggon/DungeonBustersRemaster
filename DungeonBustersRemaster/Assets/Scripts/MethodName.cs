using System.Diagnostics;

public static class MethodName
{
    public static string GetName()
    {
        // 현재 메서드 이름을 출력
        StackTrace stackTrace = new StackTrace();
        return stackTrace.GetFrame(1).GetMethod().Name;
    }

    public static void DebugLog()
    {
        StackTrace stackTrace = new StackTrace();
        StackFrame frame = stackTrace.GetFrame(1);
        var method = frame.GetMethod();
        string callerClassName = method.DeclaringType.Name;
        string callerMethodName = method.Name;

        UnityEngine.Debug.Log($"{callerClassName}: {callerMethodName}");
    }
}
