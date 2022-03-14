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
    internal AiObjectLog Model;
    public AiObjectLogComponent()
    {
        var root = AssetDatabaseService.GetTemplateContainer("AiObjectLogComponent");
        Add(root);

        NameLabel = root.Q<Label>("Name-Label");
        DescriptionLabel = root.Q<Label>("Description-Label");
        ScoreContainer = root.Q<VisualElement>("ScoreContainer");
        Body = root.Q<VisualElement>("Body");
        Footer = root.Q<VisualElement>("Footer");
        styleSheets.Add(StylesService.GetStyleSheet("Logger"));
    }

    internal override void UpdateUi(ILogModel model)
    {
        if (model == null) 
            return;
        Model = model as AiObjectLog;
        this.style.display = DisplayStyle.Flex;
        NameLabel.text = Model.Name + " (" + Model.Type + ")";
        DescriptionLabel.text = Model.Description;
        UpdateUiInternal(Model);
    }
    protected abstract void UpdateUiInternal(AiObjectLog aiObjectDebug);

    internal virtual void SetColor() { }
}