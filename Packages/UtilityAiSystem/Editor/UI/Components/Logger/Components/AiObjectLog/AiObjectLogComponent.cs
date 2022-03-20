using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal abstract class AiObjectLogComponent: LogComponent
{
    protected Label TypeLabel;
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

        TypeLabel = root.Q<Label>("Type-Label");
        NameLabel = root.Q<Label>("Name-Label");
        DescriptionLabel = root.Q<Label>("Description-Label");
        ScoreContainer = root.Q<VisualElement>("ScoreContainer");
        Body = root.Q<VisualElement>("Body");
        Footer = root.Q<VisualElement>("Footer");
        styleSheets.Add(StylesService.GetStyleSheet("Logger"));
    }

    internal override string GetUiName()
    {
        return Model.UiName;
    }

    internal override void UpdateUi(ILogModel model)
    {
        if (model == null) 
            return;
        Model = model as AiObjectLog;
        this.style.display = DisplayStyle.Flex;

        TypeLabel.text = Model.Type;
        if (Model.CurrentTick == Model.LastSelectedTick)
        {
            NameLabel.text = Model.Name + "***Selected***";
        } 
        else if (Model.CurrentTick != Model.LastEvaluatedTick && 
            Model.GetType() != typeof(ResponseCurveLog) &&
            Model.GetType() != typeof(AiLog))
        {
            NameLabel.text = Model.Name + "***NotEvaluated***";
        }
        else
        {
            NameLabel.text = Model.Name;
        }
        DescriptionLabel.text = Model.Description;
        UpdateUiInternal(Model);
    }
    protected abstract void UpdateUiInternal(AiObjectLog aiObjectDebug);

    internal virtual void SetColor() { }

    internal virtual void ResetColor()
    {
        ColorService.ResetColor(this);
    }
}