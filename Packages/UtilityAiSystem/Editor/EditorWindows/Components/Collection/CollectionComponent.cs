using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRxExtension;
using UnityEngine.UIElements;

public class CollectionComponent<T> : VisualElement where T : AiObjectModel
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private CompositeDisposable listViewSubscriptions = new CompositeDisposable();
    private IDisposable collectionUpdatedSub;

    private TemplateContainer root;

    private Label tempLabel;
    private Button addcopyButton;
    private VisualElement tempBody;

    private Label elementsLabel;
    private ScrollView elementsBody;
    private DropdownField elementsDropdown;

    private T tempElement;
    private ReactiveList<T> collection;
    private ReactiveList<AiObjectModel> templates;
    private List<string> dropDownChoices;

    public CollectionComponent(ReactiveList<T> collection, ReactiveList<AiObjectModel> templates, string tempLabel, string elementsLabel, string dropDownLabel = "Templates")
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType());
        Add(root);

        this.collection = collection;
        this.templates = templates;


        this.tempLabel = root.Q<Label>("Temp-Label");
        addcopyButton = root.Q<Button>("AddCopy-Button");
        tempBody = root.Q<VisualElement>("TempBody");

        this.elementsLabel = root.Q<Label>("Elements-Label");
        elementsBody = root.Q<ScrollView>("ElementsBody");
        elementsDropdown = root.Q<DropdownField>("Temp-DropdownField");

        this.tempLabel.text = tempLabel;

        addcopyButton.RegisterCallback<MouseUpEvent>(_ =>
            AddTempConsiderationCopy());

        elementsDropdown.label = dropDownLabel;

        elementsDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            SetTempElementFromName(evt.newValue);
        });

        this.elementsLabel.text = elementsLabel;

        collectionUpdatedSub?.Dispose();
        collectionUpdatedSub = collection.OnValueChanged
            .Subscribe(elements => UpdateElements(elements));
        UpdateElements(collection.Values);

        templates.OnValueChanged
            .Subscribe(_ => UpdateChoices())
            .AddTo(subscriptions);
        UpdateChoices();
    }

    private void UpdateChoices()
    {
        dropDownChoices = templates.Values
            .Select(c => c.Name)
            .ToList();

        elementsDropdown.choices.Clear();
        foreach (var choice in this.dropDownChoices)
        {
            elementsDropdown.choices.Add(choice);
        }
    }

    private void AddTempConsiderationCopy()
    {
        if (tempElement == null)
        {
            return;
        }
        collection.Add(tempElement);
        ClearElement();
    }

    private void SetTempElementFromName(string name)
    {
        //var element = (MainWindowModel)UASModel.Instance.Considerations.Values.FirstOrDefault(c => c.Name == name);
        var element = templates.Values.FirstOrDefault(e  => e.Name == name);
        var enableCopyButton = element != null ? true : false;
        addcopyButton.SetEnabled(enableCopyButton);
        if (element == null)
        {
            return;
        }
        tempElement = (T)element.Clone();
        tempBody.Clear();
        var elementComponent =  MainWindowService.GetComponent(tempElement);
        tempBody.Add(elementComponent);
    }

    private void ClearElement()
    {
        tempBody.Clear();
        tempElement = null;
        elementsDropdown.value = null;
    }

    private void UpdateElements(List<T> elements)
    {
        elementsBody.Clear();
        listViewSubscriptions.Clear();
        foreach (var element in elements)
        {
            var folded = new MainWindowFoldedComponent(element);
            var expanded = MainWindowService.GetComponent(element);
            var foldAble = new FoldableComponent(expanded, folded);
            var listView = new ListViewComponent(foldAble);

            listView.OnRemoveClicked
                .Subscribe(_ => collection.Remove(element))
                .AddTo(listViewSubscriptions);

            elementsBody.Add(listView);
        }
    }

    private void ClearSubscriptions()
    {
        collectionUpdatedSub?.Dispose();
        listViewSubscriptions.Clear();
        subscriptions.Clear();
    }

    ~CollectionComponent()
    {
        ClearSubscriptions();
    }
}