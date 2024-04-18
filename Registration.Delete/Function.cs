using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Registration.Data.Helpers;
using Registration.Data.Entities;
using System.Net;
using Newtonsoft.Json;
using Dapper;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Registration.Delete;

public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest APIGatewayProxyRequest, ILambdaContext context)
    {
        string connectionString = "";


        RegistrationDeleteResponse objRegistrationDeleteResponse = new RegistrationDeleteResponse();
        RegistrationDeleteRequest objRegistrationDeleteRequest = null;

        if (APIGatewayProxyRequest.QueryStringParameters != null)
        {
            try
            {
                objRegistrationDeleteRequest = JsonConvert.DeserializeObject<RegistrationDeleteRequest>(JsonConvert.SerializeObject(APIGatewayProxyRequest.QueryStringParameters));
            }
            catch (Exception)
            {

                return ResponseProcess("", (int)HttpStatusCode.BadRequest);
            }
        }

        string? host = Environment.GetEnvironmentVariable("Host");
        if (string.IsNullOrEmpty(host))
        {
            LambdaLogger.Log("The environment variable Host is required");
            return ResponseProcess(Newtonsoft.Json.JsonConvert.SerializeObject(objRegistrationDeleteResponse), (int)HttpStatusCode.InternalServerError);

        }
        string? port = Environment.GetEnvironmentVariable("Port");
        if (string.IsNullOrEmpty(port))
        {
            LambdaLogger.Log("The environment variable Port is required");
            return ResponseProcess(Newtonsoft.Json.JsonConvert.SerializeObject(objRegistrationDeleteResponse), (int)HttpStatusCode.InternalServerError);

        }
        string? db = Environment.GetEnvironmentVariable("Db");
        if (string.IsNullOrEmpty(db))
        {
            LambdaLogger.Log("The environment variable Db is required");
            return ResponseProcess(Newtonsoft.Json.JsonConvert.SerializeObject(objRegistrationDeleteResponse), (int)HttpStatusCode.InternalServerError);

        }
        string? dbUser = Environment.GetEnvironmentVariable("DbUser");
        if (string.IsNullOrEmpty(dbUser))
        {
            LambdaLogger.Log("The environment variable DbUser is required");
            return ResponseProcess(Newtonsoft.Json.JsonConvert.SerializeObject(objRegistrationDeleteResponse), (int)HttpStatusCode.InternalServerError);

        }
        string? dbPassword = Environment.GetEnvironmentVariable("DbPassword");
        if (string.IsNullOrEmpty(dbPassword))
        {
            LambdaLogger.Log("The environment variable DbPassword is required");
            return ResponseProcess(Newtonsoft.Json.JsonConvert.SerializeObject(objRegistrationDeleteResponse), (int)HttpStatusCode.InternalServerError);

        }


        //connectionString = $"Server=" + host + ";Port=" + port + ";Database=" + db + ";Uid=" + dbUser + ";Pwd=" + dbPassword + ";default command timeout=120;SslMode=none";

        connectionString = $"Server={host};Database={db};User Id={dbUser};Password={dbPassword};";

        DapperHelper objDapperHelper = new DapperHelper(connectionString);
        DynamicParameters b = new DynamicParameters();
        b.Add("pEventLogId", objRegistrationDeleteRequest.EventLogId);

        int result = objDapperHelper.ExecuteSingleRow<int>("spDeleteEventLogs", b);

        return ResponseProcess(Newtonsoft.Json.JsonConvert.SerializeObject(objRegistrationDeleteResponse), (int)HttpStatusCode.OK);

    }
    private APIGatewayProxyResponse ResponseProcess(string body, int HttpStatusCode)
    {
        IDictionary<string, string> openWith = new Dictionary<string, string>();
        openWith.Add("Access-Control-Allow-Origin", "*");
        openWith.Add("Access-Control-Allow-Methods", "DELETE");
        openWith.Add("Content-Type", "application/json");
        var response = new APIGatewayProxyResponse
        {
            StatusCode = HttpStatusCode,
            Body = body,
            Headers = openWith
        };
        return response;

    }
}