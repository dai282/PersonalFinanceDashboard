using Amazon.CDK;
using Amazon.CDK.Pipelines;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.IAM;
using Constructs;

namespace Infrastructure
{
    public class PersonalFinancePipelineStack : Stack
    {
        internal PersonalFinancePipelineStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // Source: GitHub repository
            var source = CodePipelineSource.GitHub(
                "dai282/PersonalFinanceDashboard",
                "main", // or "master"
                new GitHubSourceOptions
                {
                    Authentication = SecretValue.SecretsManager("github-token") // We'll create this
                }
            );

            // Build Docker image step
            var dockerBuildStep = new CodeBuildStep("BuildDockerImage", new CodeBuildStepProps
            {
                Input = source,
                Commands = new[]
                {
                    "cd backend",
                    "echo Build started on `date`",
                    "echo Building Docker image...",
                    "docker build -t personalfinance-api:latest -f PersonalFinance.API/Dockerfile .",
                    "echo Build completed"
                },
                BuildEnvironment = new BuildEnvironment
                {
                    BuildImage = LinuxBuildImage.STANDARD_7_0,
                    Privileged = true
                }
            });

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

            // Create the pipeline
            var pipeline = new CodePipeline(this, "PersonalFinancePipeline", new CodePipelineProps
            {
                PipelineName = "PersonalFinancePipeline",
                Synth = synthStep,
                SelfMutation = true,
                DockerEnabledForSynth = true,
                CodeBuildDefaults = new CodeBuildOptions
                {
                    BuildEnvironment = new BuildEnvironment
                    {
                        BuildImage = LinuxBuildImage.STANDARD_7_0,
                        Privileged = true // Needed for Docker
                    }
                }
            });

            // Add application stage
            var appStage = new PersonalFinanceAppStage(this, "Production", new StageProps
            {
                Env = props?.Env
            });

            var stageDeployment = pipeline.AddStage(appStage);
            stageDeployment.AddPre(dockerBuildStep);
        }
    }
}