using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public static class ExtensionMethods
{
    public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<object>();
        asyncOp.completed += obj => { tcs.SetResult(null); };
        return ((Task)tcs.Task).GetAwaiter();
    }
}

/* Example:
var getRequest = UnityWebRequest.Get("http://www.google.com");
await getRequest.SendWebRequest();
var result = getRequest.downloadHandler.text;
*/