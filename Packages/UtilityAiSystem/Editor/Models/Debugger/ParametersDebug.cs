using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ParametersDebug
{
    public List<KeyValuePair<string, string>> Parameters { get; private set; } = new List<KeyValuePair<string, string>>();

    public ParametersDebug(List<Parameter> parameters)
    {
        Parameters = new List<KeyValuePair<string, string>>();
        foreach (var parameter in parameters)
        {
            var kv = new KeyValuePair<string, string>(parameter.Name, parameter.Value.ToString());
            Parameters.Add(kv);
        }
    }
}
