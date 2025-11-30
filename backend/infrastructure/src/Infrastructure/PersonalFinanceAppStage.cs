using Amazon.CDK;
using Constructs;

namespace Infrastructure
{
    public class PersonalFinanceAppStage : Stage
    {
        internal PersonalFinanceAppStage(Construct scope, string id, IStageProps props = null) : base(scope, id, props)
        {
            new PersonalFinanceAppStack(this, "PersonalFinanceApp");
        }
    }
}
