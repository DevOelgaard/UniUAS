using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class UtilityContainerSelector: IIdentifier
{
    internal List<Parameter> Parameters;
    protected UtilityContainerSelector()
    {
        Parameters = GetParameters();
    }

    public abstract Bucket GetBestUtilityContainer(List<Bucket> containers, AiContext context);
    public abstract Decision GetBestUtilityContainer(List<Decision> containers, AiContext context);

    public abstract string GetDescription();

    public abstract string GetName();

    public abstract List<Parameter> GetParameters();
}
