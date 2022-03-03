using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IAgent
{
    AgentModel Model { get; }
    string TypeIdentifier { get; }
    void Tick();
    void SetAi(UAIModel model);
    UAIModel Ai { get; set; }
}