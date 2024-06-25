using Amazon.Lambda.Core;
using System;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloWorldLambda;

public class Function
{
    
    /// <summary>
    /// A simple function that says the current time.
    /// </summary>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public string FunctionHandler(ILambdaContext context)
    {
        string now = DateTime.Now.ToString();
        Console.WriteLine(now);
        return now;
    }
}
