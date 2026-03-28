using UnityEngine;

namespace Gameplay
{
public class SaveSystem : Singleton<SaveSystem> 
{
    private SaveFile[] SaveFiles = new SaveFile[3];
    SaveFile currentSave = null; 

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetSaveFile(sbyte _numIn)
    {
        currentSave = SaveFiles[_numIn];
    }

    public void LoadGame()
    {
        // Read out the current save file to currentSave and sets up the game according to currentSave.
    }

    public void SaveGame()
    {
        // Collect the data from wherever its stored in the game and set the members of currentSave to whats happpening in the game.
        // After that write to somewhere, have not thought of where yet.
    }

}

    /// <summary>
    /// This class will be where all data gets stored that needs to persists after game launch 
    /// </summary>
    public class SaveFile
    {

    }
}