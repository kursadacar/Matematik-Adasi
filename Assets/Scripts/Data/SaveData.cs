[System.Serializable]
public class SaveData
{
    public bool FishingFirstTime = true;
    public bool ShoppingFirstTime = true;
    public bool PlayingFirstTime = true;

    public float questionTime = 10;

    public CharacterData playerData = new CharacterData();
}