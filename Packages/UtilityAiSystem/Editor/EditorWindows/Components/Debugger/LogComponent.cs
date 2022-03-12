using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal abstract class LogComponent: VisualElement
{
    internal abstract void Display(ILogModel element);

    internal virtual void Hide()
    {
        this.style.opacity = 0;
    }
}
