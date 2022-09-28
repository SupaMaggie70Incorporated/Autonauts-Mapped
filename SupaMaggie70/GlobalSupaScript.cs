using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace SupaMaggie70
{
    public class GlobalSupaScript : MonoBehaviour
    {
        public static GlobalSupaScript Instance = null;
        public void Start()
        {

        }
        public void Update()
        {

        }
        public void FixedUpdate()
        {

        }
        public void Awake()
        {
            if (Instance != null) Destroy(this);
            Instance = this;
            DontDestroyOnLoad(this);
            PluginManagerMain.Init();
            
        }
    }
}
