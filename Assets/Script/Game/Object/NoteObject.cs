using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [SerializeField] TextConvertLanguage _sendConvertText;

    public void SendNoteText()
    {
        FindObjectOfType<Inventory>().GetNote(TextConvertLanguage.GetText(_sendConvertText));
    }
}
