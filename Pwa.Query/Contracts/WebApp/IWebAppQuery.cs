﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Domain;
using WebFramework.Utilities;

namespace Pwa.Query.Contracts.WebApp
{
    public interface IWebAppQuery
    {
        Task<List<WebAppQueryModel>> GetBests();
        Task<List<WebAppQueryModel>> GetGames();
        Task<List<WebAppQueryModel>> GetMostVisit();
        Task<OperationResult<WebAppQueryModel>> GetSingle(int id);
        Task<List<WebAppQueryModel>> RelatedApps(int id);
        Task<ResponseDto<WebAppQueryModel>> List(ResponseDto<WebAppQueryModel> response);
        Task<OperationResult<string>> Install(int id, CancellationToken cancellationToken);
    }
}
