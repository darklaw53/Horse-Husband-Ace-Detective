using System.Text;
using TMPro;

public class TextCommands : Singleton<TextCommands>
{
    public int Commands(TMP_Text textBox, int currentVisibleCharacterIndex)
    {
        TMP_TextInfo textInfo = textBox.textInfo;
        StringBuilder visibleText = new StringBuilder();
        int currentIndex = 0;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (IsCustomCommandStart(textBox.text, i))
            {
                i = FindCustomCommandEnd(textBox.text, i);
                continue;
            }

            if (textInfo.characterInfo[i].isVisible)
                currentVisibleCharacterIndex = currentIndex;

            visibleText.Append(textBox.text[i]);
            currentIndex++;
        }

        textBox.text = visibleText.ToString();
        return currentVisibleCharacterIndex;
    }

    private bool IsCustomCommandStart(string text, int index)
    {
        if (text[index] == '<')
        {
            if (index + 1 < text.Length && text[index + 1] != '/' && index + 2 < text.Length && text[index + 2] == ',')
                return true;
        }
        return false;
    }

    private int FindCustomCommandEnd(string text, int startIndex)
    {
        int endIndex = text.IndexOf('>', startIndex);
        if (endIndex != -1)
        {
            return endIndex + 1;
        }
        return text.Length - 1; // This might not be ideal, it depends on how you want to handle unclosed commands
    }
}