using UnityEngine;
using System;


/********************************************************************************************************************************
	 * This would be useful when we want to load game object into the scene when it is actually needed.
	 * We don't have to manage all the singleton game object when generating scene.
	 * It is always optional to use this class.
	********************************************************************************************************************************/

/// <summary>
/// Prefab attribute. Use this on child classes
/// to define if they have a prefab associated or not
/// By default will attempt to load a prefab
/// that has the same name as the class,
/// otherwise [Prefab("path/to/prefab")] to define it specifically. 
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class PrefabAttribute : Attribute
{
	string _name;

	public string Name
	{ 
		get
		{ 
			return this._name; 
		} 
	}

	public PrefabAttribute ()
	{ 
		this._name = ""; 
	}

	public PrefabAttribute (string name)
	{ 
		this._name = name; 
	}
}

/// <summary>
/// MONOBEHAVIOR PSEUDO SINGLETON ABSTRACT CLASS
/// usage        : can be attached to a gameobject and if not
///              : this will create one on first access
/// example      : public sealed class MyClass : Singleton<MyClass> {
/// references   : https://gist.github.com/timofei7/fed468b0d23c58875bb9
/// 			 : http://tinyurl.com/d498g8c
///              : http://tinyurl.com/cc73a9h
///              : http://unifycommunity.com/wiki/index.php?title=Singleton
/// 			 : http://wiki.unity3d.com/index.php/Singleton
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance = null;
	private static object _lock = new object();
	private static bool applicationIsQuitting = false;

	public static bool IsAwake
	{ 
		get
		{ 
			return (_instance != null); 
		} 
	}

	/// <summary>
	/// gets the instance of this Singleton
	/// use this for all instance calls:
	/// MyClass.Instance.MyMethod();
	/// or make your public methods static
	/// and have them use Instance internally
	/// for a nice clean interface
	/// </summary>
	public static T Instance
	{
		get
		{
			if (applicationIsQuitting)
			{
				Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
				"' already destroyed on application quit." +
				" Won't create again - returning null.");
				return null;
			}

			lock (_lock)
			{
				if (_instance == null)
				{
					Type m_Type = typeof(T);
					_instance = (T)FindObjectOfType(m_Type);
					string singletonObjName = m_Type.Name;
					GameObject singletonObj = GameObject.Find(singletonObjName);
					// try with (Clone)
					if (singletonObj == null)
					{
						singletonObj = GameObject.Find(singletonObjName + "(Clone)");
					}

					// no singleton game object found.
					if (singletonObj == null)
					{
						// checks if the [Prefab] attribute is set and pulls that if it can
						bool hasPrefab = Attribute.IsDefined(m_Type, typeof(PrefabAttribute));
						if (hasPrefab)
						{
							PrefabAttribute attr = (PrefabAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(PrefabAttribute));
							string prefabName = attr.Name;
							try
							{
								if (prefabName != "")
								{
									singletonObj = (GameObject)Instantiate(Resources.Load(prefabName, typeof(GameObject)));
								}
								else
								{
									singletonObj = (GameObject)Instantiate(Resources.Load(singletonObjName, typeof(GameObject)));
								}
							}
							catch (Exception e)
							{
								Debug.LogError("could not instantiate prefab even though prefab attribute was set: " + e.Message + "\n" + e.StackTrace);
							}
						}

						if (singletonObj == null)
						{
							singletonObj = new GameObject();
						}

						DontDestroyOnLoad(singletonObj);
						singletonObj.name = singletonObjName;

						_instance = singletonObj.GetComponent<T>();
						if (_instance == null)
						{
							_instance = singletonObj.AddComponent<T>();
						}
					}
					else
					{
						int objCount = FindObjectsOfType(m_Type).Length;
						if (objCount > 1)
						{
							Debug.LogWarning("There are more than one Singleton <" + m_Type.Name + ">.");
						}
					}
				}

				return _instance;
			}
		}
	}

	// in your child class you can implement Awake()
	// and add any initialization code you want.


	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	/// it will create a buggy ghost object that will stay on the Editor scene
	/// even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	public void OnDestroy ()
	{
		applicationIsQuitting = true;
		_instance = null;
	}

	/// <summary>
	/// Parent this to another gameobject.
	/// call from Awake if you so desire
	/// </summary>
	protected void SetParent (GameObject parentGO)
	{
		if (parentGO != null)
		{
			this.transform.parent = parentGO.transform;
		}
	}

	protected void SetParent (string parentObjName)
	{
		if (parentObjName != null)
		{
			GameObject parentObj = GameObject.Find(parentObjName);
			if (parentObj == null)
			{
				parentObj = new GameObject(parentObjName);
			}

			this.transform.parent = parentObj.transform;
		}
	}

    public void Init()
    {
        
    }
}
