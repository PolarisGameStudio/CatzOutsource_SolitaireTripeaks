using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// The is destroy on load.
    /// </summary>
    [SerializeField]
    protected bool dontDestroyOnLoad = true;

    /// <summary>
    /// The instance.
    /// </summary>
    private static T instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    Debug.LogError(typeof(T) + " cannot be found");
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Awake this instance.
    /// </summary>
    public virtual void Awake()
    {
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}