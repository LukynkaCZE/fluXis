using System.Linq;
using System.Net.Http;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Scoring;
using fluXis.Shared.Utils;

namespace fluXis.Game.Online.API.Requests.Scores;

public class ScoreSubmitRequest : APIRequest<APIScoreResponse>
{
    protected override string Path => "/scores/upload";
    protected override HttpMethod Method => HttpMethod.Post;

    private ScoreInfo score { get; }

    public ScoreSubmitRequest(ScoreInfo score)
    {
        this.score = score;
    }

    public override void Perform(FluxelClient fluxel)
    {
        if (fluxel.Token == null)
        {
            TriggerSuccess(new APIResponse<APIScoreResponse>(401, "Not logged in.", null));
            return;
        }

        if (score.Mods.Any(m => m == "PA"))
        {
            TriggerSuccess(new APIResponse<APIScoreResponse>(400, "Score not submittable.", null));
            return;
        }

        base.Perform(fluxel);
    }

    protected override void CreatePostData(FluXisJsonWebRequest<APIScoreResponse> request)
    {
        request.AddRaw(score.Serialize());
    }
}
