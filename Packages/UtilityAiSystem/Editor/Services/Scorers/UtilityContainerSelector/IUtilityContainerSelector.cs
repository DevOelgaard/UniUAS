using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IUtilityContainerSelector: IIdentifier
{
    public Bucket GetBestUtilityContainer(List<Bucket> containers, AiContext context);
    public Decision GetBestUtilityContainer(List<Decision> containers, AiContext context);
}
