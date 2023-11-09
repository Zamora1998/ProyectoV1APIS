using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace ProyectoV1.App_Start
{
    public class AccesPolicyCors : Attribute, ICorsPolicyProvider
    {
        public async Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var corsRequestContext = request.GetCorsRequestContext();
            var originRequested = corsRequestContext.Origin;
            if (await IsOriginFromCostumer(originRequested))
            {
                var policy = new CorsPolicy
                {
                    AllowAnyHeader = true,
                    AllowAnyMethod = true,
                };

                policy.Origins.Add(originRequested);
                return policy;
            }
            return null;
        }

        private async Task<bool> IsOriginFromCostumer(string originRequested)
        {
            return true;
        }
    }
}