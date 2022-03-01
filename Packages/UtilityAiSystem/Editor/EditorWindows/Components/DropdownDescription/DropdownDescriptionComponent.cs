using System;
using UnityEngine.UIElements;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using UniRxExtension;

public class DropdownDescriptionComponent<T> : VisualElement where T : IIdentifier
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private TemplateContainer root;
    private DropdownField dropDown;
    private Label description;
    private ReactiveList<T> elements;
    public IObservable<T> OnDropdownValueChanged => onDropdownValueChanged;
    private Subject<T> onDropdownValueChanged = new Subject<T>();

    public DropdownDescriptionComponent(ReactiveList<T> elements, string labelName, string initialValue)
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        this.Add(root);
        dropDown = root.Q<DropdownField>("Elements-DropdownField");
        dropDown.label = labelName;
        dropDown.RegisterCallback<ChangeEvent<string>>(evt => ValueChanged(evt.newValue));

        description = root.Q<Label>("DescriptionText-Label");

        this.elements = elements;
        this.elements
            .OnValueChanged
            .Subscribe(_ => UpdateDropdown(dropDown.value))
            .AddTo(subscriptions);

        UpdateDropdown(initialValue);
    }

    private void ValueChanged(string newValue)
    {
        var element = elements.Values.FirstOrDefault(e => e.GetName() == newValue);
        if (element != null)
        {
            onDropdownValueChanged.OnNext(element);
            description.text = element.GetDescription();
        }
    }

    private void UpdateDropdown(string initialValue)
    {
        dropDown.choices.Clear();
        if (elements.Values.Count <= 0)
        {
            return;
        }
        foreach (var e in elements.Values)
        {
            dropDown.choices.Add(e.GetName());
            if (e.GetName() == initialValue)
            {
                dropDown.value = e.GetName();
                description.text = e.GetDescription();
            }
        }
    }

    ~DropdownDescriptionComponent()
    {
        subscriptions.Clear();
    }

}
