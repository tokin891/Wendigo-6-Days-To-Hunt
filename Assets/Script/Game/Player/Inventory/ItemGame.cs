using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Default Item", menuName = "CreateItem")]
public class ItemGame : ScriptableObject
{
    public int ID;

    [Header("English")]
    [TextArea(2, 2),SerializeField]
    private string Name_E;
    [TextArea(10, 10), SerializeField]
    private string Description_E;

    [Header("Polish")]
    [TextArea(2, 2), SerializeField]
    private string Name_P;
    [TextArea(10, 10), SerializeField]
    private string Description_P;

    public string name_
    {
        get
        {
            switch((int)GameInput.LanguageKey)
            {
                case 0:
                    return Name_E;
                case 1:
                    return Name_P;
            }

            return default;
        }
    }
    public string description_
    {
        get
        {
            switch ((int)GameInput.LanguageKey)
            {
                case 0:
                    return Description_E;
                case 1:
                    return Description_P;
            }

            return default;
        }
    }

    [Space(45)]
    public Sprite icon_;
    public TypeItem _Type;
}

public enum TypeItem
{
    Item,
    Rifle,
    Pistol
}
