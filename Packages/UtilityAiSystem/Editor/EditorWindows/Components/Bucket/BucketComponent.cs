using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BucketComponent : MainWindowComponent
{
    private TemplateContainer root;
    private CollectionComponent<Consideration> considerationCollections;
    private CollectionComponent<Decision> decisionCollections;

    public BucketComponent(Bucket model) : base(model)
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        considerationCollections = 
            new CollectionComponent<Consideration>(model.Considerations, 
            UASTemplateService.Instance.Considerations, "Consideration-Template", "Considerations");
        root.Add(considerationCollections);
        
        decisionCollections = 
            new CollectionComponent<Decision>(model.Decisions, 
            UASTemplateService.Instance.Decisions, "Decision-Template", "Decisions");
        root.Add(decisionCollections);

        Body.Clear();
        Body.Add(root);
    }
}