
public class Singleton<T> where T : class
{
    private static T instance;
    private static readonly object obj = new object();

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                lock(obj){
                    if (instance == null){
                        instance = (T)System.Activator.CreateInstance(typeof(T), true);
                    }
                }
            }
            return instance;
        }
    }
}

