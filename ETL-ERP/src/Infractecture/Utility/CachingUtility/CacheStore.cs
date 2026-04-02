namespace Utility.CachingUtility;

public static class CacheStore
{
    #region Config
    private static readonly Dictionary<string, object> Cache;
    private static readonly object Sync;

    static CacheStore()
    {
        Cache = new Dictionary<string, object>();
        Sync = new object();
    }

    #endregion

    public static bool IsExist<T>() where T : class
    {
        var type = typeof(T);
        lock (Sync)
        {
            return Cache.ContainsKey(type.Name);
        }
    }

    public static bool IsExist<T>(string key) where T : class
    {
        lock (Sync)
        {
            return Cache.ContainsKey(key);
        }
    }

    public static T Get<T>() where T : class, new()
    {
        var type = typeof(T);
        lock (Sync)
        {
            if (Cache.ContainsKey(type.Name) == false) return null;
            lock (Sync)
            {
                return (T)Cache[type.Name];
            }
        }
    }

    public static T Get<T>(string key) where T : class, new()
    {
        var type = typeof(T);
        lock (Sync)
        {
            if (Cache.ContainsKey(key) == false) return null;
            lock (Sync)
            {
                return (T)Cache[key];
            }
        }
    }

    public static T Create<T>(string key, params object[] constructorParameters) where T : class, new()
    {
        var type = typeof(T);
        T value = (T)Activator.CreateInstance(type, constructorParameters);

        lock (Sync)
        {
            if (Cache.ContainsKey(key + type.Name))
            {
                return null;
            }
            lock (Sync)
            {
                Cache.Add(key, value);
            }
        }
        return value;
    }

    public static T Create<T>(params object[] constructorParameters) where T : class
    {
        var type = typeof(T);
        T value = (T)Activator.CreateInstance(type, constructorParameters);
        lock (Sync)
        {
            if (Cache.ContainsKey(type.Name))
            {
                throw new Exception("Sorry! This Item Already Added");
            }
            lock (Sync)
            {
                Cache.Add(type.Name, value);
            }
        }
        return value;
    }

    public static void Add<T>(T value) where T : class
    {
        if (value == null) return;
        var type = typeof(T);
        if (value.GetType() != type)
        {
            throw new ApplicationException($"The Type of Value Passed to Cache {value.GetType().FullName} Does Not Match the Cache Type {type.FullName}");
        }

        lock (Sync)
        {
            if (Cache.ContainsKey(type.Name))
            {
                throw new Exception("Sorry! This Item Already Exists");
            }
            lock (Sync)
            {
                Cache.Add(type.Name, value);
            }
        }
    }

    public static void Add<T>(string key, T value) where T : class
    {
        if (value == null) return;
        var type = typeof(T);
        if (value.GetType() != type)
        {
            throw new ApplicationException($"The Type of Value Passed to Cache {value.GetType().FullName} Does Not Match the Cache Type {type.FullName} For Key {key}");
        }

        lock (Sync)
        {
            if (!Cache.ContainsKey(key))
            {
                lock (Sync)
                {
                    Cache.Add(key, value);
                }
            }

        }
    }

    public static void AddOrUpdate<T>(string key, T value) where T : class
    {
        lock (Sync)
        {
            if (Cache.ContainsKey(key))
            {
                lock (Sync)
                {
                    Cache.Remove(key);
                    Cache.Add(key, value);
                }
            }
            else
            {
                lock (Sync)
                {
                    Cache.Add(key, value);
                }
            }

        }
    }

    public static void Remove<T>() where T : class
    {
        var type = typeof(T);
        lock (Sync)
        {
            if (Cache.ContainsKey(type.Name) == false)
            {
                throw new ApplicationException($"Sorry This Type of Item '{type.Name}' Does Not Exists in Cache");
            }
            lock (Sync)
            {
                Cache.Remove(type.Name);
            }
        }
    }

    public static void Remove(string key)
    {
        lock (Sync)
        {
            if (!Cache.ContainsKey(key)) return;
            lock (Sync)
            {
                Cache.Remove(key);
            }

        }
    }

    public static void Update<T>(T value) where T : class
    {
        var type = typeof(T);
        lock (Sync)
        {
            if (Cache.ContainsKey(type.Name) == false)
            {
                throw new ApplicationException($"Sorry This Type of Item '{type.Name}' Does Not Exists in Cache");
            }
            lock (Sync)
            {
                Cache.Remove(type.Name);
                Add(value);
            }
        }
    }

    public static void Update<T>(string key, T value) where T : class
    {
        var type = typeof(T);
        lock (Sync)
        {
            if (Cache.ContainsKey(key) == false)
            {
                throw new ApplicationException($"Sorry This Type of Item '{type.Name}' Does Not Exists in Cache With The Key '{key}'");
            }
            lock (Sync)
            {
                Cache.Remove(key);
                Add(key, value);
            }
        }
    }

    public static void Clear()
    {
        lock (Sync)
        {
            Cache?.Clear();
        }
    }
}
