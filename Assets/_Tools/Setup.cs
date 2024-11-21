using UnityEngine;
using System.IO;
using UnityEditor;

namespace Platformer.MyTools
{
    public static class Setup
    {
        [MenuItem("Tools/Setup/Refresh/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDefault(new FolderStructure
            {
                Name = "",
                Subfolders = new[]
                {
                    new FolderStructure
                    {
                        Name = "Art",
                        Subfolders = new[]
                        {
                            new FolderStructure { Name = "Animations" },
                            new FolderStructure { Name = "Models" },
                            new FolderStructure { Name = "Materials" },
                            new FolderStructure { Name = "Shaders"}
                        }
                    },
                    new FolderStructure
                    {
                        Name = "Scripts",
                        Subfolders = new[]
                        {
                            new FolderStructure { Name = "ScriptableObjects" }
                        }
                    },
                    new FolderStructure
                    {
                        Name = "Misc",
                        Subfolders = new[]
                        {
                            new FolderStructure { Name = "Inputs" }
                        }
                    },
                    new FolderStructure { Name = "Prefabs" },
                    new FolderStructure { Name = "Sounds" },
                    new FolderStructure { Name = "UI" }
                }
            });
            
            AssetDatabase.Refresh(); // Refresh the Asset Database after creating folders
        }

        static class Folders
        {
            public static void CreateDefault(FolderStructure root)
            {
                CreateFoldersRecursively(Application.dataPath, root);
            }

            private static void CreateFoldersRecursively(string parentPath, FolderStructure folderStructure)
            {
                if (!string.IsNullOrEmpty(folderStructure.Name))
                {
                    parentPath = Path.Combine(parentPath, folderStructure.Name);
                    if (!Directory.Exists(parentPath))
                    {
                        Directory.CreateDirectory(parentPath);
                    }
                }

                if (folderStructure.Subfolders == null) return;

                foreach (var subfolder in folderStructure.Subfolders)
                {
                    CreateFoldersRecursively(parentPath, subfolder);
                }
            }
        }
    }

    public class FolderStructure
    {
        public string Name;
        public FolderStructure[] Subfolders;
    }
}
