using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Damas.Core
{
    public static class WebApi
    {
        private const string URL = "http://dama.semprenegocio.com.br/";

        private const string metodoEnviaDadosAposJogo = "?controller=Ranking&action=insertRanking";
        public static bool EnviaDadosAposJogo(string nomeJogador, int score, string timer, int movimentos)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                string url = metodoEnviaDadosAposJogo + string.Format("&name={0}&score={1}&time={2}&mov={3}",
                    nomeJogador,
                    score.ToString(),
                    timer,
                    movimentos.ToString());

                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var retorno = response.Content.ReadAsAsync<dynamic>().Result;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }            

        }
    }
}
