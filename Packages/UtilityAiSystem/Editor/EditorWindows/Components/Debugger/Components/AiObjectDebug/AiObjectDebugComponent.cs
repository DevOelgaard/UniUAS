using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal abstract class AiObjectDebugComponent: VisualElement
{
    protected Label NameLabel;
    protected Label DescriptionLabel;
    protected VisualElement ScoreContainer;
    protected VisualElement Body;
    protected VisualElement Footer;
    public AiObjectDebugComponent(AiObjectDebug aiObjectDebug)
    {
        var root = AssetDatabaseService.GetTemplateContainer("AiObjectDebugComponent");
        Add(root);

        NameLabel = root.Q<Label>("Name-Label");
        DescriptionLabel = root.Q<Label>("Description-Label");
        ScoreContainer = root.Q<VisualElement>("ScoreContainer");
        Body = root.Q<VisualElement>("Body");
        Footer = root.Q<VisualElement>("Footer");

        NameLabel.text = aiObjectDebug.Name + " (" + aiObjectDebug.Type + ")";
        DescriptionLabel.text = aiObjectDebug.Description;
    }
}