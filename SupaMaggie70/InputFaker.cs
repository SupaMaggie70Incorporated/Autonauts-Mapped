using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SupaMaggie70
{
    public class InputFaker
    {
        public FarmerPlayer player;
        public TileCoord TargetLocation
        {
            get
            {
                return player.m_GoToTilePosition;
            }
            set
            {
                MoveTo(value);
            }
        }
        public TileCoord CurrentLocation
        {
            get
            {
                return player.m_TileCoord;
            }
        }

        public static InputFaker Instance = null;
        public InputFaker()
        {
            Instance = this;
            player = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
        }
        public void MoveTo(TileCoord tile,Actionable target = null,AFO.AT at = AFO.AT.Primary)
        {
            player.GoToAndAction(tile,target,at);
        }
        
        
        public static void SceneLoaded(Scene scene,LoadSceneMode mode)
        {
            if (scene.name == "Main")
            {
                new InputFaker();
            }
        }
        public static void SceneUnloaded(Scene scene)
        {
            if (scene.name == "Main") Instance = null;
        }
        public static bool InGame()
        {
            return SceneManager.GetActiveScene().name == "Main";
        }
    }
}
