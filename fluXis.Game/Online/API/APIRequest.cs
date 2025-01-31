using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Utils;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Online.API;

public class APIRequest<T> where T : class
{
    protected virtual string Path => string.Empty;
    protected virtual HttpMethod Method => HttpMethod.Get;
    protected virtual string RootUrl => Config.APIUrl;

    protected APIEndpointConfig Config => fluxel.Endpoint;
    private FluxelClient fluxel;

    public event Action<APIResponse<T>> Success;
    public event Action<Exception> Failure;
    public event Action<long, long> Progress;

    public APIResponse<T> Response { get; protected set; }

    public virtual void Perform(FluxelClient fluxel)
    {
        this.fluxel = fluxel;

        Logger.Log($"Performing API request {GetType().Name.Split('.').Last()}...", LoggingTarget.Network);

        var request = new FluXisJsonWebRequest<T>($"{RootUrl}{Path}");
        request.Method = Method;
        request.AllowInsecureRequests = true;
        request.UploadProgress += (current, total) => Progress?.Invoke(current, total);

        if (!string.IsNullOrEmpty(fluxel.Token))
            request.AddHeader("Authorization", fluxel.Token);

        try
        {
            CreatePostData(request);
            request.Perform();
            TriggerSuccess(request.ResponseObject);
        }
        catch (Exception e)
        {
            Logger.Error(e, $"API request {GetType().Name.Split('.').Last()} failed!");
            Response = new APIResponse<T>(400, e.Message, null);
            Failure?.Invoke(e);
        }
    }

    protected void TriggerSuccess(APIResponse<T> res)
    {
        Response = res;

        if (res != null)
            fluxel.Schedule(() => Success?.Invoke(Response));
    }

    public Task PerformAsync(FluxelClient fluxel)
    {
        var task = new Task(() => Perform(fluxel));
        task.Start();
        return task;
    }

    protected virtual void CreatePostData(FluXisJsonWebRequest<T> request) { }
}

public class FluXisJsonWebRequest<T2> : JsonWebRequest<APIResponse<T2>>
{
    public new APIResponse<T2> ResponseObject { get; private set; }

    public FluXisJsonWebRequest(string url = null, params object[] args)
        : base(url, args)
    {
    }

    protected override void ProcessResponse()
    {
        var response = GetResponseString();

        if (response != null)
            ResponseObject = response.Deserialize<APIResponse<T2>>();
    }
}
