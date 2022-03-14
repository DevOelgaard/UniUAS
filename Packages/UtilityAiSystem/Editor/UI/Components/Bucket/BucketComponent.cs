using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;

internal class BucketComponent : AiObjectComponent 
{
    private CompositeDisposable disposables = new CompositeDisposable();
    private TemplateContainer root;
    private CollectionComponent<Consideration> considerationCollections;
    private CollectionComponent<Decision> decisionCollections;
    private Bucket model;
    private ParameterComponent weightComponent;

    internal BucketComponent() : base()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);

        weightComponent = new ParameterComponent();
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
        weightComponent.UpdateUi(bucket.Weight);
    }

    ~BucketComponent()
    {
        disposables.Clear();
    }


}