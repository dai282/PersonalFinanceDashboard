using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.RDS;
using Constructs;
using System.Collections.Generic;

namespace Infrastructure
{
    public class PersonalFinanceAppStack: Stack
    {
        internal PersonalFinanceAppStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // VPC
            var vpc = new Vpc(this, "PersonalFinanceVPC", new VpcProps
            {
                MaxAzs = 2,
                NatGateways = 0, // to reduce cost,
                SubnetConfiguration = new[]
                {
                    new SubnetConfiguration
                    {
                        Name = "Public",
                        SubnetType = SubnetType.PUBLIC,
                        CidrMask = 24
                    },
                    new SubnetConfiguration
                    {
                        Name = "Isolated",
                        SubnetType = SubnetType.PRIVATE_ISOLATED, // DB goes here (no internet access)
                        CidrMask = 24
                    }
                }
            });

            // RDS Database
            var dbInstance = new DatabaseInstance(this, "PersonalFinanceDB", new DatabaseInstanceProps
            {
                Engine = DatabaseInstanceEngine.SqlServerEx(new SqlServerExInstanceEngineProps
                {
                    Version = SqlServerEngineVersion.VER_15
                }),
                InstanceType = Amazon.CDK.AWS.EC2.InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MICRO),
                Vpc = vpc,
                //PRIVATE_ISOLATED because we have no NAT Gateway
                VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_ISOLATED },
                AllocatedStorage = 20,
                DatabaseName = "PersonalFinanceDB",
                Credentials = Credentials.FromGeneratedSecret("admin"),
            });

            // ECS Cluster
            var cluster = new Cluster(this, "PersonalFinanceCluster", new ClusterProps
            {
                Vpc = vpc
            });

            // Fargate Service with Application Load Balancer. Build from Dockerfile & Config
            var fargateService = new ApplicationLoadBalancedFargateService(this, "PersonalFinanceService",
                new ApplicationLoadBalancedFargateServiceProps
                {
                    Cluster = cluster,
                    Cpu = 256,
                    DesiredCount = 1,
                    // IMPORTANT: Must be true because we have no NAT. 
                    // This allows the container to pull the Docker image from ECR via the Public Internet.
                    AssignPublicIp = true,
                    TaskSubnets = new SubnetSelection { SubnetType = SubnetType.PUBLIC },
                    TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                    {
                        // AUTOMATION: This tells CDK to look at the directory above (".."), 
                        // find a Dockerfile, build it, and push to ECR.
                        Image = ContainerImage.FromAsset("../PersonalFinance.API"),
                        ContainerPort = 8080,
                        Environment = new Dictionary<string, string>
                        {
                            { "ASPNETCORE_ENVIRONMENT", "Production" },
                            // Pass the ARN only. Your .NET code must read the secret using Amazon.SecretsManager SDK
                            { "DB_SECRET_ARN", dbInstance.Secret?.SecretArn },
                            { "DB_HOST", dbInstance.DbInstanceEndpointAddress }
                            //{ "ConnectionStrings__PersonalFinanceDB",
                            //    $"Server={dbInstance.DbInstanceEndpointAddress};Database=PersonalFinanceDB;User Id=admin;Password=PLACEHOLDER" }
                        }
                    },
                    MemoryLimitMiB = 512,
                    PublicLoadBalancer = true
                });

            // Allow service to connect to database
            dbInstance.Connections.AllowDefaultPortFrom(fargateService.Service);

            // Grant the container permission to read the secret value
            dbInstance.Secret?.GrantRead(fargateService.TaskDefinition.TaskRole);

            // Outputs
            new CfnOutput(this, "LoadBalancerDNS", new CfnOutputProps
            {
                Value = fargateService.LoadBalancer.LoadBalancerDnsName,
                Description = "Load Balancer DNS"
            });

            new CfnOutput(this, "DatabaseEndpoint", new CfnOutputProps
            {
                Value = dbInstance.DbInstanceEndpointAddress,
                Description = "Database Endpoint"
            });

            new CfnOutput(this, "DatabaseSecretArn", new CfnOutputProps
            {
                Value = dbInstance.Secret?.SecretArn ?? "No secret",
                Description = "Database Secret ARN"
            });
        }
    }
}
