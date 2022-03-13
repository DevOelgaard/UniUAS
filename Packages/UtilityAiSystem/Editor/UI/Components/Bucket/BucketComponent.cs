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

    internal BucketComponent() : base()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);

        var weightComponent = new ParameterComponent(model.Weight);

        ScoreContainer.Add(weightComponent);

        considerationCollections = new CollectionComponent<Consideration>(UASTemplateService.Instance.Considerations, "Consideration", "Considerations");
        root.Add(considerationCollections);
        
        decisionCollections = new CollectionComponent<Decision>(UASTemplateService.Instance.Decisions, "Decision", "Decisions");
        root.Add(decisionCollections);

        Body.Clear();
        Body.Add(root);
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        var bucket = model as Bucket;
        considerationCollections.SetElements(bucket.Considerations);
        decisionCollections.SetElements(bucket.Decisions);

    }

    ~BucketComponent()
    {
        disposables.Clear();
    }


}