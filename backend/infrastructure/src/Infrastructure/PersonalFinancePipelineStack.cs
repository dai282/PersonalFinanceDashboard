using Amazon.CDK;
using Amazon.CDK.Pipelines;
using Constructs;

namespace Infrastructure
{
    public class PersonalFinancePipelineStack : Stack
    {
        internal PersonalFinancePipelineStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // Source: GitHub repository
            var source = CodePipelineSource.GitHub(
                "dai282/PersonalFinanceDashboard", // Update this
                "main", // or "master"
                new GitHubSourceOptions
                {
                    Authentication = SecretValue.SecretsManager("github-token") // We'll create this
                }
            );

            // Synth step: Build CDK application
            var synthStep = new ShellStep("Synth", new ShellStepProps
            {
                Input = source,
                Commands = new[]
                {
                    "cd backend/infrastructure",
                    "npm install -g aws-cdk",
                    "dotnet build",
                    "cdk synth"
                },
                PrimaryOutputDirectory = "backend/infrastructure/cdk.out"
            });

            var pipeline = new CodePipeline(this, "pipeline", new CodePipelineProps
            {
                PipelineName = "PersonalFinancePipeline",
                Synth = synthStep
            });
        }
    }
}
