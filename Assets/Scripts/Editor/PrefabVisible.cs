#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static Unity.Cinemachine.CinemachineCore;

namespace Gameplay
{
    [InitializeOnLoad]
    public class PrefabVisible : MonoBehaviour
    {

        public static bool ChangeObjStates(bool value)
        {
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            bool inPrefabMode = stage != null;

            if (inPrefabMode)
            {
                PrefabObjVisible[] prefObjs = stage.prefabContentsRoot.GetComponentsInChildren<PrefabObjVisible>();


                foreach (PrefabObjVisible prefobj in prefObjs)
                {
                    if (prefobj.IsOn == value) { continue; }
                    prefobj.TurnOnRenderers(value);
                    EditorUtility.SetDirty(prefobj);
                }
            }
            return value;
        }
        public static bool ChangeObjStatesWName(bool value,string name)
        {
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            bool inPrefabMode = stage != null;

            if (inPrefabMode)
            {
                PrefabObjVisible[] prefObjs = stage.prefabContentsRoot.GetComponentsInChildren<PrefabObjVisible>();

                foreach (PrefabObjVisible prefobj in prefObjs)
                {
                    if (prefobj.IsOn == value) { continue; }
                    if (!prefobj.TurnOnRendererIfNameFits(value, name)) { continue; } ;
                    EditorUtility.SetDirty(prefobj);
                }
            }
            return value;
        }
    }
}
#endif
