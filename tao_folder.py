import os

# Định nghĩa các folder cần tạo
folders = [
    "Assets/Art/Characters",
    "Assets/Art/Environment",
    "Assets/Art/Monsters",
    "Assets/Art/UI",
    "Assets/Art/VFX",
    "Assets/Audio/BGM",
    "Assets/Audio/SFX",
    "Assets/Prefabs/Characters",
    "Assets/Prefabs/Monsters",
    "Assets/Prefabs/Environment",
    "Assets/Prefabs/Items",
    "Assets/Prefabs/NPCs",
    "Assets/Scenes",
    "Assets/Scripts/Core",
    "Assets/Scripts/Characters",
    "Assets/Scripts/Monsters",
    "Assets/Scripts/Items",
    "Assets/Scripts/NPCs",
    "Assets/Scripts/Quests",
    "Assets/Scripts/Map",
    "Assets/Scripts/UI",
    "Assets/Scripts/Networking",
    "Assets/Data/ScriptableObjects",
    "Assets/Data/Tables",
    "Assets/Plugins",
    "Assets/Resources",
    "Assets/Editor"
]

def create_folders(folders):
    for folder in folders:
        try:
            os.makedirs(folder, exist_ok=True)
            print(f"Created: {folder}")
        except Exception as e:
            print(f"Error creating {folder}: {e}")

if __name__ == "__main__":
    create_folders(folders)
    print("\n✅ Folder structure created (existing folders skipped).")

