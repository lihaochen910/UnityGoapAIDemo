using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.SceneManagement;

#if UNITY_5_6_OR_NEWER
using UnityEditor.Build;
#endif

#if UNITY_2018_3_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace HutongGames.PlayMakerEditor
{
#if UNITY_2018_3_OR_NEWER    

    public class PlayMakerPreProcessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }
        
        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("PlayMakerPreProcessBuild...");
            ProjectTools.PreprocessPrefabFSMs();
        }
    }

#elif UNITY_5_6_OR_NEWER

    public class PlayMakerPreProcessBuild : IPreprocessBuild
    {
        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            Debug.Log("PlayMakerPreProcessBuild...");
            ProjectTools.PreprocessPrefabFSMs();
        }
    }

#endif


    public class PlayMakerBuildCallbacks
    {
        [PostProcessSceneAttribute(2)]
        public static void OnPostprocessScene()
        {
            /* TODO: Figure out if we need to do this!
            // OnPostprocessScene is called when loading a scene in the editor 
            // Might not want to post process in that case...?
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }*/

            //Debug.Log("OnPostprocessScene: " + SceneManager.GetActiveScene().name);

            if (Application.isPlaying) // playing in editor, not really making a build
            {
                return;
            }

            PlayMakerGlobals.IsBuilding = true;
            PlayMakerGlobals.InitApplicationFlags();

            var fsmList = Resources.FindObjectsOfTypeAll<PlayMakerFSM>();
            foreach (var playMakerFSM in fsmList)
            {
                if (playMakerFSM == null) continue; // not sure when this happens, but need to catch it...
                    
 #if UNITY_5_6_OR_NEWER                   
                if (FsmPrefabs.IsPrefab(playMakerFSM)) 
                {
                    // already processed by PlayMakerPreProcessBuild
                    continue;
                }
#endif

                playMakerFSM.Preprocess();
            }

            PlayMakerGlobals.IsBuilding = false;

            //Debug.Log("EndPostProcessScene");
        }
    }
}
