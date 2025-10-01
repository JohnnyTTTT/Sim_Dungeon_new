using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using SoulGames.EasyGridBuilderPro;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class SimDungeonEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Johnny/SimDungeon")]
        private static void Open()
        {
            var window = GetWindow<SimDungeonEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false)
            {
                {"Buildings", this,EditorIcons.House},
                {"BuildingAssets", this,SdfIconType.Diagram3Fill},
            };
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;

            tree.AddAllAssetsAtPath("Buildings/Large Buildings", "Assets/UnityApp/Assets - EGB/LargeCell", typeof(BuildableObjectSO), true);
            tree.AddAllAssetsAtPath("BuildingAssets", "Assets/UnityApp/ScriptableObjects", typeof(BuildableGenAssets), true);


            return tree;
        }
        protected override void OnBeginDrawEditors()
        {
            if (MenuTree != null)
            {
                var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;
                if (this.MenuTree.Selection != null && this.MenuTree.Selection.Count > 0)
                {
                    var selected = this.MenuTree.Selection.FirstOrDefault();
                    SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
                    {
                        if (selected != null)
                        {
                            GUILayout.Label(selected.Name);
                        }
                        if (selected.Value is UnityEngine.Object obj)
                        {
                            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Pin")))
                            {
                                {
                                    EditorGUIUtility.PingObject(obj);
                                    Selection.activeObject = obj;
                                }
                            }
                        }
                    }
                }
            }





            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}
