//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//public class AiManager
//{
//    internal Dictionary<IAgent,UAIModel> aisByAgent = new Dictionary<IAgent,UAIModel>();

//    private static AiManager instance;

//    private AiManager()
//    {
//    }

//    public void SetAi(IAgent agent, UAIModel ai)
//    {
//        if (aisByAgent.ContainsKey(agent))
//        {
//            aisByAgent[agent] = ai;
//        } else
//        {
//            aisByAgent.Add(agent, ai);
//        }
//        ai.Context.Agent = agent;
//    }

//    public UAIModel GetAi(IAgent agent)
//    {
//        if (aisByAgent.ContainsKey(agent))
//        {
//            return aisByAgent[agent];
//        }
//        return null;
//    }

//    public void RemoveAgent(IAgent agent)
//    {
//        aisByAgent.Remove(agent);
//    }


//    public static AiManager Instance => instance ?? (instance = new AiManager());




//}
