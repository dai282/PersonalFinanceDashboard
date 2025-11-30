using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ECR;
using Constructs;

namespace Infrastructure
{
    public class PersonalFinanceAppStack : Stack
    {
        internal PersonalFinanceAppStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here
            // VPC
            var vpc = new Vpc(this, "PersonalFinanceVPC", new VpcProps
            {
                MaxAzs = 2,
                NatGateways = 1 // Need 1 for private subnets
            });

            // RDS Database
            var dbInstance = new DatabaseInstance(this, "PersonalFinanceDB", new DatabaseInstanceProps
            {
                Engine = DatabaseInstanceEngine.SqlServerEx(new SqlServerExInstanceEngineProps
                {
                    Version = SqlServerEngineVersion.VER_15
                }),
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MICRO),
                Vpc = vpc,
                VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_WITH_EGRESS },
                AllocatedStorage = 20,
                DatabaseName = "PersonalFinanceDB",
                Credentials = Credentials.FromGeneratedSecret("admin"),
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            // ECS Cluster
            var cluster = new Cluster(this, "PersonalFinanceCluster", new ClusterProps
            {
                Vpc = vpc
            });

            // Fargate Service with Application Load Balancer
            var fargateService = new ApplicationLoadBalancedFargateService(this, "PersonalFinanceService",
                new ApplicationLoadBalancedFargateServiceProps
                {
                    Cluster = cluster,
                    Cpu = 256,
                    DesiredCount = 1,
                    TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                    {
                        // This will be updated by the pipeline
                        Image = ContainerImage.FromRegistry("amazon/amazon-ecs-sample"),
                        ContainerPort = 8080,
                        Environment = new Dictionary<string, string>
                        {
                            { "ASPNETCORE_ENVIRONMENT", "Production" },
                            { "ConnectionStrings__PersonalFinanceDB", 
                                $"Server={dbInstance.DbInstanceEndpointAddress};Database=PersonalFinanceDB;User Id=admin;Password=PLACEHOLDER" }
                        }
                    },
                    MemoryLimitMiB = 512,
                    PublicLoadBalancer = true
                });

            // Allow service to connect to database
            dbInstance.Connections.AllowDefaultPortFrom(fargateService.Service);

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
