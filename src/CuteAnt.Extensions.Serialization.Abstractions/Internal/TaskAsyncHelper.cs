#if NET451
using System;
using System.Threading.Tasks;

namespace CuteAnt.Extensions.Serialization
{
  internal static class TaskAsyncHelper
  {
    private static readonly Task _emptyTask = TaskCache<AsyncVoid>.Empty;
    private static readonly Task<bool> _trueTask = CreateCachedTaskFromResult(true);
    private static readonly Task<bool> _falseTask = CreateCachedTaskFromResult(false);

    public static Task Empty => _emptyTask;

    public static Task<bool> True => _trueTask;

    public static Task<bool> False => _falseTask;

    public static Task OrEmpty(this Task task) => task ?? Empty;

    public static Task<T> OrEmpty<T>(this Task<T> task) => task ?? TaskCache<T>.Empty;

    /// <summary>A <see cref="Task"/> that has been completed.</summary>
    public static Task Completed => TaskCache<AsyncVoid>.Empty;

    #region ==& FromError &==

    internal static Task FromError(Exception e)
    {
      return FromError<object>(e);
    }

    internal static Task<T> FromError<T>(Exception e)
    {
      if (e is AggregateException aggregateException)
      {
        var tcs = new TaskCompletionSource<T>();
        tcs.SetException(aggregateException.InnerExceptions);
        return tcs.Task;
      }
      else
      {
        var atmb = System.Runtime.CompilerServices.AsyncTaskMethodBuilder<T>.Create();
        atmb.SetException(e);
        return atmb.Task;
      }
    }

    #endregion

    #region ==& Canceled &==

    internal static Task Canceled()
    {
      var tcs = new TaskCompletionSource<object>();
      tcs.SetCanceled();
      return tcs.Task;
    }

    internal static Task<T> Canceled<T>()
    {
      var tcs = new TaskCompletionSource<T>();
      tcs.SetCanceled();
      return tcs.Task;
    }

    #endregion

    #region **& class TaskCache<T> &**

    private static class TaskCache<T>
    {
      public static Task<T> Empty = CreateCachedTaskFromResult<T>(default(T));
    }

    #endregion

    #region **& CreateCachedTaskFromResult &**

    /// <summary>Creates a task we can cache for the desired {TResult} result.</summary>
    /// <param name="value">The value of the {TResult}.</param>
    /// <returns>A task that may be cached.</returns>
    private static Task<TResult> CreateCachedTaskFromResult<TResult>(TResult value)
    {
      // AsyncTaskMethodBuilder<TResult> caches tasks that are non-disposable.
      // By using these same tasks, we're a bit more robust against disposals,
      // in that such a disposed task's ((IAsyncResult)task).AsyncWaitHandle
      // is still valid.
      var atmb = System.Runtime.CompilerServices.AsyncTaskMethodBuilder<TResult>.Create();
      atmb.SetResult(value);
      return atmb.Task; // must be accessed after SetResult to get the cached task
    }

    #endregion

    #region ** AsyncVoid **

    /// <summary>Used as the T in a "conversion" of a Task into a Task{T}</summary>
    private struct AsyncVoid
    {
    }

    #endregion
  }
}
#endif
