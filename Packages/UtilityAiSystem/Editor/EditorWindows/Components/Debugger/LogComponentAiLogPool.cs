using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class LogComponentPool<T> where T: LogComponent
{
    private VisualElement root;
    private List<T> logComponents = new List<T>();

    internal LogComponentPool(VisualElement root)
    {
        this.root = root;
    }

    internal void Display(List<ILogModel> elements)
    {
        root.style.opacity = 1;
        for (var i = 0; i < elements.Count; i++)
        {
            if (i >= logComponents.Count)
            {
                var p = (T)Activator.CreateInstance(typeof(T));
                logComponents.Add(p);
                p.Display(elements[i]);
            }
            else
            {
                logComponents[i].Display(elements[i]);
            }
        }

        for (var i = elements.Count; i < logComponents.Count; i++)
        {
            logComponents[i].Hide();
        }
    }

    internal void Hide()
    {
        root.style.opacity = 0;
    }
}
