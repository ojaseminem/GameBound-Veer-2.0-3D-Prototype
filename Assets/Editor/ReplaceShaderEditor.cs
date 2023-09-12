using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ReplaceShaderEditor : EditorWindow
    {
        private Shader sourceShader;
        private Shader replacementShader;

        [MenuItem("Tools/Replace Material Shader")]
        static void Init()
        {
            ReplaceShaderEditor window = (ReplaceShaderEditor)EditorWindow.GetWindow(typeof(ReplaceShaderEditor));
            window.titleContent = new GUIContent("Replace Shader");
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Shader Replacement", EditorStyles.boldLabel);

            sourceShader = (Shader)EditorGUILayout.ObjectField("Source Shader:", sourceShader, typeof(Shader), false);
            replacementShader = (Shader)EditorGUILayout.ObjectField("Replacement Shader:", replacementShader, typeof(Shader), false);

            if (GUILayout.Button("Replace Shaders"))
            {
                if (sourceShader == null || replacementShader == null)
                {
                    Debug.LogError("Please select both source and replacement shaders.");
                    return;
                }

                ReplaceMaterials();
            }
        }

        void ReplaceMaterials()
        {
            Material[] allMaterials = Resources.FindObjectsOfTypeAll<Material>();

            foreach (Material material in allMaterials)
            {
                if (material.shader == sourceShader)
                {
                    material.shader = replacementShader;
                    EditorUtility.SetDirty(material);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}