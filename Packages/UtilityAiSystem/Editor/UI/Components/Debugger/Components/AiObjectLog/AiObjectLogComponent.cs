using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal abstract class AiObjectLogComponent: LogComponent
{
    protected Label NameLabel;
    protected Label DescriptionLabel;
    protected VisualElement ScoreContainer;
    protected VisualElement Body;
    protected VisualElement Footer;
    public AiObjectLogComponent()
    {
        var root = AssetDatabaseService.GetTemplateContainer("AiObjectLogComponent");
        Add(root);

        NameLabel = root.Q<Label>("Name-Label");
        DescriptionLabel = root.Q<Label>("Description-Label");
        ScoreContainer = root.Q<VisualElement>("ScoreContainer");
        Body = root.Q<VisualElement>("Body");
        Footer = root.Q<VisualElement>("Footer");
    }

    internal override void Display(ILogModel model)
    {
        var aiObjectDebug = model as AiObjectLog;
        this.style.display = DisplayStyle.Flex;
        NameLabel.text = aiObjectDebug.Name + " (" + aiObjectDebug.Type + ")";
        DescriptionLabel.text = aiObjectDebug.Description;
        DisplayInternal(aiObjectDebug);
    }
    protected abstract void DisplayInternal(AiObjectLog aiObjectDebug);


}