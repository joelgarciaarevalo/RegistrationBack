using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Net;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Registration.Post;

public class Function
{
    public AmazonSQSClient clientSqs = new();
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest APIGatewayProxyRequest, ILambdaContext context)
     {
        await SendMessage("https://sqs.us-east-1.amazonaws.com/002696638365/EventLogQueue", APIGatewayProxyRequest.Body);
    
        return new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = "Esta es la respuesta"
        };

    }

    private async Task SendMessage(
         string qUrl, string messageBody)
    {
        SendMessageResponse responseSendMsg =
          await clientSqs.SendMessageAsync(qUrl, messageBody);
        Console.WriteLine($"Message added to queue\n  {qUrl}");
        Console.WriteLine($"HttpStatusCode: {responseSendMsg.HttpStatusCode}");
    }

}

