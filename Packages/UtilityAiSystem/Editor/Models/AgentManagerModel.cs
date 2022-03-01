using System;
using System.Collections;
using System.Collections.Generic;
using UniRxExtension;
using UnityEngine;

public class AgentManagerModel
{
    public Dictionary<string, ReactiveList<IAgent>> AgentsByType = new Dictionary<string, ReactiveList<IAgent>>();
    public List<string> AgentTypes = new List<string>();

}