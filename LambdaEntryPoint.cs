using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace AngularRestfulAPI
{
    /// <summary>
    /// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the 
    /// actual Lambda function entry point. The Lambda handler field should be set to
    /// 
    /// AngularRestfulAPI::AngularRestfulAPI.LambdaEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaEntryPoint :

        // The base class must be set to match the AWS service invoking the Lambda function. If not Amazon.Lambda.AspNetCoreServer
        // will fail to convert the incoming request correctly into a valid ASP.NET Core request.
        //
        // API Gateway REST API                         -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
        // API Gateway HTTP API payload version 1.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
        // API Gateway HTTP API payload version 2.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction
        // Application Load Balancer                    -> Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction
        // 
        // Note: When using the AWS::Serverless::Function resource with an event type of "HttpApi" then payload version 2.0
        // will be the default and you must make Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction the base class.

        Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
 

        /// <summary>
        /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
        /// needs to be configured in this method using the UseStartup<>() method.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseStartup<Startup>();
        }

        /// <summary>
        /// Use this override to customize the services registered with the IHostBuilder. 
        /// 
        /// It is recommended not to call ConfigureWebHostDefaults to configure the IWebHostBuilder inside this method.
        /// Instead customize the IWebHostBuilder in the Init(IWebHostBuilder) overload.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Init(IHostBuilder builder)
        {
            SQSClient = new AmazonSQSClient();
        }
        IAmazonSQS SQSClient { get; set; }
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            var awsCredentials = new BasicAWSCredentials("bbbbbbbbbbbbbbbbbbbb", "nnnnnnnnnnnnnnnnnnnn");
            var region = RegionEndpoint.GetBySystemName("us-east-1");


            
            if (request.HttpMethod == "POST")
            {
                Console.WriteLine($"REquest {request.Body}");
                using (var client = new AmazonLambdaClient(awsCredentials, region))
                {
                    var request_ = new Amazon.Lambda.Model.InvokeRequest
                    {
                        FunctionName = "HotchocolateService-GraphQLReceivedMutationFunctio-1OUT8F062N8X6",
                        InvocationType = InvocationType.RequestResponse,
                        LogType = LogType.Tail,
                        Payload = JsonConvert.SerializeObject(request.Body)
                    };

                    var result = Task.Run(() => _ = client.InvokeAsync(request_));
                      var res = result.GetAwaiter().GetResult();


                }

              /*  var sqsRequest = new SendMessageRequest
                {
                    QueueUrl = "https://sqs.us-east-1.amazonaws.com/280449388741/GraphQLDataQueue",
                    MessageBody = request.Body
                };

                await SQSClient.SendMessageAsync(sqsRequest);*/
            }
            Console.WriteLine($"REquest {request.Body}");
           

            object obj = new { data = new { addMessage = new { Message = new { content = "Test Testing again", SentAt = DateTime.Now.ToString(), MessageFrom = new { Id = "1", DisplayName = "Test" } } } }, type = "data", id = "1" };
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(obj),
                Headers = new Dictionary<string, string> {
                    { "Content-Type", "application/json" }, 
                    { "Access-Control-Allow-Origin","*"  }, 
                    { "Access-Control-Allow-Methods","OPTIONS,POST,GET"}, 
                    {"Access-Control-Allow-Headers", "Origin,Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token"}
    }
            };

            return response;
        }
    }
}
