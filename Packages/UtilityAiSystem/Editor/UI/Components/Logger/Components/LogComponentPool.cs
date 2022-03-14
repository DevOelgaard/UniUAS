using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class LogComponentPool<T> where T: LogComponent
{
    private VisualElement root;
    internal List<T> LogComponents = new List<T>();

    internal LogComponentPool(VisualElement r, int initialPoolSize = 1)
    {
        this.root = r;
        for(var i = 0; i < initialPoolSize; i++)
        {
            var component = (T)Activator.CreateInstance(typeof(T));
            LogComponents.Add(component);
            root.Add(component);
            component.Hide();
        }
    }

    internal void Display(List<ILogModel> elements)
    {
        root.style.display = DisplayStyle.Flex;
        for (var i = 0; i < elements.Count; i++)
        {
            if (i >= LogComponents.Count)
            {
                var p = (T)Activator.CreateInstance(typeof(T));
                LogComponents.Add(p);
                p.UpdateUi(elements[i]);
            }
            else
            {
                LogComponents[i].UpdateUi(elements[i]);
            }
        }

        for (var i = elements.Count; i < LogComponents.Count; i++)
        {
            LogComponents[i].Hide();
        }
    }

    internal void Hide()
    {
        root.style.display = DisplayStyle.None;
    }
}
