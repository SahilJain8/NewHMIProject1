#region Using directives
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.DataLogger;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.NetLogic;
using FTOptix.ODBCStore;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.Store;
using FTOptix.UI;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UAManagedCore;
using FTOptix.CommunicationDriver;
using OpcUa = UAManagedCore.OpcUa;
#endregion

public class RuntimeNetLogic1 : BaseNetLogic
{
    private static readonly HttpClient _httpClient = new HttpClient();

    // Example: your custom GET function
    public async Task<string> DoGetRequest(string url)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string body = await response.Content.ReadAsStringAsync();

            Log.Info($"GET {url} OK, body length={body.Length}");
            return body;
        }
        catch (Exception ex)
        {
            Log.Error($"GET {url} failed: {ex}");
            return null;
        }
    }
    public override async void Start()
    {
       

    var varibale = LogicObject.GetVariable("MachineSpeed");
        varibale.Value = 1234;
    
        var result = await DoGetRequest("https://api.restful-api.dev/objects/1");

        Log.Info(result.ToString() );

        Log.Info(varibale.ToString());

   
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
}
