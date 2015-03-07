using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueLite;

namespace KeyValueTest
{
  class Program
  {
    static void Main(string[] args)
    {
      //  create a key value store object
      using (var kvs = new KeyValueStore())
      {
        //  initialize the key value store
        Initialize(true, kvs);
        //  now open it
        kvs.Open();

        //  new database should have no keys
        Console.WriteLine("Key Count: {0}", kvs.KeyCount());
        //  prove that this key doesnt exist
        Console.WriteLine("Does the key 'NotExistantKey' exist: {0}", kvs.KeyExists("NotExistantKey"));
        //  add some data
        kvs.Set("KeyOne", "DataOne");
        //  we should have 1 key
        Console.WriteLine("Key Count: {0}", kvs.KeyCount());
        //  prove the key exists
        Console.WriteLine("Does the key 'KeyOne' exist: {0}", kvs.KeyExists("KeyOne"));
        //  show the data for key one
        Console.WriteLine("The key 'KeyOne' has the following value: {0}", kvs.Get("KeyOne"));

        //  add another element
        kvs.Set(new KeyValueElement { Key = "KeyTwo", Value = "DataTwo" });

        //  query returns the raw enumeration
        Console.WriteLine("Query All Keys");
        foreach (var key in kvs.QueryAllKeys())
        {
          Console.WriteLine("\tKey={0}", key);
        }

        //  fetch copies to a new list before returning
        Console.WriteLine("Fetch All Keys");
        foreach (var key in kvs.FetchAllKeys())
        {
          Console.WriteLine("\tKey={0}", key);
        }

        //  query returns the raw enumeration
        Console.WriteLine("Query All Key Value pairs");
        foreach (var kv in kvs.QueryAllKeyValuesStartingWithKey("%"))
        {
          Console.WriteLine("\tKey={0}  Value={1}", kv.Key, kv.Value);
        }

        //  fetch copies to a new list before returning
        Console.WriteLine("Fetch All Key Value pairs");
        foreach (var kv in kvs.FetchAllKeyValuesStartingWithKey("%"))
        {
          Console.WriteLine("\tKey={0}  Value={1}", kv.Key, kv.Value);
        }

        Console.WriteLine("The value of 'KeyOne' is {0}", kvs.Get("KeyOne"));
        Console.WriteLine("The value of 'KeyTwo' is {0}", kvs.Get("KeyTwo"));

        Console.WriteLine("Clearing Key One");
        kvs.Clear("KeyOne");
        //  we should have 1 key left
        Console.WriteLine("Key Count: {0}", kvs.KeyCount());
        //  prove the key exists
        Console.WriteLine("Prove the key 'KeyOne' doesn't exist: {0}", kvs.KeyExists("KeyOne"));

        Console.WriteLine("Clearing Key Two");
        kvs.Clear("KeyTwo");
        //  we should have 0 key left
        Console.WriteLine("Key Count: {0}", kvs.KeyCount());
        //  prove the key exists
        Console.WriteLine("Prove the key 'KeyTwo' doesn't exist: {0}", kvs.KeyExists("KeyTwo"));
      }

      Console.ReadLine();
    }

    #region Private Methods
    private IEnumerable<KeyValueElement> GenerateData(int count, string prefix = "Key")
    {
      for (var idx = 0; idx < count; ++idx)
      {
        var key = string.Format("{0}/{1}", prefix, idx);
        var data = string.Format("Data Item for {0}", key);
        yield return new KeyValueElement { Key = key, Value = data };
      }
    }
    private static void Initialize(bool memoryDb, KeyValueStore kvs, bool preventClearAll = false, bool throwOnKeyNotFound = false)
    {
      if (memoryDb)
      { //  initialize the key value store with options
        kvs.Initialize(options =>
                       {
                         //  we want an in memory database
                         options.InMemory = true;
                         // do we want to prevent someone from clearing the whole key value store in one shot
                         options.ThrowOnClearAll = preventClearAll;
                         // do we want to throw if the key is not found on get
                         options.ThrowOnGetKeyNotFound = throwOnKeyNotFound;
                       });
      }
      else
      { //  generate a temp file name to use
        var dbName = Path.GetTempFileName();
        kvs.Initialize(options =>
                       {  //  we want a physical file
                         options.DatabaseName = dbName;
                         // do we want to prevent someone from clearing the whole key value store in one shot
                         options.ThrowOnClearAll = preventClearAll;
                         // do we want to throw if the key is not found on get
                         options.ThrowOnGetKeyNotFound = throwOnKeyNotFound;
                       });
      }
    }
    #endregion

  }
}
