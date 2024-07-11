using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _06_Scripts
{
    public class CreateFolders : EditorWindow
    {
        private static string _projectName = "<Name>";
        [MenuItem("MyTools/Create Default Folders")]
        private static void SetUpFolders()
        {
            var window = ScriptableObject.CreateInstance<CreateFolders>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 150);
            window.ShowPopup();
        }

        
        private static void CreateAlLFolders()
        {
            Debug.Log("CreateAlLFolders");
            var folders = new List<string>
            {
                "Animations",
                "Audio",
                "Editor",
                "Materials",
                "Meshes",
                "Prefabs",
                "Scripts",
                "Scenes",
                "Shaders",
                "Textures",
                "UI"
            };

            foreach (var folder in folders.Where(folder => !Directory.Exists("Assets/" + folder)))
            {
                Directory.CreateDirectory("Assets/" + _projectName + "/" + folder);
            }

            var uiFolders = new List<string>
            {
                "Assets",
                "Fonts",
                "Icon",
            };

            foreach (var subfolder in uiFolders.Where(subfolder => !Directory.Exists("Assets/" + _projectName + "/UI/" + subfolder)))
            {
                Directory.CreateDirectory("Assets/" + _projectName + "/UI/" + subfolder);
            }
            AssetDatabase.Refresh();
        }

        
        void OnGUI()
        {
            EditorGUILayout.LabelField("Insert the Project name used as the root folder");
            _projectName = EditorGUILayout.TextField("Project Name: ", _projectName);
            
            this.Repaint();
            GUILayout.Space(70);
            if (GUILayout.Button("Generate!"))
            {
                CreateAlLFolders();
                this.Close();
            }
        }
    }
}