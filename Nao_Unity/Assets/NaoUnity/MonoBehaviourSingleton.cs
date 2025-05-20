using UnityEngine;

namespace NaoUnity
{
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
    {
        #region Fields
        private static T m_Instance;

        private static readonly object Lock = new object();

        [SerializeField]
        private bool m_Persistent = false;

        public static bool Quitting { get; private set; } = false;
        #endregion

        #region Properties
        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    return m_Instance;
                }
                lock (Lock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = FindObjectOfType<T>();
                        if (m_Instance == null)
                        {
                            System.Type componentType = typeof(T);
                            Debug.Log(string.Format("An instance of '{0}' is needed in in the scene and no existing instances were found, so a new instance will be created.",
                                                    componentType.Name));
                            // We create a new game object to store the Singleton
                            var newGO = new GameObject(componentType.Name, componentType);
                            m_Instance = newGO.GetComponent<T>();

                            // Because of scene changes, we have to make sure this singleton is not destroyed
                            DontDestroyOnLoad(newGO);
                        }
                    }
                    return m_Instance;
                }
            }
        }
        #endregion

        #region Methods
        void Awake()
        {
            // We override the Awake in order to add the Instance check
            // so that we can destroy the component if there is already another instance
            if (Instance != null && Instance != this)
            {
                System.Type componentType = typeof(T);
                Debug.LogError(string.Format("An instance of '{0}' already in the scene, destroying this one",
                                             componentType.Name));
                Destroy(this);
                return;
            }

            if (m_Persistent)
                DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            Quitting = true;
        }
        #endregion
    }
}
