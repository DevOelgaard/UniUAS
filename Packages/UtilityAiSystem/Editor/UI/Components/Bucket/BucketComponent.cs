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
    private TabViewComponent tabView;
    private Button considerationsTab;
    private Button decisionTab;

    internal BucketComponent() : base()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);

        weightComponent = new ParameterComponent();
        tabView = new TabViewComponent();
        considerationCollections = new CollectionComponent<Consideration>(UASTemplateService.Instance.Considerations, "Consideration", "Considerations");
        decisionCollections = new CollectionComponent<Decision>(UASTemplateService.Instance.Decisions, "Decision", "Decisions");

        considerationsTab = tabView.AddTabGroup("Considerations", considerationCollections);
        decisionTab = tabView.AddTabGroup("Decisions", decisionCollections);
        root.Add(tabView);

        Body.Clear();
        Body.Add(root);
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        var bucket = model as Bucket;
        disposables.Clear();

        considerationsTab.text = "Considerations (" + bucket.Considerations.Count + ")";
        decisionTab.text = "Decisions (" + bucket.Decisions.Count + ")";
        
        ScoreContainer.Add(weightComponent);
        bucket.Considerations.OnValueChanged
            .Subscribe(list => considerationsTab.text = "Considerations (" + list.Count + ")")
            .AddTo(disposables);

        bucket.Decisions.OnValueChanged
            .Subscribe(list => decisionTab.text = "Decisions (" + list.Count + ")")
            .AddTo(disposables);

        considerationCollections.SetElements(bucket.Considerations);
        decisionCollections.SetElements(bucket.Decisions);
        weightComponent.UpdateUi(bucket.Weight);
    }

    ~BucketComponent()
    {
        disposables.Clear();
    }


}