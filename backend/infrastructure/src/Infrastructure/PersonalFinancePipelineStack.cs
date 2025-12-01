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

            // CHANGED: Use CodeBuildStep instead of ShellStep to enable Docker
            var synthStep = new CodeBuildStep("Synth", new CodeBuildStepProps
            {
                Input = source,
                Commands = new[]
                {
                    // Install .NET 10 SDK
                    "wget https://dot.net/v1/dotnet-install.sh",
                    "chmod +x dotnet-install.sh",
                    "./dotnet-install.sh --channel 10.0 --install-dir /usr/share/dotnet",
                    "export PATH=$PATH:/usr/share/dotnet",
                    "dotnet --version",

        
                    // Navigate and build
                    "cd backend/infrastructure/src",
                    "npm install -g aws-cdk",

                    "dotnet restore",
                    "dotnet build",
        
                    // Synth CDK
                    "cdk synth"
                },
                PrimaryOutputDirectory = "backend/infrastructure/cdk.out",
                // CRITICAL: This enables Docker-in-Docker so CDK can build your image
                BuildEnvironment = new Amazon.CDK.AWS.CodeBuild.BuildEnvironment
                {
                    Privileged = true
                }
            });

            var pipeline = new CodePipeline(this, "pipeline", new CodePipelineProps
            {
                PipelineName = "PersonalFinancePipeline",
                Synth = synthStep,
                // Optional: Reduce cost of the pipeline execution itself
                SelfMutation = true,
                DockerEnabledForSynth = true
            });

            // Add application stage
            var appStage = new PersonalFinanceAppStage(this, "Production", new StageProps
            {
                Env = props?.Env
            });

            pipeline.AddStage(appStage);
        }
    }
}
