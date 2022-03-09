using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;

internal class ResponseFunctionComponent: VisualElement
{
    private CompositeDisposable disposables = new CompositeDisposable();

    private DropdownField typeDropdown;
    private VisualElement body;
    private Button removeButton;

    private ResponseFunction responseFunction;

    public IObservable<ResponseFunction> OnResponseFunctionChanged => onResponseFunctionChanged;
    private Subject<ResponseFunction> onResponseFunctionChanged = new Subject<ResponseFunction>();

    public IObservable<ResponseFunction> OnRemoveClicked => onRemoveClicked;
    private Subject<ResponseFunction> onRemoveClicked = new Subject<ResponseFunction>();

    public IObservable<bool> OnParametersChanged => onParametersChanged;
    private Subject<bool> onParametersChanged = new Subject<bool>();


    public ResponseFunctionComponent(ResponseFunction responseFunction)
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);

        this.responseFunction = responseFunction;
        typeDropdown = root.Q<DropdownField>("TypeDropdown");
        body = root.Q<VisualElement>("Body");
        removeButton = root.Q<Button>("RemoveButton");

        typeDropdown.choices = AssetDatabaseService
            .GetInstancesOfType<ResponseFunction>()
            .Select(rF => rF.Name)
            .ToList();

        typeDropdown.value = responseFunction.Name;

        typeDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            this.responseFunction = AssetDatabaseService.GetInstancesOfType<ResponseFunction>()
             .First(rF => rF.Name == evt.newValue);
            onResponseFunctionChanged.OnNext(this.responseFunction);
        });

        removeButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            onRemoveClicked.OnNext(responseFunction);
        });


        UpdateUi();
    }

    private void UpdateUi()
    {
        body.Clear();
        disposables.Clear();
        foreach(var parameter in responseFunction.Parameters)
        {
            body.Add(new ParameterComponent(parameter));
            parameter.OnValueChange
                .Subscribe(_ => onParametersChanged.OnNext(true))
                .AddTo(disposables);
        }
    }

    ~ResponseFunctionComponent()
    {
        disposables.Clear();
    }
}