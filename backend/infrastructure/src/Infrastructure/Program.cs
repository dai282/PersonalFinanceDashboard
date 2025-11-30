using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            // Create the PIPELINE stack (not the app stack directly)
            new PersonalFinancePipelineStack(app, "InfrastructureStack", new StackProps
            {

                Env = new Amazon.CDK.Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION") ?? "us-east-1",
                }
            });
            app.Synth();
        }
    }
}
