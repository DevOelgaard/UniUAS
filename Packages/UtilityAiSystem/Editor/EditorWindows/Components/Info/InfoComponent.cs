using UnityEngine;
using UnityEngine.UIElements;

internal class InfoComponent: VisualElement
{
    private TemplateContainer root;
    private Label infoText;

    internal InfoComponent()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        infoText = root.Q<Label>("InfoText");
    }

    internal void DisplayInfo(string text)
    {
        infoText.text = text; 
    }

    internal void DisplayWarning(string text)
    {
        infoText.text = text;
        infoText.style.color = Color.red;
    }
}
