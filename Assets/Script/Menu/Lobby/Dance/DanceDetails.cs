using UnityEngine;

[CreateAssetMenu(fileName ="New Dance ", menuName ="Menu/NewDance")]
public class DanceDetails : ScriptableObject
{
    public int idDance;

    [SerializeField, TextArea(10,10)]
    string _nameE;
    [SerializeField, TextArea(10, 10)]
    string _nameP;

    public string GetNameDance
    {
        get
        {
            switch((int)GameInput.LanguageKey)
            {
                case 0:
                    return _nameE;

                case 1:
                    return _nameP;
            }

            return "";
        }
    }
}
