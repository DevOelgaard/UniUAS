using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;

internal class BucketComponent : MainWindowComponent
{
    private CompositeDisposable disposables = new CompositeDisposable();
    private TemplateContainer root;
    private CollectionComponent<Consideration> considerationCollections;
    private CollectionComponent<Decision> decisionCollections;
    private Bucket model;

    internal BucketComponent(Bucket model) : base(model)
    {
        this.model = model;
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);

        var weightComponent = new ParameterComponent(model.Weight);

        ScoreContainer.Add(weightComponent);

        considerationCollections = 
            new CollectionComponent<Consideration>(model.Considerations, 
            UASTemplateService.Instance.Considerations, "Consideration", "Considerations");
        root.Add(considerationCollections);
        
        decisionCollections = 
            new CollectionComponent<Decision>(model.Decisions, 
            UASTemplateService.Instance.Decisions, "Decision", "Decisions");
        root.Add(decisionCollections);

        Body.Clear();
        Body.Add(root);
    }

    ~BucketComponent()
    {
        disposables.Clear();
    }
}